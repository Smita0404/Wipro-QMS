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
        url: '/CSOTracker/GetAll',
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
Tabulator.extendModule("edit", "editors", {
    autocomplete_ajax: function (cell, onRendered, success, cancel, editorParams) {
        const input = document.createElement("input");
        input.setAttribute("type", "text");
        input.style.width = "100%";
        input.value = cell.getValue() || "";

        let dropdown = null;

        function removeDropdown() {
            if (dropdown && dropdown.parentNode && document.body.contains(dropdown)) {
                dropdown.parentNode.removeChild(dropdown);
            }
            dropdown = null;
        }

        function fetchSuggestions(query) {
            $.ajax({
                url: '/CSOTracker/GetCodeSearch',
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
                            row.update({ ProductDescription: item.description });
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
                CSOId: item.csoId,
                CSOLogDate: formatDate(item.csoLogDate),
                CSONo: item.csoNo,
                ClassAB: item.classAB,
                ProductCatRef: item.productCatRef,
                ProductDescription: item.productDescription,
                SourceOfCSO: item.sourceOfCSO,
                InternalExternal: item.internalExternal,
                PKDBatchCode: item.pkdBatchCode,
                ProblemStatement: item.problemStatement,
                SuppliedQty: item.suppliedQty,
                FailedQty: item.failedQty,
                RootCause: item.rootCause,
                CorrectiveAction: item.correctiveAction,
                PreventiveAction: item.preventiveAction,
                CSOsClosureDate: formatDate(item.csosClosureDate),
                Aging: item.aging,
                AttachmentCAPAReport: item.attachmentCAPAReport
            });
        });
    }

    // Formatter for CAPA Report column (show link + upload button)
    function fileFormatter(cell, formatterParams, onRendered) {
        //const filename = cell.getValue();
        //if (filename) {
        //    return `
        //        <a href="/CSOTrackerAttachments/${filename}" target="_blank">${filename}</a><br/>
        //        <button class="btn btn-sm btn-outline-primary upload-btn">Change</button>
        //    `;
        //}
        return `<button class="btn btn-sm btn-outline-primary upload-btn">Upload</button>`;
    }

    // Editor for CAPA Report column (file input)
    function fileEditor(cell, onRendered, success, cancel, editorParams) {
        const input = document.createElement("input");
        input.setAttribute("type", "file");
        input.style.width = "100%";

        input.addEventListener("change", function (e) {
            const file = e.target.files[0];
            if (!file) {
                cancel();
                return;
            }
            const csoId = cell.getRow().getData().CSOId;
            if (!csoId || csoId === 0) {
                alert("Please save the record before uploading a file.");
                cancel();
                return;
            }
            uploadCAPAFile(csoId, file);
            cancel();  // Close editor immediately as upload is async
        });

        onRendered(() => {
            input.focus();
            input.style.height = "100%";
        });

        return input;
    }

    var columns = [
        {
            title: "Action", field: "Action", frozen: true, hozAlign: "center", headerSort: false,
            width: 90,
            headerMenu: headerMenu,
            formatter: function (cell) {
                const rowData = cell.getRow().getData();
                return `<i onclick="delConfirm(${rowData.CSOId})" class="fas fa-trash-alt text-danger" title="Delete" style="cursor:pointer;"></i>`;
            }
        },
        { title: "S.No", field: "Sr_No", frozen: true, hozAlign: "center", headerSort: false, headerMenu: headerMenu, width: 80 },

        editableColumn("CSO Log Date", "CSOLogDate", "date", "left", "input", {}, {}, 120),
        editableColumn("CSO No", "CSONo", "input", "left", "input", {}, {}, 120),
        editableColumn("Class A/B", "ClassAB", "list", "center", "list", {
            values: { "A": "A", "B": "B" }
        }, { values: { "A": "A", "B": "B" } }, 130),
        editableColumn("Product Cat Ref", "ProductCatRef", "autocomplete_ajax"),
        editableColumn("Product Description", "ProductDescription"),
        editableColumn("Source Of CSO", "SourceOfCSO", "input", "left", null, {}, {}, 130),
        editableColumn("Internal/External", "InternalExternal", "list", "center", "list", {
            values: { "Internal": "Internal", "External": "External" }
        }, { values: { "Internal": "Internal", "External": "External" } }, 130),
        editableColumn("PKD Batch Code", "PKDBatchCode", "input", "left", null, {}, {}, 120),
        editableColumn("Problem Statement", "ProblemStatement", "input", "left", null, {}, {}, 150),
        editableColumn("Supplied Qty", "SuppliedQty", "input", "left", null, {}, {}, 100),
        editableColumn("Failed Qty", "FailedQty", "input", "left", null, {}, {}, 100),
        editableColumn("Root Cause", "RootCause", "input", "left", null, {}, {}, 150),
        editableColumn("Corrective Action", "CorrectiveAction", "input", "left", null, {}, {}, 150),
        editableColumn("Preventive Action", "PreventiveAction", "input", "left", null, {}, {}, 150),
        editableColumn("Closure Date", "CSOsClosureDate", "date", "left", null, {}, {}, 120),
        editableColumn("Aging", "Aging", "input", "left", null, {}, {}, 80),

        // CAPA Report column with file upload support
        {
            title: "CAPA Report",
            field: "AttachmentCAPAReport",
            formatter: fileFormatter,
            editor: fileEditor,
            hozAlign: "left",
            headerSort: false, headerMenu: headerMenu,
            width: 180
        },
        {
            title: "Attachment",
            field: "AttachmentCAPAReport",
            formatter: function (cell) {
                const value = cell.getValue();
                if (!value) return "";
                const files = value.split(/[,;]+/).map(f => f.trim()).filter(Boolean);
                return files.map(path =>
                    `<a href="/${path}" target="_blank" download title="Download">
                    <i class="fas fa-download text-primary"></i>
                </a>`
                ).join(" ");
            },
           // editor: fileEditor,
            hozAlign: "left",
            headerSort: false,
            width: 180
        }
    ];

    table = new Tabulator("#cso_table", {
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
            CSOId: 0,
            Sr_No: table.getDataCount() + 1,
            CSOLogDate: "", CSONo: "", ClassAB: "", ProductCatRef: "", ProductDescription: "",
            SourceOfCSO: "", InternalExternal: "", PKDBatchCode: "", ProblemStatement: "",
            SuppliedQty: "", FailedQty: "", RootCause: "", CorrectiveAction: "", PreventiveAction: "",
            CSOsClosureDate: "", Aging: "", AttachmentCAPAReport: ""
        };
        table.addRow(newRow, false);
    });

    Blockloaderhide();
}

