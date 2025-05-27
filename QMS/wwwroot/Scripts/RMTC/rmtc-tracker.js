var tabledata = [];
var table = '';
let vendorOptions = {};
let filterStartDate = moment().startOf('week').format('YYYY-MM-DD');
let filterEndDate = moment().endOf('week').format('YYYY-MM-DD');

$(document).ready(function () {
    $('#dateRangeText').text(
        moment(filterStartDate).format('MMMM D, YYYY') + ' - ' + moment(filterEndDate).format('MMMM D, YYYY')
    );

    // Initialize Litepicker for date range selection
    const picker = new Litepicker({
        element: document.getElementById('customDateTrigger'),
        singleMode: false,
        format: 'DD-MM-YYYY',
        numberOfMonths: 2,
        numberOfColumns: 2,
        dropdowns: {
            minYear: 2020,
            maxYear: null,
            months: true,
            years: true
        },
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

    // Open calendar on click
    $('#customDateTrigger').on('click', function () {
        picker.show();
    });

    $('#backButton').on('click', function () {
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
    $.ajax({
        url: '/RMTC/GetVendors', 
        type: 'GET'
    }).done(function (vendorData) {
        if (Array.isArray(vendorData)) {
            vendorOptions = vendorData.reduce((acc, v) => {
                acc[v.value] = v.label;
                return acc;
            }, {});
        }

        $.ajax({
            url: '/RMTC/GetAll',  // Change to your actual endpoint
            type: 'GET',
            dataType: 'json',
            data: {
                startDate: filterStartDate,
                endDate: filterEndDate
            }
        })
            .done(function (data) {
                const safeData = Array.isArray(data) ? data : [];
                if (safeData.length) {
                    OnTabGridLoad(safeData);
                } else {
                    showDangerAlert('No data available to load.');
                    OnTabGridLoad(safeData);
                }
            })
            .fail(function (xhr, status, error) {
                console.error('Error retrieving data:', status, error, xhr.responseText);
                showDangerAlert('Error retrieving data: ' + (error || status));
                OnTabGridLoad(safeData);
            })
            .always(function () {
                Blockloaderhide();
            });
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
                url: '/RMTC/GetProductCodeSearch',
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

function editableColumn({
    title,
    field,
    editorType,
    editorParams = {},
    formatter,
    headerFilterType = "input",
    headerFilterParams = {},
    headerMenu,
    align = "center"
}) {
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
        headerHozAlign: "center",
    };

    if (field === "ProductCatRef") {
        columnDef.width = 220;
        columnDef.minWidth = 220;
    } else if (field === "ProductDescription") {
        columnDef.width = 290;
        columnDef.minWidth = 290;
        columnDef.hozAlign = "left";
    }

    return columnDef;
}

function OnTabGridLoad(response) {
    Blockloadershow();
    console.log(response);
    function formatDate(value) {
        return value ? new Date(value).toLocaleDateString("en-GB") : "";
    }

    let tabledata = response.map((item, index) => ({
       
        Sr_No: index + 1,
        Id: item.id,
        Vendor: item.vendor || "",
        ProductCatRef: item.productCatRef || "",
        ProductDescription: item.productDescription || "",
        RMTCDate: formatDate(item.rmtcDate),
        Remarks: item.remarks || "",
        HousingBody: item.housingBody || "",
        WiresCables: item.wiresCables || "",
        DiffuserLens: item.diffuserLens || "",
        PCB: item.pcb || "",
        Connectors: item.connectors || "",
        PowderCoat: item.powderCoat || "",
        LEDLM80PhotoBiological: item.ledlM80PhotoBiological || "",
        LEDPurchaseProof: item.ledPurchaseProof || "",
        Driver: item.driver || "",
        Pretreatment: item.pretreatment || "",
        Hardware: item.hardware || "",
        OtherCriticalItems: item.otherCriticalItems || "",
        CreatedDate: item.createdDate || "",
        CreatedBy: item.createdBy || "",
        UpdatedDate: item.updatedDate || "",
        UpdatedBy: item.updatedBy || "",
        Filename: item.filename || ""
    }));
    console.log(tabledata);
    function fileFormatter(cell) {
        return `<button class="btn btn-sm btn-outline-primary">Upload</button>`;
    }

    function uploadRMTCFile(Id, file) {
        const formData = new FormData();
        formData.append("file", file);
        formData.append("Id", Id);

        $.ajax({
            url: '/RMTC/UploadFile',
            type: 'POST',
            data: formData,
            processData: false,
            contentType: false,
            success: function (response) {
                if (response.success) {
                    showSuccessAlert("File uploaded and record updated!");
                } else {
                    showDangerAlert(response.message);
                }
                loadData();
            },
            error: function (xhr) {
                showDangerAlert("File upload failed: " + (xhr.responseJSON?.message || "Unknown error"));
            }
        });
    }

    function fileEditor(cell, onRendered, success, cancel) {
        const input = document.createElement("input");
        input.setAttribute("type", "file");
        input.style.width = "100%";

        input.addEventListener("change", function (e) {
            const file = e.target.files[0];
            if (!file) {
                cancel();
                return;
            }
            const Id = cell.getRow().getData().Id;
            if (!Id || Id === 0) {
                alert("Please save the record before uploading a file.");
                cancel();
                return;
            }
            uploadRMTCFile(Id, file);
            cancel();
        });

        onRendered(() => {
            input.focus();
            input.style.height = "100%";
        });

        return input;
    }

    const columns = [
        {
            title: "Action",
            field: "action",
            frozen: true,
            hozAlign: "center", headerMenu: headerMenu,
            headerSort: false,
            width: 90,
            formatter: function (cell) {
                const rowData = cell.getRow().getData();
                return `<i onclick="delConfirm(${rowData.Id})" class="fas fa-trash-alt text-danger" title="Delete" style="cursor:pointer;"></i>`;
            }
        },
        { title: "S.No", field: "Sr_No", frozen: true, hozAlign: "center", headerSort: false, width: 80, headerMenu: headerMenu },

        editableColumn({ title: "RMTC Date", field: "RMTCDate", editorType: "date", headerMenu: headerMenu }),
        editableColumn({
            title: "Vendor Details",
            field: "Vendor", headerMenu: headerMenu,
            editorType: "select2",  // use "list" for dropdown editor
            editorParams: {
                values: vendorOptions // { key1: "Vendor 1", key2: "Vendor 2", ... }
            },
            formatter: cell => vendorOptions[cell.getValue()] || cell.getValue(),
            headerFilterType: "input",
            headerFilterParams: {
                values: vendorOptions
            }
        })
,

        editableColumn({ title: "Product Code", field: "ProductCatRef", editorType: "autocomplete_ajax", headerMenu: headerMenu }),
        editableColumn({ title: "Product Description", field: "ProductDescription", editorType: "input", headerMenu: headerMenu }),
        editableColumn({ title: "Remarks", field: "Remarks", editorType: "input", align: "left", headerMenu: headerMenu }),
        editableColumn({
            title: "Housing Body", field: "HousingBody", editorType: "list", headerMenu: headerMenu,
            editorParams: { values: { "Metal": "Metal", "Plastic": "Plastic" } },
            headerFilterParams: { values: { "Metal": "Metal", "Plastic": "Plastic" } }
        }),
        editableColumn({
            title: "Wires / Cables", field: "WiresCables", editorType: "list", headerMenu: headerMenu,
            editorParams: { values: { "Wires": "Wires", "Cables": "Cables" } },
            headerFilterParams: { values: { "Wires": "Wires", "Cables": "Cables" } }
        }),
        editableColumn({
            title: "Diffuser / Lens", field: "DiffuserLens", editorType: "list", headerMenu: headerMenu,
            editorParams: { values: { "Diffuser": "Diffuser", "Lens": "Lens" } },
            headerFilterParams: { values: { "Diffuser": "Diffuser", "Lens": "Lens" } }
        }),
        editableColumn({ title: "PCB", field: "PCB", editorType: "input", headerMenu: headerMenu }),
        editableColumn({ title: "Connectors", field: "Connectors", editorType: "input", headerMenu: headerMenu }),
        editableColumn({ title: "Powder Coat", field: "PowderCoat", editorType: "input", headerMenu: headerMenu }),
        editableColumn({
            title: "LED LM80 / Photo Biological", field: "LEDLM80PhotoBiological", editorType: "list", headerMenu: headerMenu,
            editorParams: { values: { "LED LM80": "LED LM80", "Photo Biological": "Photo Biological" } },
            headerFilterParams: { values: { "LED LM80": "LED LM80", "Photo Biological": "Photo Biological" } }
        }),
        editableColumn({ title: "LED Purchase Proof", field: "LEDPurchaseProof", editorType: "input", headerMenu: headerMenu }),
        editableColumn({ title: "Driver", field: "Driver", editorType: "input", headerMenu: headerMenu }),
        editableColumn({ title: "Pretreatment", field: "Pretreatment", editorType: "input", headerMenu: headerMenu }),
        editableColumn({
            title: "Hardware", field: "Hardware", editorType: "list", headerMenu: headerMenu,
            editorParams: { values: { "SS": "SS", "MS": "MS" } },
            headerFilterParams: { values: { "SS": "SS", "MS": "MS" } }
        }),
        editableColumn({ title: "Other Critical Items", field: "OtherCriticalItems", editorType: "input", headerMenu: headerMenu }),

        {
            title: "Upload Report", field: "Filename", formatter: fileFormatter, headerMenu: headerMenu,
            editor: fileEditor, hozAlign: "center", headerSort: false, width: 140
        },
        {
            title: "Attachment", field: "Filename", headerMenu: headerMenu, formatter: function (cell) {
                const value = cell.getValue();
                if (!value) return "";
                const files = value.split(/[,;]+/).map(f => f.trim()).filter(Boolean);
                return files.map(path =>
                    `<a href="/${path}" target="_blank" download title="Download">
                        <i class="fas fa-download text-primary"></i>
                    </a>`
                ).join(" ");
            }, hozAlign: "center", headerSort: false, width: 120
        },

        { title: "Created Date", field: "CreatedDate", visible: false, headerMenu: headerMenu },
        { title: "Created By", field: "CreatedBy", visible: false, headerMenu: headerMenu },
        { title: "Updated Date", field: "UpdatedDate", visible: false },
        { title: "Updated By", field: "UpdatedBy", visible: false, headerMenu: headerMenu }
    ];

    if (table) {
        table.replaceData(tabledata);
    } else {
        table = new Tabulator("#rmctTracker", {
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

    $("#addButton").on("click", function () {
        const newRow = {
            Id: 0,
            Sr_No: table.getDataCount() + 1,
            RMTCDate: "",
            Vendor: "",
            ProductCatRef: "",
            ProductDescription: "",
            Remarks: "",
            HousingBody: "",
            WiresCables: "",
            DiffuserLens: "",
            PCB: "",
            Connectors: "",
            PowderCoat: "",
            LEDLM80PhotoBiological: "",
            LEDPurchaseProof: "",
            Driver: "",
            Pretreatment: "",
            Hardware: "",
            OtherCriticalItems: ""
        };
        table.addRow(newRow, false);
    });

    Blockloaderhide();
}


function saveEditedRow(rowData) {
    console.log(rowData);

    function toIsoDate(value) {
        if (!value) return null;
        const parts = value.split('/');
        if (parts.length === 3) {
            return `${parts[2]}-${parts[1].padStart(2, '0')}-${parts[0].padStart(2, '0')}`;
        }
        return value;
    }

    const cleanedData = {
        Id: rowData.Id || 0,
        Vendor: rowData.Vendor ? rowData.Vendor : null,
        ProductCatRef: rowData.ProductCatRef ? rowData.ProductCatRef : null,
        ProductDescription: rowData.ProductDescription ? rowData.ProductDescription : null,
        RMTCDate: toIsoDate(rowData.RMTCDate) || null,
        Remarks: rowData.Remarks ? rowData.Remarks : null,

        HousingBody: rowData.HousingBody ? rowData.HousingBody : null,
        WiresCables: rowData.WiresCables ? rowData.WiresCables : null,
        DiffuserLens: rowData.DiffuserLens ? rowData.DiffuserLens : null,
        PCB: rowData.PCB ? rowData.PCB : null,
        Connectors: rowData.Connectors ? rowData.Connectors : null,
        PowderCoat: rowData.PowderCoat ? rowData.PowderCoat : null,
        LEDLM80PhotoBiological: rowData.LEDLM80PhotoBiological ? rowData.LEDLM80PhotoBiological : null,
        LEDPurchaseProof: rowData.LEDPurchaseProof ? rowData.LEDPurchaseProof : null,
        Driver: rowData.Driver ? rowData.Driver : null,
        Pretreatment: rowData.Pretreatment ? rowData.Pretreatment : null,
        Hardware: rowData.Hardware ? rowData.Hardware : null,
        OtherCriticalItems: rowData.OtherCriticalItems ? rowData.OtherCriticalItems : null,

        Filename: rowData.Filename ? rowData.Filename : null
    };

    console.log(cleanedData);

    const isNew = cleanedData.Id === 0;
    const url = isNew ? "/RMTC/CreateAsync" : "/RMTC/UpdateAsync";

    $.ajax({
        url: url,
        type: "POST",
        data: JSON.stringify(cleanedData),
        contentType: "application/json",
        success: function (res) {
            if (res && res.success) {
                // if (isNew) {
                   loadData();
             //  }
                if (isNew && res.id) {
                    rowData.Id = res.id;
                }
              //  showSuccessAlert(isNew ? "Record created." : "Record updated.");
            } else {
                showDangerAlert((res && res.message) || (isNew ? "Create failed." : "Update failed."));
            }
        },
        error: function (xhr) {
            showDangerAlert(xhr.responseText || "Error saving record.");
        }
    });
}

function delConfirm(Id) {
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
            url: '/RMTC/Delete',
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