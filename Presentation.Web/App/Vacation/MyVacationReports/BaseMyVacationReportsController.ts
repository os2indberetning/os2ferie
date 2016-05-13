module app.vacation {
    "use strict";

    export abstract class BaseMyVacationReportsController {

        vacationReportsGrid: kendo.ui.Grid;
        vacationReportsOptions: kendo.ui.GridOptions;
        vacationYear: number;
        personId: number;

        constructor(
            protected $modal,
            protected $rootScope,
            protected VacationReport,
            protected $timeout,
            protected Person,
            protected moment,
            protected ReportStatus) {

            // Set personId. The value on $rootScope is set in resolve in application.js
            this.personId = $rootScope.CurrentUser.Id;

            this.loadInitialDates();
        }

        refreshGrid() {
            /// <summary>
            /// Refreshes kendo grid datasource.
            /// </summary>
            this.vacationReportsGrid.refresh();
        }

        loadInitialDates() {
            /// <summary>
            /// Sets initial date filters.
            /// </summary>
            // Set initial values for kendo datepickers.
            // date for kendo filter.
            this.vacationYear = this.moment().year();

            if (this.moment().isBefore(`${this.vacationYear}-05-01`)) {
                this.vacationYear--;
            }
        }

        editClick = (id) => {
            /// <summary>
            /// Opens edit report modal
            /// </summary>
            /// <param name="id"></param>
            var modalInstance = this.$modal.open({
                templateUrl: '/App/Vacation/MyVacationReports/EditVacationReportTemplate.html',
                controller: 'ReportVacationController as rvc',
                backdrop: "static",
                windowClass: "app-modal-window-full",
                resolve: {
                    adminEditCurrentUser() {
                        return 0;
                    },
                    ReportId: () => {
                        return id;
                    }
                }
            });

            modalInstance.result.then(() => {
                this.vacationReportsGrid.refresh();
            });
        }

        clearClicked() {
            this.loadInitialDates();
            this.searchClicked();
        }

        searchClicked() {
            this.vacationReportsGrid.dataSource.read();
        }


        // Format for datepickers.
        dateOptions: kendo.ui.DatePickerOptions = {
            format: "yyyy",
            start: "decade",
            depth: "decade"
        };

    }
}
