module app.core.controllers {
    "use strict";

    import GridService = app.core.services.SubstitutesGridService;
    import SubstituteType = app.core.models.SubstituteType;

    export class BaseSubstitutesController {
        
        private _currentUser;
        private FromDate: number;
        private ToDate: number;
        private InitialLoad: number;
        
        People;
        OrgUnits;
        Substitutes;
        PersonalApprovers;
        SubstituteGrid;
        ApproverGrid;
        ApproverToDate: Date;
        ApproverFromDate: Date;
        SubstituteToDate: Date;
        SubstituteFromDate: Date;
        SubInfinitePeriod;
        AppInfinitePeriod;
        
        constructor(protected $scope, protected Person, protected $rootScope, protected HelpText, protected Autocomplete, protected SubstitutesGridService: GridService, protected $modal, protected $timeout, private substituteType: SubstituteType) {
            this._currentUser = $scope.CurrentUser;

            var date = new Date();
            date.setMonth(date.getMonth() - 1);
            this.FromDate = this.StartOfDay(date);
            this.ToDate = this.EndOfDay(new Date());

            this.People = Autocomplete.activeUsers();
            this.OrgUnits = Autocomplete.orgUnits();

            this.Substitutes = SubstitutesGridService.GetSubstitutesGrid(substituteType, this.FromDate, this.ToDate);
            this.PersonalApprovers = SubstitutesGridService.GetPersonalApproversGrid(substituteType, this.FromDate, this.ToDate);

            this.LoadInitialDates();
        }

        ClearClicked(type) {
            var from = new Date();
            from.setMonth(from.getMonth() - 1);

            if (type === "substitute") {
                this.SubInfinitePeriod = false;
                this.SubstituteToDate = new Date();
                this.SubstituteFromDate = from;
                this.SubstituteGrid.dataSource.filter([]);
            }
            else if (type === "approver") {
                this.AppInfinitePeriod = false;
                this.ApproverToDate = new Date();
                this.ApproverFromDate = from;
                this.ApproverGrid.dataSource.filter([]);
            }
        }

        DateChanged(type) {
            this.$timeout(() => {
                if (type == "substitute") {
                    var subFrom = this.StartOfDay(this.SubstituteFromDate);
                    var subTo = this.EndOfDay(this.SubstituteToDate);

                    // Initial load is also a bit of a hack.
                    // dateChanged is called twice when the default values for the datepickers are set.
                    // This leads to sorting the grid content on load, which is not what we want.
                    // Therefore the sorting is not done the first 2 times the dates change - Which are the 2 times we set the default values.
                    if (this.InitialLoad <= 0) {
                        this.ApplyDateFilter(subFrom, subTo, "substitute");
                    }
                } else if (type == "approver") {
                    var from = this.StartOfDay(this.ApproverFromDate);
                    var to = this.EndOfDay(this.ApproverToDate);

                    // Initial load is also a bit of a hack.
                    // dateChanged is called twice when the default values for the datepickers are set.
                    // This leads to sorting the grid content on load, which is not what we want.
                    // Therefore the sorting is not done the first 2 times the dates change - Which are the 2 times we set the default values.
                    if (this.InitialLoad <= 0) {
                        this.ApplyDateFilter(from, to, "approver");
                    }
                }

                this.InitialLoad--;
            }, 0);
        }

        ApplyDateFilter(fromDateStamp, toDateStamp, type) {
            if (type == "substitute") {
                this.SubstituteGrid.dataSource.filter([]);
                var subFilters = [];
                subFilters.push({ field: "StartDateTimestamp", operator: "lte", value: toDateStamp });

                if (!this.SubInfinitePeriod) {
                    subFilters.push({ field: "EndDateTimestamp", operator: "gte", value: fromDateStamp });
                }
                this.SubstituteGrid.dataSource.filter(subFilters);
            } else if (type == "approver") {
                this.ApproverGrid.dataSource.filter([]);
                var appFilters = [];
                appFilters.push({ field: "StartDateTimestamp", operator: "lte", value: toDateStamp });

                if (!this.AppInfinitePeriod) {
                    appFilters.push({ field: "EndDateTimestamp", operator: "gte", value: fromDateStamp });
                }
                this.ApproverGrid.dataSource.filter(appFilters);
            }
        }

        DateOptions = {
            format: "dd/MM/yyyy",
        }

        RefreshGrids() {
            this.SubstituteGrid.dataSource.read();
            this.ApproverGrid.dataSource.read();
        }

        LoadInitialDates() {
            this.InitialLoad = 4;

            var from = new Date();

            from.setMonth(from.getMonth() - 1);
            this.ApproverToDate = new Date();
            this.ApproverFromDate = from;

            this.SubstituteToDate = new Date();
            this.SubstituteFromDate = from;
        }

