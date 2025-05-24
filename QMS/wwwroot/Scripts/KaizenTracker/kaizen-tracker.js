var table = null;
let vendorOptions = {};
let filterStartDate = moment().startOf('week').format('YYYY-MM-DD');
let filterEndDate = moment().endOf('week').format('YYYY-MM-DD');

$(document).ready(function () {

    $('#dateRangeText').text(
        moment(filterStartDate).format('MMMM D, YYYY') + ' - ' + moment(filterEndDate).format('MMMM D, YYYY')
    );

    // Initialize Litepicker
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
function uploadCAPAFile(kId, file) {
 
    var formData = new FormData();
    formData.append("file", file);
    formData.append("kId", kId);
    console.log(kId);
    $.ajax({
        url: '/KaizenTracker/UploadFile',
        type: 'POST',
        data: formData,
        processData: false,
        contentType: false,
        success: function (response) {
            if (response.success) {
                showSuccessAlert("File uploaded and record updated!");
               
            }
            else { showDangerAlert(response.message) }
           
            loadData(); 
        },
        error: function (xhr) {
            showDangerAlert("File upload failed: " + (xhr.responseJSON?.message || "Unknown error"));
        }
    });
}

function OnTabGridLoad(response) {
    Blockloadershow();

    let tabledata = [];

    if (response.length > 0) {
        $.each(response, function (index, item) {
            tabledata.push({
                Sr_No: index + 1,
                Id: item.id,
                Vendor: item.vendor || "",
                KaizenTheme: item.kaizenTheme || "",
                KMonth: item.kMonth || "",
                Team: item.team || "",
                KaizenFile: item.kaizenFile || "",
                Remarks: item.remarks || "",
                CreatedDate: item.createdDate || "",
                UpdatedDate: item.updatedDate || "",
                CreatedBy: item.createdBy || "",
                UpdatedBy: item.updatedBy || ""
            });
        });
    }
    console.log(tabledata);
    function fileFormatter(cell, formatterParams, onRendered) {
        
        return `<button class="btn btn-sm btn-outline-primary">Upload</button>`;
    }

    
    function fileEditor(cell, onRendered, success, cancel, editorParams) {
        const input = document.createElement("input");
        input.setAttribute("type", "file");
        input.style.width = "100%";
        console.log(cell);
        input.addEventListener("change", function (e) {
            const file = e.target.files[0];
            if (!file) {
                cancel();
                return;
            }
            const rowData = cell.getRow().getData();
            const kId = rowData.Id;
            console.log(kId);
            if (!kId || kId === 0) {
                showDangerAlert("Please save the record before uploading a file.");
                cancel();
                return;
            }
            uploadCAPAFile(kId, file);
            cancel();  // Close editor immediately as upload is async
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
            field: "Action",
            frozen: true,
            hozAlign: "center", headerMenu: headerMenu,
            width: 130,
            formatter: function (cell) {
                const rowData = cell.getRow().getData();
                return `<i onclick="delConfirm(${rowData.Id})" class="fas fa-trash-alt text-danger" title="Delete" style="cursor:pointer;"></i>`;
            }
        },
        { title: "ID", field: "Id", hozAlign: "center", frozen: true, visible: false },
        { title: "SNo", field: "Sr_No", frozen: true, hozAlign: "center", headerMenu: headerMenu, width: 110 },

        editableColumn("Vendor", "Vendor", "select2", "center", null, {}, { values: vendorOptions }, function (cell) {
            const val = cell.getValue();
            return vendorOptions[val] || val;
        }, 150),

        editableColumn("Kaizen Theme", "KaizenTheme", "input", "center", null, {}, {}, null, 200),

        editableColumn("Kaizen Month", "KMonth", "input", "center", null, {}, {}, null, 170),

        editableColumn("Team", "Team", "input", "center", null, {}, {}, null, 120),
        editableColumn("Remarks", "Remarks", "input", "center", null, {}, {}, null, 150),
        {
            title: "Kaizen File",
            field: "KaizenFile",
            formatter: fileFormatter,
            editor: fileEditor,
            hozAlign: "center",
            headerMenu: headerMenu,
            width: 140
        },
        {
            title: "Attachment",
            field: "KaizenFile",
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
            hozAlign: "center", headerMenu: headerMenu,
           
            width: 140
        },
       

       

        // Optional metadata columns (hidden)
        { title: "Created By", field: "CreatedBy", hozAlign: "center", visible: false },
        { title: "Created Date", field: "CreatedDate", hozAlign: "center", visible: false },
        { title: "Updated By", field: "UpdatedBy", hozAlign: "center", visible: false },
        { title: "Updated Date", field: "UpdatedDate", hozAlign: "center", visible: false }
        
    ];

    if (table) {
        table.replaceData(tabledata);
    } else {
        table = new Tabulator("#kaizen_table", {
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
            saveKaizenTrackerRow(rowData);
        });
    }

    $("#addButton").on("click", function () {
        const newRow = {
            Id: 0,
            Sr_No: table.getDataCount() + 1,
            Vendor: "",
            KaizenTheme: "",
            KMonth: "",
            Team: "", Remarks: "",
            KaizenFile: ""
             /*CreatedBy: "", UpdatedBy: "", UpdatedDate: "", CreatedDate: ""*/
        };
        table.addRow(newRow, false);
    });

    Blockloaderhide();
}

function editableColumn(
    title,
    field,
    editorType = true,
    align = "center",
    headerFilterType = "input",
    headerFilterParams = {},
    editorParams = {},
    formatter = null,
    width = null
) {
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

    if (width) {
        columnDef.width = width;
    }

    return columnDef;
}
function loadData() {
    Blockloadershow();

    $.ajax({
        url: '/KaizenTracker/GetVendor',
        type: 'GET'
    }).done(function (vendorData) {
        if (Array.isArray(vendorData)) {
            vendorOptions = vendorData.reduce((acc, v) => {
                acc[v.value] = v.label;
                return acc;
            }, {});
        }

        $.ajax({
            url: '/KaizenTracker/GetAll',
            type: 'GET',
            dataType: 'json',
            data: {
                startDate: filterStartDate,
                endDate: filterEndDate
            },
            success: function (data) {
                if (Array.isArray(data)) {
                    OnTabGridLoad(data);
                } else {
                    showDangerAlert('No Kaizen data available.');
                }
                Blockloaderhide();
            },
            error: function () {
                showDangerAlert('Error loading Kaizen data.');
                Blockloaderhide();
            }
        });

    }).fail(function () {
        showDangerAlert('Failed to load vendor dropdown options.');
        Blockloaderhide();
    });
}

function delConfirm(id) {
    PNotify.prototype.options.styling = "bootstrap3";
    (new PNotify({
        title: 'Confirm Deletion',
        text: 'Are you sure you want to delete this Kaizen record?',
        icon: 'fa fa-question-circle',
        hide: false,
        confirm: { confirm: true },
        buttons: { closer: false, sticker: false },
        history: { history: false }
    })).get().on('pnotify.confirm', function () {
        $.ajax({
            url: '/KaizenTracker/Delete',
            type: 'POST',
            data: { id: id },
            success: function (data) {
                if (data.success) {
                    showSuccessAlert("Deleted successfully.");
                    setTimeout(() => loadData(), 1500);
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
            select.appendChild(option);
        }

        select.value = cell.getValue();

        onRendered(function () {
            $(select).select2({
                dropdownParent: $('body')
            }).focus();

            $(select).on("change", function () {
                success(this.value);
            });

            $(select).on("blur", function () {
                cancel();
            });
        });

        return select;
    }
});

function saveKaizenTrackerRow(rowData) {
    function emptyToNull(value) {
        return value === "" ? null : value;
    }
    console.log(rowData);
    const cleanedData = {
        Id: rowData.Id || 0,
        Vendor: emptyToNull(rowData.Vendor),
        KaizenTheme: emptyToNull(rowData.KaizenTheme),
        KMonth: emptyToNull(rowData.KMonth),
        Team: emptyToNull(rowData.Team),
        KaizenFile: emptyToNull(rowData.KaizenFile),
        Remarks: emptyToNull(rowData.Remarks)
    };
    console.log(cleanedData);
    const isNew = cleanedData.Id === 0;
    const url = isNew ? '/KaizenTracker/CreateAsync' : '/KaizenTracker/UpdateAsync';

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
                if (isNew && data.id) {
                    rowData.Id = data.id;
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
