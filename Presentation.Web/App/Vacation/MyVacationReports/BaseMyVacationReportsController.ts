module app.vacation {
    "use strict";

    import KendoGrid = core.interfaces.KendoGrid;
    import Person = core.models.Person;

    export abstract class BaseMyVacationReportsController {

        vacationReportsGrid: KendoGrid; // Kendo typing is missing some parameters that this controller is using
        vacationReportsOptions: kendo.ui.GridOptions;
        vacationYear: Date;
        personId: number;

        private isGridLoaded = false;

        constructor(
            protected $scope,
            protected $modal: angular.ui.bootstrap.IModalService,
            protected $rootScope,
            protected VacationReport, // TODO Make $resource interface for VacationReport
            protected $timeout: ng.ITimeoutService,
            protected Person: Person,
            protected moment: moment.MomentStatic,
            protected $state: ng.ui.IStateService) {

            this.loadInitialDate();

            // Set personId. The value on $rootScope is set in resolve in application.js
            this.personId = $rootScope.CurrentUser.Id;

            this.$scope.$on("kendoRendered",
                () => {
                    if (!this.isGridLoaded) {
                        this.isGridLoaded = true;
                        this.refreshGrid();
                    }
                });
        }

        refreshGrid() {
            if (!this.isGridLoaded) return;
            this.vacationReportsGrid.dataSource.transport.options.read.url = this.getVacationReportsUrl();
            this.vacationReportsGrid.dataSource.read();
        }

        deleteClick = (id) => {
            this.$modal.open({
                templateUrl: '/App/Vacation/MyVacationReports/ConfirmDeleteVacationReportTemplate.html',
                controller: 'ConfirmDeleteVacationReportController as cdvrCtrl',
                backdrop: "static",
                resolve: {
                    itemId: () => {
                        return id;
                    }
                }
            }).result.then(() => {
                this.VacationReport.delete({ id: id }, () => {
                    this.refreshGrid();
                });
            });
        }

        clearClicked() {
            this.loadInitialDate();
            this.refreshGrid();
        }

        // Format for datepickers.
        dateOptions: kendo.ui.DatePickerOptions = {
            format: "yyyy",
            start: "decade",
            depth: "decade"
        };

        private loadInitialDate() {
            let currentDate = this.moment();

            // Vacation year changes every year on the 1th of May
            if (this.moment().isBefore(`${this.vacationYear}-05-01`)) {
                currentDate = currentDate.subtract('year', 1);
            }

            this.vacationYear = currentDate.toDate();
        }

        protected abstract getVacationReportsUrl();
    }
}
