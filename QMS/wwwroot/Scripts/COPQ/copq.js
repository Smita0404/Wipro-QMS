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

    loadData();
});

function loadData() {
    Blockloadershow();

    $.ajax({
        url: '/COPQ/GetAll',
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

//function renderCOPQTable(response) {
//    if (!Array.isArray(response)) {
//        console.error("Invalid response, expected array:", response);
//        showDangerAlert("Invalid data received.");
//        Blockloaderhide();
//        return;
//    }

//    Blockloadershow();

//    const tabledata = response.map((item, index) => ({
//        Sr_No: index + 1,
//        Id: item.id,
//        CCCNDate: item.cccnDate ? new Date(item.cccnDate).toLocaleDateString("en-GB") : "",
//        ReportedBy: item.reportedBy,
//        CLocation: item.cLocation,
//        CustName: item.custName,
//        DealerName: item.dealerName,
//        CDescription: item.cDescription,
//        CStatus: item.cStatus,
//        Completion: item.completion,
//        Remarks: item.remarks,
//        CreatedDate: item.createdDate ? new Date(item.createdDate).toLocaleDateString("en-GB") : "",
//        CreatedBy: item.createdBy,
//        UpdatedDate: item.updatedDate ? new Date(item.updatedDate).toLocaleDateString("en-GB") : "",
//        UpdatedBy: item.updatedBy,
//        IsDeleted: item.isDeleted
//    }));

//    const columns = [
//        {
//            title: "Action",
//            field: "Action",
//            hozAlign: "center",
//            headerHozAlign: "center",
//            frozen: true,
//            formatter: function (cell) {
//                const rowData = cell.getRow().getData();
//                return `<i onclick="delConfirm(${rowData.Id})" class="fas fa-trash-alt mr-2 fa-1x" title="Delete" style="color:red;cursor:pointer;margin-left: 5px;"></i>`;
//            }
//        },
//        { title: "SNo", field: "Sr_No", sorter: "number", hozAlign: "center", headerHozAlign: "center", frozen: true },
//        editableColumn("CCCN Date", "CCCNDate", "date", "center"),
//        editableColumn("Reported By", "ReportedBy"),
//        editableColumn("Location", "CLocation"),
//        editableColumn("Customer Name", "CustName"),
//        editableColumn("Dealer Name", "DealerName"),
//        editableColumn("Description", "CDescription"),
//        editableColumn("Status", "CStatus"),
//        editableColumn("Completion", "Completion"),
//        editableColumn("Remarks", "Remarks"),
//        { title: "Created By", field: "CreatedBy", visible: false },
//        { title: "Created Date", field: "CreatedDate", visible: false },
//        { title: "Updated By", field: "UpdatedBy", visible: false },
//        { title: "Updated Date", field: "UpdatedDate", visible: false }
//    ];

//    if (typeof table !== 'undefined' && table instanceof Tabulator) {
//        table.replaceData(tabledata);
//    } else {
//        table = new Tabulator("#copq-table", {
//            data: tabledata,
//            layout: "fitDataFill",
//            movableColumns: true,
//            pagination: "local",
//            paginationSize: 10,
//            paginationSizeSelector: [10, 50, 100, 500],
//            paginationCounter: "rows",
//            placeholder: "No data available",
//            columns: columns
//        });

//        table.on("cellEdited", function (cell) {
//            saveEditedRow(cell.getRow().getData());
//        });

//        $("#addButton").off("click").on("click", function () {
//            const newRow = {
//                Id: 0,
//                Sr_No: table.getDataCount() + 1,
//                CCCNDate: "",
//                ReportedBy: "",
//                CLocation: "",
//                CustName: "",
//                DealerName: "",
//                CDescription: "",
//                CStatus: "",
//                Completion: "",
//                Remarks: ""
//            };
//            table.addRow(newRow, false);
//        });
//    }

//    Blockloaderhide();
//}

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
    const url = isNew ? '/COPQ/CreateAsync' : '/COPQ/UpdateAsync';

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
            url: '/COPQ/Delete',
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
