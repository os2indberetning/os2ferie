module app.vacation {
    "use strict";

    import NotificationService = core.interfaces.NotificationService;

    class ApproveVacationPendingController {

        static $inject = [
            "$scope",
            "$rootScope",
            "VacationReport",
            "NotificationService",
            "$http",
            "moment",
            "$state",
            "$modal"
        ];

        private _maxEndDate: Date;
        private _currentUser;

        scheduler: kendo.ui.Scheduler;
        schedulerOptions: kendo.ui.SchedulerOptions;

        pendingVacations = [];

        constructor(
            private $scope,
            private $rootScope,
            private VacationReport,
            private NotificationService: NotificationService,
            private $http: ng.IHttpService,
            private moment: moment.MomentStatic,
            private $state: ng.ui.IStateService,
            private $modal) {

            this.readPendingVacations();
            
            // Why is this used?
            var self = this;

            this.schedulerOptions = {
                workDayStart: new Date("2010-01-01 00:00:00"),
                workDayEnd: new Date("2010-01-01 23:59:59"),
                views: [
                    {
                        type: "timelineMonth",
                        title: "Måned",
                        minorTickCount: 1,
                        columnWidth: 25,
                        dateHeaderTemplate: (obj) => {
                            var date = moment(obj.date);
                            var day = date.date();
                            var week = date.format('W');
                            var header = ``;
                            if (date.weekday() === 0) header += `<span style="font-size:10px;">${week}</span>`;
                            return header + `<br>${day}`;
                        },
                        workWeekStart: 0,
                        workWeekEnd: 5
                    },
                    {
                        type: "timelineWeek",
                        title: "Uge",
                        minorTickCount: 6,
                        columnWidth: 40,
                        majorTick: 1440
                    }
                ],
                timezone: "Etc/UTC",
                //This is kendo's save event. Label has been changed to 'Gem' in in "edit" event below.
                save: (e: any) => {
                    e.preventDefault();
                },
                eventTemplate: kendo.template(`<div class="schedule-vacation-template vacation-#= type.toLowerCase()#"> # if (type == 'Care') { # O # } if (type == 'SixthVacationWeek') { # 6 # } if (type == 'Regular') { #  # } if (type == 'Senior') { # S # } if (type == 'Optional') { # V # } # </div>`),
                editable: {
                    update: true,
                    move: false,
                    destroy: false,
                    resize: false,
                    confirmation: false
                },
                edit: (e: any) => {
                    e.preventDefault();
                    $modal.open({
                        templateUrl: '/App/Vacation/ApproveVacation/ShowVacationReportView.html',
                        controller: 'ShowVacationReportController as svrc',
                        backdrop: "static",
                        resolve: {
                            report() {
                                return e.event;
                            }
                        }
                    }).result.then(() => {
                        this.refresh();
                    });
                },
                dataSource: {
                    autoBind: false,
                    type: "odata-v4",
                    batch: true,
                    transport: {
                        read: {
                            url:
                                `/odata/VacationReports()?$expand=Person($select=FullName)&$filter=ResponsibleLeaderId eq ${this.$rootScope.CurrentUser.Id}`,
                            dataType: "json",
                            cache: false
                        }
                    },
                    serverFiltering: true,
                    schema: {
                        data: data => {
                            var events = [];

                            angular.forEach(data.value,
                                (value, key) => {

                                    const startsOnFullDay = value.StartTime == null;
                                    const endsOnFullDay = value.EndTime == null;

                                    if (!startsOnFullDay) {
                                        const duration = this.moment.duration(value.StartTime);
                                        value.StartTimestamp += duration.asSeconds();
                                    }

                                    if (!endsOnFullDay) {
                                        const duration = this.moment.duration(value.EndTime);
                                        value.EndTimestamp += duration.asSeconds();
                                    } else {
                                        // Add 86400/24 hours to enddate
                                        value.EndTimestamp += 86400;
                                        //value.isAllDay = true;
                                    }

                                    switch (value.VacationType) {
                                        case "Care":
                                            value.type = "Omsorgsdage";
                                            break;
                                        case "Optional":
                                            value.type = "Valgfri ferie";
                                            break;
                                        case "Regular":
                                            value.type = "Almindelig Ferie";
                                            break;
                                        case "Senior":
                                            value.type = "Seniordage";
                                            break;
                                        case "SixthVacationWeek":
                                            value.type = "6. ferieuge";
                                            break;
                                        default:
                                            value.type = "Andet";
                                            break;
                                    }
                                    
                                    events.push(value);
                                });

                            return events;
                        },
                        model: {
                            fields: {
                                id: { type: "number", from: "Id" },
                                title: {
                                    parse: data => ""
                                },
                                start: {
                                    type: "date",
                                    from: "StartTimestamp",
                                    parse: data => self.moment.unix(data).toDate()
                                },
                                end: {
                                    type: "date",
                                    from: "EndTimestamp",
                                    parse: data => self.moment.unix(data).toDate()
                                },
                                personId: { from: "PersonId" },
                                description: { from: "Purpose" },
                                status: { from: "Status" },
                                type: { from: "VacationType"}
                            }
                        }
                    }
                },
                group: {
                    resources: ["People"],
                    orientation: "vertical"
                },
                resources: [
                    {
                        field: "status",
                        dataSource: [
                                {
                                text: "Pending",
                                value: "Pending",
                                color: "#f1eb47"
                            },
                            {
                                text: "Accepted",
                                value: "Accepted",
                                color: "#5cb85c"
                            },
                            {
                                text: "Rejected",
                                value: "Rejected",
                                color: "#d9534f"
                            }
                        ]
                    },

                    {
                        field: "personId",
                        name: "People",
                        dataValueField: "Id",
                        dataTextField: "FullName",
                        dataSource:
                        {
                            type: "odata-v4",
                            transport: {
                                read: {
                                    url: "/odata/Person/Service.LeadersPeople(Type=1)",
                                    dataType: "json",
                                    cache: false
                                }
                            }
                        }
                    }

                ],
                footer: false
            }
        }

        readPendingVacations() {
            // TODO Change this to use Resource instead
            this.$http.get(`/odata/VacationReports()?status=Pending &$expand=Person($select=FullName)&$filter=ResponsibleLeaderId eq ${this.$rootScope.CurrentUser.Id}`).then((response: ng.IHttpPromiseCallbackArg<any>) => {

                //Sort of objects for Pending Vacation Reports
                response.data.value.sort((a, b) => ((a.StartTimestamp > b.StartTimestamp) ? 1 : ((b.StartTimestamp > a.StartTimestamp) ? -1 : 0)));

                this.pendingVacations = [];

                angular.forEach(response.data.value, (value, key) => {
                    var startTime = Number(value.StartTimestamp.toString() + "000");
                    var endTime = Number(value.EndTimestamp.toString() + "000");

                    var dateFrom = this.moment(startTime).format("DD.MM.YYYY");
                    var dateTo = this.moment(endTime).format("DD.MM.YYYY");

                    var obj = {
                        key: key,
                        startTime: startTime,
                        firstName: value.Person.FullName.split("[")[0],
                        dateFrom: dateFrom,
                        dateTo: dateTo,
                        reportdata: value
                };
                    this.pendingVacations.push(obj);
                });
            });
        };

        goToDate(data) {
            var time = Number(data.startTime);
            this.scheduler.date(new Date(time));

            this.$modal.open({
                templateUrl: '/App/Vacation/ApproveVacation/ShowVacationReportView.html',
                controller: 'ShowVacationReportController as svrc',
                backdrop: "static",
                resolve: {
                    report() {
                        return data.reportdata;
                    }
                }
            }).result.then(() => {
                this.refresh();
            });
        };

        private refresh() {
            this.readPendingVacations();
            this.scheduler.dataSource.read();
        }
    }

    angular.module("app.vacation").controller("ApproveVacationPendingController", ApproveVacationPendingController);
}
