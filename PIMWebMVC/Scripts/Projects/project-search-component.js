﻿function SearchCriteriaModel(projectNumber, projectName, customer, status, currentPage) {
    this.ProjectNumber = projectNumber ? projectNumber : null;
    this.ProjectName = projectName ? projectName.trim() : "";
    this.Customer = customer;
    this.Status = status ? status.trim() : "";
    this.CurrentPage = currentPage ? currentPage : 1;

    this.isNotNullCriteria = function () {
        return this.ProjectNumber || this.ProjectName || this.Customer || this.Status;
    }
    this.isProjectNumberInRange = function (min, max) {
        return min <= +this.ProjectNumber && +this.ProjectNumber <= max;
    }
}

function ProjectSearchComponent() {
    this.startPage = 1;
    this.searchBtn = $("#search-projects-btn");
    this.resetCriteriaBtn = $("#reset-criteria-btn");
    this.searchContent = $("#projects-search-result");
    this.missingCriteriaLbl = $("#missing-criteria-lbl");
    this.serverErrorMessageLbl = $("#server-error-message-lbl");
    this.projectNumberNotInRangeLbl = $("#project-number-not-in-range-lbl");
    this.confirmDeleteModal = $("#confirm-delete-modal");
    this.deleteBtnInModal = $("#delete-projects-btn-in-modal");

    this.numberSearchTxt = $("#number-search-txt");
    this.customerSearchTxt = $("#customer-search-txt");
    this.statusSearchCbb = $("#status-search-cbb");
    this.nameSearchTxt = $("#name-search-txt");

    this.tableBodyId = "projects-table-body";
    this.pagingButtonsName = "paging-button";
    this.deleteProjectCheckboxName = "delete-project-checkbox";
    this.deleteButtonId = "delete-projects-btn";
    this.isValidComponent = false;
    this.currentPage = this.startPage;
    this.searchProjectUrl = "/Projects/SearchProjects";
    this.deleteProjectsUrl = "/Projects/DeleteProjects";
    this.serverErrorPage = "Error/ServerError";

    this.isDisplayingResult = false;
}

