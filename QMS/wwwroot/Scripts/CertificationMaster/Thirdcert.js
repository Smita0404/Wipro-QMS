
$(document).ready(function () {
    $('#BbackButton').on('click', function () {
        //var url = '/ThirdPartyCertificate/Index';
        // window.location.href = url;
        $('#addButton').show(); // Hide the Add button
        $('#SaveButton').hide(); // Show the Save button
        $('#backButton').show(); // Ensure the Back button is visible
        $('#divCert').hide();
    });
   
    
    $('#addButton').show(); // Hide the Add button
    $('#SaveButton').hide(); // Show the Save button
   $('#backButton').show(); // Ensure the Back button is visible
    $('#divCert').hide();
    loadData();
});

function ShowDetail() {

    $('#addButton').hide(); // Hide the Add button
    $('#SaveButton').show(); // Show the Save button
    $('#divCert').show();
    $('#backButton').hide();
}
function loadData() {
    Blockloadershow();
    $.ajax({
        url: '/ThirdPartyCertificate/GetAll',
        type: 'GET',
        success: function (data) {
            Blockloaderhide();
            if (data && Array.isArray(data.data)) {
                OnTabGridLoad(data.data);
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
function updateCert(certId) {
    var certName = prompt("Enter new certificate name:");

    if (!certName) {
        showDangerAlert("Certificate name cannot be empty.");
        return;
    }

    var model = {
        CertificateID: certId,
        CertificateName: certName
    };

    $.ajax({
        url: '/ThirdPartyCertificate/Update',
        type: 'POST',
        data: model,
        success: function (response) {
            if (response.success) {
                showSuccessAlert(response.message);
                loadData(); // Refresh table
            } else {
                showDangerAlert(response.message);
            }
        },
        error: function () {
            showDangerAlert("Unexpected error during update.");
        }
    });
}

function deleteCert(certificateId) {
    PNotify.prototype.options.styling = "bootstrap3";

    new PNotify({
        title: 'Confirmation Needed',
        text: 'Are you sure you want to delete this certificate? It will not delete if this record is used in transactions.',
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
        }
    }).get().on('pnotify.confirm', function () {
        $.ajax({
            url: '/ThirdPartyCertificate/Delete',
            type: 'POST',
            data: { id: certificateId },
            success: function (data) {
                if (data.success === true) {
                    showSuccessAlert("Certificate deleted successfully.");
                    setTimeout(function () {
                        loadData(); // Refresh the table
                    }, 1500);
                } else if (data.success === false && data.message === "Not_Deleted") {
                    showDangerAlert("Certificate is used in related transactions and cannot be deleted.");
                } else {
                    showDangerAlert(data.message || "An unexpected error occurred.");
                }
            },
            error: function () {
                showDangerAlert('Error occurred during deletion.');
            }
        });
    }).on('pnotify.cancel', function () {
        loadData(); // Reload in case anything changed
    });
}

function editCert(certID, certName) {
    ShowDetail();
    $("#CertNameInput").val(certName);  // Set the certificate name in the input
    $("#CertIDInput").val(certID);      // Set the certificate ID in the hidden field
    $("#SaveButton").html('<i class="fas fa-edit mr-2"></i>Update');
}

var table;

function OnTabGridLoad(data) {
    if (table) {
        table.replaceData(data);
    } else {
        table = new Tabulator("#certTable", {
            data: data,
            layout: "fitColumns",
            movableColumns: true,
            responsiveLayout: "collapse",
            pagination: "local",
            paginationSize: 10,
            paginationCounter: "rows",
            paginationSizeSelector: [10, 25, 100, 500, 1200, 2000],
            placeholder: "No Data Available",
            columns: [
                {
                    title: "Sr",
                    formatter: "rownum",
                    hozAlign: "center",
                    width: 70
                },
                {
                    title: "Action", field: "certificateID", hozAlign: "right", headerHozAlign: "right", width: 100, formatter: function (cell) {
                        var id = cell.getValue();
                        return `
                            <div style="margin-right:50px">
                                <i class="fas fa-trash-alt text-danger cert-delete" style="cursor:pointer;font-size: 16px;" title="Delete" onclick="event.stopPropagation(); deleteCert(${id});"></i>
                            </div>`;
                    }
                },
                { title: "ID", field: "certificateID", visible: false },
                {
                    title: "Report Name",
                    field: "certificateName",
                    headerFilter: "input",
                    formatter: function (cell) {
                        const rowData = cell.getRow().getData();
                        const id = rowData.certificateID;
                        const name = rowData.certificateName.replace(/'/g, "\\'");
                        return `<a href="javascript:void(0);" onclick="editCert(${id}, '${name}')">${name}</a>`;
                    }
                }
            ],
            rowClick: function (e, row) {
                var rowData = row.getData();
                ShowDetail()
                console.log("Row clicked:", rowData);
            },
            tableBuilt: function () {
                // Apply Bootstrap small form control styling to header filters
                document.querySelectorAll("#certTable .tabulator-header-filter input").forEach(el => {
                    el.classList.add("form-control", "form-control-sm");
                });
            }
        });
    }
}

function InsertUpdateCert() {
    var certName = $("#CertNameInput").val().trim();
    var certID = $("#CertIDInput").val();  // Get the certificate ID

    // Ensure the certificate name is provided
    if (!certName) {
        showDangerAlert("Please enter a certificate name.");
        return;
    }

    var model = {
        CertificateName: certName,
        Id: certID || 0  // If certID is empty, it indicates new, else it's update
    };

    var url = certID ? "/ThirdPartyCertificate/Update" : "/ThirdPartyCertificate/Index"; // Use different URL for update or create

    $.ajax({
        type: "POST",
        url: url,  // The endpoint for creating or updating a certificate
        data: model,
        success: function (response) {
            if (response.success) {
                showSuccessAlert(certID ? "Certificate updated successfully." : "Certificate added successfully.");
                $("#CertNameInput").val("");  // Clear input after success
                $("#CertIDInput").val("");   // Clear certificate ID after success
                $("#SaveButton").html('<i class="fas fa-floppy-disk mr-2"></i>Save');
                $('#addButton').show(); // Hide the Add button
                $('#SaveButton').hide(); // Show the Save button
                $('#backButton').show(); // Ensure the Back button is visible
                $('#divCert').hide();
                     loadData();
            } else {
                showDangerAlert(response.message);
            }
        },
        error: function () {
            showDangerAlert("Unexpected error. Please try again.");
        }
    });
}
