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
                OnThirdPartyGridLoad();
            });

            picker.on('clear', () => {
                filterStartDate = "";
                filterEndDate = "";
                $('#dateRangeText').text("Select Date Range");
                OnThirdPartyGridLoad();
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


    $('#DashbackButton').on('click', function () {
       // window.history.back();
        window.location.href = "/Dashboard/Index" ;
    });
    $('#backButton').on('click', function () {
        // window.history.back();
        window.location.href = "/ThirdPartyInspection/Create";
    });
    $('#Third').hide();
    $('#addButton').show();
    $('#ThirdParty_Table').show();
    $('#SaveButton').hide();
    $('#DashbackButton').show();
    $('#backButton').hide();
    $('#divcustomDate').show();
    
    OnThirdPartyGridLoad();
});


function OnThirdPartyGridLoad() {
    Blockloadershow();

    $.ajax({
        url: '/ThirdPartyInspection/GetAll',
        type: 'GET',
        dataType: 'json',
        data: {
            startDate: filterStartDate,
            endDate: filterEndDate
        },
    })
        .done(function (data) {
            if (Array.isArray(data)) {
                console.log("Loaded Third-Party data:", data);
                OnTabGridLoad(data);
            } else {
                console.warn("Unexpected payload, expected array but got:", data);
                showDangerAlert('No data available to load.');
                OnTabGridLoad([]);
            }
        })
        .fail(function (xhr, status, error) {
            const msg = `Error retrieving data: ${xhr.status} ${xhr.statusText}\n${xhr.responseText}`;
            console.error(msg);
            showDangerAlert(msg);
            OnTabGridLoad([]);
        })
        .always(function () {
            Blockloaderhide();
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
        url: '/ThirdPartyInspection/Update',
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

function deleteThirdParty(Id) {
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
        $.ajax({
            url: '/ThirdPartyInspection/Delete',
            type: 'POST',
            data: { id: Id },
            success: function (data) {
                if (data.success === true) {
                    showSuccessAlert("Deleted successfully.");
                    OnThirdPartyGridLoad();
                } else if (data.success === false && data.message === "Not_Deleted") {
                    showDangerAlert("Record is used in related transactions and cannot be deleted.");
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



function ShowDetail(id) {
    $('#Third').show();
    $('#addButton').hide();
    $('#ThirdParty_Table').hide();
    $('#SaveButton').show();
    $('#DashbackButton').hide();
    $('#backButton').show(); $('#divcustomDate').hide();
}

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

var thirdPartyTable;


// Helper to format dates as YYYY-MM-DD (or change to DD-MM-YYYY as needed)
function formatDate(dateStr) {
    if (!dateStr) return "";
    const date = new Date(dateStr);
    const day = String(date.getDate()).padStart(2, '0');
    const month = String(date.getMonth() + 1).padStart(2, '0');
    const year = date.getFullYear();
    return `${year}-${month}-${day}`;
}

function OnTabGridLoad(data) {
    // Always supply an array to Tabulator
    const safeData = Array.isArray(data) ? data : [];

    if (thirdPartyTable) {
        thirdPartyTable.replaceData(safeData);
    } else {
        thirdPartyTable = new Tabulator("#ThirdParty_Table", {
            data: safeData,
            layout: "fitDataFill",
            movableColumns: true,
            pagination: "local",
            paginationSize: 10,
            paginationSizeSelector: [10, 50, 100, 500],
            paginationCounter: "rows",
            placeholder: "No data available",
            columns: [
                { title: "Sn", formatter: "rownum", width: 80, headerMenu, frozen: true, headerSort: false, hozAlign: "center" },
                { title: "ID", field: "inspectionID", width: 80, frozen: true, visible: false },
                {
                    title: "Action",
                    field: "action",
                    hozAlign: "center",
                    frozen: true,
                    width: 80,
                    headerMenu,
                    headerHozAlign: "right",
                    headerSort: false,
                    formatter: cell => {
                        const id = cell.getRow().getData().inspectionID;
                        return `
                            <div style="margin-right:50px">
                                <i class="fas fa-trash-alt text-danger"
                                   style="cursor:pointer;font-size:16px;"
                                   title="Delete"
                                   onclick="event.stopPropagation(); deleteThirdParty(${id});">
                                </i>
                            </div>`;
                    }
                },
                {
                    title: "Inspection Date",
                    field: "inspectionDate",
                    hozAlign: "center",
                    headerFilter: "input",
                    frozen: true,
                    headerMenu,
                    headerSort: false,
                    formatter: cell => formatDate(cell.getValue())
                },
                { title: "Project Name", field: "projectName", headerMenu, headerFilter: "input", frozen: true },
                { title: "Inspector Name", field: "inspName", headerMenu, headerFilter: "input", headerSort: false },
                { title: "Product Code", field: "productCode", headerMenu, headerFilter: "input", headerSort: false },
                { title: "Product Description", field: "prodDesc", headerMenu, headerFilter: "input", headerSort: false },
                { title: "LOT Quantity", field: "lotQty", hozAlign: "center", headerMenu, headerFilter: "input", headerSort: false },
                { title: "Project Value", field: "projectValue", headerMenu, headerFilter: "input", visible: false, headerSort: false },
                { title: "Location", field: "location", headerMenu, headerFilter: "input", visible: true, headerSort: false },
                { title: "Mode", field: "mode", headerMenu, headerFilter: "input", visible: true, headerSort: false },
                { title: "First Attempt", field: "firstAttempt", headerMenu, headerFilter: "input", visible: true, headerSort: false },
                { title: "Remark", field: "remark", headerMenu, headerFilter: "input", visible: true, headerSort: false },
                { title: "Action Plan", field: "actionPlan", headerMenu, headerFilter: "input", visible: true, headerSort: false },
                {
                    title: "MOM Date",
                    field: "momDate",
                    hozAlign: "center",
                    headerMenu,
                    headerFilter: "input",
                    visible: false,
                    headerSort: false,
                    formatter: cell => formatDate(cell.getValue())
                },
                {
                    title: "Attachment",
                    field: "attachment",
                    width: 100,
                    visible: false,
                    headerMenu,
                    headerSort: false,
                    formatter: cell => {
                        const v = cell.getValue();
                        if (!v) return "";
                        return v.split(/[,;]+/)
                            .map(p => p.trim())
                            .filter(p => p)
                            .map(p => `
                                <a href="/${p}"
                                   target="_blank"
                                   download
                                   title="Download"
                                   style="margin-right:8px;display:inline-block;">
                                   <i class="fas fa-download text-primary" style="font-size:16px;"></i>
                                </a>
                            `).join("");
                    }
                }
            ]
        });

        // Only open the edit modal when non-action cells are clicked
        thirdPartyTable.on("cellClick", (e, cell) => {
            if (cell.getColumn().getField() !== "action") {
                editThirdParty(cell.getRow().getData());
            }
        });
    }
}

function editThirdParty(response) {
    //  alert("Edit called for: " + response.projectName);
    $('#Third').show();
    $('#addButton').hide();
    $('#ThirdParty_Table').hide(); $('#divcustomDate').hide();
    $('#SaveButton').show().html('<i class="fas fa-edit mr-2"></i>Update');
    $('#DashbackButton').hide();
    $('#backButton').show();
    $("#InspectionID").val(response.inspectionID);
    $("#InspectionDate").val(formatDate(response.inspectionDate));
    $("#ProjectName").val(response.projectName);
    $("#InspName").val(response.inspName);
    $("#ProductCode").val(response.productCode);
    $("#ProdDesc").val(response.prodDesc);
    $("#LOTQty").val(response.lotQty);
    $("#ProjectValue").val(response.projectValue);
    $("#Location").val(response.location);
    $("#Mode").val(response.mode);
    $("#FirstAttempt").val(response.firstAttempt);
    $("#Remark").val(response.remark);
    $("#ActionPlan").val(response.actionPlan);
    $("#MOMDate").val(formatDate(response.momDate));

    // Attachment logic
    if (response.attachment) {
        const attachments = response.attachment.split(/[,;]+/).filter(f => f.trim());

        // Store in a JS array for tracking
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
        }).join("");

        $("#attachmentLinks").html(attachmentLinks);

        // Initialize hidden input for tracking (optional)
        $("#remainingAttachments").val(remainingAttachments.join(';'));
    }
    else {
        $("#attachmentLinks").empty();
        $("#remainingAttachments").val('');
    }


    // UI toggles
   
}

function clearThirdPartyForm() {
    $("#InspectionID").val("");
    $("#InspectionDate, #ProjectName, #InspName, #ProductCode, #ProdDesc, #LOTQty, #ProjectValue, #Location, #Mode, #FirstAttempt, #Remark, #ActionPlan, #MOMDate").val("");
    $("#AttachmentFile").val("");
 
}

function InsertUpdateThirdParty() {
    var id = $("#InspectionID").val();

    if (!$("#ProjectName").val().trim() || !$("#InspName").val().trim()) {
        showDangerAlert("Project Name and Inspector Name are required.");
        return;
    }

    var formData = new FormData();
    formData.append("InspectionID", id || 0);
    formData.append("InspectionDate", $("#InspectionDate").val());
    formData.append("ProjectName", $("#ProjectName").val().trim());
    formData.append("InspName", $("#InspName").val().trim());
    formData.append("ProductCode", $("#ProductCode").val().trim());
    formData.append("ProdDesc", $("#ProdDesc").val().trim());
    formData.append("LOTQty", parseInt($("#LOTQty").val()) || 0);
    formData.append("ProjectValue", $("#ProjectValue").val().trim());
    formData.append("Location", $("#Location").val().trim());
    formData.append("Mode", $("#Mode").val().trim());
    formData.append("FirstAttempt", $("#FirstAttempt").val().trim());
    formData.append("Remark", $("#Remark").val().trim());
    formData.append("ActionPlan", $("#ActionPlan").val().trim());
    formData.append("MOMDate", $("#MOMDate").val());

    // Handle multiple file uploads
    var fileInput = $("#AttachmentFile")[0];
    for (var i = 0; i < fileInput.files.length; i++) {
        formData.append("AttachmentFiles", fileInput.files[i]); // Note: key must be "AttachmentFiles"
    }
   // var url = id ? "/ThirdPartyInspection/Update" : "/ThirdPartyInspection/Create";
    var url = "/ThirdPartyInspection/Create";
    $.ajax({
        type: "POST",
        url: url,
        data: formData,
        processData: false,
        contentType: false,
        success: function (response) {
            if (response.success) {
                showSuccessAlert(id ? "Updated successfully." : "Created successfully.");
                clearThirdPartyForm();
                $('#Third').hide();
                $('#addButton').show();
                $('#ThirdParty_Table').show();
                $('#SaveButton').hide();
                $('#DashbackButton').show();
                $('#backButton').hide();
                $('#divcustomDate').show();
                OnThirdPartyGridLoad();

            } else {
                showDangerAlert(response.message || "Operation failed.");
            }
        },
        error: function () {
            showDangerAlert("Unexpected error. Please try again.");
        }
    });

}
function renderAttachments(attachmentsString) {
    const attachments = attachmentsString.split(/[,;]+/).filter(f => f.trim());
    const html = attachments.map((file, index) => {
        const fileName = file.split('/').pop();
        return `
            <div class="d-flex align-items-center justify-content-between mt-1" data-file="${file}">
                <a href="/${file}" target="_blank" class="btn btn-link me-2">📄 ${fileName}</a>
                <button type="button" class="btn btn-sm btn-light text-danger border-0 p-0 remove-attachment" data-file="${file}" title="Remove">
                    ❌
                </button>
            </div>`;
    }).join("");

    $("#attachmentLinks").html(html);

    // Store the full list in hidden input
    $("#remainingAttachments").val(attachments.join(","));
}

$(document).on("click", ".remove-attachment", function () {
    const $container = $(this).closest("div[data-index]");
    const fileToRemove = $(this).data("file");

    // Remove visually
    $container.remove();

    // Update hidden input field
    let current = $("#remainingAttachments").val().split(/[,;]+/).filter(f => f.trim());
    const updated = current.filter(f => f !== fileToRemove);
    $("#remainingAttachments").val(updated.join(';'));

    // Optional AJAX to update server
    const inspectionId = $("#InspectionID").val(); // replace with actual ID source if needed
    $.ajax({
        url: '/ThirdPartyInspection/RemoveAttachment',
        type: 'POST',
        data: {
            inspectionId: inspectionId,
            filePath: fileToRemove
        },
        success: function () {
            console.log("File deleted from server and DB updated.");
        },
        error: function () {
            alert("Error removing file.");
        }
    });
});