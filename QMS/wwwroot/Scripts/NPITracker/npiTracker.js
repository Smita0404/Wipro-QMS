var tabledata = [];
var table = '';
const searchTerms = {};
let vendorOptions = {}; 
$(document).ready(function () {

    document.addEventListener('DOMContentLoaded', function () {
        document.getElementById('backButton').addEventListener('click', function () {
            window.history.back();
        });
    });

    loadData();
});

function loadData() {
    Blockloadershow();

    $.ajax({
        url: '/NPITrac/GetVendor',
        type: 'GET',
        success: function (vendorData) {
            if (Array.isArray(vendorData)) {
                vendorOptions = vendorData.reduce((acc, v) => {
                    acc[v.value] = v.label;
                    return acc;
                }, {});
            }

            // ✅ Call GetAll *after* vendorOptions are ready
            $.ajax({
                url: '/NPITrac/GetAll',
                type: 'GET',
                success: function (data) {
                    if (data && Array.isArray(data)) {
                        OnTabGridLoad(data); // Pass only when vendorOptions is ready
                    } else {
                        showDangerAlert('No data available to load.');
                    }
                    Blockloaderhide();
                },
                error: function (xhr, status, error) {
                    showDangerAlert('Error retrieving data: ' + error);
                    Blockloaderhide();
                }
            });
        },
        error: function () {
            showDangerAlert('Error loading vendor list.');
        }
    });
}


// Register custom select2 editor
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


//define column header menu as column visibility toggle
var headerMenu = function () {
    var menu = [];
    var columns = this.getColumns();

    for (let column of columns) {

        //create checkbox element using font awesome icons
        let icon = document.createElement("i");
        icon.classList.add("fas");
        icon.classList.add(column.isVisible() ? "fa-check-square" : "fa-square");

        //build label
        let label = document.createElement("span");
        let title = document.createElement("span");

        title.textContent = " " + column.getDefinition().title;

        label.appendChild(icon);
        label.appendChild(title);

        //create menu item
        menu.push({
            label: label,
            action: function (e) {
                //prevent menu closing
                e.stopPropagation();

                //toggle current column visibility
                column.toggle();

                //change menu item icon
                if (column.isVisible()) {
                    icon.classList.remove("fa-square");
                    icon.classList.add("fa-check-square");
                } else {
                    icon.classList.remove("fa-check-square");
                    icon.classList.add("fa-square");
                }
            }
        });
    }

    return menu;
};

