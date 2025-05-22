let BISCertificateTable;

$(document).ready(function () {
    initializeBISCertificateTable();
    loadBISCertificates();
    const inputElement = $("#BISsear_Code");
    if (inputElement.length) {
        // Attach the keydown event handler
        inputElement.on('keydown', function (event) {
            handleSearch(event);
        });
    }

    $('#BIScertificateForm').on('keypress', function (e) {
        if (e.key === 'Enter') {
            e.preventDefault();
        }
    });
});

function handleSearch(event) {
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
        fetchRelatedData(searchTerms, event.target);
    }, 300); // Delay of 300ms
}

function fetchRelatedData(productCatNo, inputField) {
    debugger
    $.ajax({
        url: '/Vendor/GetCodeSearch', // API endpoint
        type: 'GET', // HTTP method
        data: { search: productCatNo }, // Query parameters
        dataType: 'json', // Expected response data type
        success: function (data) {
            if (data && data.length > 0) {
                displaySuggestions(data, inputField);
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
function displaySuggestions(data, inputField) {
    // Create a dropdown container
    const dropdown = document.createElement('div');
    dropdown.className = 'suggestion-dropdown';
    dropdown.style.position = 'absolute';
    dropdown.style.backgroundColor = 'white';
    dropdown.style.border = '1px solid #ccc';
    dropdown.style.zIndex = 1000;
    dropdown.style.overflowY = 'auto'; // Enable vertical scrolling
    dropdown.style.maxHeight = '300px'; // Set a fixed height for the dropdown
    dropdown.style.width = `${inputField.offsetWidth}px`; // Set the width to match the input field
    dropdown.style.boxShadow = '0px 4px 6px rgba(0, 0, 0, 0.1)'; // Add shadow for better visibility

    // Position the dropdown relative to the input field
    const rect = inputField.getBoundingClientRect(); // Get input field position relative to the viewport
    dropdown.style.top = `${rect.bottom + window.scrollY}px`; // Correct vertical positioning
    dropdown.style.left = `${rect.left + window.scrollX}px`; // Correct horizontal positioning

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
            const oldPartNoInput = document.getElementById('BISproductCodeInput');
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
    const existingDropdown = inputField.parentElement.querySelector('.suggestion-dropdown');
    if (existingDropdown) {
        existingDropdown.remove();
    }
}




// Remove dropdown on outside click
document.addEventListener('click', function (e) {
    if (!e.target.closest('.suggestion-dropdown') && !e.target.closest('#BISsear_Code')) {
        removeExistingDropdown();
    }
});
function initializeBISCertificateTable(data) {
    if (BISCertificateTable) {
        BISCertificateTable.replaceData(data);
    } else {
        BISCertificateTable = new Tabulator("#BISCertificateTable", {
            data: data,
            layout: "fitColumns",
            renderHorizontal: "virtual",
            movableColumns: true,
            pagination: "local",
            paginationSize: 10,
            paginationSizeSelector: [10, 50, 100, 500],
            paginationCounter: "rows",
            placeholder: "No BIS certificates available",
            columns: [
                {
                    title: "Action", field: "action", hozAlign: "center",  width:120, headerMenu: headerMenu,
                    formatter: function (cell) {
                        const rowData = cell.getRow().getData();
                        return `<i class="fas fa-trash-alt text-danger" style="cursor:pointer;" title="Delete" onclick="deleteBISCertificate(${rowData.id})"></i>`;
                    }
                },
                {
                    title: "Attachment", field: "fileName", hozAlign: "center",  visible: false, headerMenu: headerMenu,
                    formatter: function (cell) {
                        const value = cell.getValue();
                        if (!value) return "";
                        const files = value.split(/[,;]+/).map(f => f.trim()).filter(Boolean);
                        return files.map(path =>
                            `<a href="/${path}" target="_blank" download title="Download">
                    <i class="fas fa-download text-primary"></i>
                </a>`
                        ).join(" ");
                    }
                },
                { title: "Sn", formatter: "rownum", width: 90, hozAlign: "center",  headerMenu: headerMenu },
                { title: "ID", field: "id", visible: false, headerMenu: headerMenu },
                { title: "BIS Certificate", field: "certificateDetail", hozAlign: "center", headerFilter: "input",  headerMenu: headerMenu },
                { title: "Product Code", field: "productCode", hozAlign: "center", headerFilter: "input", headerMenu: headerMenu },
                { title: "Section", field: "bisSection", hozAlign: "center", headerFilter: "input", visible: false,  headerMenu: headerMenu },
                { title: "R Number", field: "rNumber", hozAlign: "center", headerFilter: "input", visible: false,  headerMenu: headerMenu },
                { title: "Model No", field: "modelNo", hozAlign: "center", headerFilter: "input",  headerMenu: headerMenu },
                {
                    title: "Issue Date", field: "issueDate", hozAlign: "center", formatter: dateFormatter,
                    headerFilter: "input",  headerMenu: headerMenu
                },
                {
                    title: "Expiry Date", field: "expiryDate", hozAlign: "center", formatter: dateFormatter,
                    headerFilter: "input",  headerMenu: headerMenu
                },
                { title: "Remarks", field: "remarks",  hozAlign: "center", headerMenu: headerMenu },
                {
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

        BISCertificateTable.on("cellClick", function (e, cell) {
           
            const column = cell.getColumn().getField();
            if (column !== "action" && column !== "fileName") {
                $('#BIScertificateModalTitle').html('<strong>Edit BIS Certificate</strong>');
                $('#BISsaveBtn').html('<i class="fas fa-pen-to-square mr-2"></i>Update');
                $('#BISCertificateModal').modal('show');
                const rowData = cell.getRow().getData();
                $('#Id').val(rowData.id || '');
                $('#BIScertificateID').val(rowData.certificateDetail || '');
                $('#BISproductCodeInput').val(rowData.productCode || '');
                $('#BISSection').val(rowData.bisSection || '');
                $('#RNumber').val(rowData.rNumber || '');
                $('#ModelNo').val(rowData.modelNo || '');
                $('#BISissueDate').val(rowData.issueDate ? rowData.issueDate.split('T')[0] : '');
                $('#BISexpiryDate').val(rowData.expiryDate ? rowData.expiryDate.split('T')[0] : '');
                $('#BISremarks').val(rowData.remarks || '');
                $('#BISattachment').val('');

                if (rowData.fileName) {
                    const attachments = rowData.fileName.split(/[,;]+/).filter(f => f.trim());
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

                    $("#BISattachmentLinks").html(attachmentLinks);
                    $("#BISremainingAttachments").val(remainingAttachments.join(';'));

                    $('#BISattachmentLinks').off('click').on('click', '.remove-attachment', function () {
                        const fileToRemove = $(this).data('file');
                        let currentFiles = $('#BISremainingAttachments').val().split(';').filter(f => f.trim());
                        currentFiles = currentFiles.filter(f => f !== fileToRemove);
                        $('#BISremainingAttachments').val(currentFiles.join(';'));
                        $(this).closest('div').remove();
                    });
                } else {
                    $("#BISattachmentLinks").empty();
                    $("#BISremainingAttachments").val('');
                }


                
            }
        });
    }
}
function loadBISCertificates() {
    Blockloadershow();
    $.ajax({
        url: '/Vendor/GetAllBISCertificates',
        type: 'GET',
        success: function (response) {
            Blockloaderhide();
            if (response.success && Array.isArray(response.data)) {
                initializeBISCertificateTable(response.data);
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


function InsertUpdateVendorBIS(e) {
    e.preventDefault();

    // Trim the vendor code and check if it's empty
    const vendorCode = $('#hdnVendorCode').val().trim();
    if (vendorCode === "") {
        showDangerAlert("Vendor code is required.");
        return;
    }

    const vendorID = $('#hdnId').val();
    const certificateDetail = $('#BIScertificateID').val().trim();
    const productCode = $('#BISproductCodeInput').val().trim();
    const bisSection = $('#BISSection').val().trim();
    const rNumber = $('#RNumber').val().trim();
    const modelNo = $('#ModelNo').val().trim();
    const issueDate = $('#BISissueDate').val().trim();
    const expiryDate = $('#BISexpiryDate').val().trim();
    const remarks = $('#BISremarks').val().trim();

    // Validate required fields
    if (certificateDetail === "") {
        showDangerAlert("BIS Certificate is required.");
        return;
    }
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

    //// Additional validation for date fields (optional)
    //if (new Date(issueDate) > new Date(expiryDate)) {
    //    showDangerAlert("Expiry Date cannot be earlier than Issue Date.");
    //    return;
    //}

    // Prepare form data
    let formData = new FormData();
    formData.append("Id", $('#Id').val());
    formData.append("VendorCode", vendorCode);
    formData.append("VendorID", vendorID);
    formData.append("CertificateDetail", certificateDetail);
    formData.append("ProductCode", productCode);
    formData.append("BISSection", bisSection);
    formData.append("RNumber", rNumber);
    formData.append("ModelNo", modelNo);
    formData.append("IssueDate", issueDate);
    formData.append("ExpiryDate", expiryDate);
    formData.append("Remarks", remarks);

    // File validation (optional)
    const fileInput = $('#BIScertUpload')[0];
    if (fileInput.files.length > 0) {
        const file = fileInput.files[0];
        // You can add file size/type validation here
        formData.append("file", file);
    }

    // Send data via AJAX
    $.ajax({
        url: '/Vendor/CreateOrUpdateBISCertificate',
        type: 'POST',
        data: formData,
        contentType: false,
        processData: false,
        success: function (res) {
            if (res.success)
            {
                showSuccessAlert(res.message);
                $('#BISCertificateModal').modal('hide');
                $('#BIScertificateModalTitle').html('<strong>Add BIS Certificate</strong>');
                $('#BISsaveBtn').html('<i class="fas fa-floppy-disk mr-2"></i>Save');
                $("#BISattachmentLinks").html('');
                $('#BIScertificateForm')[0].reset();
                loadBISCertificates();
            }
           
        },
        error: function () {
            alert("Error saving BIS certificate.");
        }
    });
}


function deleteBISCertificate(id) {
    //event.preventDefault();  // Prevent form submission
    // console.log('Deleting TPTReport with ID: ' + id); // Log the ID being passed

    PNotify.prototype.options.styling = "bootstrap3";

    new PNotify({
        title: 'Confirmation Needed',
        text: 'Are you sure you want to delete this? It will not delete if this record is used in transactions.',
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
            url: '/Vendor/DeleteBISCertificate', // Ensure URL matches exactly
            type: 'POST',
            data: { id: id },
            success: function (data) {
                if (data.success === true) {
                    showSuccessAlert("BIS Certificate deleted successfully.");
                    loadBISCertificates();
                } else if (data.success === false && data.message === "Not_Deleted") {
                    showDangerAlert("BISCertificate is used in related transactions and cannot be deleted.");
                } else {
                    showDangerAlert(data.message || "An unexpected error occurred.");
                }
            },
            error: function () {
                showDangerAlert('Error occurred during deletion.');
            }
        });
    }).on('pnotify.cancel', function () {
        loadBISCertificates();
    });
}

function BISCloseModel() {
    $('#BIScertificateForm')[0].reset();
    $('#BISCertificateModal').modal('hide');
    $('#BIScertificateModalTitle').html('<strong>Add BIS Certificate</strong>');
    $('#BISsaveBtn').html('<i class="fas fa-floppy-disk mr-2"></i>Save');
    $("#BISattachmentLinks").html('');
}
