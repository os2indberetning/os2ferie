module app.vacation {
    "use strict";

    import KendoGrid = core.interfaces.KendoGrid;
    import Person = core.models.Person;
    import NotificationService = app.core.interfaces.NotificationService;

    export abstract class BaseMyVacationReportsController {

        vacationReportsGrid: KendoGrid; // Kendo typing is missing some parameters that this controller is using
        vacationReportsOptions: kendo.ui.GridOptions;
        vacationYear: number;
        personId: number;

        vacationYears = [];

        private isGridLoaded = false;

        constructor(
            protected $scope,
            protected $modal: angular.ui.bootstrap.IModalService,
            protected $rootScope,
            protected VacationReport, // TODO Make $resource interface for VacationReport
            protected $timeout: ng.ITimeoutService,
            protected Person: Person,
            protected moment: moment.MomentStatic,
            protected $state: ng.ui.IStateService,
            protected NotificationService: NotificationService) {

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
                    this.NotificationService.AutoFadeNotification("success", "", "Indberetningen blev slettet.");
                    this.refreshGrid();
                }, () =>
                {
                    this.NotificationService.AutoFadeNotification("error", "", "Fejl under sletning af indberetning.");
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

            this.vacationYear = currentDate.year();

            const minYear = Math.max(2016, this.vacationYear - 5); // Can't show vacation before 2016
            const maxYear = this.vacationYear + 5;

            for (let i = minYear; i < maxYear; i++) {
                this.vacationYears.push({
                    Year: i,
                    YearString: `${i}/${i + 1}`
                });
            }

        }

        protected abstract getVacationReportsUrl();
    }
}
