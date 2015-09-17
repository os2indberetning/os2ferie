angular.module("application").service("PersonAutocomplete", function () {
    return {
        type: "json",
        minLength: 3,
        serverFiltering: true,
        crossDomain: true,
        transport: {
            read: {
                url: function (item) {
                    var req = "/odata/Person?$filter=contains(FullName,'" + item.filter.filters[0].value + "')";
                    return req;
                },
                dataType: "jsonp",
                data: {
                   
                }
            }
        },
        schema: {
            data: function (data) {
                return data; // <-- The result is just the data, it doesn't need to be unpacked.
            }
        },
    }
});