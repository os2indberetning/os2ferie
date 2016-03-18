angular.module("app.drive").controller("RemoveAdminModalController", [
   "$scope", "Id", "FullName", "$modalInstance",
   function ($scope, Id, FullName, $modalInstance) {

       $scope.name = FullName;

        var resPerson = { Id: Id, FullName: FullName };

        $scope.confirmRemoveAdmin = function () {
            /// <summary>
            /// Confirm remove admin
            /// </summary>
           $modalInstance.close(resPerson);
       }

        $scope.cancelRemoveAdmin = function () {
            /// <summary>
            /// Cancel remove admin
            /// </summary>
           $modalInstance.dismiss('cancel');
       }

   }
]);