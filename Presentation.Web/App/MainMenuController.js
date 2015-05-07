angular.module("application").controller("MainMenuController", [
   "$scope", "Person", "PersonalAddress", function ($scope, Person, PersonalAddress) {

       Person.GetCurrentUser().$promise.then(function (res) {
           PersonalAddress.GetHomeForUser({ id: res.Id }).$promise.then(function (addr) {
               $scope.HomeAddress = addr.StreetName + " " + addr.StreetNumber + ", " + addr.ZipCode + " " + addr.Town;
           });
           $scope.showAdministration = res.IsAdmin;
           $scope.showApproveReports = res.IsLeader;
           $scope.UserName = res.FullName;
           $scope.UserEmail = res.Mail;
       });

   }
]);