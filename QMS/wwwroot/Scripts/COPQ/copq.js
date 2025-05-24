$(document).ready(function () {
    $("#div1").show();
    $("#div2Grid").show();
    $("#div3Form").hide();
    loadData();
});
function openCOPQForm() {
    $("#div1").hide();
    $("#div2Grid").hide();
    $("#div3Form").show();
}

function loadData() {
    Blockloadershow();
    $.ajax({
        url: '/COPQ/GetAll',
        type: 'GET',
        success: function (data) {
            Blockloaderhide();
            if (data && Array.isArray(data)) {
                OnCOPQGridLoad(data);
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

function OnCOPQGridLoad(response) {
    debugger;
    Blockloadershow();

    let tabledata = [];
    let columns = [];

  
        response.forEach((item, index) => {
            let cccnDate = item.cccnDate ? new Date(item.cccnDate).toLocaleDateString("en-GB") : "";
            let createdDate = item.createdDate ? new Date(item.createdDate).toLocaleDateString("en-GB") : "";
            let updatedDate = item.updatedDate ? new Date(item.updatedDate).toLocaleDateString("en-GB") : "";

            tabledata.push({
                Sr_No: index + 1,
                Id: item.id,
                CCCNDate: cccnDate,
                ReportedBy: item.reportedBy,
                Location: item.cLocation,
                CustName: item.custName,
                DealerName: item.dealerName,
                Description: item.cDescription,
                Status: item.cStatus,
                Completion: item.completion,
                CreatedDate: createdDate,
                CreatedBy: item.createdBy,
                UpdatedDate: updatedDate,
                UpdatedBy: item.updatedBy,
                Remarks: item.remarks,
                IsDeleted: item.isDeleted
            });
        });

        columns = [
            {
                title: "Action",
                field: "Action",
                hozAlign: "center",
                headerHozAlign: "center", frozen: true,
                formatter: function (cell) {
                    const rowData = cell.getRow().getData();
                    return `
                        <i data-toggle="modal" onclick="delConfirm(${rowData.Id})" class="fas fa-trash-alt mr-2 fa-1x" title="Delete" style="color:red;cursor:pointer;margin-left: 5px;"></i>
                        
                    `;
                }
            },
            { title: "SNo", field: "Sr_No", frozen: true, sorter: "number", headerMenu: headerMenu, hozAlign: "center", headerHozAlign: "center" },
            { title: "CCCN Date", field: "CCCNDate", sorter: "date", headerMenu: headerMenu, headerFilter: "input", hozAlign: "left", headerHozAlign: "center" },
            { title: "Reported By", field: "ReportedBy", headerMenu: headerMenu, headerFilter: "input", hozAlign: "left", headerHozAlign: "center" },
            { title: "Location", field: "Location", headerMenu: headerMenu, headerFilter: "input", hozAlign: "left", headerHozAlign: "center" },
            { title: "Customer Name", field: "CustName", headerMenu: headerMenu, headerFilter: "input", hozAlign: "left", headerHozAlign: "center" },
            { title: "Dealer Name", field: "DealerName", headerMenu: headerMenu, headerFilter: "input", hozAlign: "left", headerHozAlign: "center" },
            { title: "Description", field: "Description", headerMenu: headerMenu, headerFilter: "input", hozAlign: "left", headerHozAlign: "center" },
            { title: "Status", field: "Status", headerMenu: headerMenu, headerFilter: "input", hozAlign: "left", headerHozAlign: "center" },
            { title: "Completion", field: "Completion", headerMenu: headerMenu, headerFilter: "input", hozAlign: "left",  headerSort: false, headerHozAlign: "center" },
            { title: "Created Date", field: "CreatedDate", sorter: "date", headerMenu: headerMenu, headerFilter: "input", headerSort: false, hozAlign: "left", headerHozAlign: "center", visible: false },
            { title: "Created By", field: "CreatedBy", headerMenu: headerMenu, headerFilter: "input", hozAlign: "left", headerSort: false, headerHozAlign: "center", visible: false },
            { title: "Updated Date", field: "UpdatedDate", sorter: "date", headerMenu: headerMenu, headerFilter: "input", hozAlign: "left", headerSort: false, headerHozAlign: "center", visible: false },
            { title: "Updated By", field: "UpdatedBy", headerMenu: headerMenu, headerFilter: "input", hozAlign: "left", headerHozAlign: "center", headerSort: false, visible: false },
            { title: "Remarks", field: "Remarks", headerMenu: headerMenu, headerFilter: "input", hozAlign: "left", headerHozAlign: "center" }
           
        ];

        // Initialize Tabulator
        table = new Tabulator("#copq-table", {
            data: tabledata,
            layout: "fitDataFill",
            movableColumns: true,
            pagination: "local",
            paginationSize: 10,
            paginationSizeSelector: [10, 50, 100, 500],
            paginationCounter: "rows",
            placeholder: "No data available",
            columns: columns,
            dataEmpty: "<div style='text-align: center; font-size: 1rem; color: gray;'>No data available</div>"
        });

        table.on("cellClick", function (e, cell) {
            // Prevent triggering edit if clicking on the Action buttons
            if (cell.getField() !== "Action") {
                const rowData = cell.getRow().getData();

                // Set button for update mode
                $("#saveButton").html('<i class="fas fa-edit mr-2"></i>Update');
                $("#div1").hide();
                $("#div2Grid").hide();
                $("#div3Form").show();
                // Load data into form for editing
                editComplaint(rowData.Id);
            }
        });

    

    Blockloaderhide();
}
function editComplaint(id) {
    $.ajax({
        url: '/COPQ/GetById', // Adjust to your actual controller route
        type: 'GET',
        data: { id: id },
        success: function (data) {
            if (data) {
                // Populate form fields
                $('#Id').val(data.id);
                $('#CCCNDate').val(data.cccnDate ? moment(data.cccnDate).format('YYYY-MM-DD') : '');
                $('#ReportedBy').val(data.reportedBy);
                $('#CLocation').val(data.cLocation);
                $('#CustName').val(data.custName);
                $('#DealerName').val(data.dealerName);
                $('#CDescription').val(data.cDescription);
                $('#CStatus').val(data.cStatus);
                $('#Completion').val(data.completion);
                $('#Remarks').val(data.remarks);

                // Show the modal or form
                $('#complaintModal').modal('show');
            } else {
                PNotify.error({ text: 'Complaint not found.' });
            }
        },
        error: function (err) {
            console.error('Error fetching complaint:', err);
            PNotify.error({ text: 'Error fetching complaint data.' });
        }
    });
}

function InsertUpdateDetail() {
    // Clear previous errors
    $("#Date_Error").text("");

    // Collect form data
    const model = {
        id: $("#Id").val() || 0, // Assuming you store Id in a hidden input or set to 0 for insert
        cccnDate: $("#CCCNDate").val(),
        reportedBy: $("#ReportedBy").val(),
        cLocation: $("#Location").val(),
        custName: $("#CustomerName").val(),
        dealerName: $("#DealerName").val(),
        cDescription: $("#Description").val(),
        cStatus: $("#Status").val(),
        completion: $("#Completion").val(),
        remarks: $("#ClosureRemarks").val(),
        // Add other fields if needed
    };

    // Basic validation
    if (!model.cccnDate) {
        $("#Date_Error").text("Please select CCCN Date.");
        return;
    }

    // Decide URL and method depending on Insert or Update
    const isUpdate = model.id && model.id > 0;
    const url = isUpdate ? "/COPQ/UpdateAsync" : "/COPQ/CreateAsync";
   
    $.ajax({
        type: "POST",
        url: url,
        contentType: "application/json",
        data: JSON.stringify(model),
        success: function (response) {
            if (response.success) {
                showSuccessAlert(response.message || "Saved successfully.");
                $("#div1").show();
                $("#div2Grid").show();
                $("#div3Form").hide();
                loadData(); clearComplaintForm();
            } else {
                showDangerAlert(response.message || "Failed to save.");
            }
        },
        error: function (xhr, status, error) {
            showDangerAlert("Error occurred while saving: " + error);
        }
    });
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
function clearComplaintForm() {
    $("#Id").val('');
    $("#CCCNDate").val('');
    $("#ReportedBy").val('');
    $("#Location").val('');
    $("#CustomerName").val('');
    $("#DealerName").val('');
    $("#Description").val('');
    $("#Status").val('');
    $("#Completion").val('');
    $("#ClosureRemarks").val('');
    $("#saveButton").html('<i class="fas fa-floppy-disk mr-2"></i>Save');
}
function delConfirm(Id) {
   // console.log(Id);
    PNotify.prototype.options.styling = "bootstrap3";
    (new PNotify({
        title: 'Confirm Deletion',
        text: 'Are you sure you want to delete?',
        icon: 'fa fa-question-circle',
        hide: false,
        confirm: { confirm: true },
        buttons: { closer: false, sticker: false },
        history: { history: false }
    })).get().on('pnotify.confirm', function () {
        $.ajax({
            url: '/COPQ/Delete',
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
