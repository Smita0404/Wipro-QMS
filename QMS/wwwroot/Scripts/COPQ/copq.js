var tabledata = [];
var table = null;
let filterStartDate = moment().startOf('week').format('YYYY-MM-DD');
let filterEndDate = moment().endOf('week').format('YYYY-MM-DD');

$(document).ready(function () {
    $('#dateRangeText').text(
        moment(filterStartDate).format('MMMM D, YYYY') + ' - ' + moment(filterEndDate).format('MMMM D, YYYY')
    );

    const picker = new Litepicker({
        element: document.getElementById('customDateTrigger'),
        singleMode: false,
        format: 'DD-MM-YYYY',
        numberOfMonths: 2,
        numberOfColumns: 2,
        dropdowns: { minYear: 2020, maxYear: null, months: true, years: true },
        plugins: ['ranges'],
        setup: (picker) => {
            picker.on('selected', (start, end) => {
                filterStartDate = start.format('YYYY-MM-DD');
                filterEndDate = end.format('YYYY-MM-DD');
                $('#dateRangeText').text(`${start.format('MMMM D, YYYY')} - ${end.format('MMMM D, YYYY')}`);
                loadData();
            });

            picker.on('clear', () => {
                filterStartDate = "";
                filterEndDate = "";
                $('#dateRangeText').text("Select Date Range");
                loadData();
            });
        },
        ranges: {
            Today: [moment(), moment()],
            Yesterday: [moment().subtract(1, 'days'), moment().subtract(1, 'days')],
            'Last 7 Days': [moment().subtract(6, 'days'), moment()],
            'Last 30 Days': [moment().subtract(29, 'days'), moment()],
            'This Month': [moment().startOf('month'), moment().endOf('month')],
            'Last Month': [moment().subtract(1, 'month').startOf('month'), moment().subtract(1, 'month').endOf('month')]
        },
        startDate: moment().startOf('week').format('DD-MM-YYYY'),
        endDate: moment().endOf('week').format('DD-MM-YYYY')
    });

    $('#customDateTrigger').on('click', function () {
        picker.show();
    });

    document.getElementById('backButton').addEventListener('click', function () {
        window.history.back();
    });

    $('#upload-button').on('click', async function () { 
        var expectedColumns = [
            'CCCN Date', 'Reported By', 'Location', 'Customer Name', 'Dealer Name', 'Description', 'Status', 'Completion', 'ClosureRemarks'
        ];

        var url = '/Service/UploadComplaintDumpExcel';
        handleImportExcelFile(url, expectedColumns);
    });

    loadData();
});

