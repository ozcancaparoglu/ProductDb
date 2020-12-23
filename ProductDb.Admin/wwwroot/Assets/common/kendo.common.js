var KendoGrid = {
    ExportToExcel: function (selector) {
        var grid = $("#" + selector).data("kendoGrid");
        grid.saveAsExcel();
    },
    CreateKendoGridFromTable: function (selector, ) {

    }
}