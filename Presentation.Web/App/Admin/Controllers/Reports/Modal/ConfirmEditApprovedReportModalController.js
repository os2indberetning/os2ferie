angular.module("application").controller("ConfirmEditApprovedReportModalController", [
   "$scope", "$modalInstance", "DriveReport", "$rootScope", function ($scope, $modalInstance, DriveReport, $rootScope) {


   $scope.confirmClicked = function(){
        $modalInstance.close($scope.emailText);
   }

   $scope.cancelClicked = function(){
        $modalInstance.dismiss();
   }

   }
]);


