module app.vacation {
    "use strict";

    import Person = core.models.Person;
    import NotificationService = app.core.interfaces.NotificationService;

    class MyRejectedVacationReportsController extends BaseMyVacationReportsController {

        static $inject: string[] = [
            "$scope",
            "$modal",
            "$rootScope",
            "VacationReport",
            "$timeout",
            "Person",
            "moment",
            "$state",
            "NotificationService"
        ];

        constructor(
            protected $scope,
            protected $modal: angular.ui.bootstrap.IModalService,
            protected $rootScope,
            protected VacationReport,
            protected $timeout: ng.ITimeoutService,
            protected Person: Person,
            protected moment: moment.MomentStatic,
            protected $state: ng.ui.IStateService,
            protected NotificationService: NotificationService) {

            super($scope, $modal, $rootScope, VacationReport, $timeout, Person, moment, $state, NotificationService);

            this.vacationReportsOptions = {
                autoBind: false,
                dataSource: {
                    type: "odata-v4",
                    transport: {
                        read: {
                            url: this.getVacationReportsUrl(),
                            dataType: "json",
                            cache: false
                        }
                    },
                    pageSize: 20,
                    serverPaging: true,
                    serverAggregates: false,
                    serverSorting: true,
                    serverFiltering: true,
                    sort: { field: "StartTimestamp", dir: "desc" }
                },
                pageable: {
                    messages: {
                        display: "{0} - {1} af {2} indberetninger",
                        //{0} is the index of the first record on the page, {1} - index of the last record on the page, {2} is the total amount of records
                        empty: "Ingen indberetninger at vise",
                        page: "Side",
                        of: "af {0}", //{0} is total amount of pages
                        itemsPerPage: "indberetninger pr. side",
                        first: "Gå til første side",
                        previous: "Gå til forrige side",
                        next: "Gå til næste side",
                        last: "Gå til sidste side",
                        refresh: "Genopfrisk"
                    },
                    pageSizes: [5, 10, 20, 30, 40, 50, 100, 150, 200]
                },
                dataBound: function () {
                    this.expandRow(this.tbody.find("tr.k-master-row").first());
                },
                columns: [
                    {
                        title: "Feriestart",
                        template: data => {
                            const m = this.moment.unix(data.StartTimestamp);
                            return m.format("L");
                        }
                    },
                    {
                        title: "Ferieafslutning",
                        template: data => {
                            const m = this.moment.unix(data.EndTimestamp);
                            return m.format("L");
                        }
                    },
                    {
                        template: data => {
                            if (data.Purpose != "") {
                                return `<button kendo-tooltip k-position="'right'" k-content="'${data.Purpose
                                    }'" class="transparent-background pull-right no-border"><i class="fa fa-comment-o"></i></button>`;
                            }
                            return "<i>Ingen bemærkning angivet</i>";
                        },
                        title: "Bemærkning"
                    },
                    {
                        title: "Ferietype",
                        template: data => {
                            return data.VacationType === "Regular" ?
                                "Almindelig ferie" : "6. ferieuge";
                        }
                    },
                    {
                        field: "CreatedDateTimestamp",
                        template: data => {
                            const m = this.moment.unix(data.CreatedDateTimestamp);
                            return m.format("L");
                        },
                        title: "Indberettet"
                    },
                    {
                        field: "Comment",
                        title: "Begrundelse",
                        template: data => {
                            if (data.Comment != "") {
                                return `<button kendo-tooltip k-position="'right'" k-content="'${data.Comment
                                    }'" class="transparent-background pull-right no-border"><i class="fa fa-comment-o"></i></button>`;
                            }
                            return "<i>Ingen begrundelse angivet</i>";
                        }
                    },
                    {
                        field: "ClosedDateTimestamp",
                        title: "Afvist dato",
                        template: data => {
                            const m = moment.unix(data.ClosedDateTimestamp);
                            return m.format("L");
                        }
                    },
                    {
                        field: "ApprovedBy.FullName",
                        title: "Afvist af",
                        template: data => (data.ApprovedBy.FullName)
                    }
                ],
                scrollable: false
            };
        }

        getVacationReportsUrl() {
            return `/odata/VacationReports?status=Rejected&$expand=ApprovedBy($select=FullName) &$filter=PersonId eq ${this.personId} and VacationYear eq ${
                this.vacationYear}`;
        }
    }

    angular.module("app.vacation").controller("MyRejectedVacationReportsController", MyRejectedVacationReportsController);
}
