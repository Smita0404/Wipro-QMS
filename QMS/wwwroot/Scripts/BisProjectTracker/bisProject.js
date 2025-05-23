var tabledata = [];
var table = '';
const searchTerms = {};
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
        url: '/BisProjectTrac/GetAll',
        type: 'GET',
        success: function (data) {
            Blockloaderhide();
            if (data && Array.isArray(data)) {
                OnTabGridLoad(data);
            }
            else {
                showDangerAlert('No data available to load.');
            }
        },
        error: function (xhr, status, error) {
            showDangerAlert('Error retrieving data: ' + error);
            Blockloaderhide();
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
                Financial_Year: item.fin_Year,
                Mon_PC: item.mon_PC,
                Nat_Project: item.nat_Project,
                Lea_Model_No: item.lea_Model_No,
                No_Seri_Add: item.no_Seri_Add,
                Cat_Ref_Lea_Model: item.cat_Ref_Lea_Model,
                Section: item.section,
                Manuf_Location: item.manuf_Location,
                CCL_Id: item.cCL_Id,
                Lab: item.lab,
                Report_Owner: item.report_Owner,
                Start_Date: formatDate(item.Start_Date),
                Comp_Date: formatDate(item.Comp_Date),
                Test_Duration: item.Test_Duration,
                Submitted_Date: formatDate(item.Submitted_Date),
                Received_Date: formatDate(item.Received_Date),
                Bis_Duration: item.Bis_Duration,
                Dispatch_Date: formatDate(item.Dispatch_Date),
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
        { title: "Date", field: "CreatedDate", sorter: "date", headerMenu: headerMenu, headerFilter: "input", hozAlign: "left", headerHozAlign: "center" },
        { title: "Financial Year", field: "Financial_Year", headerMenu: headerMenu, headerFilter: "input", hozAlign: "left", headerHozAlign: "left" },
        { title: "Month/PC", field: "Mon_PC", headerMenu: headerMenu, headerFilter: "input", hozAlign: "center", headerHozAlign: "center" },
        { title: "Nature of Project", field: "Nat_Project", headerMenu: headerMenu, headerFilter: "input", hozAlign: "center", headerHozAlign: "center" },
        { title: "Lead Model Number", field: "Lea_Model_No", sorter: "number", headerMenu: headerMenu, headerFilter: "input", hozAlign: "center", headerHozAlign: "center" },
        { title: "No of Series Added", field: "No_Seri_Add", sorter: "number", headerMenu: headerMenu, headerFilter: "input", hozAlign: "center", headerHozAlign: "center" },
        { title: "Cat Reference of Lead Model", field: "Cat_Ref_Lea_Model", sorter: "number", headerMenu: headerMenu, headerFilter: "input", hozAlign: "center", headerHozAlign: "center" },
        { title: "Section", field: "Section", sorter: "number", headerMenu: headerMenu, headerFilter: "input", hozAlign: "center", headerHozAlign: "center" },
        { title: "Manufacturing Location", field: "Manuf_Location", sorter: "number", headerMenu: headerMenu, headerFilter: "input", hozAlign: "center", headerHozAlign: "center" },
        { title: "CCL ID", field: "CCL_Id", sorter: "number", headerMenu: headerMenu, headerFilter: "input", hozAlign: "center", headerHozAlign: "center" },
        { title: "Lab", field: "Lab", sorter: "number", headerMenu: headerMenu, headerFilter: "input", hozAlign: "center", headerHozAlign: "center" },
        { title: "Report Owner", field: "Report_Owner", sorter: "number", headerMenu: headerMenu, headerFilter: "input", hozAlign: "center", headerHozAlign: "center" },
        { title: "Test Start Date", field: "Start_Date", sorter: "date", headerMenu: headerMenu, headerFilter: "input", hozAlign: "left", headerHozAlign: "center" },
        { title: "Test Complete Date", field: "Comp_Date", sorter: "date", headerMenu: headerMenu, headerFilter: "input", hozAlign: "left", headerHozAlign: "center" },
        { title: "Test Duration", field: "Test_Duration", sorter: "number", headerMenu: headerMenu, headerFilter: "input", hozAlign: "center", headerHozAlign: "center" },
        { title: "Bis Submitted Date", field: "Submitted_Date", sorter: "date", headerMenu: headerMenu, headerFilter: "input", hozAlign: "left", headerHozAlign: "center" },
        { title: "Bis Received Date", field: "Received_Date", sorter: "date", headerMenu: headerMenu, headerFilter: "input", hozAlign: "left", headerHozAlign: "center" },
        { title: "Bis Duration", field: "Bis_Duration", sorter: "number", headerMenu: headerMenu, headerFilter: "input", hozAlign: "center", headerHozAlign: "center" },
        { title: "First Dispatch Date", field: "Dispatch_Date", sorter: "date", headerMenu: headerMenu, headerFilter: "input", hozAlign: "left", headerHozAlign: "center" },
        { title: "User", field: "CreatedBy", headerMenu: headerMenu, headerFilter: "input", hozAlign: "center", headerHozAlign: "center" },
        { title: "Updated By", field: "UpdatedBy", headerMenu: headerMenu, headerFilter: "input", hozAlign: "center", headerHozAlign: "center", visible: false },
        { title: "Update Date", field: "UpdatedDate", sorter: "date", headerMenu: headerMenu, headerFilter: "input", hozAlign: "center", headerHozAlign: "center", visible: false },
    );

    // // Initialize Tabulator
    table = new Tabulator("#bisProject_Table", {
        data: tabledata,
        renderHorizontal: "virtual",
        movableColumns: true,
        pagination: "local",
        paginationSize: 10,
        paginationSizeSelector: [50, 100, 500, 1500, 2000],
        paginationCounter: "rows",
        dataEmpty: "<div style='text-align: center; font-size: 1rem; color: gray;'>No data available</div>", // Placeholder message
        columns: columns
    });

    table.on("cellClick", function (e, cell) {
        let columnField = cell.getColumn().getField();

        if (columnField !== "Action") {
            let rowData = cell.getRow().getData();
            showEditBisProject(rowData.Id);
        }
    });

    // Export to Excel on button click
    // document.getElementById("exportExcel").addEventListener("click", function () {
    //     table.download("xlsx", "ProductCode_Data.xlsx", { sheetName: "Product Code Data" });
    // });


    Blockloaderhide();
}

function showEditBisProject(id) {
    debugger
    var url = '/BisProjectTrac/BisProjectTracker?id=' + id;
    window.location.href = url;
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

function openBisProject(id) {
    debugger
    var url = '/BisProjectTrac/BisProjectTrackerDetail';
    url = url + '?id=' + id
    window.location.href = url;
}
