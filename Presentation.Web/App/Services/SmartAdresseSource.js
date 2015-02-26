angular.module("application").service("SmartAdresseSource", function () {
    return {
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
});