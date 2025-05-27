let filterStartDatePO = moment().startOf('month').format('YYYY-MM-DD');
let filterEndDatePO = moment().endOf('month').format('YYYY-MM-DD');
let tablePO = null;
let vendorOptions = {};
$(document).ready(function () {

    $('#POdateRangeText').text(
        moment(filterStartDatePO).format('MMMM D, YYYY') + ' - ' + moment(filterEndDatePO).format('MMMM D, YYYY')
    );

    const picker = new Litepicker({
        element: document.getElementById('customDateTriggerPO'),
        singleMode: false,
        format: 'DD-MM-YYYY',
        numberOfMonths: 2,
        numberOfColumns: 2,
        dropdowns: { minYear: 2020, maxYear: null, months: true, years: true },
        plugins: ['ranges'],
        setup: (picker) => {
            picker.on('selected', (start, end) => {
                filterStartDatePO = start.format('YYYY-MM-DD');
                filterEndDatePO = end.format('YYYY-MM-DD');
                $('#POdateRangeText').text(`${start.format('MMMM D, YYYY')} - ${end.format('MMMM D, YYYY')}`);
                loadDatapo();
            });
            picker.on('clear', () => {
                filterStartDatePO = "";
                filterEndDatePO = "";
                $('#POdateRangeText').text("Select Date Range");
                loadDatapo();
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

    $('#customDateTriggerPO').on('click', function () {
        picker.show();
    });

    document.getElementById('backButton').addEventListener('click', function () {
        window.history.back();
    });

    loadDatapo();
});

function loadDatapo() {
    Blockloadershow();

    $.ajax({
        url: '/COPQ/GetVendor',
        type: 'GET'
    }).done(function (vendorData) {
        if (Array.isArray(vendorData)) {
            vendorOptions = vendorData.reduce((acc, v) => {
                acc[v.value] = v.label;
                return acc;
            }, {});
        }

        $.ajax({
            url: '/COPQ/GetAllPO',
            type: 'GET',
            dataType: 'json',
            data: {
                startDate: filterStartDatePO,
                endDate: filterEndDatePO
            },
            success: function (data) {
                if (data.success && Array.isArray(data.data)) {
                    renderTable(data.data);
                } else {
                    showDangerAlert('No data available to load.');
                    renderTable([]); // avoid errors
                }
            },
            error: function (xhr, status, error) {
                showDangerAlert('Error retrieving data: ' + error);
                Blockloaderhide();
            }
        });

    }); // <- this was missing
}


function renderTable(response) {
    if (!Array.isArray(response)) {
        console.error("Invalid response, expected array:", response);
        showDangerAlert("Invalid data received. Expected array.");
        Blockloaderhide();
        return;
    }

    Blockloadershow();

    let tabledata = [];

    $.each(response, function (index, item) {
        tabledata.push({
            Sr_No: index + 1,
            Id: item.id,
            Vendor: item.vendor,
            Material: item.material,
            ReferenceNo: item.referenceNo,
            PONo: item.poNo,
            PODate: item.poDate ? new Date(item.poDate).toLocaleDateString("en-GB") : "",
            PRNo: item.prNo,
            BatchNo: item.batchNo,
            POQty: item.poQty,
            BalanceQty: item.balanceQty,
            Destination: item.destination,
            BalanceValue: item.balanceValue,
            CreatedDate: item.createdDate ? new Date(item.createdDate).toLocaleDateString("en-GB") : "",
            CreatedBy: item.createdBy,
            UpdatedDate: item.updatedDate ? new Date(item.updatedDate).toLocaleDateString("en-GB") : "",
            UpdatedBy: item.updatedBy,
            IsDeleted: item.isDeleted
        });
    });
    console.log(tabledata);
    if (tabledata.length === 0 && tablePO) {
        tablePO.clearData();
        Blockloaderhide();
        return;
    }

    // Define your columns here or outside renderTable if reused
    const columns = [
        {
            title: "Action",
            field: "Action",
            hozAlign: "center",
            headerHozAlign: "center",
            frozen: true, headerMenu: headerMenu,
            formatter: function (cell) {
                const rowData = cell.getRow().getData();
                return `<i onclick="delConfirmPO(${rowData.Id})" class="fas fa-trash-alt mr-2 fa-1x" title="Delete" style="color:red;cursor:pointer;margin-left: 5px;"></i>`;
            }
        },
        { title: "SNo", field: "Sr_No", frozen: true, sorter: "number", headerMenu: headerMenu, hozAlign: "center", headerHozAlign: "center" },
        editableColumn("PO Date", "PODate", "date", "center"),
        editableColumn("Vendor", "Vendor", "select2", "center", "input", {}, {
            values: vendorOptions
        }, function (cell) {
            const val = cell.getValue();
            return vendorOptions[val] || val;
        }, 130),
       // editableColumn("Vendor", "Vendor"),
        editableColumn("Material", "Material"),
        editableColumn("Reference No", "ReferenceNo"),
        editableColumn("PO No", "PONo"),
        editableColumn("PR No", "PRNo"),
        editableColumn("Batch No", "BatchNo"),
        editableColumn("PO Qty", "POQty", "input", "center"),
        editableColumn("Balance Qty", "BalanceQty", "input", "center"),
        editableColumn("Destination", "Destination"),
        editableColumn("Balance Value", "BalanceValue", "input", "center"),
        { title: "Created By", field: "CreatedBy", visible: false, headerMenu: headerMenu, hozAlign: "center" },
        { title: "Created Date", field: "CreatedDate", visible: false, headerMenu: headerMenu, hozAlign: "center" },
        { title: "Updated By", field: "UpdatedBy", visible: false, headerMenu: headerMenu, hozAlign: "center" },
        { title: "Updated Date", field: "UpdatedDate", visible: false, headerMenu: headerMenu, hozAlign: "center" }
    ];

    if (tablePO) {
        tablePO.replaceData(tabledata);
    } else {
        tablePO = new Tabulator("#po-table", {
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

        tablePO.on("cellEdited", function (cell) {
            saveEditedRowPO(cell.getRow().getData());
        });

        $("#addPOButton").on("click", function () {
            const newRow = {
                Id: 0,
                Sr_No: tablePO.getDataCount() + 1,
                PODate: "",
                Vendor: "",
                Material: "",
                ReferenceNo: "",
                PONo: "",
              
                PRNo: "",
                BatchNo: "",
                POQty: 0,
                BalanceQty: 0,
                Destination: "",
                BalanceValue: 0,
                CreatedBy: "",
                CreatedDate: "",
                UpdatedBy: "",
                UpdatedDate: ""
            };
            tablePO.addRow(newRow, false);
        });
    }

    Blockloaderhide();
}


function editableColumn(title, field, editorType = true, align = "center", headerFilterType = "input", headerFilterParams = {}, editorParams = {}, formatter = null) {
    let columnDef = {
        title: title,
        field: field,
        editor: editorType,
        editorParams: editorParams,
        formatter: formatter,
        headerFilter: headerFilterType,
        headerFilterParams: headerFilterParams,
        headerMenu: headerMenu,
        hozAlign: align,
        headerHozAlign: "left"
    };

    

    return columnDef;
}

function saveEditedRowPO(rowData) {
    

    function toIsoDate(value) {
        if (!value) return "";
        const parts = value.split('/');
        return parts.length === 3 ? `${parts[2]}-${parts[1].padStart(2, '0')}-${parts[0].padStart(2, '0')}` : value;
    }
    console.log(rowData);
    const cleanedData = {
        Id: rowData.Id || 0,
        PODate: toIsoDate(rowData.PODate),
        Vendor: rowData.Vendor|| null,
        Material: rowData.Material || null,
        ReferenceNo: rowData.ReferenceNo || null,
        PONo: rowData.PONo || null,
        PRNo: rowData.PRNo || null,
        BatchNo: rowData.BatchNo || null,
        POQty: rowData.POQty || null,
        BalanceQty: rowData.BalanceQty || null,
        Destination: rowData.Destination || null,
        BalanceValue: rowData.BalanceValue || null
    };
    console.log(cleanedData);
    const isNew = cleanedData.Id === 0;
    const url = isNew ? '/COPQ/CreatePO' : '/COPQ/UpdatePO';

    $.ajax({
        url: url,
        type: 'POST',
        data: JSON.stringify(cleanedData),
        contentType: 'application/json',
        success: function (data) {
            if (data.success) {
                loadDatapo();
            } else {
                showDangerAlert(data.message || (isNew ? "Create failed." : "Update failed."));
            }
        },
        error: function (xhr) {
            showDangerAlert(xhr.responseText || "Error saving PO record.");
        }
    });
}

var headerMenu = function () {
    var menu = [];
    var columns = this.getColumns();

    columns.forEach(column => {
        let icon = document.createElement("i");
        icon.classList.add("fas", column.isVisible() ? "fa-check-square" : "fa-square");

        let label = document.createElement("span");
        let title = document.createElement("span");
        title.textContent = " " + column.getDefinition().title;

        label.appendChild(icon);
        label.appendChild(title);

        menu.push({
            label: label,
            action: function (e) {
                e.stopPropagation();
                column.toggle();
                icon.classList.toggle("fa-square", column.isVisible() === false);
                icon.classList.toggle("fa-check-square", column.isVisible() === true);
            }
        });
    });

    return menu;
};

function delConfirmPO(Id) {
    PNotify.prototype.options.styling = "bootstrap3";
    new PNotify({
        title: 'Confirm Deletion',
        text: 'Are you sure you want to delete?',
        icon: 'fa fa-question-circle',
        hide: false,
        confirm: { confirm: true },
        buttons: { closer: false, sticker: false },
        history: { history: false }
    }).get().on('pnotify.confirm', function () {
        $.ajax({
            url: '/COPQ/PODelete',
            type: 'POST',
            data: { id: Id },
            success: function (data) {
                if (data.success) {
                    showSuccessAlert("Deleted successfully.");
                    setTimeout(() => loadDatapo(), 1000);
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
Tabulator.extendModule("edit", "editors", {
    select2: function (cell, onRendered, success, cancel, editorParams) {
        const values = editorParams.values || {};
        const select = document.createElement("select");
        select.style.width = "100%";

        for (let val in values) {
            let option = document.createElement("option");
            option.value = val;
            option.text = values[val];
            if (val === cell.getValue()) option.selected = true;
            select.appendChild(option);
        }

        onRendered(function () {
            $(select).select2({
                dropdownParent: document.body,
                width: 'resolve',
                placeholder: "Select value"
            }).on("change", function () {
                success(select.value);
            });
        });

        return select;
    }
});
