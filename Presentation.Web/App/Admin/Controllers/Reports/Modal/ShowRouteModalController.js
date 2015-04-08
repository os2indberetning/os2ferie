angular.module("application").controller("ShowRouteModalController", [
   "$scope", "$modalInstance", "routeId", "DriveReport", function ($scope, $modalInstance, routeId, DriveReport) {



       var route = DriveReport.getWithPoints({ id: routeId }, function (res) {
           OS2RouteMap.create({
               id: 'map',
           });

           var points = [];

           angular.forEach(route.DriveReportPoints, function (point, key) {
               points.push({
                   name: point.StreetName + " " + point.StreetNumber + ", " + point.ZipCode + " " + point.Town,
                   lat: point.Latitude,
                   lng: point.Longitude
               });
           });

           OS2RouteMap.set(points);
       });

       $scope.closeClicked = function () {
           $modalInstance.dismiss('cancel');
       }

   }
]);