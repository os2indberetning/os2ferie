angular.module("application").factory("AddressFormatter", ["Address", function (Address) {
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
            if (splittet.length != 2) {
                if (splittet.length == 3) {
                    splittet = [splittet[0], splittet[2]];
                }
                else if (splittet.length == 4) {
                    splittet = [splittet[0] + "," + splittet[1], splittet[3]];
                } else {
                    return undefined;
                }
            }

            var first = splittet[0].split(" ");

            if (first.length < 2) {
                return undefined;
            }

            for (var i = 0; i < first.length - 1; i++) {
                res.StreetName += first[i];
                if (!(i + 1 == first.length - 1)) {
                    res.StreetName += " ";
                }
            }

            res.StreetNumber = first[first.length - 1];


            var second = splittet[1].split(" ");

            if (second.length < 3) {
                return undefined;
            }

            res.ZipCode = second[1];


            for (var a = 2; a < second.length; a++) {
                res.Town += second[a];
                if (!(a + 1 == second.length)) {
                    res.Town += " ";
                }
            }

            return {
                Id: null,
                PersonId: 0,
                StreetName: res.StreetName,
                StreetNumber: res.StreetNumber,
                ZipCode: parseInt(res.ZipCode),
                Town: res.Town,
                Description: null
            };
        }
    }
}])