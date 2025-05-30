var tabledata = [];
var table = '';
var certificateTable;
$(document).ready(function () {
    loadCertificateData();
   
    loadCertificateDropdown();

    const inputElement = $("#sear_Certi_Code");
    if (inputElement.length) {
        // Attach the keydown event handler
        inputElement.on('keydown', function (event) {
            handleCertiSearch(event);
        });
    }

    $('#certificateForm').on('keypress', function (e) {
        if (e.key === 'Enter') {
            e.preventDefault();
        }
    });
});

let searchTimeout; // Timeout variable for debounce

function handleCertiSearch(event) {
    debugger
    if (event.key !== 'Enter') return;

    clearTimeout(searchTimeout);
    let processedQuery = '';
    let searchQuery = event.target.value.trim(); // Get value from input field
    processedQuery = searchQuery.substring(0, 4);

    let searchTerms = processedQuery;

    // Debounce to reduce the frequency of API calls
    searchTimeout = setTimeout(() => {
        pagedData = {}; // Clear cached data
        // Load data with the processed query
        fetchCertiRelatedData(searchTerms, event.target);
    }, 300); // Delay of 300ms
}

function fetchCertiRelatedData(productCatNo, inputField) {
    debugger
    $.ajax({
        url: '/Vendor/GetCodeSearch', // API endpoint
        type: 'GET', // HTTP method
        data: { search: productCatNo }, // Query parameters
        dataType: 'json', // Expected response data type
        success: function (data) {
            if (data && data.length > 0) {
                displayCertiSuggestions(data, inputField);
            } else {
                showDangerAlert('No data found for the entered Product Cat No.');
            }
        },
        error: function (xhr, status, error) {
            console.error('Error fetching data:', error);
        }
    });
}


// Function to display suggestions in a dropdown
function displayCertiSuggestions(data, inputField) {
    debugger
    // Create a dropdown container
    const dropdown = document.createElement('div');
    dropdown.className = 'suggestion-dropdown';
    dropdown.style.position = 'absolute';
    dropdown.style.backgroundColor = 'white';
    dropdown.style.border = '1px solid #ccc';
    dropdown.style.zIndex = 1000;
    dropdown.style.overflowY = 'auto'; // Enable vertical scrolling
    dropdown.style.maxHeight = '300px'; // Set a fixed height for the dropdown
    dropdown.style.width = '190px'; // Set a fixed height for the dropdown
    dropdown.style.boxShadow = '0px 4px 6px rgba(0, 0, 0, 0.1)'; // Add some shadow for better visibility

    // Position the dropdown relative to the input field
    const rect = inputField.getBoundingClientRect(); 
    //parentContainer.style.position = 'relative';

    //dropdown.style.top = `${parentContainer.bottom + window.scrollY}px`;
    //dropdown.style.left = `${parentContainer.left + window.scrollX}px`;
    // dropdown.style.width = `${inputField.offsetWidth}px`;

    // Populate dropdown with suggestions
    data.forEach(item => {
        const option = document.createElement('div');
        option.className = 'suggestion-item';
        option.textContent = `${item.oldPart_No}`;
        option.style.padding = '8px';
        option.style.cursor = 'pointer';

        // Add hover effect
        option.addEventListener('mouseover', () => {
            option.style.backgroundColor = '#4682B4'; // Light gray background on hover
        });
        option.addEventListener('mouseout', () => {
            option.style.backgroundColor = ''; // Reset background color
        });

        // On clicking a suggestion, set it as the value of the input with ID "NewPart_No"
        option.addEventListener('click', () => {
            const oldPartNoInput = document.getElementById('certi_productCodeInput');

            if (oldPartNoInput) {
                oldPartNoInput.value = item.oldPart_No;
            }
            //if (newDescriptionInput && oldDescriptionInput) {
            //    oldDescriptionInput.value = item.description;
            //    newDescriptionInput.value = item.description;
            //}

            dropdown.remove(); // Remove dropdown after selection
        });

        dropdown.appendChild(option);
    });

    // Remove existing dropdown if any and append the new one
    removeExistingDropdown(inputField);
    inputField.parentElement.appendChild(dropdown); // Append dropdown inside the parent element of the input field
}

