angular.module("app.core").service('Substitute', ["$resource", function ($resource) {
    return $resource("/odata/Substitutes(:id)?:query", { id: "@id", query: "@query" }, {
        "get": {
            method: "GET",
            isArray: false,
            headers: { 'Accept': 'application/json;odata=fullmetadata' },
            url: "odata/Substitutes(:id)?$expand=OrgUnit,Sub,Person&:query",
            data: ''
        },
        "patch": { method: "PATCH", isArray: false },
        "post": { method: "POST", isArray: false },
        'remove': {method:'DELETE'},
        'delete': {method:'DELETE'}
    });
}]);