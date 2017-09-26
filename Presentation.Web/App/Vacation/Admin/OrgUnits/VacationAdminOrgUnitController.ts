module app.vacation {
    "use strict";

    import NotificationService = core.interfaces.NotificationService;
    import Employment = core.models.Employment;
    import Person = core.models.Person;

    class VacationAdminOrgUnitController {

        static $inject = [
            "$scope",
            "$rootScope",
            "OrgUnit",
            "NotificationService",
            "Autocomplete",
            "$modal"
        ];

        public chosenUnit;
        public gridOptions;
        public autoCompleteOptions;
        public grid;
        public selectedHasAccess = -1;
        public orgUnits;
        
        constructor(
            private $scope,
            private $rootScope,
            private OrgUnit,
            private NotificationService: NotificationService,
            private Autocomplete,
            private $modal) {

            this.orgUnits = Autocomplete.orgUnits();

            this.autoCompleteOptions = {
                filter: "contains",
                select: function (e) {
                    this.orgUnit = this.dataItem(e.item.index());
                }
            }

            this.gridOptions = {
                autoBind: false,
                dataSource: {
                    type: "odata-v4",
                    transport: {
                        read: {
                            beforeSend: function (req) {
                                req.setRequestHeader('Accept', 'application/json;odata=fullmetadata');
                            },
                            url: "/odata/OrgUnits",
                            dataType: "json",
                            cache: false
                        }
                    },
                    schema: {
                        model: {
                            fields: {
                                OrgId: {
                                    editable: false
                                },
                                ShortDescription: {
                                    editable: false
                                },
                                LongDescription: {
                                    editable: false
                                },
                                HasAccessToVacation: {
                                    editable: false
                                }
                            }
                        }
                    },
                    pageSize: 20,
                    serverPaging: true,
                    serverFiltering: true
                },
                sortable: true,
                pageable: {
                    messages: {
                        display: "{0} - {1} af {2} organisationsenheder", //{0} is the index of the first record on the page, {1} - index of the last record on the page, {2} is the total amount of records
                        empty: "Ingen organisationsenheder at vise",
                        page: "Side",
                        of: "af {0}", //{0} is total amount of pages
                        itemsPerPage: "Organisationsenheder pr. side",
                        first: "Gå til første side",
                        previous: "Gå til forrige side",
                        next: "Gå til næste side",
                        last: "Gå til sidste side",
                        refresh: "Genopfrisk"
                    },
                    pageSizes: [5, 10, 20, 30, 40, 50, 100, 150, 200]
                },
                dataBound: function () {
                    this.expandRow(this.tbody.find("tr.k-master-row").first());
                },
                editable: true,
                columns: [
                    {
                        field: "OrgId",
                        title: "Organisations ID"
                    },
                    {
                        field: "ShortDescription",
                        title: "Kort beskrivelse"
                    },
                    {
                        field: "LongDescription",
                        title: "Lang beskrivelse"
                    },
                    {
                        field: "HasAccessToVacation",
                        title: "Adgang til ferie",
                        template: function (data) {
                            if (data.HasAccessToVacation) {
                                return "<input type='checkbox' ng-click='orgUnitsCtrl.rowChecked(" + data.Id + ", false)' checked></input>";
                            } else {
                                return "<input type='checkbox' ng-click='orgUnitsCtrl.rowChecked(" + data.Id + ", true)'></input>";
                            }
                        }
                    }
                ]
            };

        }

        public orgUnitChanged(item) {
            /// <summary>
            /// Filters grid content
            /// </summary>
            /// <param name="item"></param>
            this.updateSourceUrl();
        }

        private updateSourceUrl() {
            var url = "/odata/OrgUnits";

            if (Object.keys(this.chosenUnit).length !== 0) {
                url += "?$filter=contains(LongDescription," + "'" + encodeURIComponent(this.chosenUnit + "')");
            } else {
                if (this.selectedHasAccess !== -1)
                    url += "?$filter=HasAccessToFourKmRule eq " + (this.selectedHasAccess === 0 ? "false" : "true");
            }


            this.grid.dataSource.transport.options.read.url = url;
            this.grid.dataSource.read();
        }

        public refreshGrid()
        {
            this.grid.dataSource.read();
        }

        public rowChecked(orgUnitId, newValue) {
            /// <summary>
            /// Is called when the user checks an orgunit in the grid.
            /// Patches HasAccessToFourKmRule on the backend.
            /// </summary>
            /// <param name="id"></param>


            this.$modal.open({
                templateUrl: '/App/Vacation/Admin/OrgUnits/VacationHasAccessToVacationModalView.html',
                controller: 'VacationHasAccessToVacationModalController as vhatvmc',
                backdrop: "static"
            }).result.then(res => {
                this.OrgUnit.SetVacationAccess({ id: orgUnitId, value: newValue, recursive: res }, () => {
                    if (newValue) {
                        this.NotificationService.AutoFadeNotification("success", "", "Adgang til ferie tilføjet.");
                    } else {
                        this.NotificationService.AutoFadeNotification("success", "", "Adgang til ferie fjernet.");
                    }
                    this.refreshGrid();
                });
            });
        }

    }

    angular.module("app.vacation").controller("VacationAdminOrgUnitController", VacationAdminOrgUnitController);
}
