angular.module("app.vacation").controller("MainMenuController", [
   "$scope", "Person", "PersonalAddress", "HelpText", "$rootScope", "OrgUnit", "Person", function ($scope, Person, PersonalAddress, HelpText, $rootScope, OrgUnit, Person) {

       if ($rootScope.CurrentUser == undefined) {
           $rootScope.CurrentUser = Person.GetCurrentUser().$promise.then(function (res) {
               $rootScope.CurrentUser = res;
               $scope.showAdministration = res.IsAdmin;
               $scope.showApproveReports = res.IsLeader || res.IsSubstitute;
               $scope.UserName = res.FullName;
           });
       } else {
               $scope.showAdministration = $rootScope.CurrentUser.IsAdmin;
               $scope.showApproveReports = $rootScope.CurrentUser.IsLeader || $rootScope.CurrentUser.IsSubstitute;
               $scope.UserName = $rootScope.CurrentUser.FullName;
       }


    }
]);