// Remove any existing dropdown
function removeExistingDropdown() {
    const existingDropdown = document.querySelector('.suggestion-dropdown');
    if (existingDropdown) {
        existingDropdown.remove();
    }
}

// Remove dropdown on outside click
document.addEventListener('click', function (e) {
    if (!e.target.closest('.suggestion-dropdown') && !e.target.closest('#sear_Certi_Code')) {
        removeExistingDropdown();
    }
});

function loadCertificateData() {
    Blockloadershow();
    $.ajax({
        url: '/Vendor/GetCertificateData',
        type: 'GET',
        success: function (response) {
            Blockloaderhide();
            if (response.success && Array.isArray(response.data)) {
                initializeCertificateTable(response.data);
                console.log(response);
            } else {
                showDangerAlert(response.message || 'No data available to load.');
            }
        },
        error: function (xhr, status, error) {
            Blockloaderhide();
            showDangerAlert('Error retrieving data: ' + error);
        }
    });
}
function deleteCertificateVedn(id) {
    //event.preventDefault();  // Prevent form submission
    console.log('Deleting certificate with ID: ' + id); // Log the ID being passed

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
        console.log('Proceeding with certificate deletion'); // Log when deletion proceeds
        $.ajax({
            url: '/Vendor/DeleteCertificate', // Ensure URL matches exactly
            type: 'POST',
            data: { id: id },
            success: function (data) {
                if (data.success === true) {
                    showSuccessAlert("Certificate deleted successfully.");
                    loadCertificateData();
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
        loadCertificateData(); // Optional: reload data if needed
    });
}
function ShowpopupCertDetail() {
    // Reset form data on modal open
    $('#certificateForm')[0].reset(); // Clear the form on opening the modal
}

// Capture product code from input to hidden field
//$('#productCodeInput').on('input', function () {
//    $('#productCodeValue').val($(this).val());
//});

// Form submission

var headerMenu = function () {
    var menu = [];
    var columns = this.getColumns();

    for (let column of columns) {
        let columnTitle = column.getDefinition().title;
        //create checkbox element using font awesome icons
        let icon = document.createElement("i");
        icon.classList.add("fas");

        let noTickColumns = ["Sur", "Cess"];

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
                if (noTickColumns.includes(columnTitle)) {
                    icon.classList.remove("fa-check-square");
                    icon.classList.add("fa-square");
                }
                else {
                    if (column.isVisible()) {
                        icon.classList.remove("fa-square");
                        icon.classList.add("fa-check-square");
                    } else {
                        icon.classList.remove("fa-check-square");
                        icon.classList.add("fa-square");
                    }
                }

            }
        });
    }

    return menu;
};
function clearCertificateForm() {
    $('#vendorCertID').val(0); // Reset ID to indicate insert mode
    $('#productCodeInput').val('');
    $('#issueDate').val('');
    $('#expiryDate').val('');
    $('#remarks').val('');
    $('#certUpload').val(''); // Clear file input
    $('#certificateID').val('').trigger('change'); // Reset certificate dropdown

    // Clear attachment fields
    $('#remainingAttachments').val('');
    $('#Attachments').val('');
    $('#attachmentLinks').empty(); // Clear file links section
    $("#sear_Certi_Code").val('');
}
function InsertUpdateVendorcert(event) {
    event.preventDefault();  // Prevent form submission

    // Gather field values
    const vendorCode = $('#hdnVendorCode').val().trim();
    let vendorID = $('#hdnId').val();
  

    const certificateMasterId = $('#certificateID').val();
    const productCode = $('#productCodeInput').val().trim();
    const issueDate = $('#issueDate').val();
    const expiryDate = $('#expiryDate').val();
    const remarks = $('#remarks').val().trim();
    const certId = $('#vendorCertID').val();
    const isInsert = certId == "" || certId == "0";

    // Validation
    const errors = [];
   /* if (!vendorID) errors.push("Vendor ID is required.");*/
    if (!vendorCode) errors.push("Vendor Code is required.");
    //if (!certificateMasterId) errors.push("Please select a certificate.");
    //if (!productCode) errors.push("Product Code is required.");
    //if (!issueDate) errors.push("Issue Date is required.");
    //if (!expiryDate) errors.push("Expiry Date is required.");
    //if (new Date(issueDate) > new Date(expiryDate)) errors.push("Issue Date cannot be after Expiry Date.");
    //if (!remarks) errors.push("Remarks are required.");

    const fileInput = $('#certUpload')[0];
    if (isInsert && fileInput.files.length === 0) {
        errors.push("Certificate file is required.");
    }

    if (errors.length > 0) {
        showDangerAlert(errors.join("<br>"));
        return;
    }

    // Prepare FormData
    const formData = new FormData();
    formData.append("VendorCode", vendorCode);
    formData.append("VendorID", vendorID);
    formData.append("CertificateMasterId", certificateMasterId);
    formData.append("ProductCode", productCode);
    formData.append("IssueDate", issueDate);
    formData.append("ExpiryDate", expiryDate);
    formData.append("Remarks", remarks);
    formData.append("Id", certId);

    if (fileInput.files.length > 0) {
        formData.append("certUpload", fileInput.files[0]);
    }

    $.ajax({
        type: "POST",
        url: '/Vendor/CreateCertificate',
        data: formData,
        contentType: false,
        processData: false,
        success: function (res) {
            if (res.success) {
                showSuccessAlert(res.message || "Certificate saved successfully.");
                $('#certificateModal').modal('hide');
                $('#certificateModalTitle').html('<strong>Add Certificate</strong>');
                $('#certificateSaveBtn').html('<i class="fas fa-plus mr-2"></i>Save');
                clearCertificateForm();
                loadCertificateData();
            } else {
                showDangerAlert(res.message || "Failed to save certificate.");
            }
        },
        error: function () {
            showDangerAlert("Error while saving certificate.");
        }
    });
}

