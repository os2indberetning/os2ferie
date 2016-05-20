module app.vacation {
    "use strict";

    class ApproveVacationController {

        static $inject = [
            "$scope",
            "$rootScope",
            "VacationReport",
            "NotificationService",
            "$http",
            "$modal",
            "moment",
            "$state"
        ];

        constructor(private $scope, private $rootScope, private VacationReport, private NotificationService, private $http: ng.IHttpService, private $modal, private moment: moment.MomentStatic, private $state: ng.ui.IStateService) {
            this._currentUser = $scope.CurrentUser; //TODO: Remove?
            this.readPendingVacations();
            var self = this;

            this.schedulerOptions = {
                views: [
                    {
                        type: "timelineMonth",
                        title: "Måned",
                        minorTickCount: 1,
                        columnWidth: 25,
                        dateHeaderTemplate: "<span class='k-link k-nav-day' style=>#=kendo.toString(date, 'dd')#</span>",
                        //slotTemplate: "<div style='background:#=changeColorforWeekend(date)#; height: 100%;width: 100%; padding: 0;'></div>",
                        workWeekStart: 0,
                        workWeekEnd: 5
                    }
                ],
                dataBinding: function (e) {
                    var scheduler = this;

                    if (scheduler._selectedViewName != "kendo.ui.SchedulerTimelineYearView") return;

                    var ele = scheduler.wrapper.find(".k-scheduler-timelineMonthview > tbody:first-child > tr:first-child tbody >tr:nth-child(2)");
                    ele.hide();
                },
                dateHeaderTemplate: kendo.template("<strong>#=kendo.toString(date, 'd')#</strong>"),
                height: 600,
                timezone: "Etc/UTC",
                //This is kendo's save event. Label has been changed to 'Gem' in in "edit" event below.
                save: function (e) {

                    this.VacationReport.$approve({ id: e.event.id }, () => {
                        
                    })
                    
                    //this.$http.patch();
                    console.log("[save] e");
                    console.log(e);
                },
                editable: {
                    template: $("#customEditorTemplate").html()
                },
                edit: function (e: any) {
                    var container = e.container;
                    var personName = e.event.Person.FullName.split("[")[0];

                    container.find("[data-container-for=title]").append("<p class='k-edit-label modal-personName'>" + personName + "</p>");
                    container.find("[data-container-for=start]").append("<p class='k-edit-label'>" + moment(e.event.start).format("DD.MM.YYYY") + "</p>");
                    container.find("[data-container-for=end]").append("<p class='k-edit-label'>" + moment(e.event.end).format("DD.MM.YYYY") + "</p>");

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
                            url: `/odata/VacationReports()?$expand=Person($select=FullName)`,
                            dataType: "json",
                            cache: false
                        }
                    },
                    serverFiltering: true,
                    schema: {
                        data: data => data.value,
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
                                description: { from: "Comment" },
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
                                    url: "/odata/Person/Service.LeadersPeople",
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

        private _maxEndDate: Date;
        private _currentUser;

        scheduler: kendo.ui.Scheduler;
        schedulerOptions: kendo.ui.SchedulerOptions;

        pendingVacations = [];

        readPendingVacations() {
            this.$http.get("/odata/VacationReports()?status=Pending &$expand=Person($select=FullName)").then((response: ng.IHttpPromiseCallbackArg<any>) => {
                //Sort of objects for Pending Vacation Reports
                response.data.value.sort(function (a, b) { return (a.StartTimestamp > b.StartTimestamp) ? 1 : ((b.StartTimestamp > a.StartTimestamp) ? -1 : 0); });

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
    }
    angular.module("app.vacation").controller("Vacation.ApproveVacationController", ApproveVacationController);
}
