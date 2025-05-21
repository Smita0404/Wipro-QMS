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

Tabulator.extendModule("edit", "editors", {
    autocomplete_ajax: function (cell, onRendered, success, cancel, editorParams) {
        const input = document.createElement("input");
        input.setAttribute("type", "text");
        input.style.width = "100%";
        input.value = cell.getValue() || "";

        let dropdown = null;

        function removeDropdown() {
            if (dropdown && dropdown.parentNode) {
                dropdown.parentNode.removeChild(dropdown);
                dropdown = null;
            }
        }

        function fetchSuggestions(query) {
            $.ajax({
                url: '/NPITrac/GetCodeSearch',
                type: 'GET',
                data: { search: query },
                success: function (data) {
                    removeDropdown(); // clear old

                    dropdown = document.createElement("div");
                    dropdown.className = "autocomplete-dropdown";

                    data.forEach(item => {
                        const option = document.createElement("div");
                        option.textContent = item.oldPart_No;
                        option.className = "autocomplete-option";

                        option.addEventListener("mousedown", function (e) {
                            e.stopPropagation();
                            success(item.oldPart_No);

                            const row = cell.getRow();
                            row.update({ Product_Des: item.description });
                            removeDropdown();
                        });

                        dropdown.appendChild(option);
                    });

                    document.body.appendChild(dropdown);
                    const rect = input.getBoundingClientRect();
                    dropdown.style.top = (window.scrollY + rect.bottom) + "px";
                    dropdown.style.left = (window.scrollX + rect.left) + "px";
                    dropdown.style.width = rect.width + "px";
                },
                error: function () {
                    console.error("Failed to fetch suggestions.");
                }
            });
        }

        input.addEventListener("input", function () {
            const val = input.value;
            if (val.length >= 4) {
                fetchSuggestions(val);
            } else {
                removeDropdown();
            }
        });

        input.addEventListener("blur", function () {
            setTimeout(() => {
                removeDropdown();
                success(input.value);
            }, 150); // delay to allow click
        });

        return input;
    }
});


//Tabulator.extendModule("edit", "editors", {
//    autocomplete_ajax: function (cell, onRendered, success, cancel, editorParams) {
//        const input = document.createElement("input");
//        input.setAttribute("type", "text");
//        input.style.width = "100%";
//        input.value = cell.getValue() || "";

//        let dropdown;

//        function fetchSuggestions(query) {
//            $.ajax({
//                url: '/NPITrac/GetCodeSearch',
//                type: 'GET',
//                data: { search: query },
//                success: function (data) {
//                    if (dropdown) dropdown.remove();

//                    dropdown = document.createElement("div");
//                    dropdown.className = "autocomplete-dropdown";
//                    dropdown.style.position = "absolute";
//                    dropdown.style.zIndex = 1000;
//                    dropdown.style.backgroundColor = "#fff";
//                    dropdown.style.border = "1px solid #ccc";
//                    dropdown.style.maxHeight = "200px";
//                    dropdown.style.overflowY = "auto";
//                    dropdown.style.boxShadow = "0 2px 8px rgba(0,0,0,0.2)";
//                    dropdown.style.padding = "2px 0";
//                    dropdown.style.fontFamily = "Arial, sans-serif";
//                    dropdown.style.fontSize = "14px";
//                    dropdown.style.borderRadius = "4px";
//                    dropdown.style.scrollbarWidth = "thin";
//                    dropdown.style.width = input.offsetWidth + "px";

//                    data.forEach(item => {
//                        let option = document.createElement("div");
//                        option.textContent = item;
//                        option.style.padding = "6px 10px";
//                        option.style.borderBottom = "1px solid #eee";
//                        option.style.whiteSpace = "nowrap";
//                        option.style.cursor = "pointer";

//                        option.addEventListener("mousedown", function (e) {
//                            e.stopPropagation();
//                            success(item);
//                            if (dropdown) dropdown.remove();
//                        });

//                        dropdown.appendChild(option);
//                    });

//                    document.body.appendChild(dropdown);
//                    const rect = input.getBoundingClientRect();
//                    dropdown.style.top = rect.bottom + window.scrollY + "px";
//                    dropdown.style.left = rect.left + window.scrollX + "px";
//                },
//                error: function () {
//                    console.error("Autocomplete fetch failed.");
//                }
//            });
//        }

//        input.addEventListener("input", function () {
//            const val = input.value;
//            if (val.length >= 4) {
//                fetchSuggestions(val);
//            } else if (dropdown) {
//                dropdown.remove();
//            }
//        });

