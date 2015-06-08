angular.module("application").service("HelpText", ["$resource", function ($resource) {
    return $resource("/api/HelpText/:id", { id: "@id" }, {
        "get": {
            method: "GET",
            isArray: false,
            transformResponse: function (data) {
                // This sucks, but blame angular.
                // http://stackoverflow.com/questions/24876593/resource-query-return-split-strings-array-of-char-instead-of-a-string
                return { text: angular.fromJson(data) };
            }
        },
        "getAll": {
            method: "GET",
            url: "/api/HelpText/",
            isArray: false,
            transformResponse: function (data) {
                var res = {};

                angular.forEach(angular.fromJson(data), function(value, key) {
                    res[value.key] = { text: value.value };
                });
                return res;
            }
        }
    });
}]);