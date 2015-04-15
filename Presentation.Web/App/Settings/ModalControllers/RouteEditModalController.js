angular.module("application").controller('RouteEditModalInstanceController', [
    "$scope", "Route", "Point", "NotificationService", "$modalInstance", "routeId", "personId", "AddressFormatter", "SmartAdresseSource", function ($scope, Route, Point, NotificationService, $modalInstance, routeId, personId, AddressFormatter, SmartAdresseSource) {


        //Contains addresses as strings ex. "Road 1, 8220 Aarhus"
        $scope.viaPointModels = [];

        $scope.isSaveDisabled = false;


        if (routeId != undefined) {
            Route.getSingle({ id: routeId }, function (res) {
                $scope.newRouteDescription = res.Description;

                $scope.newStartPoint = res.Points[0].StreetName + " " + res.Points[0].StreetNumber + ", " + res.Points[0].ZipCode + " " + res.Points[0].Town;
                $scope.newEndPoint = res.Points[res.Points.length - 1].StreetName + " " + res.Points[res.Points.length - 1].StreetNumber + ", " + res.Points[res.Points.length - 1].ZipCode + " " + res.Points[res.Points.length - 1].Town;

                angular.forEach(res.Points, function (viaPoint, key) {
                    if (key != 0 && key != res.Points.length - 1) {
                        // If its not the first or last element -> Its a via point
                        var pointModel = viaPoint.StreetName + " " + viaPoint.StreetNumber + ", " + viaPoint.ZipCode + " " + viaPoint.Town;
                        $scope.viaPointModels.push(pointModel);
                    }
                });
            });
        }

        $scope.saveRoute = function () {
            $scope.isSaveDisabled = true;
            if (routeId != undefined) {
                // routeId is defined -> User is editing existing route ->  Delete it, and then post the edited route as a new route.
                Route.delete({ id: routeId }, function () {
                    handleSaveRoute();
                });
            } else {
                // routeId is undefined -> User is making a new route.
                handleSaveRoute();
            }

        }

        var handleSaveRoute = function () {
            // Validate start and end point
            if ($scope.newStartPoint == undefined || $scope.newStartPoint == "" || $scope.newEndPoint == undefined || $scope.newEndPoint == "") {
                NotificationService.AutoFadeNotification("danger", "", "Start- og slutadresse skal udfyldes.");
                $scope.isSaveDisabled = false;
                return;
            }

            // Validate description
            if ($scope.newRouteDescription == "" || $scope.newRouteDescription == undefined) {
                NotificationService.AutoFadeNotification("danger", "", "Beskrivelse må ikke være tom.");
                $scope.isSaveDisabled = false;
                return;
            }

            var points = [];

            var startAddress = AddressFormatter.fn($scope.newStartPoint);

            points.push({
                "StreetName": startAddress.StreetName,
                "StreetNumber": startAddress.StreetNumber,
                "ZipCode": startAddress.ZipCode,
                "Town": startAddress.Town,
                "Latitude": "",
                "Longitude": "",
                "Description": ""
            });
            angular.forEach($scope.viaPointModels, function (address, key) {
                var point = AddressFormatter.fn(address);

                points.push({
                    "StreetName": point.StreetName,
                    "StreetNumber": point.StreetNumber,
                    "ZipCode": point.ZipCode,
                    "Town": point.Town,
                    "Latitude": "",
                    "Longitude": "",
                    "Description": ""
                });
            });

            var endAddress = AddressFormatter.fn($scope.newEndPoint);

            points.push({
                "StreetName": endAddress.StreetName,
                "StreetNumber": endAddress.StreetNumber,
                "ZipCode": endAddress.ZipCode,
                "Town": endAddress.Town,
                "Latitude": "",
                "Longitude": "",
                "Description": ""
            });

            Route.post({
                "Description": $scope.newRouteDescription,
                "PersonId": personId,
                "Points": points
            }, function () {
                if (routeId != undefined) {
                    NotificationService.AutoFadeNotification("success", "", "Personlig rute blev redigeret.");
                } else {
                    NotificationService.AutoFadeNotification("success", "", "Personlig rute blev oprettet.");
                }
                $modalInstance.close();
            });
        }

        $scope.removeViaPoint = function ($index) {
           $scope.viaPointModels.splice($index, 1);
        }

        $scope.addNewViaPoint = function () {
            $scope.viaPointModels.push("");
        }

        $scope.closeRouteEditModal = function () {
            $modalInstance.dismiss();
        };

        $scope.SmartAddress = SmartAdresseSource;
    }]);