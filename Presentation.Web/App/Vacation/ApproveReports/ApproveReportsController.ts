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
            "VacationBalanceResource"
        ];

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
            dataSource: 
            {
                type: "odata-v4",
                batch: true,
                transport: {
                    read: {
                        url: "/odata/VacationReports()?$expand=Person($select=FullName)",
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
                            id: {type: "number", from:"Id"},
                            title: {
                                parse: function (data) {
                                    return "";
                                }
                            },
                            start: {
                                type: "date",
                                from: "StartTimestamp",
                                parse: function (data) {
                                    return moment.unix(data).toDate();
                                }
                            },
                            end: {
                                type: "date",
                                from: "EndTimestamp",
                                parse: function (data) {
                                    return moment.unix(data).toDate();
                                }
                            },
                            personId: {from: "PersonId"},
                            description: { from: "Comment" },
                            status: {from: "Status"}
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
                        },
                        schema: {
                            data: function (data) {
                                return data.value;
                            },
                        },
                    }
                }
                
            ],
            culture: "'da-DK'",
            footer: false,
            header: false,  
            
        }

        private _maxEndDate: Date;
        private _currentUser;

        constructor(private $scope, private Person, private $rootScope, private VacationReport, private NotificationService, private VacationBalance: VacationBalanceResource) {

            this._currentUser = $scope.CurrentUser;
            
        }

     

    }

    angular.module("app.vacation").controller("ApproveReportsController", ApproveReportsController);
}