angular.module("application").controller("ConfirmRejectApprovedReportModalController", [
   "$scope", "$modalInstance", "DriveReport", "$rootScope", function ($scope, $modalInstance, DriveReport, $rootScope) {


   $scope.confirmClicked = function(){
        console.log("confirm");
        $modalInstance.close($scope.emailText);
   }

   $scope.cancelClicked = function(){
        console.log("cancel");
        $modalInstance.dismiss();
   }

   }
]);


