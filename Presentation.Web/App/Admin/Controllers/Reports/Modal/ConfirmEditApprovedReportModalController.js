angular.module("application").controller("ConfirmEditApprovedReportModalController", [
   "$scope", "$modalInstance", "DriveReport", "$rootScope", function ($scope, $modalInstance, DriveReport, $rootScope) {


   $scope.confirmClicked = function(){
        console.log("confirm");
   }

   $scope.cancelClicked = function(){
        console.log("cancel");
   }

   }
]);


