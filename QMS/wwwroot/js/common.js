
function AutoReload() {
    setTimeout(function () {window.location.reload(); }, 1500);
}
function createUploadButton() {
    // Create the uploadData button
    var uploadButton = document.createElement('button');
    uploadButton.id = 'btnUploadData';
    uploadButton.type = 'button';
    uploadButton.dataset.toggle = 'modal';
    uploadButton.dataset.target = '#uploadModal';
    uploadButton.style.cssText = 'float: right; margin-left: 15px';
    uploadButton.className = 'btn btn-outline-success legitRipple';
    uploadButton.innerHTML = '<i class="fas fa-upload mr-2 fa-1x"></i>Upload Data';

    // Return the button so it can be appended outside the function
    return uploadButton;
}

$('#btnUploadData').on('click', function () {
    $('#fileInput').val(''); // Clear the file input5
    $('#uploadModal').modal('show').on('shown.bs.modal', function () {
        var backdrops = $('.modal-backdrop');

        if (backdrops.length === 0) {
            // If no backdrop exists, add it manually
            $('<div class="modal-backdrop fade show"></div>').appendTo(document.body);
        } else if (backdrops.length > 1) {
            // If more than one backdrop exists, remove the extras
            backdrops.not(':first').remove();
        }
    });
});

$('#fileInput').change(function (event) {
    const file = event.target.files[0];
    if (file) {
        // Validate file type
        const validTypes = [
            'application/vnd.ms-excel', // .xls
            'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet', // .xlsx
            'application/vnd.ms-excel.sheet.macroEnabled.12' // .xlsm
        ];

        if (!validTypes.includes(file.type)) {
            showDangerAlert('Please select a valid Excel file (xls or xlsx).');
            $('#fileInput').val('');
        }
    }
});

$('#close-button').on('click', function () {
    $('#uploadModal').modal('hide');
});

$('#modalClose').on('click', function () {
    $('#uploadModal').modal('hide');
});

function handleImportExcelFile(url, expectedColumns)
{
    $('#uploadModal').modal('hide').on('hidden.bs.modal', function () {
        $('.modal-backdrop').remove(); // Remove the backdrop after modal hides
    });
    Blockloadershow();
    var fileInput = $('#fileInput')[0];
    var file = fileInput.files[0];

    if (file) {
        // Validate file type
        var validTypes = ['application/vnd.ms-excel', 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet', 'application/vnd.ms-excel.sheet.macroEnabled.12'];
        if (validTypes.includes(file.type)) {
            // Read the uploaded Excel file
            var reader = new FileReader();
            reader.onload = async function (e) {
                var data = e.target.result;

                // Load the workbook using exceljs
                const ExcelJS = window.ExcelJS; // Ensure ExcelJS is included
                const workbook = new ExcelJS.Workbook();
                await workbook.xlsx.load(data);

                // Get the first sheet and its header
                const worksheet = workbook.worksheets[0];
                const firstRow = worksheet.getRow(1);
                const uploadedColumns = firstRow.values.slice(1).map(cell => {
                    if (cell === null || cell === undefined) return '';
                    if (typeof cell === 'object') {
                        if (cell.richText && Array.isArray(cell.richText)) {
                            // Concatenate rich text segments
                            return cell.richText.map(segment => segment.text || '').join('').trim();
                        }
                        return (cell.text || cell.toString()).trim();
                    }
                    return cell.toString().trim();
                });

                // Check if uploaded columns match expected columns
                var columnsMatch = uploadedColumns.length === expectedColumns.length &&
                    uploadedColumns.every(function (col, index) {
                        return col === expectedColumns[index];
                    });

                if (columnsMatch) {

                    const numberOfRecords = worksheet.rowCount - 1;
                    const uploadDate = new Date().toLocaleString();
                    var fileName = file.name;

                    var formData = new FormData();
                    formData.append('file', file);
                    formData.append('fileName', fileName);
                    formData.append('uploadDate', uploadDate);
                    formData.append('recordCount', numberOfRecords);
                    $.ajax({
                        url: url,
                        type: 'POST',
                        data: formData,
                        contentType: false,
                        processData: false,
                        success: function (response) {
                            Blockloaderhide();
                            if (response.success) {
                                showSuccessAlert("Data uploaded successfully.");

                                //// Display file details in the modal
                                //$('#uploadModal').modal('show');
                                //$('#fileNameDisplay').text(file.name);
                                //$('#uploadDateDisplay').text(uploadDate);
                                //$('#recordCountDisplay').text(numberOfRecords);

                                AutoReload();
                            }
                            else {
                                showDangerAlert(response.message);
                                AutoReload();
                            }
                        },
                        error: function () {
                            Blockloaderhide();
                            showDangerAlert('Error occured while uploading data.');
                        }
                    });
                } else {
                    Blockloaderhide();
                    $('#fileInput').val('');
                    showDangerAlert('Please upload data using the provided sample Master template file.');
                }
            };

            reader.readAsArrayBuffer(file);
        } else {
            Blockloaderhide();
            $('#fileInput').val('');
            showDangerAlert('Please select a valid Excel file (xls or xlsx).');
        }
    } else {
        Blockloaderhide();
        showDangerAlert('Please select a file to upload.');
    }
}
