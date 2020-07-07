function ProjectComponent() {
    this.confirmSaveProjectMessage = $("#confirm-save-message-span").text();
    this.btnSaveProject = $("#save-project-btn");
    this.projectForm = $("#project-form");
}

ProjectComponent.prototype = {
    initEvent: function () {
        this.btnSaveProject.on("click", this.onSavingProject.bind(this));
    },
    onSavingProject: function () {
        this.projectForm.submit();
    }
};

$(document).ready(function () {
    let projectComponent = new ProjectComponent();
    projectComponent.initEvent();
});
