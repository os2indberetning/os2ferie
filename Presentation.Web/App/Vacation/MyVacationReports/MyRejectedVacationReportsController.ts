module app.vacation {
    "use strict";

    class MyRejectedVacationReportsController extends BaseMyVacationReportsController {

        static $inject: string[] = [
            "$scope",
            "$modal",
            "$rootScope",
            "VacationReport",
            "$timeout",
            "Person",
            "moment"
        ];

        constructor(
            protected $scope,
            protected $modal,
            protected $rootScope,
            protected VacationReport,
            protected $timeout,
            protected Person,
            protected moment) {

            super($modal, $rootScope, VacationReport, $timeout, Person, moment, "Rejected");

            this.vacationReportsOptions = {
                autoBind: false,
                dataSource: {
                    type: "odata-v4",
                    transport: {
                        read: {
                            beforeSend(req) {
                                req.setRequestHeader("Accept", "application/json;odata=fullmetadata");
                            },
                            url:
                            `/odata/VacationReports?status=${this.ReportStatus}&$expand=ResponsibleLeader &$filter=PersonId eq ${this.personId}`,
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
                            const m = this.moment.utc(data.StartTimestamp, 'X');
                            return m._d.getDate() +
                                "/" +
                                (m._d.getMonth() + 1) +
                                "/" + // +1 because getMonth is zero indexed.
                                m._d.getFullYear();
                        }
                    },
                    {
                        title: "Ferieafslutning",
                        template: data => {
                            const m = this.moment.utc(data.EndTimestamp, 'X');
                            return m._d.getDate() +
                                "/" +
                                (m._d.getMonth() + 1) +
                                "/" + // +1 because getMonth is zero indexed.
                                m._d.getFullYear();
                        }
                    },
                    {
                        field: "Comment",
                        template: data => {
                            if (data.Comment != "") {
                                return data.Purpose +
                                    "<button kendo-tooltip k-position=\"'right'\" k-content=\"'" +
                                    data.Comment +
                                    "'\" class=\"transparent-background pull-right no-border\"><i class=\"fa fa-comment-o\"></i></button>";
                            }
                            return "<i>Ingen kommantar angivet</i>";

                        },
                        title: "Kommentar"
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
                            var m = this.moment.utc(data.CreatedDateTimestamp, 'X');
                            return m._d.getDate() +
                                "/" +
                                (m._d.getMonth() + 1) +
                                "/" + // +1 because getMonth is zero indexed.
                                m._d.getFullYear();
                        },
                        title: "Indberettet"
                    },
                    {
                        field: "ClosedDateTimestamp",
                        title: "Afvist dato",
                        template: data => {
                            var m = moment.utc(data.ClosedDateTimestamp, 'X');
                            var date = m._d.getDate() + "/" +
                                (m._d.getMonth() + 1) + "/" + // +1 because getMonth is zero indexed.
                                m._d.getFullYear();
                            return date;

                        },
                    },
                    {
                        field: "ApprovedBy.FullName",
                        title: "Afvist af",
                        template: data => (data.ApprovedBy.FullName + "<div kendo-tooltip k-content=\"'" + data.Comment + "'\"><i class='fa fa-comment-o'></i></div>")
                    }
                ],
                scrollable: false
            };

        }

    }

    angular.module("app.vacation").controller("Vacation.MyRejectedVacationReportsController", MyRejectedVacationReportsController);
}