// Show Success Alert (example)
function showSuccessAlert(message) {
    new Noty({
        type: 'success',
        layout: 'topRight',
        text: message,
        timeout: 3000
    }).show();
}

// Show Error Alert (example)
function showDangerAlert(message) {
    new Noty({
        type: 'error',
        layout: 'topRight',
        text: message,
        timeout: 3000
    }).show();
}

function loadCertificateDropdown() {
    $.ajax({
        url: '/Certification/GetAll',
        method: 'GET',
        success: function (response) {
            console.log('Response:', response);

            // guard against unexpected formats
            if (!response.success || !Array.isArray(response.data)) {
                console.warn('Unexpected response format', response);
                return;
            }

            const certList = response.data;
            const dd = document.getElementById('certificateID');
            dd.innerHTML = '';  // clear existing options

            // default placeholder
            const defaultOption = document.createElement('option');
            defaultOption.value = '';
            defaultOption.textContent = 'Select Certificate';
            dd.appendChild(defaultOption);

            certList.forEach(cert => {
                const option = document.createElement('option');
                option.value = cert.certificateID;
                option.textContent = cert.certificateName;
                dd.appendChild(option);
            });
        },
        error: function (err) {
            console.error('Error loading certificates:', err);
        }
    });
}
$('#attachmentLinks').on('click', '.remove-attachment', function () {
    const fileToRemove = $(this).data('file');
    let currentFiles = $('#remainingAttachments').val().split(';').filter(f => f.trim());
    currentFiles = currentFiles.filter(f => f !== fileToRemove);
    $('#remainingAttachments').val(currentFiles.join(';'));
    $(this).closest('div').remove(); // Remove link from UI
});

