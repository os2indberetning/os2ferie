angular.module("application").controller("AdminMenuController", [
   "$scope", "Person", "PersonalAddress", "HelpText", "$rootScope", function ($scope, Person, PersonalAddress, HelpText, $rootScope) {


       $scope.adminClicked = function () {
           $scope.$broadcast('administrationClicked');
       }

       $scope.emailClicked = function () {
           $scope.$broadcast('emailClicked');
       }

       $scope.ratesClicked = function () {
           $scope.$broadcast('ratesClicked');
       }

       $scope.accountClicked = function () {
           $scope.$broadcast('accountClicked');
       }
       
       $scope.fourKmClicked = function () {
           $scope.$broadcast('4kmClicked');
       }

       $scope.reportsClicked = function () {
           $scope.$broadcast('reportsClicked');
       }




   }
]);