// Reusable column setup
function editableColumn(title, field, editorType = true, align = "center", headerFilterType = "input", headerFilterParams = {}, editorParams = {}, formatter = null) {
    return {
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
}


function OnTabGridLoad(response) {
    debugger;
    Blockloadershow();

    tabledata = [];
    let columns = [];

    // Map the response to the table format
    if (response.length > 0) {
        $.each(response, function (index, item) {

            function formatDate(value) {
                return value ? new Date(value).toLocaleDateString("en-GB") : "";
            }

            //formatDate(item.pO_Date),

            tabledata.push({
                Sr_No: index + 1,
                Id: item.id,
                PC: item.PC,
                Vendor: item.Vendor,
                Prod_Category: item.Prod_Category,
                Product_Code: item.Product_Code,
                Product_Des: item.Product_Des,
                Wattage: item.Wattage,
                NPI_Category: item.NPI_Category,
                Offered_Date: formatDate(item.Offered_Date),
                Released_Date: formatDate(item.Released_Date),
                Releasded_Day: item.Releasded_Day,
                Validation_Rep_No: item.Validation_Rep_No,
                Customer_Comp: item.Customer_Comp,
                Remark: item.Remark,
                CreatedBy: item.createdBy,
                UpdatedBy: item.updatedBy,
                UpdatedDate: formatDate(item.updatedDate),
                CreatedDate: formatDate(item.createdDate),
            });
        });

    }

    columns.push(
        {
            title: "Action",
            field: "Action",
            // width: 130,
            headerMenu: headerMenu,
            hozAlign: "center",
            headerHozAlign: "center",
            formatter: function (cell, formatterParams) {
                const rowData = cell.getRow().getData();
                let actionButtons = "";

                actionButtons += `<i data-toggle="modal" onclick="delConfirm(${rowData.Id})" class="fas fa-trash-alt mr-2 fa-1x" title="Delete" style="color:red;cursor:pointer;margin-left: 5px;"></i>`

                return actionButtons;
            }
        },
        {
            title: "SNo", field: "Sr_No", sorter: "number", headerMenu: headerMenu, hozAlign: "center", headerHozAlign: "left"
        },
        editableColumn("PC", "PC"),
        editableColumn("Vendor", "Vendor", "select2", "center", "input", {}, {
            values: vendorOptions
        }, function (cell) {
            // Show Vendor Name in the cell, not the code
            const code = cell.getValue();
            return vendorOptions[code] || code;
        }),
        editableColumn("Prod Category", "Prod_Category", "select", "center", "select", {
            values: {
                "Indoor": "Indoor",
                "Outdoor": "Outdoor"
            }
        }, {
            values: {
                "Indoor": "Indoor",
                "Outdoor": "Outdoor"
            }
        }),
        editableColumn("Product Code", "Product_Code"),
        editableColumn("Product Description", "Product_Des"),
        editableColumn("Wattage", "Wattage"),
        //editableColumn("NPI Category", "NPI_Category"),
        editableColumn("NPI Category", "NPI_Category", "select", "center", "select", {
            values: {
                "Make": "Make",
                "Buy": "Buy"
            }
        }, {
            values: {
                "Make": "Make",
                "Buy": "Buy"
            }
        }),
        editableColumn("Offered Date", "Offered_Date", "date", "center", "input", {}, {}, function (cell) {
            // Optional: display formatted date
            const value = cell.getValue();
            return value ? new Date(value).toLocaleDateString("en-GB") : "";
        }),
        //editableColumn("Offered Date", "Offered_Date", "input", "left"),
        editableColumn("Released Date", "Released_Date", "date", "center", "input", {}, {}, function (cell) {
            // Optional: display formatted date
            const value = cell.getValue();
            return value ? new Date(value).toLocaleDateString("en-GB") : "";
        }),
        //editableColumn("Released Date", "Released_Date", "input", "left"),
        editableColumn("Released Day", "Releasded_Day"),
        editableColumn("Validation Rep No", "Validation_Rep_No"),
        editableColumn("Customer Comp", "Customer_Comp"),
        editableColumn("Remark", "Remark"),
        editableColumn("Created By", "CreatedBy"),
        editableColumn("Updated By", "UpdatedBy"),
        editableColumn("Updated Date", "UpdatedDate", "input", "left"),
        editableColumn("Created Date", "CreatedDate", "input", "left")
    );

    // // Initialize Tabulator
    table = new Tabulator("#npi_Table", {
        data: tabledata,
        renderHorizontal: "virtual",
        movableColumns: true,
        pagination: "local",
        paginationSize: 10,
        paginationSizeSelector: [50, 100, 500, 1500, 2000],
        paginationCounter: "rows",
        dataEmpty: "<div style='text-align: center; font-size: 1rem; color: gray;'>No data available</div>", // Placeholder message
        columns: columns,

        cellEdited: function (cell) {
            const rowData = cell.getRow().getData();
            saveEditedRow(rowData); // Auto-save on edit
        }
    });

    //table.on("cellClick", function (e, cell) {
    //    let columnField = cell.getColumn().getField();

    //    if (columnField !== "Action") {
    //        let rowData = cell.getRow().getData();
    //        showEditBisProject(rowData.Id);
    //    }
    //});

    // Export to Excel on button click
    // document.getElementById("exportExcel").addEventListener("click", function () {
    //     table.download("xlsx", "ProductCode_Data.xlsx", { sheetName: "Product Code Data" });
    // });

    $("#addButton").on("click", function () {
        const newRow = {
            Id: 0,
            Sr_No: table.getDataCount() + 1,
            PC: "", Vendor: "", Prod_Category: "", Product_Code: "", Product_Des: "", Wattage: "",
            NPI_Category: "", Offered_Date: "", Released_Date: "", Releasded_Day: "", Validation_Rep_No: "",
            Customer_Comp: "", Remark: "", CreatedBy: "", UpdatedBy: "", UpdatedDate: "", CreatedDate: ""
        };
        table.addRow(newRow, false); // false = add to bottom
    });

    Blockloaderhide();
}


function delConfirm(recid) {
    debugger;
    PNotify.prototype.options.styling = "bootstrap3";
    (new PNotify({
        title: 'Confirmation Needed',
        text: 'Are you sure to delete? It will not delete if this record is used in transactions.',
        icon: 'glyphicon glyphicon-question-sign',
        hide: false,
        confirm: {
            confirm: true
        },
        buttons: {
            closer: false,
            sticker: false
        },
        history: {
            history: false
        },
    })).get().on('pnotify.confirm', function () {
        $.ajax({
            url: '/BisProjectTrac/Delete',
            type: 'POST',
            data: { id: recid },
            success: function (data) {
                if (data.success == true) {
                    showSuccessAlert("Bis Projecet Deleted successfully.");
                    setTimeout(function () {
                        window.location.reload();
                    }, 2500);
                }
                else if (data.success == false && data.message == "Not_Deleted") {
                    showDangerAlert("Record is used in QMS Log transactions.");
                }
                else {
                    showDangerAlert(data.message);
                }
            },
            error: function () {
                showDangerAlert('Error retrieving data.');
            }
        });
    }).on('pnotify.cancel', function () {
        loadData();
    });
}