function initializeCertificateTable(data) {
    if (certificateTable) {
        certificateTable.replaceData(data);
    } else {
        certificateTable = new Tabulator("#CertificateTable", {
            data: data,
            layout: "fitColumns",
            renderHorizontal: "virtual",
            movableColumns: true,
            pagination: "local",
            paginationSize: 10,
            paginationSizeSelector: [10, 50, 100, 500],
            paginationCounter: "rows",
            placeholder: "No data available",
            columns: [
                { title: "Sn", formatter: "rownum", width: 90, headerMenu: headerMenu, frozen: true, hozAlign: "center" },
                {
                    title: "Action",
                    field: "action",
                    hozAlign: "center",
                    headerHozAlign: "center", headerMenu: headerMenu,
                    formatter: function (cell) {
                        const rowData = cell.getRow().getData();
                        return `
                            <i class="fas fa-trash-alt mr-2 fa-1x" title="Delete"
                               style="color:red;cursor:pointer;margin-left: 5px;"
                               onclick="deleteCertificateVedn(${rowData.vendorCertID})"></i>`;
                    }
                },
                { title: "ID", field: "vendorCertID", headerMenu: headerMenu, headerFilter: "input", visible: false },
                { title: "Certificate", field: "certificateID", headerMenu: headerMenu, headerFilter: "input", hozAlign: "center", visible: false },
                { title: "Certificate", field: "certificateName", hozAlign: "center", headerMenu: headerMenu, headerFilter: "input" },
                { title: "Product Code", field: "productCode", headerMenu: headerMenu, hozAlign: "center", headerFilter: "input" },
                { title: "Vendor", field: "vendorCode", headerMenu: headerMenu, hozAlign: "center", headerFilter: "input", visible: false },
                {
                    title: "Issue Date",
                    field: "issueDate",
                    formatter: dateFormatter, headerMenu: headerMenu, headerFilter: "input", 
                    hozAlign: "center"
                },
                {
                    title: "Expiry Date",
                    field: "expiryDate", headerMenu: headerMenu, headerFilter: "input", 
                    formatter: dateFormatter,
                    hozAlign: "center"
                },
                //{ title: "Remarks", field: "remarks"},
                {
                    title: "Attachment",
                    field: "certUpload",
                    hozAlign: "center", headerMenu: headerMenu,
                    headerHozAlign: "center", visible: false,
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
                    headerSort: false
                }, {
                    title: "Created By", field: "createdBy",
                    hozAlign: "center",
                    headerSort: false,
                    headerMenu: headerMenu,
                    width: 100,
                    visible: false
                },
                {
                    title: "Created Date", field: "createdDate",
                    hozAlign: "center",
                    headerSort: false,
                    headerMenu: headerMenu,
                    width: 100,
                    visible: false
                },
                {
                    title: "Updated By", field: "updatedBy",
                    hozAlign: "center",
                    headerSort: false,
                    headerMenu: headerMenu,
                    width: 100,
                    visible: false
                },
                {
                    title: "Updated Date", field: "updatedDate",
                    hozAlign: "center",
                    headerSort: false,
                    headerMenu: headerMenu,
                    width: 100,
                    visible: false
                }
            ]
        });

        certificateTable.on("cellClick", function (e, cell) {
            let columnField = cell.getColumn().getField();
            if (columnField != "action" && columnField != "certUpload") {
                let rowData = cell.getRow().getData();

                $('#vendorCertID').val(rowData.vendorCertID || '');
                $('#productCodeInput').val(rowData.productCode || '');
                $('#certificateID').val(rowData.certificateID || '').trigger('change');

                $('#vendorID').val(rowData.vendorID || '');
                $('#issueDate').val(rowData.issueDate ? rowData.issueDate.split('T')[0] : '');
                $('#expiryDate').val(rowData.expiryDate ? rowData.expiryDate.split('T')[0] : '');
                $('#remarks').val(rowData.remarks || '');
                $('#certUpload').val('');

                if (rowData.certUpload) {
                    const attachments = rowData.certUpload.split(/[,;]+/).filter(f => f.trim());
                    let remainingAttachments = [...attachments];

                    const attachmentLinks = attachments.map((file, index) => {
                        const fileName = file.split('/').pop();
                        return `
                            <div class="d-flex align-items-center justify-content-between mt-1" data-index="${index}" data-file="${file}">
                                <a href="/${file}" target="_blank" class="btn btn-link me-2">📄 ${fileName}</a>
                                <button type="button" class="btn btn-sm btn-light text-danger border-0 p-0 remove-attachment" data-file="${file}" title="Remove">
                                    ❌
                                </button>
                            </div>`;
                    }).join(" ");

                    $("#attachmentLinks").html(attachmentLinks);
                    $("#remainingAttachments").val(remainingAttachments.join(';'));

                    $('#attachmentLinks').off('click').on('click', '.remove-attachment', function () {
                        // Remove the file info
                        $('#remainingAttachments').val('');
                        $('#attachmentLinks').empty(); // Clear the file link from UI

                        // Optional: clear any related hidden field that binds to the backend
                       // $('#reportFileName').val(''); // Adjust ID if your hidden input uses a different one
                    });


                } else {
                    $("#attachmentLinks").empty();
                    $("#remainingAttachments").val('');
                }

                $('#certificateModalTitle').html('<strong>Edit Certificate</strong>');
                $('#certificateSaveBtn').html('<i class="fas fa-pen-to-square mr-2"></i>Update');
                $('#certificateModal').modal('show');
            }
        });
    }
}
function CloseModel() {
    
    $('#certificateModal').modal('hide');
    $('#certificateModalTitle').html('<strong>Add Certifcate</strong>');
    $('#certificateSaveBtn').html('<i class="fas fa-plus mr-2"></i>Save');
    clearCertificateForm();
}

