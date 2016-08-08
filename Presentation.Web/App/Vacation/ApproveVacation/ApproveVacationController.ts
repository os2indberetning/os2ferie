module app.vacation {
    "use strict";

    import NotificationService = core.interfaces.NotificationService;

    class ApproveVacationController {

        static $inject = [
            "$scope",
            "$rootScope",
            "VacationReport",
            "NotificationService",
            "$http",
            "moment",
            "$state"
        ];

        private _maxEndDate: Date;
        private _currentUser;

        scheduler: kendo.ui.Scheduler;
        schedulerOptions: kendo.ui.SchedulerOptions;

        pendingVacations = [];

        constructor(private $scope, private $rootScope, private VacationReport, private NotificationService: NotificationService, private $http: ng.IHttpService, private moment: moment.MomentStatic, private $state: ng.ui.IStateService) {
            this.readPendingVacations();

            // Why is this used?
            var self = this;

            this.schedulerOptions = {
                views: [
                    {
                        type: "timelineMonth",
                        title: "Måned",
                        minorTickCount: 1,
                        columnWidth: 25,
                        dateHeaderTemplate: (obj) => {
                            var date: Date = obj.date;
                            var momentDate = moment(date);
                            var day = date.getDate();
                            var week = momentDate.format('W');

                            var header = ``;

                            if (momentDate.weekday() === 0) header += `<span style="font-size:10px;">${week}</span>`;

                            return header + `<br>${day}`;
                        },
                        workWeekStart: 0,
                        workWeekEnd: 5
                    }
                ],
                timezone: "Etc/UTC",
                //This is kendo's save event. Label has been changed to 'Gem' in in "edit" event below.
                save: (e: any) => {
                    e.preventDefault();
                    var report = new this.VacationReport();

                    if (e.event.status == "Accepted") {
                        report.$approve({ id: e.event.id },
                        () => {
                            this.refresh();
                        });
                    } else if (e.event.status == "Rejected") {
                        report.$reject({ id: e.event.id },
                        () => {
                            this.refresh();
                        });
                    }
                },
                editable: {
                    template: $("#customEditorTemplate").html(),
                    update: true,
                    move: false,
                    destroy: false,
                    resize: false
                },
                edit: (e: any) => {
                    var container = e.container;
                    var personName = e.event.Person.FullName.split("[")[0];

                    var value = e.event;
                    var endDateTimestamp = e.event.end;

                    const startsOnFullDay = value.StartTime == null;
                    const endsOnFullDay = value.EndTime == null;

                    var startTimeFormat = null;

                    if (!startsOnFullDay) {
                        startTimeFormat = moment((moment.duration(value.StartTime) as any)._data).format('HH:mm');
                    }

                    var endTimeFormat = null;

                    if (!endsOnFullDay) {
                        endTimeFormat = moment((moment.duration(value.EndTime) as any)._data).format('HH:mm');
                    }

                    if (startsOnFullDay && endsOnFullDay) {
                        endDateTimestamp -= 86400;
                    }

                    container.find("[data-container-for=title]")
                        .append("<p class='k-edit-label modal-personName'>" + personName + "</p>");

                    container.find("[data-container-for=start]")
                        .append(`<p class='k-edit-label fill-width force-text-left'>
                            ${moment(e.event.start).format("DD.MM.YYYY")} ` +
                            (startTimeFormat != null ? ` kl. ${startTimeFormat}` : ``) +
                            `</p>`);

                    container.find("[data-container-for=end]")
                        .append(`<p class='k-edit-label fill-width force-text-left'>
                            ${moment(endDateTimestamp).format("DD.MM.YYYY")} ` +
                            (endTimeFormat != null ? ` kl. ${endTimeFormat}` : ``) +
                            `</p>`);

                    container.find("[data-container-for=purpose]")
                        .append("<p class='k-edit-label fill-width force-text-left'>" +
                            (e.event.description === "" ? "<i>Ingen angivet</i>" : e.event.description) +
                            "</p>");

                    //Setting up some final css.
                    $(".modal-personName").width("70%").css("text-align", "left");

                    //Removal of unused buttons, and change of names.
                    $(".k-scheduler-delete").remove();
                    $(".k-scheduler-update").html("Gem");
                    $(".k-scheduler-cancel").html("Afbryd");
                    $(".k-window-title").remove();
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
                                status: { from: "Status" }
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
                                color: "#6eb3fa"
                            },
                            {
                                text: "Accepted",
                                value: "Accepted",
                                color: "#19BF19"
                            },
                            {
                                text: "Rejected",
                                value: "Rejected",
                                color: "#CB0101"
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
                footer: false,
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
                        dateTo: dateTo
                };
                    this.pendingVacations.push(obj);
                });
            });
        };

        goToDate(time) {
            time = Number(time);
            this.scheduler.date(new Date(time));
        };

        private refresh() {
            this.readPendingVacations();
            this.scheduler.dataSource.read();
        }

    }

    angular.module("app.vacation").controller("ApproveVacationController", ApproveVacationController);
}
