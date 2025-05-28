var table = null;
let filterStartDate = moment().startOf('year').format('YYYY-MM-DD');
let filterEndDate = moment().endOf('year').format('YYYY-MM-DD');

$(document).ready(function () {
    $('#dateRangeText').text(
        moment(filterStartDate).format('MMMM D, YYYY') + ' - ' + moment(filterEndDate).format('MMMM D, YYYY')
    );

    const picker = new Litepicker({
        element: document.getElementById('customDateTrigger'),
        singleMode: false,
        format: 'DD-MM-YYYY',
        numberOfMonths: 2,
        numberOfColumns: 2,
        dropdowns: { minYear: 2020, months: true, years: true },
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
            'This Month': [moment().startOf('month'), moment().endOf('month')],
            'This Year': [moment().startOf('year'), moment().endOf('year')],
        },
        startDate: moment().startOf('year').format('DD-MM-YYYY'),
        endDate: moment().endOf('year').format('DD-MM-YYYY')
    });

    $('#customDateTrigger').on('click', () => picker.show());
    $('#backButton').on('click', () => window.history.back());

    loadData();
});

function loadData() {
    Blockloadershow();

    $.ajax({
        url: '/SPMReport/GetVendor',
        type: 'GET'
    }).done(function (vendorData) {
        if (Array.isArray(vendorData)) {
            vendorOptions = vendorData.reduce((acc, v) => {
                acc[v.value] = v.label;
                return acc;
            }, {});
        }

        $.ajax({
            url: '/SPMReport/GetAll',
            type: 'GET',
            dataType: 'json',
            data: { startDate: filterStartDate, endDate: filterEndDate },
            success: function (data) {
                OnTabGridLoad(data || []);
                Blockloaderhide();
            },
            error: function () {
                showDangerAlert('Error loading SPM Reports');
                Blockloaderhide();
            }
        });
    }).fail(function () {
        showDangerAlert('Error loading vendor data');
        Blockloaderhide();
    });
}

function headerMenu() {
    let menu = [];
    let columns = this.getColumns();

    columns.forEach(col => {
        let icon = document.createElement("i");
        icon.classList.add("fas");
        icon.classList.add(col.isVisible() ? "fa-check-square" : "fa-square");

        let label = document.createElement("span");
        let title = document.createElement("span");
        title.textContent = " " + col.getDefinition().title;

        label.appendChild(icon);
        label.appendChild(title);

        menu.push({
            label: label,
            action: e => {
                e.stopPropagation();
                col.toggle();
                if (col.isVisible()) {
                    icon.classList.replace("fa-square", "fa-check-square");
                } else {
                    icon.classList.replace("fa-check-square", "fa-square");
                }
            }
        });
    });
    return menu;
}