ProjectSearchComponent.prototype = {
    initEvent: function () {
        this.isValidComponent = this.isValidHtmlElements();
        if (!this.isValidComponent) {
            throw "some html elements was not initialized correctly!";
        }
        let searchFromStartPage = true;
        this.searchBtn.on("click", this.onSearching.bind(this, searchFromStartPage));
        this.resetCriteriaBtn.on("click", this.onResetCriteria.bind(this));
        this.deleteBtnInModal.on("click", this.deleteSelectedProjects.bind(this));

        this.numberSearchTxt.on("keyup", this.checkEnableResetCriteriaBtn.bind(this));
        this.customerSearchTxt.on("keyup", this.checkEnableResetCriteriaBtn.bind(this));
        this.statusSearchCbb.on("keyup change", this.checkEnableResetCriteriaBtn.bind(this));
        this.nameSearchTxt.on("keyup", this.checkEnableResetCriteriaBtn.bind(this));
    },
    onPageClick: function (event) {
        let source = event.target || event.srcElement;
        let totalPage = $("#total-page").val();
        this.currentPage = this.calculateCurrentPage.call(this, source.textContent, totalPage);
        this.onSearching.call(this);
    },
    onSearching: function (initFromStartPage) {
        this.serverErrorMessageLbl.hide();
        this.missingCriteriaLbl.hide();
        this.projectNumberNotInRangeLbl.hide()
        var searchParam = this.getSearchParam();
        if (!searchParam.isNotNullCriteria()) {
            this.missingCriteriaLbl.show();
            return;
        }
        const minProjectNumber = 0;
        const maxProjectNumber = 2147483647;
        if (!searchParam.isProjectNumberInRange(minProjectNumber, maxProjectNumber)) {
            this.projectNumberNotInRangeLbl.show();
            return;
        }
        var searchComponent = this;
        if (initFromStartPage) {
            searchParam.CurrentPage = searchComponent.startPage;
        }
        $.ajax({
            type: "post",
            url: this.searchProjectUrl,
            data: searchParam,
            success: function (data) {
                if (typeof data === "string" && data.indexOf("<html") > -1) {
                    document.write(data);
                    return;
                }
                searchComponent.renderPaginationComponent.call(searchComponent, data);
                searchComponent.isDisplayingResult = true;
            },
            error: function () {
                window.location.href = searchComponent.serverErrorPage;
            }
        });
    },
    onResetCriteria: function () {
        this.numberSearchTxt.val("");
        this.nameSearchTxt.val("");
        this.customerSearchTxt.val("");
        this.statusSearchCbb.val("");
        this.currentPage = this.startPage;
        const emptyHtmlContent = "";
        this.searchContent.html(emptyHtmlContent);
        this.isDisplayingResult = false;
        this.checkEnableResetCriteriaBtn();
    },
    renderPaginationComponent: function (htmlData) {
        this.searchContent.html(htmlData);
        this.setPagingButtonsEvent.call(this);
        this.setEventForDeleteBtn.call(this);
        this.setEventForDeleteCheckBoxes.call(this);
    },
    setPagingButtonsEvent: function () {
        var searchComponent = this;
        var pagingButtons = $("button[name='" + this.pagingButtonsName + "']");
        pagingButtons.each(function () {
            this.onclick = searchComponent.onPageClick.bind(searchComponent);
        });
    },
    setEventForDeleteCheckBoxes: function () {
        var searchComponent = this;
        var deleteCheckboxes = $("input[name='" + this.deleteProjectCheckboxName + "']");
        deleteCheckboxes.each(function () {
            this.onclick = searchComponent.checkEnableDeleteBtn.bind(searchComponent);
        });
    },
    deleteSelectedProjects: function () {
        let searchComponent = this;
        let selectedProjects = searchComponent.getSelectedProjectsFromHtmlTable.call(searchComponent);       
        $.ajax({
            type: "post",
            url: searchComponent.deleteProjectsUrl,
            data: { projects: selectedProjects },
            success: function (response) {
                if (typeof response === "string" && response.indexOf("<html") > -1) { // server error exception was throwed
                    document.write(response);
                    return;
                }
                if (response.IsSuccess) {
                    showSuccessNotify(response.Message);
                    searchComponent.onSearching();
                } else {
                    searchComponent.serverErrorMessageLbl.text(response.Message);
                    searchComponent.serverErrorMessageLbl.show();
                }
            },
            error: function () {
                window.location.href = this.serverErrorPage;
            }
        });
    },
    setEventForDeleteBtn: function () {
        let searchComponent = this;
        var deleteBtn = $("#" + searchComponent.deleteButtonId);
        deleteBtn.on("click", function () {
            searchComponent.confirmDeleteModal.modal();
            searchComponent.deleteBtnInModal.focus();
        });
    },
    getSelectedProjectsFromHtmlTable: function () {
        var tableBody = document.getElementById(this.tableBodyId);
        let rows = tableBody ? tableBody.rows : null;
        if (!rows) {
            return [];
        }
        let selectedProjects = [];
        for (let i = rows.length - 1; i >= 0; i--) {
            let checkboxRow = rows[i].cells[0].firstElementChild;
            if (checkboxRow.checked) {
                let projectModel = this.getProjectModelFromHtmlTableRow(rows[i]);
                selectedProjects.push(projectModel);
            }
        }
        return selectedProjects;
    },
    getProjectModelFromHtmlTableRow: function (htmlTableRow) {
        if (!htmlTableRow || !htmlTableRow.cells || htmlTableRow.cells.length !== 7) {
            throw "project html table was composed incorrectly";
        }
        let checkboxRow = htmlTableRow.cells[0].firstElementChild;
        let projectId = checkboxRow.value;

        let projectNumberRow = htmlTableRow.cells[1].firstElementChild;
        let projectNumber = projectNumberRow.text;

        let projectNameRow = htmlTableRow.cells[2];
        let projectName = projectNameRow.textContent;

        let statusRow = htmlTableRow.cells[3];
        let status = statusRow.textContent;

        let customerRow = htmlTableRow.cells[4];
        let customer = customerRow.textContent;

        let startDateRow = htmlTableRow.cells[5];
        let startDate = startDateRow.textContent;

        let versionRow = htmlTableRow.cells[6];
        let version = versionRow.textContent;

        return new ProjectModel(projectId, projectNumber, projectName, customer, status, startDate, null, version);
    },
    getSearchParam: function () {
        let number = this.numberSearchTxt.val();
        let name = this.nameSearchTxt.val();
        let customer = this.customerSearchTxt.val();
        let status = this.statusSearchCbb.val();
        return new SearchCriteriaModel(number, name, customer, status, this.currentPage);
    },
    isValidHtmlElements: function () {
        return (Boolean(this.numberSearchTxt.length)
            && Boolean(this.customerSearchTxt.length)
            && Boolean(this.statusSearchCbb.length)
            && Boolean(this.nameSearchTxt.length)
            && Boolean(this.searchBtn.length)
            && Boolean(this.searchContent.length)
            && Boolean(this.resetCriteriaBtn.length)
        );
    },
    calculateCurrentPage: function (pagingBtnValue, totalPage) {
        // button text value is defined in the PIMResource file: PAGINATION_*
        const firstBtn = "First";
        const previousBtn = "Previous";
        const nextBtn = "Next";
        const lastBtn = "Last";
        let totalPageNumber = +totalPage;
        totalPageNumber = totalPageNumber ? totalPageNumber : 0;
        switch (pagingBtnValue) {
            case firstBtn:
                this.currentPage = this.startPage;
                break;
            case previousBtn:
                this.currentPage = this.currentPage > this.startPage ? this.currentPage - 1 : this.currentPage;
                break;
            case nextBtn:
                this.currentPage = this.currentPage < totalPage ? this.currentPage + 1 : this.currentPage;
                break;
            case lastBtn:
                this.currentPage = totalPageNumber > 0 ? totalPageNumber : this.currentPage;
                break;
            default:
                break;
        }
        return this.currentPage;
    },
    checkEnableResetCriteriaBtn: function () {
        let searchParam = this.getSearchParam();
        if (!searchParam.isNotNullCriteria() && !this.isDisplayingResult) {
            this.resetCriteriaBtn.prop('disabled', true);
        } else {
            this.resetCriteriaBtn.prop('disabled', false);
        }
    },
    checkEnableDeleteBtn: function () {
        let deleteBtn = $("#" + this.deleteButtonId);
        let selectedProjects = this.getSelectedProjectsFromHtmlTable.call(this);
        if (!selectedProjects || !selectedProjects.length) {
            deleteBtn.prop('disabled', true);
        } else {
            deleteBtn.prop('disabled', false);
        }
    }
};


$(document).ready(function () {
    let projectSearchComponent = new ProjectSearchComponent();
    projectSearchComponent.initEvent();
});