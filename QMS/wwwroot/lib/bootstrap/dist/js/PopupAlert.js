
// Override Noty defaults
Noty.overrideDefaults({
    theme: 'limitless',
    layout: 'topRight',
    type: 'alert',
    timeout: 2500
});



// Styled with white background
function showSuccessAlert(Texts) {
    new Noty({
        theme: ' alert alert-success alert-styled-left p-0 ',
        text: Texts,
        progressBar: true,
        closeWith: ['button']
    }).show();
 }


function showDangerAlert(Texts) {
     new Noty({
         theme: ' alert alert-danger alert-styled-left p-0 ',
         text: Texts,
         progressBar: true,
         closeWith: ['button']
     }).show();
}

function showWarningAlert(Texts) {
    new Noty({
        theme: ' alert alert-warning alert-styled-left p-0 ',
        text: Texts,
        progressBar: true,
        closeWith: ['button']
    }).show();
}


function showInfoAlert(Texts) {
    new Noty({
        theme: ' alert alert-info alert-styled-left p-0 ',
        text: Texts,
        progressBar: true,
        closeWith: ['button']
    }).show();
}

function showPrimaryAlert(Texts) {
    new Noty({
        theme: ' alert alert-primary alert-styled-left p-0 ',
        text: Texts,  
        progressBar: true,
        closeWith: ['button']
    }).show();

}

function Blockloadershow() {
    $.blockUI({
        message: '<span class="font-weight-semibold"><i class="icon-spinner4 spinner mr-2"></i>&nbsp;Please Wait...</span>',
       // timeout: 2000, //unblock after 2 seconds
        overlayCSS: {
            backgroundColor: '#1b2024',
            opacity: 0.8,
            cursor: 'wait'
        },
        css: {
            border: 0,
            padding: '10px 15px',
            color: '#fff',
            left: '44%',
            width: 'auto',
            '-webkit-border-radius': 3,
            '-moz-border-radius': 3,
            backgroundColor: '#333'
        }
    });
}

function Blockloaderhide() {
    $.unblockUI({
     //   timeout: 2000,
    });
}





