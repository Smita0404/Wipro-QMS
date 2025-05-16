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
        url: '/Vendor/GetAll',
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

            let formattedDate = "";
            let updatedDate = "";
            let date_of_Issue = "";
            let rev_date = "";
            if (item.createdDate) {
                const dateObj = new Date(item.createdDate);
                formattedDate = dateObj.toLocaleDateString("en-GB");
            }
            if (item.updatedDate) {
                const updatedateObj = new Date(item.updatedDate);
                updatedDate = updatedateObj.toLocaleDateString("en-GB");
            }

            tabledata.push({
                Sr_No: index + 1,
                Id: item.id,
                Name: item.name,
                Email: item.email,
                MobileNo: item.mobileNo,
                GstNo: item.gstNo,
                Contact_Persons: item.contact_Persons,
                User_Name: item.user_Name,
                CreatedDate: formattedDate,
                CreatedBy: item.createdBy,
                UpdatedBy: item.updatedBy,
                UpdatedDate: updatedDate
            });
        });

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
            { title: "Name", field: "Name", sorter: "date", headerMenu: headerMenu, headerFilter: "input", hozAlign: "left", headerHozAlign: "center" },
            { title: "Email", field: "Email", headerMenu: headerMenu, headerFilter: "input", hozAlign: "left", headerHozAlign: "left" },
            { title: "Mobile No", field: "MobileNo", headerMenu: headerMenu, headerFilter: "input", hozAlign: "center", headerHozAlign: "center" },
            { title: "Gst No", field: "GstNo", headerMenu: headerMenu, headerFilter: "input", hozAlign: "center", headerHozAlign: "center" },
            { title: "User Name", field: "User_Name", sorter: "number", headerMenu: headerMenu, headerFilter: "input", hozAlign: "center", headerHozAlign: "center" },
            { title: "User", field: "CreatedBy", headerMenu: headerMenu, headerFilter: "input", hozAlign: "center", headerHozAlign: "center" },
            { title: "Create Date", field: "CreatedDate", sorter: "date", headerMenu: headerMenu, headerFilter: "input", hozAlign: "center", headerHozAlign: "center", visible: false },
            { title: "Updated By", field: "UpdatedBy", headerMenu: headerMenu, headerFilter: "input", hozAlign: "center", headerHozAlign: "center", visible: false },
            { title: "Update Date", field: "UpdatedDate", sorter: "date", headerMenu: headerMenu, headerFilter: "input", hozAlign: "center", headerHozAlign: "center", visible: false },
        );

        // // Initialize Tabulator
        table = new Tabulator("#ven_Table", {
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
                showEditVendor(rowData.Id);
            }
        });

        // Export to Excel on button click
        // document.getElementById("exportExcel").addEventListener("click", function () {
        //     table.download("xlsx", "ProductCode_Data.xlsx", { sheetName: "Product Code Data" });
        // });
    }
    else {
        showDangerAlert('No data available.');
    }

    // Hide loader
    Blockloaderhide();
}

function showEditVendor(id) {
    debugger
    var url = '/Vendor/VendorDetail?id=' + id;
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
            url: '/Vendor/Delete',
            type: 'POST',
            data: { id: recid },
            success: function (data) {
                if (data.success == true) {
                    showSuccessAlert("CPSLog is Deleted successfully.");
                    setTimeout(function () {
                        window.location.reload();
                    }, 2500);
                }
                else if (data.success == false && data.message == "Not_Deleted") {
                    showDangerAlert("Record is used in PPS Log transactions.");
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

function openVendor(id) {
    debugger
    var url = '/Vendor/VendorDetail';
    url = url + '?id=' + id
    window.location.href = url;
}