function loadData() {
    Blockloadershow();

    $.ajax({
        url: '/Service/GetAll',
        type: 'GET',
        dataType: 'json',
        data: {
            startDate: filterStartDate,
            endDate: filterEndDate
        },
        success: function (data) {
            if (Array.isArray(data)) {
                renderCOPQTable(data); // load into Tabulator or grid
            } else {
                showDangerAlert('No Data available.');
            }
            Blockloaderhide();
        },

        error: function (xhr, status, error) {
            showDangerAlert('Error retrieving data: ' + error);
            Blockloaderhide();
        }
    });
}
function renderCOPQTable(response) {
    if (!Array.isArray(response)) {
        console.error("Invalid response, expected array:", response);
        showDangerAlert("Invalid data received.");
        Blockloaderhide();
        return;
    }

    Blockloadershow();

    const tabledata = response.map((item, index) => ({
        Sr_No: index + 1,
        Id: item.id,
        CCCNDate: item.cccnDate ? new Date(item.cccnDate).toLocaleDateString("en-GB") : "",
        ReportedBy: item.reportedBy,
        CLocation: item.cLocation,
        CustName: item.custName,
        DealerName: item.dealerName,
        CDescription: item.cDescription,
        CStatus: item.cStatus,
        Completion: item.completion,
        Remarks: item.remarks,
        CreatedDate: item.createdDate ? new Date(item.createdDate).toLocaleDateString("en-GB") : "",
        CreatedBy: item.createdBy,
        UpdatedDate: item.updatedDate ? new Date(item.updatedDate).toLocaleDateString("en-GB") : "",
        UpdatedBy: item.updatedBy,
        IsDeleted: item.isDeleted
    }));

    const columns = [
        {
            title: "Action",
            field: "Action",
            hozAlign: "center",
            headerHozAlign: "center",
            frozen: true, headerMenu: headerMenu,
            formatter: function (cell) {
                const rowData = cell.getRow().getData();
                return `<i onclick="delConfirm(${rowData.Id})" class="fas fa-trash-alt mr-2 fa-1x" title="Delete" style="color:red;cursor:pointer;margin-left: 5px;"></i>`;
            }
        },
        { title: "SNo", field: "Sr_No", sorter: "number", hozAlign: "center", headerHozAlign: "center", headerMenu: headerMenu, frozen: true },
        editableColumn("CCCN Date", "CCCNDate", "date", "center"),
        editableColumn("Reported By", "ReportedBy"),
        editableColumn("Location", "CLocation"),
        editableColumn("Customer Name", "CustName"),
        editableColumn("Dealer Name", "DealerName"),
        editableColumn("Description", "CDescription"),
        editableColumn("Status", "CStatus"),
        editableColumn("Completion", "Completion"),
        editableColumn("Remarks", "Remarks"),
        { title: "Created By", field: "CreatedBy", headerMenu: headerMenu, visible: false },
        { title: "Created Date", field: "CreatedDate", headerMenu: headerMenu, visible: false },
        { title: "Updated By", field: "UpdatedBy", headerMenu: headerMenu, visible: false },
        { title: "Updated Date", field: "UpdatedDate", headerMenu: headerMenu, visible: false }
    ];

    if (typeof table !== 'undefined' && table instanceof Tabulator) {
        table.replaceData(tabledata);
    } else {
        table = new Tabulator("#copq-table", {
            data: tabledata,
            layout: "fitDataFill",
            movableColumns: true,
            pagination: "local",
            paginationSize: 10,
            paginationSizeSelector: [10, 50, 100, 500],
            paginationCounter: "rows",
            placeholder: "No data available",
            columns: columns
        });

        table.on("cellEdited", function (cell) {
            saveEditedRow(cell.getRow().getData());
        });

        $("#addButton").off("click").on("click", function () {
            const newRow = {
                Id: 0,
                Sr_No: table.getDataCount() + 1,
                CCCNDate: "",
                ReportedBy: "",
                CLocation: "",
                CustName: "",
                DealerName: "",
                CDescription: "",
                CStatus: "",
                Completion: "",
                Remarks: ""
            };
            table.addRow(newRow, false);
        });
    }

    Blockloaderhide();
}

// Utility for editable column config
function editableColumn(title, field, editor = "input", align = "left") {
    return {
        title: title,
        field: field,
        editor: editor,
        hozAlign: align,
        headerHozAlign: "center"
    };
}


function toIsoDatePO(value) {
    if (!value) return "";
    const parts = value.split('/');
    return parts.length === 3 ? `${parts[2]}-${parts[1].padStart(2, '0')}-${parts[0].padStart(2, '0')}` : value;
}

function saveEditedRow(rowData) {
    const cleanedData = {
        Id: rowData.Id || 0,
        CCCNDate: toIsoDatePO(rowData.CCCNDate),
        ReportedBy: rowData.ReportedBy || null,
        CLocation: rowData.CLocation || null,
        CustName: rowData.CustName || null,
        DealerName: rowData.DealerName || null,
        CDescription: rowData.CDescription || null,
        CStatus: rowData.CStatus || null,
        Completion: rowData.Completion || null,
        Remarks: rowData.Remarks || null
    };

    const isNew = cleanedData.Id === 0;
    const url = isNew ? '/Service/CreateAsync' : '/Service/UpdateAsync';

    $.ajax({
        url: url,
        type: 'POST',
        data: JSON.stringify(cleanedData),
        contentType: 'application/json',
        success: function (data) {
            if (data.success) {
                loadData();
            } else {
                showDangerAlert(data.message || (isNew ? "Create failed." : "Update failed."));
            }
        },
        error: function (xhr) {
            showDangerAlert(xhr.responseText || "Error saving record.");
        }
    });
}