function OnTabGridLoad(data) {
    const tabledata = data.map((item, index) => ({
        Sr_No: index + 1,
        ReportId: item.id,
        VendorDetail: item.vendorDetail || '',
        FY: item.fy || '',
        SPMQuarter: item.spmQuarter || '',
        FinalStarRating: item.finalStarRating,
        Top2Parameter: item.top2Parameter || '',
        Lowest2Parameter: item.lowest2Parameter || '',
        Remarks: item.remarks || '',
        CreatedBy: item.createdBy || '',
        CreatedDate: item.createdDate ? moment(item.createdDate).format('DD-MM-YYYY') : '',
        UpdatedBy: item.updatedBy || '',
        UpdatedDate: item.updatedDate ? moment(item.updatedDate).format('DD-MM-YYYY') : ''
    }));

    const columns = [
        {
            title: "Actions",
            field: "Actions",
            frozen: true,
            width: 100,
            hozAlign: "center",
            headerSort: false,
            headerMenu: headerMenu,
            formatter: function (cell) {
                const id = cell.getRow().getData().ReportId;
                return `<i onclick="deleteSPMReport(${id})" class="fas fa-trash-alt text-danger" style="cursor:pointer" title="Delete"></i>`;
            }
        },
        { title: "S.No", field: "Sr_No", frozen: true, width: 90, hozAlign: "center", headerSort: false, headerMenu: headerMenu },
        { title: "ID", field: "ReportId", frozen: true, width: 90, visible: false, hozAlign: "center", headerSort: false, headerMenu: headerMenu },
        editableColumn("Vendor", "VendorDetail", "select2", "center", "input", {}, {
            values: vendorOptions
        }, function (cell) {
            const val = cell.getValue();
            return vendorOptions[val] || val;
        }, 130),
      
        editableColumn("FY", "FY", "input", "center", "input"),
        editableColumn("SPM Quarter", "SPMQuarter", "input", "center", "input"),
        editableColumn("Final Star Rating", "FinalStarRating", "input", "center", "input"),
        editableColumn("Top 2 Parameters", "Top2Parameter", "input", "center", "input"),
        editableColumn("Lowest 2 Parameters", "Lowest2Parameter", "input", "center", "input"),
        editableColumn("Remarks", "Remarks", "input", "center", "input"),

        {
            title: "Created By", field: "CreatedBy", hozAlign: "center", headerSort: false, headerMenu: headerMenu, width: 120, visible: false
        },
        {
            title: "Created Date", field: "CreatedDate", hozAlign: "center", headerSort: false, headerMenu: headerMenu, width: 120, visible: false
        },
        {
            title: "Updated By", field: "UpdatedBy", hozAlign: "center", headerSort: false, headerMenu: headerMenu, width: 120, visible: false
        },
        {
            title: "Updated Date", field: "UpdatedDate", hozAlign: "center", headerSort: false, headerMenu: headerMenu, width: 120, visible: false
        }
    ];

    if (table) {
        table.replaceData(tabledata);
    } else {
        table = new Tabulator("#spm_table", {
            data: tabledata,
            layout: "fitDataFill",
            pagination: "local",
            paginationSize: 10,
            paginationSizeSelector: [50, 100,200,300,400,500],
            movableColumns: true,
            placeholder: "No data found",
            columns: columns
        });

        table.on("cellEdited", function (cell) {
            saveEditedRow(cell.getRow().getData());
        });
    }

    $("#addButton").on("click", function () {
        const newRow = {
            ReportId: 0,
            Sr_No: table.getDataCount() + 1,
            VendorDetail: '',
            FY: '',
            SPMQuarter: '',
            FinalStarRating: null,
            Top2Parameter: '',
            Lowest2Parameter: '',
            Remarks: ''
        };
        table.addRow(newRow, false);
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

    
    return columnDef;
}

function saveEditedRow(rowData) {
    function emptyToNull(value) {
        return value === "" ? null : value;
    }

    const cleanedData = {
        Id: rowData.ReportId || 0,
        VendorDetail: rowData.VendorDetail || null,
        FY: rowData.FY || null,
        SPMQuarter: rowData.SPMQuarter || null,
        FinalStarRating: emptyToNull(rowData.FinalStarRating),
        Top2Parameter: rowData.Top2Parameter || null,
        Lowest2Parameter: rowData.Lowest2Parameter || null,
        Remarks: rowData.Remarks || null,
      
    };
    console.log(cleanedData);
    const isNew = cleanedData.Id === 0;
    const url = isNew ? '/SPMReport/CreateAsync' : '/SPMReport/UpdateAsync';

    $.ajax({
        url: url,
        method: 'POST',
        contentType: 'application/json',
        data: JSON.stringify(cleanedData),
        success: function (response) {
            if (response.success) {
              //  showSuccessAlert(response.message || (isNew ? "Added successfully" : "Updated successfully"));
                if (isNew && response.id) {
                    rowData.ReportId = response.id; // Update the ID in the row
                }
                loadData();
            } else {
                showDangerAlert(response.message || "Failed to save");
            }
        },
        error: function () {
            showDangerAlert("Server error while saving");
        }
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

function deleteSPMReport(id) {
    PNotify.prototype.options.styling = "bootstrap3";
    (new PNotify({
        title: 'Confirm Deletion',
        text: 'Are you sure you want to delete this SPM Report?',
        icon: 'fa fa-question-circle',
        hide: false,
        confirm: { confirm: true },
        buttons: { closer: false, sticker: false },
        history: { history: false }
    })).get().on('pnotify.confirm', function () {
        $.ajax({
            url: '/SPMReport/Delete',
            method: 'POST',
            data: { id: id },
            success: function (response) {
                if (response.success) {
                    showSuccessAlert(response.message || 'Deleted successfully');
                    loadData();
                } else {
                    showDangerAlert(response.message || 'Delete failed');
                }
            },
            error: function () {
                showDangerAlert('Server error while deleting');
            }
        });
    });
}


