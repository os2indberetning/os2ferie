module app.core.controllers {
    "use strict";

    import GridService = core.services.SubstitutesGridService;
    import SubstituteType = core.models.SubstituteType;

    export class BaseSubstitutesController {

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

        constructor(protected $scope, protected Person, protected $rootScope, protected HelpText, protected Autocomplete, protected SubstitutesGridService: GridService, protected $modal, protected $timeout, private substituteType: SubstituteType) {
            this.currentUser = $scope.CurrentUser;

            var date = new Date();
            date.setMonth(date.getMonth() - 1);
            this.fromDate = this.startOfDay(date);
            this.toDate = this.endOfDay(new Date());

            this.people = Autocomplete.activeUsers();
            this.orgUnits = Autocomplete.orgUnits();

            this.substitutes = SubstitutesGridService.GetSubstitutesGrid(substituteType, this.fromDate, this.toDate);
            this.personalApprovers = SubstitutesGridService.GetPersonalApproversGrid(substituteType, this.fromDate, this.toDate);

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
                    SubstituteType: () => this.substituteType
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
                    SubstituteType: () => this.substituteType
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
                    SubstituteType: () => this.substituteType
                }
            });

            modalInstance.result.then(() => {
                this.refreshGrids();
            });
        }

        createNewSubstitute() {
            var modalInstance = this.$modal.open({
                // Change these
                templateUrl: 'App/Core/Views/Modals/NewSubstituteModal.html',
                controller: 'NewSubstituteModalInstanceController',
                backdrop: 'static',
                size: 'lg',
                resolve: {
                    persons: () => this.people,
                    orgUnits: () => this.orgUnits,
                    leader: () => this.currentUser,
                    SubstituteType: () => this.substituteType
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
                    SubstituteType: () => this.substituteType
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
                    SubstituteType: () => this.substituteType
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
