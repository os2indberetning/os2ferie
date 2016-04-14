var app;
(function (app) {
    var core;
    (function (core) {
        var controllers;
        (function (controllers) {
            "use strict";
            var BaseApproveReportsSettingsController = (function () {
                function BaseApproveReportsSettingsController($scope, Person, $rootScope, Autocomplete, $modal, substituteType) {
                    this.$scope = $scope;
                    this.Person = Person;
                    this.$rootScope = $rootScope;
                    this.Autocomplete = Autocomplete;
                    this.$modal = $modal;
                    this.substituteType = substituteType;
                    this.InfinitePeriod = 9999999999;
                    this.CollapseSubstitute = false;
                    this.CollapsePersonalApprover = false;
                    this.OrgUnits = [];
                    this.People = [];
                    this.CurrentPerson = {};
                    this.SubstituteOrgUnit = "";
                    this.Substitutes = {
                        autoBind: false,
                        dataSource: {
                            type: "odata",
                            transport: {
                                read: {
                                    beforeSend: function (req) {
                                        req.setRequestHeader('Accept', 'application/json;odata=fullmetadata');
                                    },
                                    url: "odata/Substitutes/Service.Substitute(Type=" + this.substituteType + ")?$expand=OrgUnit,Sub,Person,Leader &$filter=PersonId eq " + this.PersonId,
                                    dataType: "json",
                                    cache: false
                                },
                                parameterMap: function (options, type) {
                                    var d = kendo.data.transports.odata.parameterMap(options, type);
                                    delete d.$inlinecount; // <-- remove inlinecount parameter                                                        
                                    d.$count = true;
                                    return d;
                                }
                            },
                            pageSize: 20,
                            schema: {
                                data: function (data) {
                                    return data.value; // <-- The result is just the data, it doesn't need to be unpacked.
                                },
                                total: function (data) {
                                    return data['@odata.count']; // <-- The total items count is the data length, there is no .Count to unpack.
                                }
                            }
                        },
                        sortable: true,
                        pageable: {
                            messages: {
                                display: "{0} - {1} af {2} ",
                                empty: "Ingen stedfortrædere at vise",
                                page: "Side",
                                of: "af {0}",
                                itemsPerPage: "stedfortrædere pr. side",
                                first: "Gå til første side",
                                previous: "Gå til forrige side",
                                next: "Gå til næste side",
                                last: "Gå til sidste side",
                                refresh: "Genopfrisk",
                            },
                            pageSizes: [5, 10, 20, 30, 40, 50, 100, 150, 200]
                        },
                        dataBound: function () {
                            this.expandRow(this.tbody.find("tr.k-master-row").first());
                        },
                        columns: [{
                                field: "Sub.FullName",
                                title: "Stedfortræder"
                            },
                            {
                                field: "Person.FullName",
                                title: "Stedfortræder for"
                            }, {
                                field: "OrgUnit.LongDescription",
                                title: "Organisationsenhed",
                            }, {
                                field: "Leader.FullName",
                                title: "Opsat af"
                            }, {
                                field: "StartDateTimestamp",
                                title: "Fra",
                                template: function (data) {
                                    var m = moment.unix(data.StartDateTimestamp).toDate();
                                    return m.getDate() + "/" +
                                        (m.getMonth() + 1) + "/" +
                                        m.getFullYear();
                                }
                            }, {
                                title: "Til",
                                field: "EndDateTimestamp",
                                template: function (data) {
                                    if (data.EndDateTimestamp === this.InfinitePeriod) {
                                        return "På ubestemt tid";
                                    }
                                    var m = moment.unix(data.EndDateTimestamp).toDate();
                                    return m.getDate() + "/" +
                                        (m.getMonth() + 1) + "/" +
                                        m.getFullYear();
                                }
                            }, {
                                title: "Muligheder",
                                template: "<a ng-click='ctrl.OpenEditSubstitute(${Id})'>Rediger</a> | <a ng-click='ctrl.OpenDeleteSubstitute(${Id})'>Slet</a>"
                            }],
                        scrollable: false
                    };
                    this.PersonalApprovers = {
                        autoBind: false,
                        dataSource: {
                            type: "odata",
                            transport: {
                                read: {
                                    beforeSend: function (req) {
                                        req.setRequestHeader('Accept', 'application/json;odata=fullmetadata');
                                    },
                                    url: "odata/Substitutes/Service.Personal(Type=" + this.substituteType + ")?$expand=OrgUnit,Sub,Leader,Person&$filter=LeaderId eq " + this.PersonId,
                                    dataType: "json",
                                    cache: false
                                },
                                parameterMap: function (options, type) {
                                    var d = kendo.data.transports.odata.parameterMap(options, type);
                                    delete d.$inlinecount; // <-- remove inlinecount parameter                                                        
                                    d.$count = true;
                                    return d;
                                }
                            },
                            pageSize: 20,
                            schema: {
                                data: function (data) {
                                    return data.value; // <-- The result is just the data, it doesn't need to be unpacked.
                                },
                                total: function (data) {
                                    return data['@odata.count']; // <-- The total items count is the data length, there is no .Count to unpack.
                                }
                            },
                        },
                        sortable: true,
                        pageable: {
                            messages: {
                                display: "{0} - {1} af {2} ",
                                empty: "Ingen personlige godkendere at vise",
                                page: "Side",
                                of: "af {0}",
                                itemsPerPage: "personlige godkendere pr. side",
                                first: "Gå til første side",
                                previous: "Gå til forrige side",
                                next: "Gå til næste side",
                                last: "Gå til sidste side",
                                refresh: "Genopfrisk",
                            },
                            pageSizes: [5, 10, 20, 30, 40, 50, 100, 150, 200]
                        },
                        dataBound: function () {
                            this.expandRow(this.tbody.find("tr.k-master-row").first());
                        },
                        columns: [{
                                field: "Sub.FullName",
                                title: "Godkender"
                            }, {
                                field: "Person.FullName",
                                title: "Godkender for"
                            }, {
                                field: "Leader.FullName",
                                title: "Opsat af"
                            }, {
                                field: "StartDateTimestamp",
                                title: "Fra",
                                template: function (data) {
                                    var m = moment.unix(data.StartDateTimestamp).toDate();
                                    return m.getDate() + "/" +
                                        (m.getMonth() + 1) + "/" +
                                        m.getFullYear();
                                },
                            }, {
                                field: "EndDateTimestamp",
                                title: "Til",
                                template: function (data) {
                                    if (data.EndDateTimestamp === this.InfinitePeriod) {
                                        return "På ubestemt tid";
                                    }
                                    var m = moment.unix(data.EndDateTimestamp).toDate();
                                    return m.getDate() + "/" +
                                        (m.getMonth() + 1) + "/" +
                                        m.getFullYear();
                                },
                            }, {
                                title: "Muligheder",
                                template: "<a ng-click='ctrl.OpenEditApprover(${Id})'>Rediger</a> | <a ng-click='ctrl.OpenDeleteApprover(${Id})'>Slet</a>"
                            }],
                        scrollable: false
                    };
                    this.MySubstitutes = {
                        autoBind: false,
                        dataSource: {
                            type: "odata",
                            transport: {
                                read: {
                                    beforeSend: function (req) {
                                        req.setRequestHeader('Accept', 'application/json;odata=fullmetadata');
                                    },
                                    url: "odata/Substitutes(Type=" + this.substituteType + ")?$expand=Sub,Person,Leader,OrgUnit &$filter=PersonId eq LeaderId and SubId eq " + this.PersonId,
                                    dataType: "json",
                                    cache: false
                                },
                                parameterMap: function (options, type) {
                                    var d = kendo.data.transports.odata.parameterMap(options, type);
                                    delete d.$inlinecount; // <-- remove inlinecount parameter                                                        
                                    d.$count = true;
                                    return d;
                                }
                            },
                            pageSize: 20,
                            schema: {
                                data: function (data) {
                                    return data.value; // <-- The result is just the data, it doesn't need to be unpacked.
                                },
                                total: function (data) {
                                    return data['@odata.count']; // <-- The total items count is the data length, there is no .Count to unpack.
                                }
                            }
                        },
                        sortable: true,
                        pageable: {
                            messages: {
                                display: "{0} - {1} af {2} ",
                                empty: "Ingen stedfortrædere at vise",
                                page: "Side",
                                of: "af {0}",
                                itemsPerPage: "stedfortrædere pr. side",
                                first: "Gå til første side",
                                previous: "Gå til forrige side",
                                next: "Gå til næste side",
                                last: "Gå til sidste side",
                                refresh: "Genopfrisk",
                            },
                            pageSizes: [5, 10, 20, 30, 40, 50, 100, 150, 200],
                        },
                        dataBound: function () {
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
                            }, {
                                field: "OrgUnit.LongDescription",
                                title: "Organisationsenhed",
                            }, {
                                field: "Leader.FullName",
                                title: "Opsat af"
                            }, {
                                field: "StartDateTimestamp",
                                title: "Fra",
                                template: function (data) {
                                    var m = moment.unix(data.StartDateTimestamp).toDate();
                                    return m.getDate() + "/" +
                                        (m.getMonth() + 1) + "/" +
                                        m.getFullYear();
                                }
                            }, {
                                title: "Til",
                                field: "EndDateTimestamp",
                                template: function (data) {
                                    if (data.EndDateTimestamp === this.InfinitePeriod) {
                                        return "På ubestemt tid";
                                    }
                                    var m = moment.unix(data.EndDateTimestamp).toDate();
                                    return m.getDate() + "/" +
                                        (m.getMonth() + 1) + "/" +
                                        m.getFullYear();
                                }
                            }],
                        scrollable: false
                    };
                    this.PersonalApproverHelpText = $rootScope.HelpTexts.PersonalApproverHelpText.text;
                    this.PersonId = $rootScope.CurrentUser.Id;
                    this.CurrentPerson = $rootScope.CurrentUser;
                    this.People = Autocomplete.activeUsers();
                    this.OrgUnits = Autocomplete.orgUnits();
                    this.ShowSubstituteSettings = $rootScope.CurrentUser.IsLeader;
                }
                BaseApproveReportsSettingsController.prototype.RefreshGrids = function () {
                    this.SubstituteGrid.dataSource.read();
                    this.MySubstitutesGrid.dataSource.read();
                    this.PersonalApproverGrid.dataSource.read();
                };
                BaseApproveReportsSettingsController.prototype.OpenDeleteApprover = function (id) {
                    var _this = this;
                    var this_ = this;
                    var modalInstance = this.$modal.open({
                        templateUrl: 'App/Core/Views/Modals/ConfirmDeleteApproverModal.html',
                        controller: 'ConfirmDeleteApproverModalInstanceController',
                        backdrop: 'static',
                        size: 'lg',
                        resolve: {
                            persons: function () {
                                return this_.People;
                            },
                            orgUnits: function () {
                                return this_.OrgUnits;
                            },
                            leader: function () {
                                return this_.CurrentPerson;
                            },
                            substituteId: function () {
                                return id;
                            },
                            SubstituteType: function () {
                                return this_.substituteType;
                            }
                        }
                    });
                    modalInstance.result.then(function () {
                        _this.RefreshGrids();
                    }, function () {
                    });
                };
                BaseApproveReportsSettingsController.prototype.OpenDeleteSubstitute = function (id) {
                    var _this = this;
                    var this_ = this;
                    /// <summary>
                    /// Opens delete substitute modal.
                    /// </summary>
                    /// <param name="id">Id of substitute to delete.</param>
                    var modalInstance = this.$modal.open({
                        templateUrl: 'App/Core/Views/Modals/ConfirmDeleteSubstituteModal.html',
                        controller: 'ConfirmDeleteSubstituteModalInstanceController',
                        backdrop: 'static',
                        size: 'lg',
                        resolve: {
                            persons: function () {
                                return this_.People;
                            },
                            orgUnits: function () {
                                return this_.OrgUnits;
                            },
                            leader: function () {
                                return this_.CurrentPerson;
                            },
                            substituteId: function () {
                                return id;
                            },
                            SubstituteType: function () {
                                return this_.substituteType;
                            }
                        }
                    });
                    modalInstance.result.then(function () {
                        _this.RefreshGrids();
                    }, function () {
                    });
                };
                BaseApproveReportsSettingsController.prototype.OpenEditSubstitute = function (id) {
                    var _this = this;
                    var this_ = this;
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
                            persons: function () {
                                return this_.People;
                            },
                            orgUnits: function () {
                                return this_.OrgUnits;
                            },
                            leader: function () {
                                return this_.CurrentPerson;
                            },
                            substituteId: function () {
                                return id;
                            },
                            SubstituteType: function () {
                                return this_.substituteType;
                            }
                        }
                    });
                    modalInstance.result.then(function () {
                        _this.RefreshGrids();
                    }, function () {
                    });
                };
                BaseApproveReportsSettingsController.prototype.OpenEditApprover = function (id) {
                    var _this = this;
                    var this_ = this;
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
                            persons: function () {
                                return this_.People;
                            },
                            orgUnits: function () {
                                return this_.OrgUnits;
                            },
                            leader: function () {
                                return this_.CurrentPerson;
                            },
                            substituteId: function () {
                                return id;
                            },
                            SubstituteType: function () {
                                return this_.substituteType;
                            }
                        }
                    });
                    modalInstance.result.then(function () {
                        _this.RefreshGrids();
                    }, function () {
                    });
                };
                BaseApproveReportsSettingsController.prototype.CreateNewApprover = function () {
                    var _this = this;
                    var this_ = this;
                    var modalInstance = this.$modal.open({
                        templateUrl: 'App/Core/Views/Modals/newApproverModal.html',
                        controller: 'NewApproverModalInstanceController',
                        backdrop: 'static',
                        size: 'lg',
                        resolve: {
                            persons: function () {
                                return this_.People;
                            },
                            orgUnits: function () {
                                return this_.OrgUnits;
                            },
                            leader: function () {
                                return this_.CurrentPerson;
                            },
                            SubstituteType: function () {
                                return this_.substituteType;
                            }
                        }
                    });
                    modalInstance.result.then(function () {
                        _this.RefreshGrids();
                    }, function () {
                    });
                };
                BaseApproveReportsSettingsController.prototype.CreateNewSubstitute = function () {
                    var _this = this;
                    var this_ = this;
                    /// <summary>
                    /// Opens create new substitute modal
                    /// </summary>
                    var modalInstance = this.$modal.open({
                        templateUrl: 'App/Core/Views/Modals/newSubstituteModal.html',
                        controller: 'NewSubstituteModalInstanceController',
                        backdrop: 'static',
                        size: 'lg',
                        resolve: {
                            persons: function () {
                                return this_.People;
                            },
                            orgUnits: function () {
                                return this_.OrgUnits;
                            },
                            leader: function () {
                                return this_.CurrentPerson;
                            },
                            SubstituteType: function () {
                                return this_.substituteType;
                            }
                        }
                    });
                    modalInstance.result.then(function () {
                        _this.RefreshGrids();
                    }, function () {
                    });
                };
                return BaseApproveReportsSettingsController;
            })();
            controllers.BaseApproveReportsSettingsController = BaseApproveReportsSettingsController;
        })(controllers = core.controllers || (core.controllers = {}));
    })(core = app.core || (app.core = {}));
})(app || (app = {}));
//# sourceMappingURL=BaseApproveReportSettingsController.js.map