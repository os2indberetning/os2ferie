angular.module("application").controller("NoLicensePlateModalController", [
   "$scope", "$modalInstance","$rootScope",
   function ($scope, $modalInstance, $rootScope) {

       $scope.ok = function () {
           $modalInstance.close();
       }

       $scope.cancel = function () {
           if ($rootScope.editModalInstance != undefined) {
               // Close the report edit window. 
               //$rootScope.editModalInstance is set in MyPendingReportsController or AdminAcceptedReportsController when clicking edit.
               $rootScope.editModalInstance.dismiss();
           }
           $modalInstance.dismiss('cancel');
       }

   }
]);