function dateFormatter(cell) {
    const val = cell.getValue();
    if (!val) return "";
    const date = new Date(val);
    return `${String(date.getDate()).padStart(2, '0')}-${String(date.getMonth() + 1).padStart(2, '0')}-${date.getFullYear()}`;
}

function clearForm() {
    // Clear all input fields
    document.querySelectorAll('.form-control').forEach(function (input) {
        if (input.tagName === 'INPUT') {
            if (input.type === 'hidden' || input.readOnly) {
                // Skip hidden or readonly inputs
                return;
            }
            input.value = ''; // Clear input value
        } else if (input.tagName === 'SELECT') {
            input.selectedIndex = 0; // Reset dropdown to first option
        }
    });
    //$("[id*=Mate_Grp]").val(0);
    //$("[id*=Vender]").val(0);
    //$("[id*=NC4_Old]").val("");
    //$("[id*=FinalPart_No]").val("");
    //$("[id*=OldPart_No]").val("");
    //$("[id*=Description]").val("");
    //$("[id*=Mate_Grp]").trigger('change');
    //$("[id*=Vender]").trigger('change');
    // Clear error messages if needed
    document.querySelectorAll('.text-danger').forEach(function (error) {
        error.textContent = '';
    });
}

function InsertUpdateVendorDetail() {
    debugger;
    Blockloadershow();
    var errorMsg = "";
    var fields = "";

    if ($("#Name").val() == '' || $("#Name").val() == null || $("#Name").val() == undefined) {
        fields += " - Name" + "<br>";
    }
   
    if (fields != "") {
        errorMsg = "Please fill following mandatory field(s):" + "<br><br>" + fields;
    }
    if (errorMsg != "") {
        Blockloaderhide();
        showDangerAlert(errorMsg);
        return false;
    }

    var ajaxUrl = "";
    if ($("#hdnId").val() != "0") {
        ajaxUrl = '/Vendor/Update';
    }
    else {
        ajaxUrl = '/Vendor/Create';
    }

    var Model = {
        Id: $("#hdnId").val(),
        Name: $("#Name").val(),
        Vendor_Code: $("#Vendor_Code").val(),
        Address: $("#Address").val(),
        Contact_Persons: $("#Contact_Persons").val(),
        Email: $("#Email").val(),
        MobileNo: $("#MobileNo").val(),
        GstNo: $("#GstNo").val(),
        Owner: $("#Owner").val(),
        Owner_Email: $("#Owner_Email").val(),
        Owner_Mobile: $("#Owner_Mobile").val(),
        Plant_Head: $("#Plant_Head").val(),
        Plant_Email: $("#Plant_Email").val(),
        Plant_Mobile: $("#Plant_Mobile").val(),
        Quality_Manger: $("#Quality_Manger").val(),
        Quality_Email: $("#Quality_Email").val(),
        Quality_Mobile: $("#Quality_Mobile").val(),
        PDG_Manager: $("#PDG_Manager").val(),
        PDG_Email: $("#PDG_Email").val(),
        PDG_Mobile: $("#PDG_Mobile").val(),
        SCM_Manager: $("#SCM_Manager").val(),
        SCM_Email: $("#SCM_Email").val(),
        SCM_Mobile: $("#SCM_Mobile").val(),
        PRD_Manager: $("#PRD_Manager").val(),
        PRD_Email: $("#PRD_Email").val(),
        PRD_Mobile: $("#PRD_Mobile").val(),
        Service_Manager: $("#Service_Manager").val(),
        Service_Email: $("#Service_Email").val(),
        Service_Mobile: $("#Service_Mobile").val(),

        Other_Cont_One: $("#Other_Cont_One").val(),
        Other_Cont_OneEmail: $("#Other_Cont_OneEmail").val(),
        Other_Cont_OneMobile: $("#Other_Cont_OneMobile").val(),
        Other_Cont_Two: $("#Other_Cont_Two").val(),
        Other_Cont_TwoEmail: $("#Other_Cont_TwoEmail").val(),
        Other_Cont_TwoMobile: $("#Other_Cont_TwoMobile").val(),

        User_Name: $("#User_Name").val(),
        Password: $("#Password").val(),// Get all selected values from multi-select
        CreatedBy: $("#hdnCreateBy").val(),
        CreatedBy: $("#hdnDate").val()
    };

    $.ajax({
        type: "POST",
        url: ajaxUrl,
        data: Model,
        success: function (response) {
            Blockloaderhide();
            if (response.success) {
                if ($("#hdnId").val() != "0") {
                    showSuccessAlert("Vendor Detail is updated successfully!");
                }
                else {
                    showSuccessAlert("Vendor Detail is Saved Successfully!");

                }
                setTimeout(function () {
                    window.location.href = '/Vendor/Vendor';
                }, 2500);
            }
            else if (response.message == "Exist") {
                // let duplicateFields = Array.isArray(response.playload) ? response.playload.join(", ") : "Unknown fields";
                let duplicateFields = Array.isArray(response.payload) ? response.payload.join("") : "Unknown fields";
                errorMsg = "Vendor Detail already exist:" + "<br><br>" + duplicateFields;
                showDangerAlert(errorMsg);
            }
            else {
                var errorMessg = "";
                for (var error in response.errors) {
                    errorMessg += error + "\n";
                }
                if (errorMessg != "") {
                    showDangerAlert(errorMessg);
                }
                else {
                    showDangerAlert(response.Message);
                }
            }
        },
        error: function (xhr, ststus, errors) {
            Blockloaderhide();
            showDangerAlert("An unexpected eror occured, please refresh the page and try again.");
        }
    });
}



