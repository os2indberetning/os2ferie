angular.module("app.common").service('File', ["$resource", function ($resource) {
    return $resource("/api/File", { id: "@id" }, {
        "generateKMDFile": { method: "GET" }
    });
}]);