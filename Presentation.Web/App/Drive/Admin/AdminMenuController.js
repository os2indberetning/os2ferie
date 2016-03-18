angular.module("app.drive").controller("AdminMenuController", [
   "$scope", "Person", "PersonalAddress", "HelpText", "$rootScope", function ($scope, Person, PersonalAddress, HelpText, $rootScope) {


       $scope.emailClicked = function () {
           $scope.$broadcast('emailClicked');
       }

       $scope.ratesClicked = function () {
           $scope.$broadcast('ratesClicked');
       }

       $scope.accountClicked = function () {
           $scope.$broadcast('accountClicked');
       }

       $scope.orgSettingsClicked = function () {
           $scope.$broadcast('orgSettingsClicked');
       }

       $scope.adminClicked = function () {
           $scope.$broadcast('administrationClicked');
       }

       $scope.reportsClicked = function () {
           $scope.$broadcast('reportsClicked');
       }

       $scope.laundryClicked = function () {
           $scope.$broadcast('addressLaundryClicked');
       }




   }
]);