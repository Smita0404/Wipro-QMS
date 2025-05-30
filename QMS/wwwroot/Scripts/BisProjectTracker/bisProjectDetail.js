
$(document).ready(function () {

    // Add keydown event for Ctrl + S
    // $(document).keydown(function (event) {
    //     // Check if Ctrl key (or Cmd on Mac) is pressed along with the 'S' key
    //     if ((event.ctrlKey || event.metaKey) && event.key === "s") {
    //         event.preventDefault(); // Prevent the default browser save action
    //         if (!isLock)
    //         {
    //             InsertUpdateCPSLog(true);
    //         }
    //         else {
    //             showDangerAlert("Save is disabled because the record is locked.");
    //         }
    //         // InsertUpdateCPSLog(true); // Call the function
    //     }
    // });

    // $('.select2').select2({
    //     // placeholder: "Select CPR No.", // Placeholder text
    //     allowClear: true,
    // });
});

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
    // Clear error messages if needed
    document.querySelectorAll('.text-danger').forEach(function (error) {
        error.textContent = '';
    });
}

function InsertUpdateBisProject() {
    Blockloadershow();
    var errorMsg = "";
    var fields = "";

    if (errorMsg != "") {
        Blockloaderhide();
        showDangerAlert(errorMsg);
        return false;
    }

    var ajaxUrl = "";
    if ($("#hdnId").val() != "0") {
        ajaxUrl = '/BisProjectTrac/Update';
    }
    else {
        ajaxUrl = '/BisProjectTrac/Create';
    }

    var Model = {
        Id: $("#hdnId").val(),
        Financial_Year: $("#Fin_Year").val(),
        Mon_PC: $("#Mon_PC").val(),
        Nat_Project: $("#Nat_Project").val(),
        Lea_Model_No: $("#Lea_Model_No").val(),
        No_Seri_Add: $("#No_Seri_Add").val(),
        Cat_Ref_Lea_Model: $("#Cat_Ref_Lea_Model").val(),
        Section: $("#Section").val(),
        Manuf_Location: $("#Manuf_Location").val(),
        CCL_Id: $("#CCL_Id").val(),
        Lab: $("#Lab").val(),
        Report_Owner: $("#Report_Owner").val(),
        Start_Date: $("#Start_Date").val(),
        Comp_Date: $("#Comp_Date").val(),
        Test_Duration: $("#Test_Duration").val(),
        Submitted_Date: $("#Submitted_Date").val(),
        Received_Date: $("#Received_Date").val(),
        Bis_Duration: $("#Bis_Duration").val(),
        Dispatch_Date: $("#Dispatch_Date").val(),
        Remark: $("#Remark").val()
    };

    $.ajax({
        type: "POST",
        url: ajaxUrl,
        data: Model,
        success: function (response) {
            Blockloaderhide();
            if (response.success) {
                if ($("#hdnId").val() != "0") {
                    showSuccessAlert("Bis Projecet Tracker Detail updated successfully!");
                }
                else {
                    showSuccessAlert("Bis Projecet Tracker Detail Saved Successfully!");
                }

                setTimeout(function () {
                    window.location.href = '/BisProjectTrac/BisProjectTracker';
                }, 2500);
            }
            else if (response.message == "Exist") {
                showDangerAlert("Bis Projecet Tracker Detail already exist.");
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

