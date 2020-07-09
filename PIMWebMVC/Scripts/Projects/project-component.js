function ProjectComponent() {
    this.formHasErrorLbl = $("#form-has-error-lbl");

}


$(document).ready(function () {
    window.preventLeavePage();
    let projectComponent = new ProjectComponent();
    window.hasDirtyForm = projectComponent.formHasErrorLbl && !!projectComponent.formHasErrorLbl.text();
   

});
