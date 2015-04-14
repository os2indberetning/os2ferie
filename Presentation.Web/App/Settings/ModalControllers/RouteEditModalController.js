angular.module("application").controller('RouteEditModalInstanceController', [
    "$scope", "Route", "Point", "NotificationService", "$modalInstance", "routeId", "personId", "AddressFormatter", "SmartAdresseSource", function ($scope, Route, Point, NotificationService, $modalInstance, routeId, personId, AddressFormatter, SmartAdresseSource) {

        $scope.viaPointModels = [];
        $scope.viaPoints = [];

        $scope.saveRoute = function () {

            // Validate start and end point
            if ($scope.newStartPoint == undefined || $scope.newStartPoint == "" || $scope.newEndPoint == undefined || $scope.newEndPoint == "") {
                NotificationService.AutoFadeNotification("danger", "Fejl", "Start- og slutadresse skal udfyldes.");
                return;
            }

            // Validate description
            if ($scope.newRouteDescription == "" || $scope.newRouteDescription == undefined) {
                NotificationService.AutoFadeNotification("danger", "Fejl", "Beskrivelse må ikke være tom.");
                return;
            }

            Route.post({
                "Description": $scope.newRouteDescription,
                "PersonId": personId
            }, function (routeData) {
                $scope.newRouteId = routeData.Id;

                // Save start point

                var startPoint = AddressFormatter.fn($scope.newStartPoint);

                Point.post({
                    "PersonalRouteId": $scope.newRouteId,
                    "StreetName": startPoint.StreetName,
                    "StreetNumber": startPoint.StreetNumber,
                    "ZipCode": startPoint.ZipCode,
                    "Town": startPoint.Town,
                    "Latitude": "",
                    "Longitude": ""
                }, function (startPointData) {
                    // Save end point

                    var endPoint = AddressFormatter.fn($scope.newEndPoint);

                    Point.post({
                        "PersonalRouteId": $scope.newRouteId,
                        "PreviousPointId": startPointData.Id,
                        "StreetName": endPoint.StreetName,
                        "StreetNumber": endPoint.StreetNumber,
                        "ZipCode": endPoint.ZipCode,
                        "Town": endPoint.Town,
                        "Latitude": "",
                        "Longitude": ""
                    }, function (endPointData) {
                        Point.patch({ id: startPointData.Id }, {
                            "NextPointId": endPointData.Id
                        }, function () {
                            if ($scope.viaPointModels.length == 0) {
                                // No viapoints -> We are done.
                                NotificationService.AutoFadeNotification("success", "", "Ny personlig rute blev oprettet.");
                                $modalInstance.close();
                            } else {
                                // Viapoints exist -> They need to be added.

                                var promise;

                                angular.forEach($scope.viaPointModels, function (viaPoint, key) {
                                    var viaAddress = AddressFormatter.fn(viaPoint);
                                    if (!promise) {
                                       promise = Point.post({
                                            "PersonalRouteId": $scope.newRouteId,
                                            "PreviousPointId": startPointData.Id,
                                            "StreetName": viaAddress.StreetName,
                                            "StreetNumber": viaAddress.StreetNumber,
                                            "ZipCode": viaAddress.ZipCode,
                                            "Town": viaAddress.Town,
                                            "Latitude": "",
                                            "Longitude": ""
                                        }, function(res) {
                                            if (key == $scope.viaPointModels.length - 1) {
                                                //Last point reached -> Update endpoint
                                                Point.patch({ id: endPointData.Id }, {
                                                    "PreviousPointId": res.Id
                                                });
                                            }
                                            if (key == 0) {
                                                //First point reached -> Update startpoint
                                                Point.patch({ id: startPointData.Id }, {
                                                    "NextPointId": res.Id
                                                });
                                            }

                                       }).$promise;
                                    } else {
                                        promise = promise.then(function(result) {
                                            return Point.post({
                                                "PersonalRouteId": $scope.newRouteId,
                                                "PreviousPointId": result.Id,
                                                "StreetName": viaAddress.StreetName,
                                                "StreetNumber": viaAddress.StreetNumber,
                                                "ZipCode": viaAddress.ZipCode,
                                                "Town": viaAddress.Town,
                                                "Latitude": "",
                                                "Longitude": ""
                                            }, function(res) {
                                                if (key == $scope.viaPointModels.length - 1) {
                                                    //Last point reached -> Update endpoint
                                                    Point.patch({ id: endPointData.Id }, {
                                                        "PreviousPointId": res.Id
                                                    });
                                                }
                                                if (key == 0) {
                                                    //First point reached -> Update startpoint
                                                    Point.patch({ id: startPointData.Id }, {
                                                        "NextPointId": res.Id
                                                    });
                                                }
                                            }).$promise;
                                        });
                                    }
                                    
                                });
                            }
                        });
                    });
                });

            });

        }

        $scope.addNewViaPoint = function () {
            $scope.viaPoints.push($scope.viaPoints.length);
        }

        $scope.closeRouteEditModal = function () {
            $modalInstance.dismiss();
        };

        $scope.SmartAddress = SmartAdresseSource;
    }]);