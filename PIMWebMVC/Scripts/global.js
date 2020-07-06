$.notify.addStyle('custom-notify', {
    html: "<div><span data-notify-text/></div>",
    classes: {
        base: {
            "padding": "30px",
            "border-radius": "10px",
            "font-weight": "bold",
            "padding-left": "23px",
            "background-repeat": "no-repeat",
            "background-position": "3px 7px",
        },
        success: {
            "background-color": "#DFF0D8",
            "border": "1px solid #468847",
            "color": "#468847"
        }
        ,
        error: {
            "background-color": "#F2DEDE",
            "border": "1px solid #B94A48",
            "color": "#B94A48"
        }
    }
});
var notifyConfig = { position: "top center", style: 'custom-notify', className: 'success', autoHideDelay: 5000 };
function showErrorNotify(message) {
    notifyConfig.className = 'error';
    $.notify(message, notifyConfig);
}
function showSuccessNotify(message) {
    notifyConfig.className = 'success';
    $.notify(message, notifyConfig);
}