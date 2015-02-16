angular.module("application").service("AddressFormatter", ["Address"], function (Address) {
    return {
        fn: function (addressString) {
            var res = {
                StreetName: "",
                StreetNumber: "",
                ZipCode: 0,
                Town: "",
                Description: ""
            }

            var splittet = (addressString.split(","));
            var first = splittet[0].split(" ");

            for (i = 0; i < first.length - 1; i++) {
                res.StreetName += first[i];
                if (!(i + 1 == first.length - 1)) {
                    res.StreetName += " ";
                }
            }

            res.StreetNumber = first[first.length - 1];
            res.ZipCode = splittet[1].split(" ")[1];
            res.Town = splittet[1].split(" ")[2];

            return new {
                Id: null,
                PersonId: personId,
                StreetName: res.StreetName,
                StreetNumber: res.StreetNumber,
                ZipCode: parseInt(res.ZipCode),
                Town: res.Town,
                Description: null
            };
        }
    }
})