
var TPTReportTable;
$(document).ready(function () {
    loadTPTReportData();

    loadTPTReportDropdown();

    const inputElement = $("#TPTRsear_Code");
    if (inputElement.length) {
        // Attach the keydown event handler
        inputElement.on('keydown', function (event) {
            handleThirdTestingSearch(event);
        });
    }

    $('#TPTReportForm').on('keypress', function (e) {
        if (e.key === 'Enter') {
            e.preventDefault();
        }
    });
});

function handleThirdTestingSearch(event) {
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
        fetchThirdRelatedData(searchTerms, event.target);
    }, 300); // Delay of 300ms
}

function fetchThirdRelatedData(productCatNo, inputField) {
    debugger
    $.ajax({
        url: '/Vendor/GetCodeSearch', // API endpoint
        type: 'GET', // HTTP method
        data: { search: productCatNo }, // Query parameters
        dataType: 'json', // Expected response data type
        success: function (data) {
            if (data && data.length > 0) {
                displayThirdSuggestions(data, inputField);
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
function displayThirdSuggestions(data, inputField) {
    // Create a dropdown container
    const dropdown = document.createElement('div');
    dropdown.className = 'suggestion-dropdown';
    dropdown.style.position = 'absolute';
    dropdown.style.backgroundColor = 'white';
    dropdown.style.border = '1px solid #ccc';
    dropdown.style.zIndex = 1000;
    dropdown.style.overflowY = 'auto'; // Enable vertical scrolling
    dropdown.style.maxHeight = '300px'; // Set a fixed height for the dropdown
    dropdown.style.width = `190px`; // Set the width to match the input field
    dropdown.style.boxShadow = '0px 4px 6px rgba(0, 0, 0, 0.1)'; // Add shadow for better visibility

    // Position the dropdown relative to the input field
    const rect = inputField.getBoundingClientRect(); // Get input field position relative to the viewport
    //dropdown.style.top = `${rect.bottom + window.scrollY}px`; // Correct vertical positioning
    //dropdown.style.left = `${rect.left + window.scrollX}px`; // Correct horizontal positioning

    // Populate dropdown with suggestions
    data.forEach(item => {
        const option = document.createElement('div');
        option.className = 'suggestion-item';
        option.textContent = `${item.oldPart_No}`;
        option.style.padding = '8px';
        option.style.cursor = 'pointer';

        // Add hover effect for options
        option.addEventListener('mouseover', () => {
            option.style.backgroundColor = '#4682B4'; // Change background on hover
        });
        option.addEventListener('mouseout', () => {
            option.style.backgroundColor = ''; // Reset background on mouse out
        });

        // When an option is clicked, set the value of the input field and remove the dropdown
        option.addEventListener('click', () => {
            const oldPartNoInput = document.getElementById('tptr_productCodeInput');
            if (oldPartNoInput) {
                oldPartNoInput.value = item.oldPart_No; // Set value of the input field
            }
            dropdown.remove(); // Remove dropdown after selection
        });

        dropdown.appendChild(option); // Add option to the dropdown
    });

    // Remove any existing dropdown (if any) before appending the new one
    removeExistingDropdown(inputField);
    inputField.parentElement.appendChild(dropdown); // Append dropdown inside the parent element of the input field
}

// Function to remove any existing dropdowns
function removeExistingDropdown(inputField) {
    const existingDropdown = document.querySelector('.suggestion-dropdown');
    if (existingDropdown) {
        existingDropdown.remove();
    }
}




// Remove dropdown on outside click
document.addEventListener('click', function (e) {
    if (!e.target.closest('.suggestion-dropdown') && !e.target.closest('#TPTRsear_Code')) {
        removeExistingDropdown();
    }
});

function loadTPTReportData() {
    Blockloadershow();
    $.ajax({
        url: '/Vendor/GetTPTReportData',
        type: 'GET',
        success: function (response) {
            Blockloaderhide();
            if (response.success && Array.isArray(response.data)) {
                initializeTPTReportTable(response.data);
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
function deleteTPTReportVedn(id) {
    //event.preventDefault();  // Prevent form submission
   // console.log('Deleting TPTReport with ID: ' + id); // Log the ID being passed

    PNotify.prototype.options.styling = "bootstrap3";

    new PNotify({
        title: 'Confirmation Needed',
        text: 'Are you sure you want to delete this TPTReport? It will not delete if this record is used in transactions.',
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
        console.log('Proceeding with Report deletion'); // Log when deletion proceeds
        $.ajax({
            url: '/Vendor/DeleteTPTReport', // Ensure URL matches exactly
            type: 'POST',
            data: { id: id },
            success: function (data) {
                if (data.success === true) {
                    showSuccessAlert("Report deleted successfully.");
                    loadTPTReportData();
                } else if (data.success === false && data.message === "Not_Deleted") {
                    showDangerAlert("Report is used in related transactions and cannot be deleted.");
                } else {
                    showDangerAlert(data.message || "An unexpected error occurred.");
                }
            },
            error: function () {
                showDangerAlert('Error occurred during deletion.');
            }
        });
    }).on('pnotify.cancel', function () {
        loadTPTReportData(); 
    });
}
function Showpopup() {
    $('#TPTReportForm')[0].reset(); 
}



var headerMenu = function () {
    var menu = [];
    var columns = this.getColumns();

    for (let column of columns) {
        let columnTitle = column.getDefinition().title;
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
function clearTPTReportForm() {
    // Clear all form inputs and reset values
    $('#tptr_Id').val('');  // Reset the ID field (for insert mode)
    $('#TPTReportID').val('').trigger('change');  // Reset the certificate dropdown
    $('#tptr_productCodeInput').val('');  // Clear product code input
    $('#TPTRissueDate').val('');  // Clear issue date input
    $('#TPTRexpiryDate').val('');  // Clear expiry date input
    $('#TPTRremarks').val('');  // Clear remarks textarea
    $('#reportUpload').val('');  // Clear file input
    $('#tptr_attachmentLinks').html('');  // Clear file attachment links if any

    // Optionally, clear any hidden fields or reset any custom data
    $('#tptr_remainingAttachments').val('');
    $('#tptr_Attachments').val('');
    $("#TPTRsear_Code").val('');
}

function InsertUpdateVendorReport(event) {
    event.preventDefault();  // Prevent default form submission

    // Validation
    const vendorCode = $('#hdnVendorCode').val().trim();
    const vendorID = $('#hdnId').val();
    const reportID = $('#TPTReportID').val();
    const productCode = $('#tptr_productCodeInput').val().trim();
    const issueDate = $('#TPTRissueDate').val();
    const expiryDate = $('#TPTRexpiryDate').val();
    const remarks = $('#TPTRremarks').val().trim();

    // Collect validation errors
    // Validate required fields
    if (vendorCode === "") {
        showDangerAlert("Vendor Code is required.");
        return;
    }
    //if (reportID === "") {
    //    showDangerAlert("Please select a Third Party Report.");
    //    return;
    //}
    //if (productCode === "") {
    //    showDangerAlert("Product Code is required.");
    //    return;
    //}
    //if (issueDate === "") {
    //    showDangerAlert("Issue Date is required.");
    //    return;
    //}
    //if (expiryDate === "") {
    //    showDangerAlert("Expiry Date is required.");
    //    return;
    //}
    //if (new Date(issueDate) > new Date(expiryDate)) {
    //    showDangerAlert("Issue Date cannot be after Expiry Date.");
    //    return;
    //}
    //if (remarks === "") {
    //    showDangerAlert("Remarks are required.");
    //    return;
    //}

    // File validation (only for insert)
    const isInsert = $('#tptr_Id').val() === "";
    const fileInput = $('#reportUpload')[0];
    if (isInsert && fileInput.files.length === 0) {
        showDangerAlert("Report file is required.");
        return;
    }


    // All validations passed — prepare form data
    const formData = new FormData();
    formData.append("VendorCode", vendorCode);
    formData.append("VendorID", vendorID);
    formData.append("ThirdPartyReportID", reportID);
    formData.append("ProductCode", productCode);
    formData.append("IssueDate", issueDate);
    formData.append("ExpiryDate", expiryDate);
    formData.append("Remarks", remarks);
    formData.append("Id", $('#tptr_Id').val());

    if (fileInput.files.length > 0) {
        formData.append("reportUpload", fileInput.files[0]);
    }

    $.ajax({
        type: "POST",
        url: "/Vendor/CreateTPTReport",
        data: formData,
        contentType: false,
        processData: false,
        success: function (res) {
            if (res.success) {
                showSuccessAlert(res.message || "Report saved successfully.");
                $('#TPTReportModalTitle').html('<strong>Add Third Party Testing Report</strong>');
                $('#reportSaveBtn').html('<i class="fas fa-plus mr-2"></i>Save');
                $('#TPTReportModal').modal('hide');
                clearTPTReportForm();
                loadTPTReportData();
            } else {
                showDangerAlert(res.message || "Failed to save the report.");
            }
        },
        error: function () {
            showDangerAlert("Error occurred while saving the report.");
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

function loadTPTReportDropdown() {
    $.ajax({
        url: '/ThirdPartyCertificate/GetAll',
        method: 'GET',
        success: function (response) {
            console.log('Response:', response);

            // Guard against unexpected formats
            if (!response.success || !Array.isArray(response.data)) {
                console.warn('Unexpected response format', response);
                return;
            }

            const certList = response.data;
            const dd = document.getElementById('TPTReportID');
            dd.innerHTML = '';  // Clear existing options

            // Default placeholder
            const defaultOption = document.createElement('option');
            defaultOption.value = '';
            defaultOption.textContent = 'Select Report';
            dd.appendChild(defaultOption);

            // Populate dropdown with fetched certification reports
            certList.forEach(cert => {
                const option = document.createElement('option');
                option.value = cert.certificateID;
                option.textContent = cert.certificateName;
                dd.appendChild(option);
            });
        },
        error: function (err) {
            console.error('Error loading TPTReports:', err);
        }
    });
}
// Handling the removal of attachment links
$('#attachmentLinks').on('click', '.remove-attachment', function () {
    const fileToRemove = $(this).data('file');

    // Get the list of remaining attachments
    let currentFiles = $('#remainingAttachments').val().split(';').filter(f => f.trim());

    // Remove the selected file from the list
    currentFiles = currentFiles.filter(f => f !== fileToRemove);

    // Update the remaining attachments value
    $('#remainingAttachments').val(currentFiles.join(';'));

    // Remove the attachment link from the UI
    $(this).closest('div').remove();
});

function initializeTPTReportTable(data) {
    if (TPTReportTable) {
        TPTReportTable.replaceData(data); // Replace data if the table is already initialized
    } else {
      
        TPTReportTable = new Tabulator("#TPTReportTable", {
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
                { title: "Sn", formatter: "rownum", width: 90, headerSort: false, headerMenu: headerMenu, frozen: true, hozAlign: "center" },
                {
                    title: "Action",
                    field: "action",
                    hozAlign: "center", headerMenu: headerMenu,
                    headerHozAlign: "center", width: 120,
                    formatter: function (cell) {
                        const rowData = cell.getRow().getData();
                        return `
                            <i class="fas fa-trash-alt mr-2 fa-1x" title="Delete"
                               style="color:red;cursor:pointer;margin-left: 5px;"
                               onclick="deleteTPTReportVedn(${rowData.id})"></i>`;
                    }
                },
                { title: "ID", field: "id", headerFilter: "input", headerMenu: headerMenu, visible:false},
                {
                    title: "Third Party Testing Report", field: "certificateName", headerFilter: "input" },
                { title: "Product Code", field: "productCode", headerMenu: headerMenu, hozAlign: "center", headerFilter: "input" },
                { title: "Vendor", field: "vendorCode", headerMenu: headerMenu, headerFilter: "input", headerSort: false, visible: false },
                {
                    title: "Issue Date",
                    field: "issueDate",
                    formatter: dateFormatter, headerMenu: headerMenu,
                    hozAlign: "center", headerFilter: "input"
                },
                {
                    title: "Expiry Date",
                    field: "expiryDate",
                    formatter: dateFormatter, headerMenu: headerMenu,
                    hozAlign: "center", headerFilter: "input"
                },
                /*{ title: "Remarks", field: "remarks" },*/
                {
                    title: "Attachment",
                    field: "reportFileName",
                    hozAlign: "center", headerMenu: headerMenu,
                    headerHozAlign: "center", 
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
        
        // Handle cell click events
        TPTReportTable.on("cellClick", function (e, cell) {
            let columnField = cell.getColumn().getField();
            if (columnField !== "action" && columnField !== "reportFileName") {
                let rowData = cell.getRow().getData();
                console.log(rowData);
                // Pre-populate the form fields with the selected row data
                $('#tptr_Id').val(rowData.id || '');
                $('#tptr_productCodeInput').val(rowData.productCode || '');
                $('#TPTReportID').val(rowData.thirdPartyReportID || '').trigger('change');
            //    $('#vendorID').val(rowData.vendorID || '');
                $('#TPTRissueDate').val(rowData.issueDate ? rowData.issueDate.split('T')[0] : '');
                $('#TPTRexpiryDate').val(rowData.expiryDate ? rowData.expiryDate.split('T')[0] : '');
                $('#TPTRremarks').val(rowData.remarks || '');
                $('#reportUpload').val('');

                // Handle the attachments for the selected row
                if (rowData.reportFileName) {
                    const attachments = rowData.reportFileName.split(/[,;]+/).filter(f => f.trim());
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

                    $("#tptr_attachmentLinks").html(attachmentLinks);
                    $("#tptr_remainingAttachments").val(remainingAttachments.join(';'));

                    // Handle remove attachment click event
                    $('#tptr_attachmentLinks').off('click').on('click', '.remove-attachment', function () {
                        const fileToRemove = $(this).data('file');
                        let currentFiles = $('#tptr_remainingAttachments').val().split(';').filter(f => f.trim());
                        currentFiles = currentFiles.filter(f => f !== fileToRemove);
                        $('#tptr_remainingAttachments').val(currentFiles.join(';'));
                        $(this).closest('div').remove();
                    });

                } else {
                    // If no attachments, clear the attachment section
                    $("#tptr_attachmentLinks").empty();
                    $("#tptr_remainingAttachments").val('');
                }

                // Update modal title and button for editing
                $('#TPTReportModalTitle').html('<strong>Edit Third Party Testing Report</strong>');
                $('#reportSaveBtn').html('<i class="fas fa-pen-to-square mr-2"></i>Update');
                $('#TPTReportModal').modal('show');
            }
        });
    }
}
function TPTCloseModel() {
    $('#TPTReportModalTitle').html('<strong>Add Third Party Testing Report</strong>');
    $('#reportSaveBtn').html('<i class="fas fa-plus mr-2"></i>Save');
    $('#TPTReportModal').modal('hide');
    clearTPTReportForm();
}

function dateFormatter(cell) {
    const val = cell.getValue();
    if (!val) return "";
    const date = new Date(val);
    return `${String(date.getDate()).padStart(2, '0')}-${String(date.getMonth() + 1).padStart(2, '0')}-${date.getFullYear()}`;
}