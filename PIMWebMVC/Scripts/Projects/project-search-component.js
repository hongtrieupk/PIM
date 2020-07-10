function SearchCriteriaModel(projectNumber, projectName, customer, status, currentPage) {
    this.ProjectNumber = projectNumber ? projectNumber : null;
    this.ProjectName = projectName ? projectName.trim() : "";
    this.Customer = customer;
    this.Status = status ? status.trim() : "";
    this.CurrentPage = currentPage ? currentPage : 1;

    this.isNotNullCriteria = function () {
        return this.ProjectNumber || this.ProjectName || this.Customer || this.Status;
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
    this.firstPagingBtnName = "first-pagination-btn";
    this.lastPagingBtnName = "last-pagination-btn";
    this.nextPagingBtnName = "next-pagination-btn";
    this.previousPagingBtnName = "previous-pagination-btn";
    this.totalPageInputId = "total-page-val";
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
    onSearching: function (initFromStartPage) {
        this.serverErrorMessageLbl.hide();
        this.missingCriteriaLbl.hide();
        this.projectNumberNotInRangeLbl.hide()
        var searchParam = this.getSearchParam();
        if (!searchParam.isNotNullCriteria()) {
            this.missingCriteriaLbl.show();
            return;
        }
        var searchComponent = this;
        if (initFromStartPage) {
            this.currentPage = searchComponent.startPage;
            searchParam.CurrentPage = this.currentPage;
        }
        $.ajax({
            type: "post",
            url: this.searchProjectUrl,
            data: searchParam,
            success: function (response) {
                // in case of the whole exception page is returned from the server => re-write the whole current html page
                if (searchComponent.isAnExceptionPageResponse(response)) {
                    document.write(response);
                    return;
                }
                // append html from data response to the <div id="projects-search-result">
                searchComponent.renderPaginationComponent.call(searchComponent, response);
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
        $("button[name='" + searchComponent.firstPagingBtnName + "']").on("click", searchComponent.onFirstPageClick.bind(searchComponent));
        $("button[name='" + searchComponent.nextPagingBtnName + "']").on("click", searchComponent.onNextPageClick.bind(searchComponent));
        $("button[name='" + searchComponent.lastPagingBtnName + "']").on("click", searchComponent.onLastPageClick.bind(searchComponent));
        $("button[name='" + searchComponent.previousPagingBtnName + "']").on("click", searchComponent.onPreviousPageClick.bind(searchComponent));
    },
    onFirstPageClick: function () {
        this.currentPage = this.startPage;
        this.onSearching.call(this);
    },
    onNextPageClick: function () {
        let totalPage = $("#" + this.totalPageInputId).val();
        this.currentPage = this.currentPage < totalPage ? this.currentPage + 1 : this.currentPage;
        this.onSearching.call(this);
    },
    onPreviousPageClick: function () {
        this.currentPage = this.currentPage > this.startPage ? this.currentPage - 1 : this.currentPage;
        this.onSearching.call(this);
    },
    onLastPageClick: function () {
        let totalPage = $("#" + this.totalPageInputId).val();
        this.currentPage = totalPage > 0 ? totalPage : this.currentPage;
        this.onSearching.call(this);
    },
    setEventForDeleteCheckBoxes: function () {
        let searchComponent = this;
        let deleteCheckboxes = $("input[name='" + this.deleteProjectCheckboxName + "']");
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
                if (searchComponent.isAnExceptionPageResponse(response)) { // server error exception was throwed
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
    },
    isAnExceptionPageResponse: function (serverResponse) {
        return typeof serverResponse === "string" && serverResponse.indexOf("<html") > -1;
    }
};

$(document).ready(function () {
    let projectSearchComponent = new ProjectSearchComponent();
    projectSearchComponent.initEvent();
});
