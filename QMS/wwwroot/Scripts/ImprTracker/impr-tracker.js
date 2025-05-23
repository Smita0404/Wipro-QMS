var table = null;
let filterStartDate = moment().startOf('week').format('YYYY-MM-DD');
let filterEndDate = moment().endOf('week').format('YYYY-MM-DD');

$(document).ready(function () {

    $('#dateRangeText').text(
        moment(filterStartDate).format('MMMM D, YYYY') + ' - ' + moment(filterEndDate).format('MMMM D, YYYY')
    );

    // Initialize Litepicker and store reference
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
                loadTableData();
            });

            picker.on('clear', () => {
                filterStartDate = "";
                filterEndDate = "";
                $('#dateRangeText').text("Select Date Range");
                loadTableData();
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

    // 🔑 Ensure calendar opens on click
    $('#customDateTrigger').on('click', function () {
        picker.show();
    });

    $('#backButton').on('click', () => window.history.back());
    loadTableData();
  

    $('#addButton').on('click', function () {
        if (!table) return;
        const newRow = {
            Id: 0,
            StartDate: "",
            FunctionalArea: "",
            Issue: "",
            Problem: "",
            CorrectiveAction: "",
            Responsible: "",
           
            ImprovementAchieved: ""
        };
        table.addRow(newRow, false); // add to bottom
    });
});

function loadTableData() {
    Blockloadershow(); // Show loader before AJAX

    $.ajax({
        url: '/ImprTracker/GetAll',
        type: 'GET',
        dataType: 'json',
        data: {
            startDate: filterStartDate,
            endDate: filterEndDate
        },
    })
        .done(function (data) {
            if (Array.isArray(data)) {
                console.log("Loaded Improvement Tracker data:", data);
                setupTabulator(data); // Call your tabulator setup here
            } else {
                console.warn("Unexpected response format:", data);
                showDangerAlert('No data available to load.');
                setupTabulator([]); // Load empty table
            }
        })
        .fail(function (xhr, status, error) {
            const msg = `Error retrieving data: ${xhr.status} ${xhr.statusText}\n${xhr.responseText}`;
            console.error(msg);
            showDangerAlert(msg);
            setupTabulator([]); // Load empty table on failure
        })
        .always(function () {
            Blockloaderhide(); // Always hide loader
        });
}

//function loadTableData() {
//    $.ajax({
//        url: '/ImprTracker/GetAll',
//        type: 'GET',
//        success: function (data) {
//            if (Array.isArray(data)) {
//                setupTabulator(data);
//            } else {
//                showDangerAlert('No data available.');
//            }
//            Blockloaderhide();
//        },
//        error: function () {
//            showDangerAlert('Failed to load improvement tracker data.');
//            Blockloaderhide();
//        }
//    });
//}

