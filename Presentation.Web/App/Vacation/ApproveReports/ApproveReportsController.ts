module app.vacation {
    "use strict";

    import VacationBalanceResource = app.vacation.resources.IVacationBalanceResource;
    import IVacationBalance = app.vacation.resources.IVacationBalance;

    class ApproveReportsController {

        static $inject = [
            "$scope",
            "Person",
            "$rootScope",
            "VacationReport",
            "NotificationService",
            "VacationBalanceResource",
            "$http",
        ];

        constructor(private $scope, private Person, private $rootScope, private VacationReport, private NotificationService, private VacationBalance: VacationBalanceResource, private $http: ng.IHttpService, private $modal) {

            this._currentUser = $scope.CurrentUser;
            this.toggleBold(this.startupDate.getMonth());
            this.readPendingVacations();
        }

        private _maxEndDate: Date;
        private _currentUser;

        months = {
            0: 'Jan',
            1: 'Feb',
            2: 'Mar',
            3: 'Apr',
            4: 'May',
            5: 'Jun',
            6: 'Jul',
            7: 'Aug',
            8: 'Sep',
            9: 'Oct',
            10: 'Nov',
            11: 'Dec'
        };

        scheduler;
        startupDate = new Date();

        //Used to sort vacation reports
        startMonth = new Date(this.startupDate.getFullYear(), this.startupDate.getMonth(), 1);
        startNextMonth = new Date(this.startupDate.getFullYear(), this.startupDate.getMonth() + 1, 1);

        currentYear = this.startupDate.getFullYear();
        prevYear = this.currentYear - 1;
        nextYear = this.currentYear + 1;
        toggleMonth = '#' + this.months[this.startupDate.getMonth()].toString();
        pendingVacations = [];

        setDate(month, year) {
            if (year != undefined) {
                var yearDiff = 0;
                var date = this.scheduler.date();

                if (year < this.currentYear) {
                    yearDiff = this.currentYear - year;
                    this.currentYear = (this.currentYear - yearDiff);
                    this.prevYear = (this.prevYear - yearDiff);
                    this.nextYear = (this.nextYear - yearDiff);

                    this.scheduler.date(new Date(this.currentYear, date.getMonth(), date.getDate()));

                    this.toggleBold(date.getMonth());

                    this.startMonth.setFullYear((this.startMonth.getFullYear() - yearDiff));
                    this.startNextMonth.setFullYear((this.startNextMonth.getFullYear() - yearDiff));
                }
                else if (year > this.currentYear) {
                    yearDiff = year - this.currentYear;
                    this.currentYear = (this.currentYear + yearDiff);
                    this.prevYear = (this.prevYear + yearDiff);
                    this.nextYear = (this.nextYear + yearDiff);

                    this.scheduler.date(new Date(this.currentYear, date.getMonth(), date.getDate()));

                    this.toggleBold(date.getMonth());

                    this.startMonth.setFullYear(this.startMonth.getFullYear() + yearDiff);
                    this.startNextMonth.setFullYear(this.startNextMonth.getFullYear() + yearDiff);
                }
            }

            if (month != undefined) {
                this.scheduler.date(new Date(this.currentYear, month, 1));
                this.toggleBold(month);
            
                //Change month of the sorting
                if (month > 0) {
                    this.startMonth.setMonth((month - 1));
                }
                else {
                    this.startMonth.setFullYear((this.startMonth.getFullYear() - 1));
                    this.startMonth.setMonth(11);
                }

                if (month < 11) {
                    this.startNextMonth.setMonth((month + 1));
                } else {
                    this.startNextMonth.setFullYear((this.startNextMonth.getFullYear() + 1));
                    this.startNextMonth.setMonth(0);
                }
            }
            this.scheduler.dataSource.transport.options.read.url = "/odata/VacationReports()?$expand=Person($select=FullName) &$filter=StartTimestamp ge " + this.startMonth.getTime().toString().substring(0, 10) + " and StartTimestamp le " + this.startNextMonth.getTime().toString().substring(0, 10);
            this.scheduler.dataSource.read();
        }

        setMonth(month) {
            this.setDate(month, undefined);
        };

        setYear(year) {
            this.setDate(undefined, year);
        };

        goToDate(time) {
            time = Number(time);
            var date = new Date(time);

            this.setDate(date.getMonth(), date.getFullYear());
        };

        toggleBold(month) {
            $(this.toggleMonth).css("font-weight", "normal");
            this.toggleMonth = '#' + this.months[month].toString();
            $(this.toggleMonth).css("font-weight", "bold");
        }

        readPendingVacations() {
            this.$http.get("/odata/VacationReports()?status=Pending").then((response: ng.IHttpPromiseCallbackArg<any>) => {
                //Sort of objects for Pending Vacation Reports
                response.data.value.sort(function (a, b) { return (a.StartTimestamp > b.StartTimestamp) ? 1 : ((b.StartTimestamp > a.StartTimestamp) ? -1 : 0); });

                angular.forEach(response.data.value, (value, key) => {
                    //There has to be added "000" since the output from DB is without these.
                    var startTime = value.StartTimestamp.toString() + "000";
                    var endTime = value.EndTimestamp.toString() + "000";

                    var startDate = new Date(Number(startTime));
                    var endDate = new Date(Number(endTime));

                    var date = startDate.getDate() + "." + (startDate.getMonth() + 1) + "." + startDate.getFullYear() + " - " + endDate.getDate() + "." + (endDate.getMonth() + 1) + "." + endDate.getFullYear();

                    var obj = {
                        key: key,
                        startTime: startTime,
                        date: date
                    };
                    this.pendingVacations.push(obj);
                });
            });
        };

        SchdulerOptions = {
            views: [
                {
                    type: "kendo.ui.SchedulerTimelineYearView",
                    title: "Timeline",
                    majorTick: 1440,
                    minorTickCount: 1,
                    columnWidth: 25,
                    dateHeaderTemplate: "<span class='k-link k-nav-day' style=>#=kendo.toString(date, 'dd')#</span>"
                },
                "month"
            ],
            editable: false,
            dataBinding: function (e) {
                var view = this.view();

                //view.times.hide();
                //view.timesHeader.hide();
                
                var scheduler = this;

                scheduler.toolbar.remove();
                if (scheduler._selectedViewName != "kendo.ui.SchedulerTimelineYearView") return;

                var ele = scheduler.wrapper.find(".k-scheduler-layout:nth-child(1) > tbody:first-child > tr:first-child tbody >tr:nth-child(2)");
                ele.hide();
            },
            dataHeaderTemplate: kendo.template("<strong>#=kendo.toString(date, 'd')#</strong>"),
            height: 600,
            timezone: "Etc/UTC",
            selectable: true,
            change: function (e) {
                if (e.events != 0) {
                    
                    //var modalInstance = $modal.open({
                    //    templateUrl: 'ApproveReportsModalView.html',
                    //    controller: 'Vacation.ApproveReportsController',
                    //    size: 'sm',
                    //});

                    console.log("e:");
                    console.log(e);
                    //createModal;
                }
            },
            dataSource:
            {
                autoBind: false,
                type: "odata-v4",
                batch: true,
                transport: {
                    read: {
                        url: "/odata/VacationReports()?$expand=Person($select=FullName) &$filter=StartTimestamp ge " + this.startMonth.getTime().toString().substring(0, 10) + " and StartTimestamp le " + this.startNextMonth.getTime().toString().substring(0, 10),
                        dataType: "json",
                        cache: false
                    }
                },
                serverFiltering: true,
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
                                parse: data => moment.unix(data).toDate()
                            },
                            end: {
                                type: "date",
                                from: "EndTimestamp",
                                parse: data => moment.unix(data).toDate()
                            },
                            personId: { from: "PersonId" },
                            description: { from: "Comment" },
                            status: { from: "Status" },
                        }
                    }
                },
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
            culture: "'da-DK'",
            footer: false,
            header: false,

        }
    }
    angular.module("app.vacation").controller("Vacation.ApproveReportsController", ApproveReportsController);
}