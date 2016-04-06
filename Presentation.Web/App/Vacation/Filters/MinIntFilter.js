angular.module("app.vacation").filter("minint", function() {
    return function(input, value = 0) {
        return input > value ? input : value;
    };
});