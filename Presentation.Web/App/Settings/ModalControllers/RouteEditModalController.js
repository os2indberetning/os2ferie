angular.module("application").controller('RouteEditModalController', [
    "$scope", "Route", "Point", "NotificationService", "$modalInstance", "routeId", "personId", "AddressFormatter", function ($scope, Route, Point, NotificationService, $modalInstance, routeId, personId, AddressFormatter) {
    $scope.newStartPoint = "";
    $scope.newEndPoint = "";
    $scope.oldStartPointId = 0;
    $scope.oldEndPointId = 0;
    $scope.oldStartPoint = "";
    $scope.oldEndPoint = "";
    $scope.routeDescription = Route.get({ id: routeId }, function(data) {

    }, function() {

    });

    $scope.viaPoints = [];

    $scope.routePoints = Point.get({ id: personId, query: "$filter=PersonalRouteId eq " + routeId }, function(data) {
        angular.forEach(data.value, function(value, key) {
            if (value.NextPointId == null) {
                $scope.oldEndPointId = value.Id;
                $scope.oldEndPoint = value.StreetName + " " + value.StreetNumber + ", " + value.ZipCode + " " + value.Town;
            }
            if (value.PreviousPointId == null) {
                $scope.oldStartPointId = value.Id;
                $scope.oldStartPoint = value.StreetName + " " + value.StreetNumber + ", " + value.ZipCode + " " + value.Town;
            } else {
                $scope.viaPoints.push(value);
            }
        });
    }, function() {
        NotificationService.AutoFadeNotification("danger", "Fejl", "Kunne ikke hente ruteinformation");
    });

    $scope.saveUpdatedRoute = function () {
        $scope.newStartPoint = $scope.oldStartPoint;

        var result1 = AddressFormatter.fn($scope.newStartPoint);

        result1.Id = $scope.oldStartPointId;
        result1.PersonId = personId;        

        var startPoint = new Point({
            Id: result1.Id,
            PersonalRouteId: routeId,
            StreetName: result1.StreetName,
            StreetNumber: result1.StreetNumber,
            ZipCode: result1.ZipCode,
            Town: result1.Town
        });

        startPoint.$patch({ id: result1.Id }, function () {
            NotificationService.AutoFadeNotification("success", "Success", "Startadresse opdateret");
        }, function () {
            NotificationService.AutoFadeNotification("danger", "Fejl", "Startadresse blev ikke opdateret");
        });

        $scope.newEndPoint = $scope.oldEndPoint;

        var result2 = AddressFormatter.fn($scope.newEndPoint);

        result2.Id = $scope.oldEndPointId;
        result2.PersonId = personId;

        var endPoint = new Point({
            Id: result2.Id,
            PersonalRouteId: routeId,
            StreetName: result2.StreetName,
            StreetNumber: result2.StreetNumber,
            ZipCode: result2.ZipCode,
            Town: result2.Town
        });

        endPoint.$patch({ id: result2.Id }, function () {
            NotificationService.AutoFadeNotification("success", "Success", "Slutadresse opdateret");
        }, function () {
            NotificationService.AutoFadeNotification("danger", "Fejl", "Slutadresse blev ikke opdateret");
        });
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