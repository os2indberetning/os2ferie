declare namespace  app.core.interfaces {
    export interface KendoGrid extends kendo.ui.Grid {
        dataSource: any; // This is the type kendo uses in GridOptions
    }
}