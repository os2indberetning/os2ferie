angular.module("application").service('Substitute', ["$resource", function ($resource) {
    return $resource("/odata/Substitutes(:id)?:query", { id: "@id", query: "@query" }, {
        "get": {
            method: "GET",
            isArray: false,
            headers: { 'Accept': 'application/json;odata=fullmetadata' },
            url: "odata/Substitutes(:id)?$expand=OrgUnit,Sub,Persons&:query",
            data: ''
        },
        "patch": { method: "PATCH", isArray: false },
        "post": { method: "POST", isArray: false }
    });
}]);