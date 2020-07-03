﻿function SearchCriteriaModel(projectNumber, projectName, customer, status, currentPage) {
    this.ProjectNumber = Boolean(projectNumber) ? projectNumber : null;
    this.ProjectName = Boolean(projectName) ? projectName.trim() : "";
    this.Customer = customer;
    this.Status = Boolean(status) ? status.trim() : "";
    this.CurrentPage = Boolean(currentPage) ? currentPage : 1;

    this.isValidCriteria = function () {
        return this.ProjectNumber || this.ProjectName || this.Customer || this.Status;
    }
}

function ProjectSearchComponent() {
    this.startPage = 1;
    this.searchBtn = $("#search-projects-btn");
    this.resetCriteriaBtn = $("#reset-criteria-btn");
    this.searchContent = $("#projects-search-result");
    this.missingCriteriaLbl = $("#missing-criteria-lbl");

    this.numberSearchTxt = $("#number-search-txt");
    this.customerSearchTxt = $("#customer-search-txt");
    this.statusSearchCbb = $("#status-search-cbb");
    this.nameSearchTxt = $("#name-search-txt");

    this.pagingButtonsName = "paging-button";
    this.isValidComponent = false;
    this.currentPage = this.startPage;
    this.searchProjectUrl = "/Projects/SearchProjects";
}

ProjectSearchComponent.prototype = {
    initEvent: function () {
        this.isValidComponent = this.isValidHtmlElements();
        if (!this.isValidComponent) {
            throw "some html elements was not initialized correctly!";
        }
        this.searchBtn.on("click", this.onSearching.bind(this));
        this.resetCriteriaBtn.on("click", this.onResetCriteria.bind(this))
    },
    onPageClick: function (event) {
        let source = event.target || event.srcElement;
        let totalPage = $("#total-page").val();
        this.currentPage = this.calculateCurrentPage.call(this, source.text, totalPage);
        this.onSearching.call(this);
    },
    onSearching: function () {
        var searchParam = this.getSearchParam();
        if (!searchParam.isValidCriteria()) {
            this.missingCriteriaLbl.show();
            return;
        }
        this.missingCriteriaLbl.hide();
        var searchComponent = this;
        $.ajax({
            type: "post",
            url: this.searchProjectUrl,
            data: searchParam,
            success: function (data) {
                searchComponent.renderPaginationComponent.call(searchComponent, data);
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
    },
    renderPaginationComponent: function (htmlData) {
        this.searchContent.html(htmlData);
        this.setPagingButtonsEvent.call(this);
        this.setEventForDeleteBtn();
    },
    setPagingButtonsEvent: function () {
        var searchComponent = this;
        var pagingButtons = $("a[name='" + this.pagingButtonsName + "']");
        pagingButtons.each(function () {
            this.onclick = searchComponent.onPageClick.bind(searchComponent);
        });
    },
    setEventForDeleteBtn: function () {
        var deleteBtn = $("#delete-projects-btn");
        var tableBody = document.getElementById("projects-table-body");
        let rows = Boolean(tableBody) ? tableBody.rows : null;
        if (!Boolean(deleteBtn.length) || !Boolean(rows)) {
            return;
        }
        deleteBtn.on("click", function () {
            let selectedProjectIds = [];
            for (let i = rows.length - 1; i >= 0; i--) {
                let rowCheckbox = rows[i].cells[0].firstElementChild;
                if (rowCheckbox.checked) {
                    selectedProjectIds.push(rowCheckbox.value);
                }
            }
            console.log(selectedProjectIds);
        });
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
        totalPageNumber = Boolean(totalPageNumber) ? totalPageNumber : 0;
        switch (pagingBtnValue) {
            case firstBtn:
                this.currentPage = this.startPage;
                break;
            case previousBtn:
                this.currentPage = this.currentPage > this.startPage ? this.currentPage -1 : this.currentPage;
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
    }
};


$(document).ready(function () {
    let projectSearchComponent = new ProjectSearchComponent();
    projectSearchComponent.initEvent();
});