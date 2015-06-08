angular.module("application").controller("MainMenuController", [
   "$scope", "Person", "PersonalAddress", "HelpText", "$rootScope", function ($scope, Person, PersonalAddress, HelpText, $rootScope) {


       HelpText.get({ id: "InformationHelpLink" }).$promise.then(function (res) {
           $scope.helpLink = res;
       });

       if ($rootScope.CurrentUser == undefined) {
           $rootScope.CurrentUser = Person.GetCurrentUser().$promise.then(function (res) {
               $rootScope.CurrentUser = res;
               $scope.showAdministration = res.IsAdmin;
               $scope.showApproveReports = res.IsLeader || res.IsSubstitute;
               $scope.UserName = res.FullName;
           });
       }

   }
]);