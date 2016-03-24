angular.module("app.vacation").config([
    "$stateProvider", "$urlRouterProvider", function($stateProvider, $urlRouterProvider) {

        $stateProvider
            .state("vacation.report", {
                url: "/report",
                templateUrl: "/App/Vacation/ReportVacation/ReportVacationView.html",
                controller: "ReportVacationController",
                resolve: {
                    adminEditCurrentUser: function() { return 0; },
                    ReportId: function() { return -1; },
                    CurrentUser: [
                        "$rootScope", "Person", function($rootScope, Person) {
                            if ($rootScope.CurrentUser == undefined) {
                                return Person.GetCurrentUser().$promise.then(function(res) {
                                    $rootScope.CurrentUser = res;
                                });
                            } else {
                                return $rootScope.CurrentUser;
                            }
                            
                        }
                    ],
                    $modalInstance: function() { return -1; }
                }
            });

    }
]);