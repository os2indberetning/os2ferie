angular.module("application").controller('AddressEditModalInstanceController', ["$scope", "$modalInstance", "Address", "personId", "addressId", "NotificationService", "AddressFormatter", function ($scope, $modalInstance, Address, personId, addressId, NotificationService, AddressFormatter) {
    $scope.newAddress = "";
    $scope.oldAddressId = 0;
    $scope.oldAddress = "";
    $scope.addressDescription = "";

    Address.get({ id: personId, query: "$filter=Id eq " + addressId }, function (data) {
        $scope.oldAddressId = data.value[0].Id;
        $scope.addressDescription = data.value[0].Description;
        $scope.oldAddress = data.value[0].StreetName + " " + data.value[0].StreetNumber + ", " + data.value[0].ZipCode + " " + data.value[0].Town;
    });

    $scope.saveEditedAddress = function () {
        $scope.newAddress = $scope.oldAddress;

        var result = AddressFormatter($scope.newAddress);

        result.Id = $scope.oldAddressId;

        result.Description = $scope.addressDescription;

        //var res = {
        //    Id: $scope.oldAddressId,
        //    StreetName: "",
        //    StreetNumber: "",
        //    ZipCode: 0,
        //    Town: "",
        //    Description: ""
        //}

        //var splittet = ($scope.newAddress.split(","));
        //var first = splittet[0].split(" ");
        
        //for (i = 0; i < first.length - 1; i++) {
        //    res.StreetName += first[i];
        //    if (!(i + 1 == first.length - 1)) {
        //        res.StreetName += " ";
        //    }
        //}

        //res.StreetNumber = first[first.length - 1];
        //res.ZipCode = splittet[1].split(" ")[1];
        //res.Town = splittet[1].split(" ")[2];
        
        //var address = new Address({
        //    PersonId: personId,
        //    StreetName: res.StreetName,
        //    StreetNumber: res.StreetNumber,
        //    ZipCode: parseInt(res.ZipCode),
        //    Town: res.Town,
        //    Description: $scope.addressDescription
        //});

        result.$patch({ id: result.Id }, function() {
            NotificationService.AutoFadeNotification("success", "Success", "Adresse opdateret");
        }, function() {
            NotificationService.AutoFadeNotification("danger", "Fejl", "Adresse blev ikke opdateret");
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

    $scope.closeAddressEditModal = function () {
        $modalInstance.close({

        });
    };
}]);