angular.module("application").filter('FormatKmNumber', function () {
    return function (input) {
        return (+(Math.round(+(Number(input) + 'e' + 2)) + 'e' + -2)).toFixed(2);
    };
}).filter('FormatKmNumberString', function () {
    return function (input) {
        return (+(Math.round(+(Number(input) + 'e' + 2)) + 'e' + -2)).toFixed(2).toString().replace(".", ",");
    };
})