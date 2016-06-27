angular.module("app.drive").config(["$stateProvider", function ($stateProvider) {

    $stateProvider
        .state("drive.driving", {
            url: "/driving",
            templateUrl: "/App/Drive/Driving/DrivingView.html",
            controller: "DrivingController",
            resolve: {
                adminEditCurrentUser : function() {return 0;},
                ReportId: function () { return -1; },
                $modalInstance: function () { return -1; }
            }
        })
        .state("drive.myreports", {
            url: "/myreports",
            templateUrl: "/App/Drive/MyReports/MyReportsView.html"
        })
        .state("drive.approvereports", {
            url: "/approvereports",
            templateUrl: "/App/Drive/ApproveReports/ApproveReportsView.html",
            data: {
                roles: ['Approver']
            }
        })
        .state("drive.settings", {
            url: "/settings",
            templateUrl: "/App/Drive/Settings/SettingsView.html",
            controller: "SettingsController"
        })
        .state("drive.admin", {
            url: "/admin",
            templateUrl: "/App/Drive/Admin/AdminView.html",
            controller: "AdminMenuController",
            data: {
                roles: ['Admin']
            }
        });
}]);