angular.module("application").controller('RouteEditModalInstanceController', [
    "$scope", "Route", "Point", "NotificationService", "$modalInstance", "routeId", "personId", "AddressFormatter", function ($scope, Route, Point, NotificationService, $modalInstance, routeId, personId, AddressFormatter) {
    $scope.newStartPoint = "";
    $scope.newEndPoint = "";
    $scope.oldStartPointId = 0;
    $scope.oldEndPointId = 0;
    $scope.oldStartPoint = "";
    $scope.oldEndPoint = "";
    $scope.oldRouteDescription = "";
    $scope.newRouteDescription = "";
    $scope.viaPoints = [];
    $scope.viaPointModels = [];
    $scope.updatedViaPointModels = [];

    //Properties for creating new route
    $scope.newRouteId = 0;
    $scope.newRouteStartPointId = 0;
    $scope.newRouteEndPointId = 0;

    $scope.getRouteInformation = function () {
        if (routeId == undefined) {
            return;
        }

        Route.get({ id: personId, query: "$filter=Id eq " + routeId }, function (data) {
            $scope.oldRouteDescription = data.value[0].Description;
            $scope.newRouteDescription = $scope.oldRouteDescription;
        }, function () {
            NotificationService.AutoFadeNotification("danger", "Fejl", "Kunne ikke hente ruteinformation");
        });

        $scope.routePoints = Point.get({ id: personId, query: "$filter=PersonalRouteId eq " + routeId }, function (data) {
            angular.forEach(data.value, function (value, key) {
                if (value.NextPointId == null) {
                    $scope.oldEndPointId = value.Id;
                    $scope.oldEndPoint = value.StreetName + " " + value.StreetNumber + ", " + value.ZipCode + " " + value.Town;
                    $scope.newEndPoint = $scope.oldEndPoint;
                }
                if (value.PreviousPointId == null) {
                    $scope.oldStartPointId = value.Id;
                    $scope.oldStartPoint = value.StreetName + " " + value.StreetNumber + ", " + value.ZipCode + " " + value.Town;
                    $scope.newStartPoint = $scope.oldStartPoint;
                }
                if (value.PreviousPointId != null && value.NextPointId != null) {
                    $scope.viaPoints.push(value);
                    $scope.viaPointModels.push(value.StreetName + " " + value.StreetNumber + ", " + value.ZipCode + " " + value.Town);
                    $scope.updatedViaPointModels.push(value.StreetName + " " + value.StreetNumber + ", " + value.ZipCode + " " + value.Town);
                }
            });
        }, function () {
            NotificationService.AutoFadeNotification("danger", "Fejl", "Kunne ikke hente ruteinformation");
        });
    }

    $scope.getRouteInformation();

    $scope.addNewViaPoint = function () {
        $scope.viaPoints.push({
            StreetName: "",
            StreetNumber: "",
            ZipCode: 0,
            Town: ""
        });
        $scope.viaPointModels.push("");
        $scope.updatedViaPointModels.push("");
    }

    $scope.removeViaPoint = function (index, viaPoint) {
        var obj = viaPoint;
        var next = {};
        var previous = {};

        if (obj.Id > 0) {
            Point.get({ query: "$filter=Id eq " + obj.NextPointId }, function (data) {
                next = data.value[0];
                console.log(next);

                next.PreviousPointId = obj.NextPointId;

                var editedNext = new Point({
                    PreviousPointId: obj.NextPointId,
                    PersonalRouteId: routeId
                });

                editedNext.$patch({ id: next.Id }, function () {
                    Point.get({ query: "$filter=Id eq " + obj.PreviousPointId }, function (data) {
                        previous = data.value[0];

                        previous.NextPointId = obj.PreviousPointId;

                        var editedPrevious = new Point({
                            NextPointId: obj.PreviousPointId,
                            PersonalRouteId: routeId
                        });

                        editedPrevious.$patch({ id: previous.Id }, function () {
                            Point.delete({ id: next.Id }, function () {
                                NotificationService.AutoFadeNotification("success", "Success", "Viapunkt fjernet");
                            });
                        }, function () {

                        });
                    }, function () {

                    });
                }, function () {
                    NotificationService.AutoFadeNotification("danger", "Fejl", "Kunne ikke fjerne viapunkt");
                });
            });
        }

        $scope.viaPoints.splice(obj, 1);
        $scope.viaPointModels.splice(index, 1);
        $scope.updatedViaPointModels.splice(index, 1);
    }

    $scope.saveViaPoints = function () {
        angular.forEach($scope.viaPoints, function (value, key) {
            if ($scope.viaPointModels[key] != $scope.updatedViaPointModels[key] && $scope.viaPointModels[key] == "") { // NEW VIAPOINT = CREATE IT
                var newViaPointAddress = AddressFormatter.fn($scope.updatedViaPointModels[key]);

                var newViaPoint = new Point({
                    StreetName: newViaPointAddress.StreetName,
                    StreetNumber: newViaPointAddress.StreetNumber,
                    ZipCode: parseInt(newViaPointAddress.ZipCode),
                    Town: newViaPointAddress.Town,
                    NextPointId: 0,
                    PreviousPointId: 0,
                    PersonalRouteId: routeId,
                    Latitude: "",
                    Longitude: ""
                });

                if (routeId == undefined) {
                    newViaPoint.PersonalRouteId = $scope.newRouteId;
                }

                var queryOne = "";
                var queryTwo = "";

                if (key == 0) { //First viapoint, get start address as previous point id
                    if ($scope.oldStartPointId != 0) {
                        queryOne = $scope.oldStartPointId;
                    } else {
                        queryOne = $scope.newRouteStartPointId;
                    }
                } else if ($scope.viaPoints.length > 1 && key != 0) { // Else take previous array item and set previous id to that
                    queryOne = $scope.viaPoints[key - 1].Id;
                }

                if ($scope.viaPoints.length == 1 || key == $scope.viaPoints.length - 1) { //Only one viapoint, set next point id to end address
                    if ($scope.oldStartPointId != 0) {
                        queryTwo = $scope.oldEndPointId;
                    } else {
                        queryTwo = $scope.newRouteEndPointId;
                    }
                } else if (key > 0 || key < $scope.viaPoints.length - 1) {
                    queryTwo = $scope.viaPoints[key + 1].Id;
                }

                Point.get({ id: personId, query: "$filter=Id eq " + queryOne }, function (data1) {
                    newViaPoint.PreviousPointId = data1.value[0].Id;

                    Point.get({ id: personId, query: "$filter=Id eq " + queryTwo }, function (data2) {
                        newViaPoint.NextPointId = data2.value[0].Id;

                        var id = routeId;

                        if (routeId == undefined) {
                            id = $scope.newRouteId;
                        }

                        newViaPoint.$post(function (data3) {
                            var previous = new Point({
                                NextPointId: data3.value[0].Id,
                                PersonalRouteId: id
                            });

                            var next = new Point({
                                PreviousPointId: data3.value[0].Id,
                                PersonalRouteId: id
                            });

                            previous.$patch({ id: data3.value[0].PreviousPointId }, function () {
                                next.$patch({ id: data3.value[0].NextPointId }, function () {
                                    NotificationService.AutoFadeNotification("success", "Success", "Viapunkt oprettet");
                                });
                            });

                        });
                    });
                });
            } else if ($scope.viaPointModels[key] != $scope.updatedViaPointModels[key] && $scope.viaPointModels[key] != "") { // EXISTING VIAPOINT = UPDATE IT
                var formatedAddress = AddressFormatter.fn($scope.updatedViaPointModels[key]);

                var existingViaPoint = new Point({
                    StreetName: formatedAddress.StreetName,
                    StreetNumber: formatedAddress.StreetNumber,
                    ZipCode: parseInt(formatedAddress.ZipCode),
                    Town: formatedAddress.Town,
                    PersonalRouteId: routeId
                });

                existingViaPoint.$patch({ id: $scope.viaPoints[key].Id }, function () {
                    NotificationService.AutoFadeNotification("success", "Success", "Viapunkt opdateret");
                });
            }
        });
    }

    $scope.saveRoute = function () {
        if ($scope.newEndPoint == "" || $scope.newStartPoint == "") {
            NotificationService.AutoFadeNotification("danger", "Fejl", "Startadresse og/eller slutadresse må ikke være tom");
            return;
        }

        if ($scope.newRouteDescription == "") {
            NotificationService.AutoFadeNotification("danger", "Fejl", "Beskrivelse må ikke være tom");
            return;
        }

        $scope.handleRouteOnSave();

        
    }

    $scope.handleRouteOnSave = function () {
        $scope.handleDescriptionOnSave();
    }

    //Handle description  
    $scope.handleDescriptionOnSave = function () {
        if ($scope.newRouteDescription != $scope.oldRouteDescription && $scope.oldRouteDescription != "") {

            var route = new Route({
                Description: $scope.newRouteDescription,
                PersonId: personId
            });

            route.$patch({ id: routeId }, function () {
                $scope.handleStartPointOnSave();
                NotificationService.AutoFadeNotification("success", "Success", "Rutebeskrivelse opdateret");
            }, function () {
                NotificationService.AutoFadeNotification("danger", "Fejl", "Rutebeskrivelse blev ikke opdateret");
            });
        } else if ($scope.newRouteDescription != $scope.oldRouteDescription && $scope.oldRouteDescription == "") {
            var newRoute = new Route({
                Description: $scope.newRouteDescription,
                PersonId: personId
            });

            newRoute.$post(function (data) {
                $scope.newRouteId = data.Id;
                $scope.handleStartPointOnSave();
                NotificationService.AutoFadeNotification("success", "Success", "Ny rute oprettet");
            }, function () {
                NotificationService.AutoFadeNotification("danger", "Fejl", "Kunne ikke oprette ny rute");
                return;
            });
        } else {
            $scope.handleStartPointOnSave();            
        }
    }

    //Handle start address
    $scope.handleStartPointOnSave = function () {
        if ($scope.newStartPoint != $scope.oldStartPoint && $scope.oldStartPoint != "") { //IF EXISTING

            var result1 = AddressFormatter.fn($scope.newStartPoint);

            result1.Id = $scope.oldStartPointId;
            result1.PersonId = personId;

            var startPoint = new Point({
                Id: result1.Id,
                PersonalRouteId: routeId,
                StreetName: result1.StreetName,
                StreetNumber: result1.StreetNumber,
                ZipCode: parseInt(result1.ZipCode),
                Town: result1.Town
            });

            startPoint.$patch({ id: result1.Id }, function (data) {
                $scope.handleEndpointOnSave();
                NotificationService.AutoFadeNotification("success", "Success", "Startadresse opdateret");
            }, function () {
                NotificationService.AutoFadeNotification("danger", "Fejl", "Startadresse blev ikke opdateret");
            });

        } else if ($scope.newStartPoint != $scope.oldStartPoint && $scope.oldStartPoint == "") { //IF NEW
            var result3 = AddressFormatter.fn($scope.newStartPoint);

            var newStartPoint = new Point({
                PersonalRouteId: $scope.newRouteId,
                StreetName: result3.StreetName,
                StreetNumber: result3.StreetNumber,
                ZipCode: parseInt(result3.ZipCode),
                Town: result3.Town,
                Latitude: "",
                Longitude: ""
            });

            newStartPoint.$post(function (data) {
                $scope.newRouteStartPointId = data.Id;
                $scope.handleEndpointOnSave();
                NotificationService.AutoFadeNotification("success", "Success", "Startadresse til ny rute oprettet");
            }, function () {
                NotificationService.AutoFadeNotification("danger", "Fejl", "Startadresse blev ikke opdateret");
                return;
            });

        } else { // IF NO CHANGES
            $scope.handleEndpointOnSave();
        }
    }

    //Handle end address
    $scope.handleEndpointOnSave = function () {
        if ($scope.newEndPoint != $scope.oldEndPoint && $scope.oldEndPoint != "") { // IF EXISTING

            var result2 = AddressFormatter.fn($scope.newEndPoint);

            result2.Id = $scope.oldEndPointId;
            result2.PersonId = personId;

            var endPoint = new Point({
                Id: result2.Id,
                PersonalRouteId: routeId,
                StreetName: result2.StreetName,
                StreetNumber: result2.StreetNumber,
                ZipCode: parseInt(result2.ZipCode),
                Town: result2.Town
            });

            endPoint.$patch({ id: result2.Id }, function () {
                NotificationService.AutoFadeNotification("success", "Success", "Slutadresse opdateret");
            }, function () {
                NotificationService.AutoFadeNotification("danger", "Fejl", "Slutadresse blev ikke opdateret");
            });
        } else if ($scope.newEndPoint != $scope.oldEndPoint && $scope.oldEndPoint == "") { // IF NEW
            var result4 = AddressFormatter.fn($scope.newEndPoint);

            var newEndPoint = new Point({
                PersonalRouteId: $scope.newRouteId,
                StreetName: result4.StreetName,
                StreetNumber: result4.StreetNumber,
                ZipCode: parseInt(result4.ZipCode),
                Town: result4.Town,
                PreviousPointId: parseInt($scope.newRouteStartPointId),
                Latitude: "",
                Longitude: ""
            });

            newEndPoint.$post(function (data1) {
                $scope.newRouteEndPointId = data1.Id;

                //PATCH START ADDRESS FOR NEW ROUTE
                Point.get({ id: personId, query: "$filter=Id eq " + $scope.newRouteStartPointId }, function (data2) {
                    var patchedStartPoint = new Point({
                        NextPointId: data1.value[0].Id,
                        PersonalRouteId: data1.PersonalRouteId
                    });

                    patchedStartPoint.$patch({ id: data2.Id }, function() {
                        //Handle via points
                        $scope.saveViaPoints();
                    });
                });

                NotificationService.AutoFadeNotification("success", "Success", "Slutadresse til ny rute oprettet");
            }, function () {
                NotificationService.AutoFadeNotification("danger", "Fejl", "Slutadresse blev ikke opdateret");
                return;
            });

        } else { // IF NO CHANGES
            $scope.saveViaPoints();
        }
    }

    $scope.closeRouteEditModal = function () {
        $modalInstance.close({

        });
    };

    $scope.SmartAddress = {
        type: "json",
        minLength: 3,
        serverFiltering: true,
        crossDomain: true,
        transport: {
            read: {
                url: function (item) {
                    return 'https://smartadresse.dk/service/locations/3/detect/json/' + item.filter.filters[0].value + '%200';
                },
                dataType: "jsonp",
                data: {
                    apikey: 'FCF3FC50-C9F6-4D89-9D7E-6E3706C1A0BD',
                    limit: 15,                   // REST limit
                    crs: 'EPSG:25832',           // REST projection
                    nogeo: 'true',                 // REST nogeo
                    noadrspec: 'true'             // REST noadrspec
                }
            }
        },
        schema: {
            data: function (data) {
                return data.data; // <-- The result is just the data, it doesn't need to be unpacked.
            }
        },
    }
}]);