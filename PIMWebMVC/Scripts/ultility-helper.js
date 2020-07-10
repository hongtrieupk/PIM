function UltilityHelper() {

}
UltilityHelper.prototype = {
    getUserLanguageCode: function () {
        if (navigator.languages && navigator.languages.length) {
            return navigator.languages[0];
        } else {
            return navigator.userLanguage || navigator.language || navigator.browserLanguage || 'en';
        }
    },
    renderjQueryDatePicker: function () {
        let currentLanguageCode = this.getUserLanguageCode();
        let localeDatepickerConfig = $.datepicker.regional[currentLanguageCode];
        localeDatepickerConfig = !!localeDatepickerConfig ? localeDatepickerConfig : {};
        localeDatepickerConfig.dateFormat = $("#currentDateFormat").text(),
            localeDatepickerConfig.changeMonth = true;
        localeDatepickerConfig.changeYear = true;
        $(".date-picker").datepicker(localeDatepickerConfig).keypress(function (event) { event.preventDefault(); });
    },
    setEnterKeyPressForDefaulButton: function () {
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
    }
}