// Function to upload file and update record on server
function uploadCAPAFile(csoId, file) {
    var formData = new FormData();
    formData.append("file", file);
    formData.append("csoId", csoId);

    $.ajax({
        url: '/CSOTracker/UploadFile',
        type: 'POST',
        data: formData,
        processData: false,
        contentType: false,
        success: function (response) {
            if (response.success) {
                showSuccessAlert("File uploaded and record updated!");
                //const row = table.getRow(response.csoId);
                //if (row) {
                //    row.update({ AttachmentCAPAReport: response.filePath });
                //}// Ensure server returns updated path
            }
            else { showDangerAlert(response.message) }
            // Reload data or update the row's AttachmentCAPAReport value
            loadData(); // Make sure you have a function to reload your table data
        },
        error: function (xhr) {
            showDangerAlert("File upload failed: " + (xhr.responseJSON?.message || "Unknown error"));
        }
    });
}
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

function delConfirm(csoId) {
    PNotify.prototype.options.styling = "bootstrap3";
    (new PNotify({
        title: 'Confirm Deletion',
        text: 'Are you sure you want to delete this CSO?',
        icon: 'fa fa-question-circle',
        hide: false,
        confirm: { confirm: true },
        buttons: { closer: false, sticker: false },
        history: { history: false }
    })).get().on('pnotify.confirm', function () {
        $.ajax({
            url: '/CSOTracker/Delete',
            type: 'POST',
            data: { id: csoId },
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
    if (field === "ProductCatRef") {
        columnDef.width = 220;
        columnDef.minWidth = 220;
    }
    else if (field === "ProductDescription") {
        columnDef.width = 290;
        columnDef.minWidth = 290;
        columnDef.hozAlign = "left";
    }

    return columnDef;
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
        Id: rowData.CSOId,
        CSOLogDate: toIsoDate(rowData.CSOLogDate),
        CSONo: rowData.CSONo || "",
        ClassAB: rowData.ClassAB || "",
        ProductCatRef: rowData.ProductCatRef || "",
        ProductDescription: rowData.ProductDescription || "",
        SourceOfCSO: rowData.SourceOfCSO || "",
        InternalExternal: rowData.InternalExternal || "",
        PKDBatchCode: rowData.PKDBatchCode || "",
        ProblemStatement: rowData.ProblemStatement || "",
        SuppliedQty: emptyToNull(rowData.SuppliedQty),
        FailedQty: emptyToNull(rowData.FailedQty),
        RootCause: rowData.RootCause || "",
        CorrectiveAction: rowData.CorrectiveAction || "",
        PreventiveAction: rowData.PreventiveAction || "",
        CSOsClosureDate: toIsoDate(rowData.CSOsClosureDate),
        Aging: emptyToNull(rowData.Aging),
        AttachmentCAPAReport: rowData.AttachmentCAPAReport || ""
    };

    console.log("Cleaned data:", cleanedData);

    const isNew = cleanedData.Id === 0;
    const url = isNew ? '/CSOTracker/CreateAsync' : '/CSOTracker/UpdateAsync';

    $.ajax({
        url: url,
        type: 'POST',
        data: JSON.stringify(cleanedData),
        contentType: 'application/json',
        success: function (data) {
            if (data.success) {
                if (isNew)
                {
                    loadData();
                }
              //  showSuccessAlert(isNew ? "Created successfully." : "Updated successfully.");
                if (isNew && data.id) {
                    rowData.CSOId = data.id;
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
