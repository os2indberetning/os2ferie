module app.vacation {
    "use strict";

    export abstract class BaseMyVacationReportsController {

        vacationReportsGrid: kendo.ui.Grid;
        vacationReportsOptions: kendo.ui.GridOptions;
        fromDateFilter;
        fromDate: Date;
        toDate: Date;
        toDateFilter;
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


            // dates for kendo filter.
            this.fromDateFilter = new Date();
            this.fromDateFilter.setFullYear(this.fromDateFilter.getFullYear() - 2);
            this.fromDateFilter = this.getStartOfDayStamp(this.fromDateFilter);
            this.toDateFilter = this.getEndOfDayStamp(new Date());

            this.loadInitialDates();
        }

        getEndOfDayStamp(d) {
            var m = this.moment(d);
            return m.endOf('day').unix();
        }

        getStartOfDayStamp(d) {
            var m = this.moment(d);
            return m.startOf('day').unix();
        }

        refreshGrid() {
            /// <summary>
            /// Refreshes kendo grid datasource.
            /// </summary>
            this.vacationReportsGrid.refresh();
        }

        getDataUrl(from, to) {
            const url = `/odata/VacationReports?status=${this.ReportStatus} &$expand=ResponsibleLeader`;
            var filters = "&$filter=PersonId eq " +
                this.personId +
                " and CreatedDateTimestamp ge " +
                from +
                " and CreatedDateTimestamp le " +
                to;
            var result = url + filters;
            return result;
        }

        loadInitialDates() {
            /// <summary>
            /// Sets initial date filters.
            /// </summary>
            // Set initial values for kendo datepickers.
            var from = new Date();
            from.setFullYear(from.getFullYear() - 2);

            this.toDate = new Date();
            this.toDate.setFullYear(this.toDate.getFullYear() + 2);
            this.fromDate = from;
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

            modalInstance.result.then(res => {
                this.vacationReportsGrid.refresh();
            });
        }

        clearClicked() {
            this.loadInitialDates();
            this.searchClicked();
        }

        searchClicked() {
            var from = this.getStartOfDayStamp(this.fromDate);
            var to = this.getEndOfDayStamp(this.toDate);
            this.vacationReportsOptions.dataSource.transport.read.url = this.getDataUrl(from, to);
            this.vacationReportsGrid.dataSource.read();
        }


        // Format for datepickers.
        dateOptions = {
            format: "dd/MM/yyyy"
        };

    }
}
