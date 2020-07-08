function ProjectComponent() {
    this.formHasErrorLbl = $("#form-has-error-lbl");
    this.currentDateFormatLbl = $("#currentDateFormat");
    this.currentDateFormatValue = $("#currentDateFormat").text();

}


$(document).ready(function () {
    window.preventLeavePage();
    let projectComponent = new ProjectComponent();
    window.hasDirtyForm = projectComponent.formHasErrorLbl && !!projectComponent.formHasErrorLbl.text();
    console.log(projectComponent.currentDateFormatValue);
    $(".date-picker").datepicker({
        dateFormat: projectComponent.currentDateFormatValue,
        showButtonPanel: true,
        changeMonth: true,
        changeYear: true,
        showOtherMonths: true,
        selectOtherMonths: true
    }).keypress(function (event) { event.preventDefault(); });

});
