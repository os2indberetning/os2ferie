module app.core.controllers {
    "use strict";

    import ReportType = app.core.models.ReportType;

    export abstract class BaseSubstitutesController {

        private currentUser;
        private fromDate: number;
        private toDate: number;
        private initialLoad: number;

        people;
        orgUnits;
        substitutes;
        personalApprovers;
        substituteGrid: kendo.ui.Grid;
        approverGrid: kendo.ui.Grid;
        approverToDate: Date;
        approverFromDate: Date;
        substituteToDate: Date;
        substituteFromDate: Date;
        subInfinitePeriod;
        appInfinitePeriod;

        constructor(protected $scope, protected Person, protected $rootScope, protected HelpText, protected Autocomplete, protected $modal, protected $timeout, protected moment, private reportType: ReportType) {
            this.currentUser = $scope.CurrentUser;

            var date = new Date();
            date.setMonth(date.getMonth() - 1);
            this.fromDate = this.startOfDay(date);
            this.toDate = this.endOfDay(new Date());

            this.people = Autocomplete.activeUsers();
            this.orgUnits = Autocomplete.orgUnits();

            this.substitutes = {
                dataSource: {
                    pageSize: 20,
                    type: "odata-v4",
                    transport: {
                        read: {
                            beforeSend(req) {
                                req.setRequestHeader('Accept', 'application/json;odata=fullmetadata');
                            },
                            url: "odata/Substitutes/Service.Substitute(Type=" + this.reportType + ")?$expand=OrgUnit,Sub,Person,Leader,CreatedBy",
                            dataType: "json",
                            cache: false
                        }
                    }
                },
                serverPaging: true,
                serverAggregates: false,
                serverSorting: true,
                serverFiltering: true,
                sortable: true,
                scrollable: false,
                filter: [
                    { field: "StartDateTimestamp", operator: "lte", value: this.toDate },
                    { field: "EndDateTimestamp", operator: "gte", value: this.fromDate }
                ],
                pageable: {
                    messages: {
                        display: "{0} - {1} af {2} stedfortrædere", //{0} is the index of the first record on the page, {1} - index of the last record on the page, {2} is the total amount of records
                        empty: "Ingen stedfortrædere at vise",
                        page: "Side",
                        of: "af {0}", //{0} is total amount of pages
                        itemsPerPage: "stedfortrædere pr. side",
                        first: "Gå til første side",
                        previous: "Gå til forrige side",
                        next: "Gå til næste side",
                        last: "Gå til sidste side",
                        refresh: "Genopfrisk"
                    },
                    pageSizes: [5, 10, 20, 30, 40, 50, 100, 150, 200]
                },
                dataBound() {
                    this.expandRow(this.tbody.find("tr.k-master-row").first());
                },
                columns: [
                    {
                        field: "Sub.FullName",
                        title: "Stedfortræder"
                    },
                    {
                        field: "Person.FullName",
                        title: "Stedfortræder for"
                    },
                    {
                        field: "OrgUnit.LongDescription",
                        title: "Organisationsenhed"
                    },
                    {
                        field: "Leader.FullName",
                        title: "Opsat af",
                        template(data) {
                            if (data.CreatedBy == undefined) return "<i>Ikke tilgængelig</i>";
                            return data.CreatedBy.FullName;
                        }
                    },
                    {
                        field: "StartDateTimestamp",
                        title: "Fra",
                        template: (data) => {
                            var m = this.moment.unix(data.StartDateTimestamp).toDate();
                            return m.getDate() + "/" +
                                (m.getMonth() + 1) + "/" + // +1 because getMonth is zero indexed.
                                m.getFullYear();
                        }
                    },
                    {
                        field: "EndDateTimestamp",
                        title: "Til",
                        template: (data) => {
                            if (data.EndDateTimestamp == 9999999999) {
                                return "På ubestemt tid";
                            }
                            var m = this.moment.unix(data.EndDateTimestamp).toDate();
                            return m.getDate() + "/" +
                                (m.getMonth() + 1) + "/" + // +1 because getMonth is zero indexed.
                                m.getFullYear();
                        }
                    },
                    {
                        title: "Muligheder",
                        template: (data) => `<a ng-click='subCtrl.openEditSubstitute(${data.Id})'>Rediger</a> | <a ng-click='subCtrl.openDeleteSubstitute(${data.Id})'>Slet</a>`
                    }
                ]
            };


            this.personalApprovers = {
                dataSource: {
                    pageSize: 20,
                    type: "odata-v4",
                    transport: {
                        read: {
                            beforeSend(req) {
                                req.setRequestHeader("Accept", "application/json;odata=fullmetadata");
                            },
                            url: "odata/Substitutes/Service.Personal(Type=" + this.reportType + ")?$expand=OrgUnit,Sub,Leader,Person,CreatedBy",
                            dataType: "json",
                            cache: false
                        }
                    }
                },
                serverPaging: true,
                serverAggregates: false,
                serverSorting: true,
                serverFiltering: true,
                sortable: true,
                scrollable: false,
                pageable: {
                    messages: {
                        display: "{0} - {1} af {2} personlige godkendere", //{0} is the index of the first record on the page, {1} - index of the last record on the page, {2} is the total amount of records
                        empty: "Ingen personlige godkendere at vise",
                        page: "Side",
                        of: "af {0}", //{0} is total amount of pages
                        itemsPerPage: "personlige godkendere pr. side",
                        first: "Gå til første side",
                        previous: "Gå til forrige side",
                        next: "Gå til næste side",
                        last: "Gå til sidste side",
                        refresh: "Genopfrisk"
                    },
                    pageSizes: [5, 10, 20, 30, 40, 50, 100, 150, 200]
                },
                dataBound() {
                    this.expandRow(this.tbody.find("tr.k-master-row").first());
                },
                columns: [
                    {
                        field: "Sub.FullName",
                        title: "Godkender"
                    },
                    {
                        field: "Person.FullName",
                        title: "Godkender for"
                    },
                    {
                        field: "CreatedBy",
                        title: "Opsat af",
                        template: (data) => {
                            if (data.CreatedBy == undefined) return "<i>Ikke tilgængelig</i>";
                            return data.CreatedBy.FullName;
                        }
                    },
                    {
                        field: "StartDateTimestamp",
                        title: "Fra",
                        template: (data) => {
                            var m = this.moment.unix(data.StartDateTimestamp).toDate();
                            return m.getDate() + "/" +
                                (m.getMonth() + 1) + "/" + // +1 because getMonth is zero indexed.
                                m.getFullYear();
                        }
                    },
                    {
                        field: "EndDateTimestamp",
                        title: "Til",
                        template: (data) => {
                            if (data.EndDateTimestamp == 9999999999) {
                                return "På ubestemt tid";
                            }
                            var m = this.moment.unix(data.EndDateTimestamp).toDate();
                            return m.getDate() + "/" +
                                (m.getMonth() + 1) + "/" + // +1 because getMonth is zero indexed.
                                m.getFullYear();
                        }
                    },
                    {
                        title: "Muligheder",
                        template: (data) => `<a ng-click='subCtrl.openEditApprover(${data.Id})'>Rediger</a> | <a ng-click='subCtrl.openDeleteApprover(${data.Id})'>Slet</a>`
                    }
                ]
            }

            this.loadInitialDates();
        }

        clearClicked(type) {
            var from = new Date();
            from.setMonth(from.getMonth() - 1);

            if (type === "substitute") {
                this.subInfinitePeriod = false;
                this.substituteToDate = new Date();
                this.substituteFromDate = from;
                this.substituteGrid.dataSource.filter([]);
            }
            else if (type === "approver") {
                this.appInfinitePeriod = false;
                this.approverToDate = new Date();
                this.approverFromDate = from;
                this.approverGrid.dataSource.filter([]);
            }
        }

        dateChanged(type) {
            this.$timeout(() => {
                if (type === "substitute") {
                    var subFrom = this.startOfDay(this.substituteFromDate);
                    var subTo = this.endOfDay(this.substituteToDate);

                    // Initial load is also a bit of a hack.
                    // dateChanged is called twice when the default values for the datepickers are set.
                    // This leads to sorting the grid content on load, which is not what we want.
                    // Therefore the sorting is not done the first 2 times the dates change - Which are the 2 times we set the default values.
                    if (this.initialLoad <= 0) {
                        this.applyDateFilter(subFrom, subTo, "substitute");
                    }
                } else if (type === "approver") {
                    var from = this.startOfDay(this.approverFromDate);
                    var to = this.endOfDay(this.approverToDate);

                    // Initial load is also a bit of a hack.
                    // dateChanged is called twice when the default values for the datepickers are set.
                    // This leads to sorting the grid content on load, which is not what we want.
                    // Therefore the sorting is not done the first 2 times the dates change - Which are the 2 times we set the default values.
                    if (this.initialLoad <= 0) {
                        this.applyDateFilter(from, to, "approver");
                    }
                }

                this.initialLoad--;
            }, 0);
        }

        applyDateFilter(fromDateStamp, toDateStamp, type) {
            if (type === "substitute") {
                this.substituteGrid.dataSource.filter([]);
                var subFilters = [];
                subFilters.push({ field: "StartDateTimestamp", operator: "lte", value: toDateStamp });

                if (!this.subInfinitePeriod) {
                    subFilters.push({ field: "EndDateTimestamp", operator: "gte", value: fromDateStamp });
                }
                this.substituteGrid.dataSource.filter(subFilters);
            } else if (type === "approver") {
                this.approverGrid.dataSource.filter([]);
                var appFilters = [];
                appFilters.push({ field: "StartDateTimestamp", operator: "lte", value: toDateStamp });

                if (!this.appInfinitePeriod) {
                    appFilters.push({ field: "EndDateTimestamp", operator: "gte", value: fromDateStamp });
                }
                this.approverGrid.dataSource.filter(appFilters);
            }
        }

        dateOptions = {
            format: "dd/MM/yyyy"
        }

        refreshGrids() {
            this.substituteGrid.dataSource.read();
            this.approverGrid.dataSource.read();
        }

        loadInitialDates() {
            this.initialLoad = 4;

            var from = new Date();

            from.setMonth(from.getMonth() - 1);
            this.approverToDate = new Date();
            this.approverFromDate = from;

            this.substituteToDate = new Date();
            this.substituteFromDate = from;
        }

        openEditSubstitute(id) {
            var modalInstance = this.$modal.open({
                templateUrl: 'App/Core/Views/Modals/EditSubstituteModal.html',
                controller: 'EditSubstituteModalInstanceController',
                backdrop: 'static',
                size: 'lg',
                resolve: {
                    persons: () => this.people,
                    orgUnits: () => this.orgUnits,
                    leader: () => this.currentUser,
                    substituteId: () => id,
                    ReportType: () => this.reportType
                }
            });

            modalInstance.result.then(() => {
                this.refreshGrids();
            });
        }

        openEditApprover(id) {
            this.$modal.open({
                templateUrl: "App/Core/Views/Modals/EditApproverModal.html",
                controller: "EditApproverModalInstanceController",
                controllerAs: "ctrl",
                backdrop: "static",
                size: "lg",
                resolve: {
                    persons: () => this.people,
                    orgUnits: () => this.orgUnits,
                    leader: () => this.currentUser,
                    substituteId: () => id,
                    ReportType: () => this.reportType
                }
            }).result.then(() => {
                this.approverGrid.dataSource.read();
            });
        }

        createNewApprover() {
            var modalInstance = this.$modal.open({
                // Change these
                templateUrl: 'App/Core/Views/Modals/NewApproverModal.html',
                controller: 'NewApproverModalInstanceController',
                backdrop: 'static',
                size: 'lg',
                resolve: {
                    persons: () => this.people,
                    orgUnits: () => this.orgUnits,
                    leader: () => this.currentUser,
                    ReportType: () => this.reportType
                }
            });

            modalInstance.result.then(() => {
                this.refreshGrids();
            });
        }

        createNewSubstitute() {
            var modalInstance = this.$modal.open({
                // Change these
                templateUrl: 'App/Core/Views/Modals/AdminNewSubstituteModal.html',
                controller: 'AdminNewSubstituteModalInstanceController',
                backdrop: 'static',
                size: 'lg',
                resolve: {
                    persons: () => this.people,
                    orgUnits: () => this.orgUnits,
                    leader: () => this.currentUser,
                    ReportType: () => this.reportType
                }
            });

            modalInstance.result.then(() => {
                this.refreshGrids();
            }, () => {

            });
        }

        openDeleteApprover(id) {
            var modalInstance = this.$modal.open({
                // Change these
                templateUrl: 'App/Core/Views/Modals/ConfirmDeleteApproverModal.html',
                controller: 'ConfirmDeleteApproverModalInstanceController',
                backdrop: 'static',
                size: 'lg',
                resolve: {
                    persons: () => this.people,
                    orgUnits: () => this.orgUnits,
                    leader: () => this.currentUser,
                    substituteId: () => id,
                    ReportType: () => this.reportType
                }
            });

            modalInstance.result.then(() => {
                this.refreshGrids();
            });
        }

        openDeleteSubstitute(id) {
            var modalInstance = this.$modal.open({
                // Change these
                templateUrl: 'App/Core/Views/Modals/ConfirmDeleteSubstituteModal.html',
                controller: 'ConfirmDeleteSubstituteModalInstanceController',
                backdrop: 'static',
                size: 'lg',
                resolve: {
                    persons: () => this.people,
                    orgUnits: () => this.orgUnits,
                    leader: () => this.currentUser,
                    substituteId: () => id,
                    ReportType: () => this.reportType
                }
            });

            modalInstance.result.then(() => {
                this.refreshGrids();
            });
        }

        private startOfDay(d: Date): number {
            return moment(d).startOf("day").unix();
        }

        private endOfDay(d: Date): number {
            return moment(d).endOf("day").unix();
        }

    }
}
