let filterStartDateIndent = moment().startOf('month').format('YYYY-MM-DD');
let filterEndDateIndent = moment().endOf('month').format('YYYY-MM-DD');
let tableIndent = null;

$(document).ready(function () {

    $('#IndentdateRangeText').text(
        moment(filterStartDateIndent).format('MMMM D, YYYY') + ' - ' + moment(filterEndDateIndent).format('MMMM D, YYYY')
    );

    const picker = new Litepicker({
        element: document.getElementById('customDateTriggerIndent'),
        singleMode: false,
        format: 'DD-MM-YYYY',
        numberOfMonths: 2,
        numberOfColumns: 2,
        dropdowns: { minYear: 2020, maxYear: null, months: true, years: true },
        plugins: ['ranges'],
        setup: (picker) => {
            picker.on('selected', (start, end) => {
                filterStartDateIndent = start.format('YYYY-MM-DD');
                filterEndDateIndent = end.format('YYYY-MM-DD');
                $('#IndentdateRangeText').text(`${start.format('MMMM D, YYYY')} - ${end.format('MMMM D, YYYY')}`);
                loadDatapo();
            });
            picker.on('clear', () => {
                filterStartDateIndent = "";
                filterEndDateIndent = "";
                $('#IndentdateRangeText').text("Select Date Range");
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

    $('#customDateTriggerIndent').on('click', function () {
        picker.show();
    });

    document.getElementById('backButton').addEventListener('click', function () {
        window.history.back();
    });

    $('#upload-button').on('click', async function () {
        var expectedColumns = [
            'Indent No', 'Indent Date', 'Business Unit', 'Vertical', 'Branch', 'Indent Status', 'End Customer Name', 'Complaint Id', 'Customer Code', 'Customer Name', 'Bill Request Date',
            'Created By', 'Wipro Commit Date', 'Material No', 'Item Description', 'Quantity', 'Price', 'Final Price', 'SAPSO No', 'Create SoQty', 'Inv_qnty', 'Inv_value', 'Wipro Catelog No',
            'Batch Code', 'Batch Date', 'MainProd Code', 'User Name'
        ];

        var url = '/Service/UploadPoDumpExcel';
        handleImportExcelFile(url, expectedColumns);
    });

    loadIndentData();
});

function loadIndentData() {
    Blockloadershow();

    $.ajax({
        url: '/Service/GetAllIndent',
        type: 'GET',
        dataType: 'json',
        data: {
            startDate: filterStartDateIndent,
            endDate: filterEndDateIndent
        },
        success: function (data) {
            if (data.success && Array.isArray(data.data)) {
                renderIndentTable(data.data);
            } else {
                showDangerAlert('No data available to load.');
                renderIndentTable([]); // avoid errors
            }
        },
        error: function (xhr, status, error) {
            showDangerAlert('Error retrieving data: ' + error);
            Blockloaderhide();
        }
    });

}

function renderIndentTable(response) {
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
            Indent_No : item.indent_No,
            Indent_Date : item.indent_Date ? new Date(item.indent_Date).toLocaleDateString("en-GB") : "",
            Business_Unit : item.business_Unit,
            Vertical : item.vertical,
            Branch : item.branch,
            Indent_Status : item.indent_Status,
            End_Cust_Name : item.end_Cust_Name,
            Complaint_Id : item.complaint_Id,
            Customer_Code : item.customer_Code,
            Customer_Name : item.customer_Name,
            Bill_Req_Date : item.bill_Req_Date ? new Date(item.bill_Req_Date).toLocaleDateString("en-GB") : "",
            Created_By : item.created_By,
            Wipro_Commit_Date : item.wipro_Commit_Date ? new Date(item.wipro_Commit_Date).toLocaleDateString("en-GB") : "",
            Material_No : item.material_No,
            Item_Description : item.item_Description,
            Quantity : item.quantity,
            Price : item.price,
            Final_Price : item.final_Price,
            SapSoNo : item.sapSoNo,
            CreateSoQty : item.createSoQty,
            Inv_Qty : item.inv_Qty,
            Inv_Value : item.inv_Value,
            WiproCatelog_No : item.wiproCatelog_No,
            Batch_Code : item.batch_Code,
            Batch_Date : item.batch_Date ? new Date(item.batch_Date).toLocaleDateString("en-GB") : "",
            Main_Prodcode : item.main_Prodcode,
            User_Name : item.user_Name,
            CreatedDate: item.createdDate ? new Date(item.createdDate).toLocaleDateString("en-GB") : "",
            CreatedBy: item.createdBy,
            UpdatedDate: item.updatedDate ? new Date(item.updatedDate).toLocaleDateString("en-GB") : "",
            UpdatedBy: item.updatedBy,
            IsDeleted: item.isDeleted
        });
    });

    console.log(tabledata);
    if (tabledata.length === 0 && tableIndent) {
        tableIndent.clearData();
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
                return `<i onclick="delConfirmIndent(${rowData.Id})" class="fas fa-trash-alt mr-2 fa-1x" title="Delete" style="color:red;cursor:pointer;margin-left: 5px;"></i>`;
            }
        },
        { title: "SNo", field: "Sr_No", frozen: true, sorter: "number", headerMenu: headerMenu, hozAlign: "center", headerHozAlign: "center" },
        editableColumn("Indent No", "Indent_No"),
        editableColumn("Indent Date", "Indent_Date", "date", "center"),
        editableColumn("Business Unit", "Business_Unit"),
        editableColumn("Vertical", "Vertical"),
        editableColumn("Branch", "Branch"),
        editableColumn("Indent Status", "Indent_Status"),
        editableColumn("End Customer Name", "End_Cust_Name"),
        editableColumn("Complaint Id", "Complaint_Id"),
        editableColumn("Customer Code", "Customer_Code"),
        editableColumn("Customer Name", "Customer_Name"),
        editableColumn("Bill Request Date", "Bill_Req_Date", "date", "center"),
        editableColumn("Created_By", "Created_By"),
        editableColumn("Wipro Commit Date", "Wipro_Commit_Date", "date", "center"),
        editableColumn("Material No", "Material_No"),
        editableColumn("Item Description", "Item_Description"),
        editableColumn("Quantity", "Quantity"),
        editableColumn("Material No", "Material_No"),
        editableColumn("Price", "Price"),
        editableColumn("Final Price", "Final_Price"),
        editableColumn("SAPSO No", "SapSoNo"),
        editableColumn("Create SoQty", "CreateSoQty"),
        editableColumn("Inv Qty", "Inv_Qty"),
        editableColumn("Inv Value", "Inv_Value"),
        editableColumn("WiproCatelog No", "WiproCatelog_No"),
        editableColumn("Batch Code", "Batch_Code"),
        editableColumn("Batch Date", "Batch_Date"),
        editableColumn("User Name", "User_Name"),
        { title: "Created By", field: "CreatedBy", visible: false, headerMenu: headerMenu, hozAlign: "center" },
        { title: "Created Date", field: "CreatedDate", visible: false, headerMenu: headerMenu, hozAlign: "center" },
        { title: "Updated By", field: "UpdatedBy", visible: false, headerMenu: headerMenu, hozAlign: "center" },
        { title: "Updated Date", field: "UpdatedDate", visible: false, headerMenu: headerMenu, hozAlign: "center" }
    ];

    if (tableIndent) {
        tableIndent.replaceData(tabledata);
    } else {
        tableIndent = new Tabulator("#indent-table", {
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

        tableIndent.on("cellEdited", function (cell) {
            InsertUpdateIndentDump(cell.getRow().getData());
        });

        $("#addIndentButton").on("click", function () {
            const newRow = {
                Id: 0,
                Sr_No: tableIndent.getDataCount() + 1,
                Indent_No     : "",
                Indent_Date   : "",
                Business_Unit : "",
                Vertical      : "",
                Branch        : "",
                Indent_Status : "",
                End_Cust_Name : "",
                Complaint_Id  : "",
                Customer_Code : "",
                Customer_Name : "",
                Bill_Req_Date : "",
                Created_By        : "",
                Wipro_Commit_Date : "",
                Material_No       : "",
                Item_Description  : "",
                Quantity          : "",
                Price             : "",
                Final_Price       : "",
                SapSoNo           : "",
                CreateSoQty       : "",
                Inv_Qty           : "",
                Inv_Value         : "",
                WiproCatelog_No   : "",
                Batch_Code        : "",
                Batch_Date        : "",
                Main_Prodcode     : "",
                User_Name         : "",
                CreatedBy: "",
                CreatedDate: "",
                UpdatedBy: "",
                UpdatedDate: ""
            };
            tableIndent.addRow(newRow, false);
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

function InsertUpdateIndentDump(rowData) {

    function toIsoDate(value) {
        if (!value) return "";
        const parts = value.split('/');
        return parts.length === 3 ? `${parts[2]}-${parts[1].padStart(2, '0')}-${parts[0].padStart(2, '0')}` : value;
    }

    const cleanedData = {
        Id: rowData.Id || 0,
        PODate: toIsoDate(rowData.PODate),
        Vendor: rowData.Vendor || null,
        Indent_No: rowData.Vendor || null,
        Indent_Date: rowData.Vendor || null,
        Business_Unit: rowData.Vendor || null,
        Vertical: rowData.Vendor || null,
        Branch: rowData.Vendor || null,
        Indent_Status: rowData.Vendor || null,
        End_Cust_Name: rowData.Vendor || null,
        Complaint_Id: rowData.Vendor || null,
        Customer_Code: rowData.Vendor || null,
        Customer_Name: rowData.Vendor || null,
        Bill_Req_Date: rowData.Vendor || null,
        Created_By: rowData.Vendor || null,
        Wipro_Commit_Date: rowData.Vendor || null,
        Material_No: rowData.Vendor || null,
        Item_Description: rowData.Vendor || null,
        Quantity: rowData.Vendor || "",
        Price: rowData.Vendor || null,
        Final_Price: rowData.Vendor || null,
        SapSoNo: rowData.Vendor || null,
        CreateSoQty: rowData.Vendor || "",
        Inv_Qty: rowData.Vendor || "",
        Inv_Value: rowData.Vendor || null,
        WiproCatelog_No: rowData.Vendor || null,
        Batch_Code: rowData.Vendor || null,
        Batch_Date: rowData.Vendor || null,
        Main_Prodcode: rowData.Vendor || null,
        User_Name: rowData.Vendor || null
    };
    console.log(cleanedData);
    const isNew = cleanedData.Id === 0;
    const url = isNew ? '/Service/CreateIndent' : '/Service/UpdateIndent';

    $.ajax({
        url: url,
        type: 'POST',
        data: JSON.stringify(cleanedData),
        contentType: 'application/json',
        success: function (data) {
            if (data.success) {
                loadIndentData();
            } else {
                showDangerAlert(data.message || (isNew ? "Create failed." : "Update failed."));
            }
        },
        error: function (xhr) {
            showDangerAlert(xhr.responseText || "Error saving Indent Dump record.");
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

function delConfirmIndent(Id) {
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
            url: '/Service/IndentDelete',
            type: 'POST',
            data: { id: Id },
            success: function (data) {
                if (data.success) {
                    showSuccessAlert("Deleted successfully.");
                    setTimeout(() => loadIndentData(), 1000);
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

function BlankIndentDumpDown() {
    Blockloadershow();

    var expectedColumns = [
        'Indent No', 'Indent Date', 'Business Unit', 'Vertical', 'Branch', 'Indent Status', 'End Customer Name', 'Complaint Id', 'Customer Code', 'Customer Name', 'Bill Request Date',
        'Created By', 'Wipro Commit Date', 'Material No', 'Item Description', 'Quantity', 'Price', 'Final Price', 'SAPSO No', 'Create SoQty', 'Inv_qnty', 'Inv_value','Wipro Catelog No',
        'Batch Code', 'Batch Date', 'MainProd Code', 'User Name'
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
    XLSX.utils.book_append_sheet(wb, ws, "PO Dump Temaplate");

    XLSX.writeFile(wb, "PO_Dump_Temaplate.xlsx");

    Blockloaderhide();
};

function openIndentUpload() {
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