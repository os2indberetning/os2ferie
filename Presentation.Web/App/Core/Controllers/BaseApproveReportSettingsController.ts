module app.core.controllers {
    "use strict";

    import ReportType = app.core.models.ReportType;

    export class BaseApproveReportsSettingsController {

        private infinitePeriod = 9999999999;
        private personId;
        private isGridLoaded = false;

        collapseSubstitute = false;
        collapsePersonalApprover = false;
        orgUnits = [];
        people = [];
        currentPerson = {}
        personalApproverHelpText;

        substituteOrgUnit = "";
        showSubstituteSettings;
        substitutes: kendo.ui.GridOptions;
        mySubstitutesGrid: kendo.ui.Grid;
        substituteGrid: kendo.ui.Grid;
        personalApproverGrid: kendo.ui.Grid;
        personalApprovers: kendo.ui.GridOptions;
        mySubstitutes: kendo.ui.GridOptions;

        constructor(protected $scope, protected Person, protected $rootScope, protected Autocomplete, protected $modal, protected moment, private reportType: ReportType) {

            this.personalApproverHelpText = $rootScope.HelpTexts.PersonalApproverHelpText.text;
            this.personId = $rootScope.CurrentUser.Id;

            this.currentPerson = $rootScope.CurrentUser;

            this.people = Autocomplete.activeUsers();
            this.orgUnits = Autocomplete.orgUnits();

            this.showSubstituteSettings = $rootScope.CurrentUser.IsLeader;

            this.substitutes = {
                autoBind: false,
                dataSource: {
                    type: "odata-v4",
                    transport: {
                        read: {
                            beforeSend(req) {
                                req.setRequestHeader('Accept', 'application/json;odata=fullmetadata');
                            },
                            url: "odata/Substitutes/Service.Substitute(Type=" + this.reportType + ")?$expand=OrgUnit,Sub,Person,Leader &$filter=PersonId eq " + this.personId,
                            dataType: "json",
                            cache: false
                        }
                    },
                    pageSize: 20
                },
                sortable: true,
                pageable: {
                    messages: {
                        display: "{0} - {1} af {2} ", //{0} is the index of the first record on the page, {1} - index of the last record on the page, {2} is the total amount of records
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
                        title: "Opsat af"
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
                        title: "Til",
                        field: "EndDateTimestamp",
                        template: (data) => {
                            if (data.EndDateTimestamp === this.infinitePeriod) {
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
                        template: (data) => `<a ng-click='arsCtrl.openEditSubstitute(${data.Id})'>Rediger</a> | <a ng-click='arsCtrl.openDeleteSubstitute(${data.Id})'>Slet</a>`
                    }
                ],
                scrollable: false
            };

            this.personalApprovers = {
                autoBind: false,
                dataSource: {
                    type: "odata-v4",
                    transport: {
                        read: {
                            beforeSend(req) {
                                req.setRequestHeader('Accept', 'application/json;odata=fullmetadata');
                            },
                            url: "odata/Substitutes/Service.Personal(Type=" + this.reportType + ")?$expand=OrgUnit,Sub,Leader,Person&$filter=LeaderId eq " + this.personId,
                            dataType: "json",
                            cache: false
                        }
                    },
                    pageSize: 20
                },
                sortable: true,
                pageable: {
                    messages: {
                        display: "{0} - {1} af {2} ", //{0} is the index of the first record on the page, {1} - index of the last record on the page, {2} is the total amount of records
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
                        field: "Leader.FullName",
                        title: "Opsat af"
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
                            if (data.EndDateTimestamp === this.infinitePeriod) {
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
                        template: (data) => `<a ng-click='arsCtrl.openEditApprover(${data.Id})'>Rediger</a> | <a ng-click='arsCtrl.openDeleteApprover(${data.Id})'>Slet</a>`
                    }
                ],
                scrollable: false
            };

            this.mySubstitutes = {
                autoBind: false,
                dataSource: {
                    type: "odata-v4",
                    transport: {
                        read: {
                            beforeSend(req) {
                                req.setRequestHeader('Accept', 'application/json;odata=fullmetadata');
                            },
                            url: "odata/Substitutes/Service.Substitute(Type=" + this.reportType + ")?$expand=Sub,Person,Leader,OrgUnit &$filter=PersonId eq LeaderId and SubId eq " + this.personId,
                            dataType: "json",
                            cache: false
                        }
                    },
                    pageSize: 20
                },
                sortable: true,
                pageable: {
                    messages: {
                        display: "{0} - {1} af {2} ", //{0} is the index of the first record on the page, {1} - index of the last record on the page, {2} is the total amount of records
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
                        title: "Opsat af"
                    },
                    {
                        field: "StartDateTimestamp",
                        title: "Fra",
                        template: (data) => {
                            var m = moment.unix(data.StartDateTimestamp).toDate();
                            return m.getDate() + "/" +
                                (m.getMonth() + 1) + "/" + // +1 because getMonth is zero indexed.
                                m.getFullYear();
                        }
                    },
                    {
                        title: "Til",
                        field: "EndDateTimestamp",
                        template: (data) => {
                            if (data.EndDateTimestamp === this.infinitePeriod) {
                                return "På ubestemt tid";
                            }
                            var m = moment.unix(data.EndDateTimestamp).toDate();
                            return m.getDate() + "/" +
                                (m.getMonth() + 1) + "/" + // +1 because getMonth is zero indexed.
                                m.getFullYear();
                        }
                    }
                ],
                scrollable: false
            };

            this.$scope.$on("kendoRendered",
                () => {
                    if (!this.isGridLoaded) {
                        this.isGridLoaded = true;
                        this.refreshGrids();
                    }
                });
        }

        refreshGrids() {
            if (!this.isGridLoaded) return;
            this.substituteGrid.dataSource.read();
            this.mySubstitutesGrid.dataSource.read();
            this.personalApproverGrid.dataSource.read();
        }

        openDeleteApprover(id) {
            var modalInstance = this.$modal.open({
                templateUrl: 'App/Core/Views/Modals/ConfirmDeleteApproverModal.html',
                controller: 'ConfirmDeleteApproverModalInstanceController',
                backdrop: 'static',
                size: 'lg',
                resolve: {
                    persons: () => this.people,
                    orgUnits: () => this.orgUnits,
                    leader: () => this.currentPerson,
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
                templateUrl: 'App/Core/Views/Modals/ConfirmDeleteSubstituteModal.html',
                controller: 'ConfirmDeleteSubstituteModalInstanceController',
                backdrop: 'static',
                size: 'lg',
                resolve: {
                    persons: () => this.people,
                    orgUnits: () => this.orgUnits,
                    leader: () => this.currentPerson,
                    substituteId: () => id,
                    ReportType: () => this.reportType
                }
            });

            modalInstance.result.then(() => {
                this.refreshGrids();
            });
        }

        openEditSubstitute(id) {
            /// <summary>
            /// Opens edit substitute modal
            /// </summary>
            /// <param name="id"></param>
            var modalInstance = this.$modal.open({
                templateUrl: 'App/Core/Views/Modals/editSubstituteModal.html',
                controller: 'EditSubstituteModalInstanceController',
                backdrop: 'static',
                size: 'lg',
                resolve: {
                    persons: () => this.people,
                    orgUnits: () => this.orgUnits,
                    leader: () => this.currentPerson,
                    substituteId: () => id,
                    ReportType: () => this.reportType
                }
            });

            modalInstance.result.then(() => {
                this.refreshGrids();
            });
        }

        openEditApprover(id) {
            /// <summary>
            /// Opens edit approver modal
            /// </summary>
            /// <param name="id">Id of approver to edit.</param>
            var modalInstance = this.$modal.open({
                templateUrl: 'App/Core/Views/Modals/EditApproverModal.html',
                controller: 'EditApproverModalInstanceController',
                backdrop: 'static',
                size: 'lg',
                resolve: {
                    persons: () => this.people,
                    orgUnits: () => this.orgUnits,
                    leader: () => this.currentPerson,
                    substituteId: () => id,
                    ReportType: () => this.reportType
                }
            });

            modalInstance.result.then(() => {
                this.refreshGrids();
            });
        }

        createNewApprover() {
            var modalInstance = this.$modal.open({
                templateUrl: 'App/Core/Views/Modals/newApproverModal.html',
                controller: 'NewApproverModalInstanceController',
                backdrop: 'static',
                size: 'lg',
                resolve: {
                    persons: () => this.people,
                    orgUnits: () => this.orgUnits,
                    leader: () => this.currentPerson,
                    ReportType: () => this.reportType
                }
            });

            modalInstance.result.then(() => {
                this.refreshGrids();
            });
        }

        createNewSubstitute() {
            /// <summary>
            /// Opens create new substitute modal
            /// </summary>
            var modalInstance = this.$modal.open({
                templateUrl: 'App/Core/Views/Modals/newSubstituteModal.html',
                controller: 'NewSubstituteModalInstanceController',
                backdrop: 'static',
                size: 'lg',
                resolve: {
                    persons: () => this.people,
                    orgUnits: () => this.orgUnits,
                    leader: () => this.currentPerson,
                    ReportType: () => this.reportType
                }
            });

            modalInstance.result.then(() => {
                this.refreshGrids();
            });
        }
    }
}
