$.notify.addStyle("custom-notify", {
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
window.showErrorNotify = function(message) {
    notifyConfig.className = "error";
    $.notify(message, notifyConfig);
}
window.showSuccessNotify = function(message) {
    notifyConfig.className = "success";
    $.notify(message, notifyConfig);
}
window.defaulButtonName = "default-button";
window.hasDirtyForm = false;

window.preventLeavePage = function() {
    $(":input").change(function () {
        if (!hasDirtyForm) {
            hasDirtyForm = true;
        }
        $("input[type=submit]").prop("disabled", false);
    });
    $(":input").keydown(function () {
        if (!hasDirtyForm) {
            hasDirtyForm = true;
        }
    });
    $("form").submit(function (event) {
            hasDirtyForm = false;
    });
    window.onbeforeunload = function (e) {
        if (hasDirtyForm) {
            return false;
        }
        $("input[type=submit]").prop("disabled", true);
    }
}
$(document).ready(function () {
    $('body').keypress(function (e) {
        var code = e.keyCode || e.which;
        if (code === 13) {
            e.preventDefault();
            let defaultBtns = $("button[name='" + window.defaulButtonName + "']");
            if (defaultBtns && defaultBtns.length > 0) {
                defaultBtns[0].click();
            }
            let submitBtn = $("input[type='submit']");
            submitBtn.click();
        }
    });
    $(".date-picker").datepicker({
        dateFormat: $("#currentDateFormat").text(),
        showButtonPanel: true,
        changeMonth: true,
        changeYear: true,
        showOtherMonths: true,
        selectOtherMonths: true
    }).keypress(function (event) { event.preventDefault(); });
});