function setupTabulator(data) {
    const formatDate = (value) => value ? new Date(value).toLocaleDateString('en-GB') : '';
    console.log(data);
    const tableData = data.map(item => ({
        Id: item.id,  // capital I to match C# property
        FunctionalArea: item.funcArea || "",
        Issue: item.issue || "",
        Problem: item.problem || "",
        CorrectiveAction: item.correctiveAction || "",
        Responsible: item.responsible || "",
        StartDate: formatDate(item.startDate),
        ImprovementAchieved: item.ImprAchieved || "",
        UpdatedDate: item.updatedDate || "",
        CreatedDate: item.createdDate || "",
        CreatedBy: item.createdBy || "",
        UpdatedBy: item.updatedBy || ""
    }));


    const columns = [
        {
            title: "Action", field: "action", frozen: true, hozAlign: "center", headerSort: false, headerMenu: headerMenu,
            width: 90,
            formatter: function (cell) {
                const row = cell.getRow();
                return `<i class="fas fa-trash-alt text-danger" title="Delete" style="cursor:pointer;" onclick="deleteConfirm(${row.getData().Id})"></i>`;
            }
        },
        {
            title: "SNo",
            hozAlign: "center",
            headerSort: false,
            headerMenu: headerMenu,
            width: 100,
            formatter: function (cell) {
                // Dynamic row number considering sorting/filtering/pagination
                return cell.getRow().getPosition(true);
            }
        },
        editableColumn("Start Date/ Implementation Date", "StartDate", "date", "center"),
        editableColumn("Functional Area/process", "FunctionalArea", "input", "center"),
        editableColumn("Existing Product/Existing Issues", "Issue", "input", "center"),
        editableColumn("Problem Pertaining to ISO/EMS/OHSAS", "Problem", "input", "center"),
        editableColumn("Corrective Action", "CorrectiveAction", "input", "center"),
        editableColumn("Responsible", "Responsible", "input", "center"),
        editableColumn("Improvements/Benefits Achieved ", "ImprovementAchieved", "input", "center"),
        {
            title: "Created By", field: "CreatedBy",
            hozAlign: "center",
            headerSort: false,
            headerMenu: headerMenu,
            width: 100,
            visible: false
        },
        {
            title: "Created Date", field: "CreatedDate",
            hozAlign: "center",
            headerSort: false,
            headerMenu: headerMenu,
            width: 100,
            visible: false
        },
        {
            title: "Updated By", field: "UpdatedBy",
            hozAlign: "center",
            headerSort: false,
            headerMenu: headerMenu,
            width: 100,
            visible: false
        },
        {
            title: "Updated Date", field: "UpdatedDate",
            hozAlign: "center",
            headerSort: false,
            headerMenu: headerMenu,
            width: 100,
            visible: false
        }
       
    ];

    if (table) {
        table.replaceData(tableData);
    } else {
        table = new Tabulator("#impr_tracker_table", {
            data: tableData,
            layout: "fitData",
            renderHorizontal: "virtual",
            movableColumns: true,
            pagination: "local",
            paginationSize: 10,
            paginationSizeSelector: [50, 100, 500, 1500, 2000],
            paginationCounter: "rows",
            placeholder: "No data available", // corrected property name
            columns: columns
        });

        table.on("cellEdited", function (cell) {
            const rowData = cell.getRow().getData();
            saveRow(rowData);
        });
    }
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

function editableColumn(title, field, editor = "input", align = "center", headerFilter = null, headerFilterParams = {}, editorParams = {}, formatter = null, width = null) {
    const col = {
        title: title,
        field: field,
        editor: editor,
        hozAlign: align,
        headerHozAlign: "left",
        headerMenu: headerMenu,
        headerFilter: headerFilter,
        headerFilterParams: headerFilterParams,
        editorParams: editorParams
    };
    if (formatter) col.formatter = formatter;
    if (width) col.width = width;
    return col;
}


function toIsoDate(value) {
    if (!value) return "";
    const parts = value.split('/');
    if (parts.length === 3) {
        return `${parts[2]}-${parts[1].padStart(2, '0')}-${parts[0].padStart(2, '0')}`;
    }
    return value;
}


function saveRow(rowData) {
    const isNew = !rowData.Id || rowData.Id === 0;
    console.log(rowData);
    const payload = {
        Id: rowData.Id || 0,
        FuncArea: rowData.FunctionalArea || "",
        Issue: rowData.Issue || "",
        Problem: rowData.Problem || "",
        CorrectiveAction: rowData.CorrectiveAction || "",
        Responsible: rowData.Responsible || "",
        StartDate: toIsoDate(rowData.StartDate),
        ImprAchieved: rowData.ImprovementAchieved || ""
    };
    
    console.log(payload);
    $.ajax({
        url: isNew ? '/ImprTracker/CreateAsync' : `/ImprTracker/UpdateAsync`,
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify(payload),
        success: function (response) {
            if (response.success) {
                if (isNew) {
                    // Reload all data to get new ID and refresh
                    loadTableData();
                }
               
            } else {
                showDangerAlert(response.message || "Failed to save.");
            }
        },
        error: function (xhr) {
            showDangerAlert(xhr.responseText || "Error saving data.");
        }
    });
}

function deleteConfirm(id) {
    PNotify.prototype.options.styling = "bootstrap3";
    (new PNotify({
        title: 'Confirm Deletion',
        text: 'Are you sure you want to delete this record?',
        icon: 'fa fa-question-circle',
        hide: false,
        confirm: { confirm: true },
        buttons: { closer: false, sticker: false },
        history: { history: false }
    })).get().on('pnotify.confirm', function () {
        $.ajax({
            url: '/ImprTracker/Delete',
            type: 'POST',
            data: { id: id },
            success: function (data) {
                if (data.success) {
                    notify("Deleted successfully", "success");
                    loadTableData();
                } else {
                    showDangerAlert(data.message || "Deletion failed.");
                }
            },
            error: function () {
                showDangerAlert("Error occurred during deletion.");
            }
        });
    });
}

function notify(message, type = "success") {
    new PNotify({
        text: message,
        type: type,
        delay: 2000,
        styling: "bootstrap3"
    });
}

function showDangerAlert(message) {
    notify(message, "error");
}


