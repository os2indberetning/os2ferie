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
                        title: "Month",
                        majorTick: 1440,
                        minorTickCount: 1,
                        columnWidth: 25,
                        dateHeaderTemplate: "<span class='k-link k-nav-day' style=>#=kendo.toString(date, 'dd')#</span>"
                    },
                ],
                editable: false,
                dataBinding: function (e) {
                    var scheduler = this;

                    //scheduler.toolbar.remove();
                    if (scheduler._selectedViewName != "kendo.ui.SchedulerTimelineYearView") return;

                    var ele = scheduler.wrapper.find(".k-scheduler-timelineMonthview > tbody:first-child > tr:first-child tbody >tr:nth-child(2)");
                    ele.hide();
                },
                dateHeaderTemplate: kendo.template("<strong>#=kendo.toString(date, 'd')#</strong>"),
                height: 600,
                timezone: "Etc/UTC",
                selectable: true,
                change: function (e) {
                    if (e.events != 0) {

                        // TODO: Insert data that is needed for modal
                        var obj = {
                            
                        }

                        self.$state.go("vacation.approvereports.responsevacation");
                        
                        var modalInstance = self.$modal.open({
                            templateUrl: 'VacationInfoModal.html',
                            controller: 'Vacation.VacationResponseController',
                            size: 'sm'
                        });

                        console.log("e:");
                        console.log(e);
                    }
                },
                dataSource: {
                    autoBind: false,
                    type: "odata-v4",
                    batch: true,
                    transport: {
                        read: {
                            url: `/odata/VacationReports()?$expand=Person($select=FullName) &$filter=StartTimestamp ge ${this.startMonth.getTime().toString().substring(0, 10)} and StartTimestamp le ${this.startNextMonth.getTime().toString().substring(0, 10)}`,
                            dataType: "json",
                            cache: false
                        }
                    },
                    schema: {
                        data: function (data) {
                            return data.value;
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
        startupDate = new Date();

        //Used to sort vacation reports
        startMonth = new Date(this.startupDate.getFullYear(), this.startupDate.getMonth(), 1);
        startNextMonth = new Date(this.startupDate.getFullYear(), this.startupDate.getMonth() + 1, 1);

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
    }
    angular.module("app.vacation").controller("Vacation.ApproveVacationController", ApproveVacationController);
}
