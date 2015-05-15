angular.module("application").controller("MainMenuController", [
   "$scope", "Person", "PersonalAddress", "HelpText", function ($scope, Person, PersonalAddress, HelpText) {


        HelpText.get({ id: "InformationHelpLink" }).$promise.then(function(res) {
            $scope.helpLink = res;
        });

       Person.GetCurrentUser().$promise.then(function (res) {
           PersonalAddress.GetHomeForUser({ id: res.Id }).$promise.then(function (addr) {
               $scope.HomeAddress = addr.StreetName + " " + addr.StreetNumber + ", " + addr.ZipCode + " " + addr.Town;
           });
           $scope.showAdministration = res.IsAdmin;
           $scope.showApproveReports = res.IsLeader || res.IsSubstitute;
           $scope.UserName = res.FullName;
           $scope.UserEmail = res.Mail;
       });

   }
]);