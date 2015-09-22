angular.module("application").service("Autocomplete", function () {
    return {
        allUsers: function () {
            return {
                type: "json",
                minLength: 3,
                serverFiltering: true,
                crossDomain: true,
                transport: {
                    read: {
                        url: function (item) {

                            var req = "/odata/Person?$filter=contains(FullName," + "'" + encodeURIComponent(item.filter.filters[0].value) + "')";
                            return req;
                        },
                        dataType: "json",
                        data: {

                        }
                    }
                },
                schema: {
                    data: function (data) {
                        return data.value;
                    }
                },
            }
        },
        activeUsers: function () {
            return {
                type: "json",
                minLength: 3,
                serverFiltering: true,
                crossDomain: true,
                transport: {
                    read: {
                        url: function (item) {

                            var req = "/odata/Person?$filter=contains(FullName," + "'" + encodeURIComponent(item.filter.filters[0].value) + "') and IsActive";
                            return req;
                        },
                        dataType: "json",
                        data: {

                        }
                    }
                },
                schema: {
                    data: function (data) {
                        return data.value;
                    }
                },
            }
        },
        activeUsersWithoutLeader: function (leaderId) {
            return {
                type: "json",
                minLength: 3,
                serverFiltering: true,
                crossDomain: true,
                transport: {
                    read: {
                        url: function (item) {

                            var req = "/odata/Person?$filter=contains(FullName," + "'" + encodeURIComponent(item.filter.filters[0].value) + "') and IsActive and Id ne " + leaderId;
                            return req;
                        },
                        dataType: "json",
                        data: {

                        }
                    }
                },
                schema: {
                    data: function (data) {
                        return data.value;
                    }
                },
            }
        },
        admins: function () {
            return {
                type: "json",
                minLength: 3,
                serverFiltering: true,
                crossDomain: true,
                transport: {
                    read: {
                        url: function (item) {

                            var req = "/odata/Person?$filter=contains(FullName," + "'" + encodeURIComponent(item.filter.filters[0].value) + "') and IsActive and IsAdmin";
                            return req;
                        },
                        dataType: "json",
                        data: {

                        }
                    }
                },
                schema: {
                    data: function (data) {
                        return data.value;
                    }
                },
            }
        },
        nonAdmins: function () {
            return {
                type: "json",
                minLength: 3,
                serverFiltering: true,
                crossDomain: true,
                transport: {
                    read: {
                        url: function (item) {

                            var req = "/odata/Person?$filter=contains(FullName," + "'" + encodeURIComponent(item.filter.filters[0].value) + "') and IsActive and not IsAdmin";
                            return req;
                        },
                        dataType: "json",
                        data: {

                        }
                    }
                },
                schema: {
                    data: function (data) {
                        return data.value;
                    }
                },
            }
        },
        leaders: function () {
            return {
                type: "json",
                minLength: 3,
                serverFiltering: true,
                crossDomain: true,
                transport: {
                    read: {
                        url: function (item) {
                            var req = "/odata/Employments?$filter=contains(Person/FullName," + "'" + encodeURIComponent(item.filter.filters[0].value) + "') and IsLeader &$expand=Person &$select=Person";
                            return req;
                        },
                        dataType: "json",
                        data: {

                        }
                    }
                },
                schema: {
                    data: function (data) {
                        var map = {};
                        var result = [];
                        var leaders = angular.fromJson(data).value;

                        // Remove duplicate values.
                        for (var i = 0; i < leaders.length; i++) {
                            if (map[leaders[i].Person.FullName] == undefined) {
                                result.push(leaders[i].Person);
                                map[leaders[i].Person.FullName] = leaders[i].Person;
                            }
                        }

                        return result;
                    }
                },
            }
        },
        orgUnits: function () {
            return {
                type: "json",
                minLength: 3,
                serverFiltering: true,
                crossDomain: true,
                transport: {
                    read: {
                        url: function (item) {

                            var req = "/odata/OrgUnits?$filter=contains(LongDescription," + "'" + encodeURIComponent(item.filter.filters[0].value) + "')";
                            return req;
                        },
                        dataType: "json",
                        data: {

                        }
                    }
                },
                schema: {
                    data: function (data) {
                        return data.value;
                    }
                },
            }
        }
    }
});