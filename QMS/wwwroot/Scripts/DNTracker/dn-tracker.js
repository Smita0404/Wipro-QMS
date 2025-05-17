var tabledata = [];
var table = '';
let productCodeOptions = {};
let vendorOptions = {};
let dnoteCategoryOptions = {};

$(document).ready(function () {
    document.getElementById('backButton').addEventListener('click', function () {
        window.history.back();
    });

    loadData();
});

var headerMenu = function () {
    var menu = [];
    var columns = this.getColumns();

    for (let column of columns) {
        let icon = document.createElement("i");
        icon.classList.add("fas");
        icon.classList.add(column.isVisible() ? "fa-check-square" : "fa-square");

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

function loadData() {
    Blockloadershow();

    // Load all dropdown options first (product codes, vendors, categories)
    $.when(
        $.ajax({ url: '/DNTracker/GetProductCodes', type: 'GET' }),
        $.ajax({ url: '/DNTracker/GetVendors', type: 'GET' })
        
    ).done(function (productCodesData, vendorsData) {
        // productCodesData, vendorsData, categoriesData are arrays inside array wrappers because of $.when
        productCodeOptions = (Array.isArray(productCodesData[0])) ? productCodesData[0].reduce((acc, p) => { acc[p.value] = p.label; return acc; }, {}) : {};
        vendorOptions = (Array.isArray(vendorsData[0])) ? vendorsData[0].reduce((acc, v) => { acc[v.value] = v.label; return acc; }, {}) : {};
       
        // After loading options, load main table data
        $.ajax({
            url: '/DNTracker/GetAll',
            type: 'GET',
            success: function (data) {
                if (Array.isArray(data)) {
                    OnTabGridLoad(data);
                } else {
                    showDangerAlert('No Deviation Note data available.');
                }
                Blockloaderhide();
            },
            error: function () {
                showDangerAlert('Error loading deviation note data.');
                Blockloaderhide();
            }
        });

    }).fail(function () {
        showDangerAlert('Failed to load dropdown options.');
        Blockloaderhide();
    });
}


function OnTabGridLoad(response) {
    Blockloadershow();
    let tabledata = [];

    if (response.length > 0) {
        $.each(response, function (index, item) {
            tabledata.push({
                Sr_No: index + 1,
                DNoteId: item.dNoteId,
                DNoteNumber: item.dNoteNumber || "",
                DNoteCategory: item.dNoteCategory || "",
                ProductCode: item.productCode || "",
                ProductDescription: item.productDescription || "",
                Wattage: item.wattage || "",
                DQty: item.dQty || null,
                DRequisitionBy: item.dRequisitionBy || "",
                Vendor: item.vendor || "",
                Remark: item.remark || ""
            });
        });
    }

    const columns = [
        {
            title: "Action", field: "Action", frozen: true, hozAlign: "center", headerSort: false,
            width: 90,
            headerMenu: headerMenu,
            formatter: function (cell) {
                const rowData = cell.getRow().getData();
                return `<i onclick="delConfirm(${rowData.DNoteId})" class="fas fa-trash-alt text-danger" title="Delete" style="cursor:pointer;"></i>`;
            }
        },
        { title: "S.No", field: "Sr_No", frozen: true, hozAlign: "center", headerSort: false, headerMenu: headerMenu, width: 70 },

        editableColumn("DNote Number", "DNoteNumber", "input", "left", "input", {}, {}, 150),

        editableColumn("Category", "DNoteCategory", "input", "center", "input", {}, {}, 130),

        editableColumn("Product Code", "ProductCode", "select2", "center", "input", {}, {
            values: productCodeOptions
        }, function (cell) {
            const val = cell.getValue();
            return productCodeOptions[val] || val;
        }, 130),

        editableColumn("Product Description", "ProductDescription", "input", "left", null, {}, {}, 180),

        editableColumn("Wattage", "Wattage", "input", "center", null, {}, {}, 100),

        editableColumn("Quantity", "DQty", "input", "right", null, {}, {}, 90),

        editableColumn("Requisition By", "DRequisitionBy", "input", "left", null, {}, {}, 150),

        editableColumn("Vendor", "Vendor", "select2", "center", "input", {}, {
            values: vendorOptions
        }, function (cell) {
            const val = cell.getValue();
            return vendorOptions[val] || val;
        }, 130),

        editableColumn("Remark", "Remark", "input", "left", null, {}, {}, 200)
    ];

    if (table) {
        table.replaceData(tabledata);
    } else {
        table = new Tabulator("#dn_table", {
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
            const rowData = cell.getRow().getData();
            saveEditedRow(rowData);
        });
    }

    $("#addButton").off("click").on("click", function () {
        const newRow = {
            DNoteId: 0,
            Sr_No: table.getDataCount() + 1,
            DNoteNumber: "",
            DNoteCategory: "",
            ProductCode: "",
            ProductDescription: "",
            Wattage: "",
            DQty: null,
            DRequisitionBy: "",
            Vendor: "",
            Remark: ""
        };
        table.addRow(newRow, false); // add to bottom
    });

    Blockloaderhide();
}

function editableColumn(title, field, editorType = true, align = "center", headerFilterType = "input", headerFilterParams = {}, editorParams = {}, formatter = null, width = null) {
    const column = {
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

    if (width !== null) {
        column.width = width;
    }

    return column;
}

function delConfirm(DNoteId) {
    PNotify.prototype.options.styling = "bootstrap3";
    (new PNotify({
        title: 'Confirm Deletion',
        text: 'Are you sure you want to delete this Deviation Note?',
        icon: 'fa fa-question-circle',
        hide: false,
        confirm: { confirm: true },
        buttons: { closer: false, sticker: false },
        history: { history: false }
    })).get().on('pnotify.confirm', function () {
        $.ajax({
            url: '/DNTracker/Delete',
            type: 'POST',
            data: { id: DNoteId },
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

function saveEditedRow(rowData) {
    function emptyToNull(value) {
        return value === "" ? null : value;
    }

    const cleanedData = {
        Id: rowData.DNoteId || 0,
        DNoteNumber: rowData.DNoteNumber || "",
        DNoteCategory: rowData.DNoteCategory || null,
        ProductCode: rowData.ProductCode || null,
        ProductDescription: rowData.ProductDescription || null,
        Wattage: rowData.Wattage || null,
        DQty: emptyToNull(rowData.DQty),
        DRequisitionBy: rowData.DRequisitionBy || null,
        Vendor: rowData.Vendor || null,
        Remark: rowData.Remark || null
    };

    const isNew = cleanedData.Id === 0;
    const url = isNew ? '/DNTracker/CreateAsync' : '/DNTracker/UpdateAsync';

    $.ajax({
        url: url,
        type: 'POST',
        data: JSON.stringify(cleanedData),
        contentType: 'application/json',
        success: function (data) {
            if (data.success) {
                if (isNew && data.id) {
                    rowData.DNoteId = data.id;
                }
                if (isNew) {
                    loadData();
                }
            } else {
                showDangerAlert(data.message || (isNew ? "Create failed." : "Update failed."));
            }
        },
        error: function (xhr) {
            showDangerAlert(xhr.responseText || "Error saving record.");
        }
    });
}