var headerMenu = function () {
    const menu = [];
    const columns = this.getColumns();

    columns.forEach(column => {
        const icon = document.createElement("i");
        icon.classList.add("fas", column.isVisible() ? "fa-check-square" : "fa-square");

        const label = document.createElement("span");
        const title = document.createElement("span");
        title.textContent = " " + column.getDefinition().title;

        label.appendChild(icon);
        label.appendChild(title);

        menu.push({
            label: label,
            action: function (e) {
                e.stopPropagation();
                column.toggle();
                icon.classList.toggle("fa-check-square", column.isVisible());
                icon.classList.toggle("fa-square", !column.isVisible());
            }
        });
    });

    return menu;
};

function delConfirm(Id) {
    console.log(Id);
    PNotify.prototype.options.styling = "bootstrap3";
    (new PNotify({
        title: 'Confirm Deletion',
        text: 'Are you sure you want to delete?',
        icon: 'fa fa-question-circle',
        hide: false,
        confirm: { confirm: true },
        buttons: { closer: false, sticker: false },
        history: { history: false }
    })).get().on('pnotify.confirm', function () {
        $.ajax({
            url: '/Service/Delete',
            type: 'POST',
            data: { id: Id },
            success: function (data) {
                if (data.success) {
                    showSuccessAlert("Deleted successfully.");
                    setTimeout(() => window.location.reload(), 1500);
                } else {
                    showDangerAlert(data.message || "Deletion failed.");
                }
            },
            error: function () {
                showDangerAlert('Error occurred during deletion.');
            }
        });
    });
}

function BlankComplaintDumpDown() {
    Blockloadershow();

    var expectedColumns = [
        'CCCN Date', 'Reported By', 'Location', 'Customer Name', 'Dealer Name', 'Description', 'Status', 'Completion', 'ClosureRemarks'
    ];

    // Create worksheet with only the header row
    var data = [expectedColumns];
    var ws = XLSX.utils.aoa_to_sheet(data);

    // Apply bold style to header cells
    expectedColumns.forEach((col, index) => {
        const cellRef = XLSX.utils.encode_cell({ c: index, r: 0 }); // r: 0 => first row
        if (!ws[cellRef]) return;
        ws[cellRef].s = {
            font: {
                bold: true
            }
        };
    });

    // Auto-width calculation
    const columnWidths = expectedColumns.map(col => ({ wch: col.length + 2 }));
    ws['!cols'] = columnWidths;


    // Create workbook and export
    var wb = XLSX.utils.book_new();
    XLSX.utils.book_append_sheet(wb, ws, "Complaint Dump Temaplate");

    XLSX.writeFile(wb, "Complaint_Dump_Temaplate.xlsx");

    Blockloaderhide();
};

function openUpload() {

    clearForm();
    if (!$('#uploadModal').length) {
        $('body').append(partialView);
    }
    $('#uploadModal').modal('show');
}

function clearForm() {
    // Clear all input fields
    document.querySelectorAll('.form-control').forEach(function (input) {
        if (input.tagName === 'INPUT') {
            if (input.type === 'hidden' || input.readOnly) {
                // Skip hidden or readonly inputs
                return;
            }
            input.value = ''; // Clear input value
        } else if (input.tagName === 'SELECT') {
            input.selectedIndex = 0; // Reset dropdown to first option
        }
    });

    // Clear error messages if needed
    document.querySelectorAll('.text-danger').forEach(function (error) {
        error.textContent = '';
    });
}
