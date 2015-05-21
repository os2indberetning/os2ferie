angular.module("application").controller("ShowRouteModalController", [
   "$scope", "$modalInstance", "routeId", "DriveReport", function ($scope, $modalInstance, routeId, DriveReport) {



       var route = DriveReport.getWithPoints({ id: routeId }, function (res) {
           OS2RouteMap.create({
               id: 'map',
           });

           OS2RouteMap.viewRoute(res.RouteGeometry, true);
       });

       $scope.closeClicked = function () {
           $modalInstance.dismiss('cancel');
       }

   }
]);