//        input.addEventListener("blur", function () {
//            setTimeout(() => {
//                if (dropdown) dropdown.remove();
//                success(input.value);
//            }, 200);
//        });

//        return input;
//    }
//});



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

    // Set custom width for specific fields
    if (field === "Product_Code") {
        columnDef.width = 220;
        columnDef.minWidth = 220;
    }
    else if (field === "Product_Des") {
        columnDef.width = 290;
        columnDef.minWidth = 290;
        columnDef.hozAlign = "left";
    } 

    return columnDef;
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
                PC: item.pc,
                Vendor: item.vendor,
                Prod_Category: item.prod_Category,
                Product_Code: item.poduct_Code,
                Product_Des: item.product_Des,
                Wattage: item.wattage,
                NPI_Category: item.nPI_Category,
                Offered_Date: formatDate(item.offered_Date),
                Released_Date: formatDate(item.released_Date),
                Releasded_Day: item.releasded_Day,
                Validation_Rep_No: item.validation_Rep_No,
                Customer_Comp: item.customer_Comp,
                Remark: item.remark,
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
        editableColumn("Product Code", "Product_Code", "autocomplete_ajax"),
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
        //editableColumn("Created By", "CreatedBy"),
        //editableColumn("Updated By", "UpdatedBy"),
        //editableColumn("Updated Date", "UpdatedDate", "input", "left"),
        //editableColumn("Created Date", "CreatedDate", "input", "left")
    );

    // // Initialize Tabulator
    table = new Tabulator("#npi_Table", {
        data: tabledata,
        layout: "fitData", // Add this
        renderHorizontal: "virtual",
        movableColumns: true,
        pagination: "local",
        paginationSize: 10,
        paginationSizeSelector: [50, 100, 500, 1500, 2000],
        paginationCounter: "rows",
        dataEmpty: "<div style='text-align: center; font-size: 1rem; color: gray;'>No data available</div>", // Placeholder message
        columns: columns,

        //cellEdited: function (cell) {
        //    const rowData = cell.getRow().getData();
        //    InsertUpdateNpiTracker(rowData); // Auto-save on edit
        //}
    });

    table.on("cellEdited", function (cell) {
        const rowData = cell.getRow().getData();
        InsertUpdateNpiTracker(rowData);
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

function InsertUpdateNpiTracker(rowData) {
    debugger;
    Blockloadershow();
    var errorMsg = "";
    var fields = "";

    console.log(rowData);
    //if ($("#Name").val() == '' || $("#Name").val() == null || $("#Name").val() == undefined) {
    //    fields += " - Name" + "<br>";
    //}

    if (fields != "") {
        errorMsg = "Please fill following mandatory field(s):" + "<br><br>" + fields;
    }

    if (errorMsg != "") {
        Blockloaderhide();
        showDangerAlert(errorMsg);
        return false;
    }

    var ajaxUrl = "";
    if (rowData.Id != 0) {
        ajaxUrl = '/NPITrac/Update';
    }
    else {
        ajaxUrl = '/NPITrac/Create';
    }

    var data = {
        Id: rowData.Id || 0,
        PC: rowData.PC || "",
        Vendor: rowData.Vendor || "",
        Prod_Category: rowData.Prod_Category || "",
        Product_Code: rowData.Product_Code || "",
        Product_Des: rowData.Product_Des || "",
        Wattage: rowData.Wattage || "",
        NPI_Category: rowData.NPI_Category || "",
        Offered_Date: rowData.Offered_Date || null,
        Released_Date: rowData.Released_Date || null,
        Releasded_Day: rowData.Releasded_Day || "",
        Validation_Rep_No: rowData.Validation_Rep_No || "",
        Customer_Comp: rowData.Customer_Comp || "",
        Remark: rowData.Remark || ""
    };

    $.ajax({
        url: ajaxUrl, 
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify(data),
        success: function (response) {
            Blockloaderhide();
            if (response.success) {
                //showSuccessAlert("Row saved successfully.");
                setTimeout(function () {
                    window.location.reload();
                }, 2500);
            } else {
                showDangerAlert("Failed to save row: " + response.message);
            }
        },
        error: function (xhr, status, error) {
            showDangerAlert("Error saving row: " + error);
        }
    });
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
            url: '/NPITrac/Delete',
            type: 'POST',
            data: { id: recid },
            success: function (data) {
                if (data.success == true) {
                    showSuccessAlert("NPI Tracker Detail Deleted successfully.");
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