        OpenEditSubstitute(id) {
            var this_ = this;
            var modalInstance = this.$modal.open({
                templateUrl: 'App/Core/Views/Modals/EditSubstituteModal.html',
                controller: 'EditSubstituteModalInstanceController',
                backdrop: 'static',
                size: 'lg',
                resolve: {
                    persons() {
                        return this_.People;
                    },
                    orgUnits() {
                        return this_.OrgUnits;
                    },
                    leader() {
                        return this_._currentUser;
                    },
                    substituteId() {
                        return id;
                    },
                    SubstituteType() {
                        return this_.substituteType;
                    }
                }
            });

            modalInstance.result.then(() => {
                this.RefreshGrids();
            }, () => {

            });
        }

        OpenEditApprover(id) {
            var this_ = this;
            this.$modal.open({
                templateUrl: "App/Core/Views/Modals/EditApproverModal.html",
                controller: "EditApproverModalInstanceController",
                controllerAs: "ctrl",
                backdrop: "static",
                size: "lg",
                resolve: {
                    persons() {
                        return this_.People;
                    },
                    orgUnits() {
                        return this_.OrgUnits;
                    },
                    leader() {
                        return this_._currentUser;
                    },
                    substituteId() {
                        return id;
                    },
                    SubstituteType() {
                        return this_.substituteType;
                    }
                }
            }).result.then(() => {
                this.ApproverGrid.dataSource.read();
            });
        }

        CreateNewApprover() {
            var this_ = this;
            var modalInstance = this.$modal.open({
                // Change these
                templateUrl: 'App/Core/Views/Modals/NewApproverModal.html',
                controller: 'NewApproverModalInstanceController',
                backdrop: 'static',
                size: 'lg',
                resolve: {
                    persons() {
                        return this_.People;
                    },
                    orgUnits() {
                        return this_.OrgUnits;
                    },
                    leader() {
                        return this_._currentUser;
                    },
                    SubstituteType() {
                        return this_.substituteType;
                    }
                }
            });

            modalInstance.result.then(() => {
                this.RefreshGrids();
            }, () => {

            });
        }

        CreateNewSubstitute() {
            var this_ = this;
            var modalInstance = this.$modal.open({
                // Change these
                templateUrl: 'App/Core/Views/Modals/NewSubstituteModal.html',
                controller: 'NewSubstituteModalInstanceController',
                backdrop: 'static',
                size: 'lg',
                resolve: {
                    persons() {
                        return this_.People;
                    },
                    orgUnits() {
                        return this_.OrgUnits;
                    },
                    leader() {
                        return this_._currentUser;
                    },
                    SubstituteType() {
                        return this_.substituteType;
                    }
                }
            });

            modalInstance.result.then(() => {
                this.RefreshGrids();
            }, () => {

            });
        }

        OpenDeleteApprover(id) {
            var this_ = this;
            var modalInstance = this.$modal.open({
                // Change these
                templateUrl: 'App/Core/Views/Modals/ConfirmDeleteApproverModal.html',
                controller: 'ConfirmDeleteApproverModalInstanceController',
                backdrop: 'static',
                size: 'lg',
                resolve: {
                    persons() {
                        return this_.People;
                    },
                    orgUnits() {
                        return this_.OrgUnits;
                    },
                    leader() {
                        return this_._currentUser;
                    },
                    substituteType() {
                        return this_.substituteType;
                    },
                    substituteId() {
                        return id;
                    }
                }
            });

            modalInstance.result.then(() => {
                this.RefreshGrids();
            }, () => {

            });
        }

        OpenDeleteSubstitute(id) {
            var this_ = this;
            var modalInstance = this.$modal.open({
                // Change these
                templateUrl: 'App/Core/Views/Modals/ConfirmDeleteSubstituteModal.html',
                controller: 'ConfirmDeleteSubstituteModalInstanceController',
                backdrop: 'static',
                size: 'lg',
                resolve: {
                    persons() {
                        return this_.People;
                    },
                    orgUnits() {
                        return this_.OrgUnits;
                    },
                    leader() {
                        return this_._currentUser;
                    },
                    substituteType() {
                        return this_.substituteType;
                    },
                    substituteId() {
                        return id;
                    }
                }
            });

            modalInstance.result.then(() => {
                this.RefreshGrids();
            }, () => {

            });
        }

        private StartOfDay(d: Date): number {
            return moment(d).startOf("day").unix();
        }

        private EndOfDay(d: Date): number {
            return moment(d).endOf("day").unix();
        }

    }
}