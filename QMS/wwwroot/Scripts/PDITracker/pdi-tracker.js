var tabledata = [];
var table = '';
$(document).ready(function () {
    document.getElementById('backButton').addEventListener('click', function () {
        window.history.back();
    });

    loadData();
});

function loadData() {
    Blockloadershow();

    $.ajax({
        url: '/PDITracker/GetAll',
        type: 'GET',
        success: function (data) {
            if (data && Array.isArray(data)) {
                console.log(data);
                OnTabGridLoad(data);
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
}
// Define a custom editor for Tabulator using Select2


function OnTabGridLoad(response) {
    Blockloadershow();
    let tabledata = [];

    if (response.length > 0) {
        $.each(response, function (index, item) {
            function formatDate(value) {
                return value ? new Date(value).toLocaleDateString("en-GB") : "";
            }

            tabledata.push({
                Sr_No: index + 1,
                PDIId: item.id,
                PC: item.pc || "",
                DispatchDate: formatDate(item.dispatchDate),
                ProductCode: item.productCode || "",
                ProductDescription: item.productDescription || "",
                BatchCodeVendor: item.batchCodeVendor || "",
                PONo: item.poNo || "",
                PDIDate: formatDate(item.pdiDate),
                PDIRefNo: item.pdiRefNo || "",
                OfferedQty: item.offeredQty,
                ClearedQty: item.clearedQty,
                BISCompliance: item.bisCompliance,
                InspectedBy: item.inspectedBy || "",
                Remark: item.remark || ""
            });
        });
    }

    var columns = [
        {
            title: "Action", field: "Action", frozen: true, hozAlign: "center", headerSort: false,
            width: 90,
            headerMenu: headerMenu,
            formatter: function (cell) {
                const rowData = cell.getRow().getData();
                return `<i onclick="delConfirm(${rowData.PDIId})" class="fas fa-trash-alt text-danger" title="Delete" style="cursor:pointer;"></i>`;
            }
        },
        { title: "S.No", field: "Sr_No", frozen: true, hozAlign: "center", headerSort: false, headerMenu: headerMenu, width: 80 },
        editableColumn("Dispatch Date", "DispatchDate", "date", "center", "input", {}, {}, 120),
        ////editableColumn("Product Code", "ProductCode", "select2", "center", "input", {}, {
        ////    values: productCodeOptions
        ////}, function (cell) {
        ////    const code = cell.getValue();
        ////    return productCodeOptions[code] || code;
        ////}),

        editableColumn("Product Description", "ProductDescription", "input", "left", null, {}, {}, 160),
        editableColumn("Batch Code (Vendor)", "BatchCodeVendor", "input", "left", null, {}, {}, 140),
        editableColumn("PO No", "PONo", "input", "left", null, {}, {}, 120),
        editableColumn("PDI Date", "PDIDate", "date", "center", "input", {}, {}, 120),
        editableColumn("PDI Ref No", "PDIRefNo", "input", "left", null, {}, {}, 130),
        editableColumn("Offered Qty", "OfferedQty", "input", "right", null, {}, {}, 110),
        editableColumn("Cleared Qty", "ClearedQty", "input", "right", null, {}, {}, 110),
        editableColumn("BIS Compliance", "BISCompliance", "tickCross", "center", null, {}, {}, 130),
        editableColumn("Inspected By", "InspectedBy", "input", "left", null, {}, {}, 130),
        editableColumn("Remark", "Remark", "input", "left", null, {}, {}, 180)
    ];

    table = new Tabulator("#pdi_table", {
        data: tabledata,
        layout: "fitDataFill",
        movableColumns: true,
        pagination: "local",
        paginationSize: 10,
        paginationSizeSelector: [10, 50, 100, 500],
        paginationCounter: "rows",
        placeholder: "No data available",
        columns: columns,
    });

    table.on("cellEdited", function (cell) {
        const rowData = cell.getRow().getData();
        saveEditedRow(rowData);
    });

    $("#addButton").on("click", function () {
        const newRow = {
            PDIId: 0,
            Sr_No: table.getDataCount() + 1,
            PC: "", DispatchDate: "", ProductCode: "", ProductDescription: "",
            BatchCodeVendor: "", PONo: "", PDIDate: "", PDIRefNo: "",
            OfferedQty: "", ClearedQty: "", BISCompliance: false,
            InspectedBy: "", Remark: ""
        };
        table.addRow(newRow, false);
    });

    Blockloaderhide();
}

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

function delConfirm(PDIId) {
    PNotify.prototype.options.styling = "bootstrap3";
    (new PNotify({
        title: 'Confirm Deletion',
        text: 'Are you sure you want to delete this PDI?',
        icon: 'fa fa-question-circle',
        hide: false,
        confirm: { confirm: true },
        buttons: { closer: false, sticker: false },
        history: { history: false }
    })).get().on('pnotify.confirm', function () {
        $.ajax({
            url: '/PDITracker/Delete',
            type: 'POST',
            data: { id: PDIId },
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
function filterTableByProductCode(value) {
    $.ajax({
        url: '/PDITracker/GetFilteredData',
        method: 'GET',
        data: { productCode: value },
        success: function (response) {
            table.replaceData(response); // Reload data with filtered results
        },
        error: function () {
            alert('Error loading filtered data.');
        }
    });
}

function saveEditedRow(rowData) {
    function emptyToNull(value) {
        return value === "" ? null : value;
    }

    // Converts "dd/MM/yyyy" to "yyyy-MM-dd"
    function toIsoDate(value) {
        if (!value) return null;
        const parts = value.split('/');
        if (parts.length === 3) {
            return `${parts[2]}-${parts[1].padStart(2, '0')}-${parts[0].padStart(2, '0')}`;
        }
        return value;
    }

    const cleanedData = {
        Id: rowData.PDIId || 0,
        PC: rowData.PC || "",
        DispatchDate: toIsoDate(rowData.DispatchDate),
        ProductCode: rowData.ProductCode || "",
        ProductDescription: rowData.ProductDescription || "",
        BatchCodeVendor: rowData.BatchCodeVendor || "",
        PONo: rowData.PONo || "",
        PDIDate: toIsoDate(rowData.PDIDate),
        PDIRefNo: rowData.PDIRefNo || "",
        OfferedQty: emptyToNull(rowData.OfferedQty),
        ClearedQty: emptyToNull(rowData.ClearedQty),
        BISCompliance: rowData.BISCompliance ?? null,
        InspectedBy: rowData.InspectedBy || "",
        Remark: rowData.Remark || ""
    };


    console.log("Cleaned data:", cleanedData);

    const isNew = cleanedData.Id === 0;
    const url = isNew ? '/PDITracker/CreateAsync' : '/PDITracker/UpdateAsync';

    $.ajax({
        url: url,
        type: 'POST',
        data: JSON.stringify(cleanedData),
        contentType: 'application/json',
        success: function (data) {
            if (data.success) {
                if (isNew) {
                    loadData();
                }
                //  showSuccessAlert(isNew ? "Created successfully." : "Updated successfully.");
                if (isNew && data.id) {
                    rowData.PDIId = data.id;
                }
            } else {
                showDangerAlert(data.message || (isNew ? "Create failed." : "Update failed."));
            }
        },
        error: function (xhr, status, error) {
            showDangerAlert(xhr.responseText || "Error saving record.");
        }
    });
}
