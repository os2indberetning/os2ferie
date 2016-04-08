angular.module("app.core").service('File', ["$resource", function ($resource) {
    return $resource("/api/File", { id: "@id" }, {
        "generateKMDFile": { method: "GET" }
    });
}]);