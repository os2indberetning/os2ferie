// ng-enter in html to attach eventhandler to clicking enter.
angular.module("app.core").directive('ngEnter', function () {
    return function (scope, element, attrs) {
        element.bind("keydown keypress", function (event) {
            if (event.which === 13) {
                scope.$apply(function () {
                    
                    scope.$eval(attrs.ngEnter);
                });

                event.preventDefault();
            }
        });
    };
});