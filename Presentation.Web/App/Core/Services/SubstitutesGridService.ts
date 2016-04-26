module app.core.services {
    import SubstituteType = app.core.models.SubstituteType;

    export class SubstitutesGridService {

        GetSubstitutesGrid(SubstituteType: SubstituteType, FromDate, ToDate) {
            return {
                dataSource: {
                    pageSize: 20,
                    type: "odata",
                    transport: {
                        read: {
                            beforeSend(req) {
                                req.setRequestHeader('Accept', 'application/json;odata=fullmetadata');
                            },
                            url: "odata/Substitutes/Service.Substitute(Type=" + SubstituteType +")?$expand=OrgUnit,Sub,Person,Leader,CreatedBy",
                            dataType: "json",
                            cache: false
                        },
                        parameterMap(options, type) {
                            var d = kendo.data.transports.odata.parameterMap(options, type);

                            delete d.$inlinecount; // <-- remove inlinecount parameter

                            d.$count = true;

                            return d;
                        }
                    },
                    schema: {
                        data(data) {
                            return data.value;
                        },
                        total(data) {
                            return data['@odata.count']; // <-- The total items count is the data length, there is no .Count to unpack.
                        }
                    },
                },
                serverPaging: true,
                serverAggregates: false,
                serverSorting: true,
                serverFiltering: true,
                sortable: true,
                scrollable: false,
                filter: [
                    { field: "StartDateTimestamp", operator: "lte", value: ToDate },
                    { field: "EndDateTimestamp", operator: "gte", value: FromDate }
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
                    }, {
                        field: "Person.FullName",
                        title: "Stedfortræder for"
                    }, {
                        field: "OrgUnit.LongDescription",
                        title: "Organisationsenhed"
                    }, {
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
                        template(data) {
                            var m = moment.unix(data.StartDateTimestamp).toDate();
                            return m.getDate() + "/" +
                                (m.getMonth() + 1) + "/" + // +1 because getMonth is zero indexed.
                                m.getFullYear();
                        }
                    }, {
                        field: "EndDateTimestamp",
                        title: "Til",
                        template(data) {
                            if (data.EndDateTimestamp == 9999999999) {
                                return "På ubestemt tid";
                            }
                            var m = moment.unix(data.EndDateTimestamp).toDate();
                            return m.getDate() + "/" +
                                (m.getMonth() + 1) + "/" + // +1 because getMonth is zero indexed.
                                m.getFullYear();
                        }
                    },
                    {
                        title: "Muligheder",
                        template: "<a ng-click='subCtrl.openEditSubstitute(${Id})'>Rediger</a> | <a ng-click='subCtrl.openDeleteSubstitute(${Id})'>Slet</a>"
                    }
                ]
            };
        }

        GetPersonalApproversGrid(SubstituteType: SubstituteType, FromDate, ToDate) {
            return {
                dataSource: {
                    pageSize: 20,
                    type: "odata",
                    transport: {
                        read: {
                            beforeSend(req) {
                                req.setRequestHeader("Accept", "application/json;odata=fullmetadata");
                            },
                            url: "odata/Substitutes/Service.Personal(Type=" + SubstituteType +")?$expand=OrgUnit,Sub,Leader,Person,CreatedBy",
                            dataType: "json",
                            cache: false
                        },
                        parameterMap(options, type) {
                            var d = kendo.data.transports.odata.parameterMap(options, type);

                            delete d.$inlinecount; // <-- remove inlinecount parameter

                            d.$count = true;

                            return d;
                        }
                    },
                    schema: {
                        data(data) {
                            return data.value;
                        },
                        total(data) {
                            return data["@odata.count"]; // <-- The total items count is the data length, there is no .Count to unpack.
                        }
                    },
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
                columns: [{
                    field: "Sub.FullName",
                    title: "Godkender"
                }, {
                        field: "Person.FullName",
                        title: "Godkender for"
                    }, {
                        field: "CreatedBy",
                        title: "Opsat af",
                        template(data) {
                            if (data.CreatedBy == undefined) return "<i>Ikke tilgængelig</i>";
                            return data.CreatedBy.FullName;
                        }
                }, {
                        field: "StartDateTimestamp",
                        title: "Fra",
                        template(data) {
                            var m = moment.unix(data.StartDateTimestamp).toDate();
                            return m.getDate() + "/" +
                                (m.getMonth() + 1) + "/" + // +1 because getMonth is zero indexed.
                                m.getFullYear();
                        }
                }, {
                        field: "EndDateTimestamp",
                        title: "Til",
                        template(data) {
                            if (data.EndDateTimestamp == 9999999999) {
                                return "På ubestemt tid";
                            }
                            var m = moment.unix(data.EndDateTimestamp).toDate();
                            return m.getDate() + "/" +
                                (m.getMonth() + 1) + "/" + // +1 because getMonth is zero indexed.
                                m.getFullYear();
                        }
                },
                    {
                        title: "Muligheder",
                        template: "<a ng-click='subCtrl.openEditApprover(${Id})'>Rediger</a> | <a ng-click='subCtrl.openDeleteApprover(${Id})'>Slet</a>"
                    }]
            }
        }

    }

    angular.module('app.core').service("SubstitutesGridService", SubstitutesGridService);
}
