angular.module("app.vacation").service('VacationReport', ["$resource", function ($resource) {
    return $resource("/odata/VacationReports(:id)?:query&emailText=:emailText", { id: "@id", query: "@query", emailText: "@emailText" }, {
        "get": {
            method: "GET",
            isArray: false,
            transformResponse: function (res) {
                return angular.fromJson(res).value[0];
            }
        },
        "create": {
            method: "POST",
            isArray: false
        },
        "approve": {
            method: "GET",
            isArray: false,
            url: "/odata/VacationReports/Service.ApproveReport(Key=:id)"
        },
        "reject": {
            method: "GET",
            isArray: false,
            url: "/odata/VacationReports/Service.RejectReport(Key=:id)?comment=:comment"
        },
        "update": {
            method: "PUT",
            isArray: false
        },
        "getLatest": {
            method: "GET",
            isArray: false,
            url: "/odata/VacationReports/Service.GetLatestReportForUser?personId=:id",
            transformResponse: function (data) {
                var res = angular.fromJson(data);
                return res;
            }
        }
    });
}]);