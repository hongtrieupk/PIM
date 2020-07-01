function ProjectComponent() {
    this.startDateTxt = $("#start-date");
    this.endDateTxt = $("#end-date");
    this.isValidComponent = false;
}

ProjectComponent.prototype = {
    initEvents: function () {
        this.isValidComponent = this.isValidHtmlElements();
        if (!this.isValidComponent) {
            throw "some html elements was not initialized correctly!";
        }
        this.setValidateFormWhenInputChange();
    },
    setValidateFormWhenInputChange: function () {
        let startDateInput = this.startDateTxt;
        let endDateInput = this.endDateTxt;
        startDateInput.change(function () {
            endDateInput.valid();
        });
        endDateInput.change(function () {
            startDateInput.valid();
        });
    },
    isValidHtmlElements: function () {
        return (Boolean(this.startDateTxt.length)
            && Boolean(this.endDateTxt.length)
        );
    }
};

$(document).ready(function () {
    let projectComponent = new ProjectComponent();
    projectComponent.initEvents();
});
