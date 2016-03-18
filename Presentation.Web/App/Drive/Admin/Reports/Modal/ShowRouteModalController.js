angular.module("app.drive").controller("ShowRouteModalController", [
   "$scope", "$modalInstance", "routeId", "DriveReport", "$rootScope", function ($scope, $modalInstance, routeId, DriveReport, $rootScope) {



       var route = DriveReport.getWithPoints({ id: routeId }, function (res) {
           OS2RouteMap.create({
               id: 'map',
               routeToken: $rootScope.HelpTexts.SEPTIMA_API_KEY.text,
           });

           OS2RouteMap.viewRoute(res.RouteGeometry, true);
       });

       $scope.closeClicked = function () {
           $modalInstance.dismiss('cancel');
       }

   }
]);


