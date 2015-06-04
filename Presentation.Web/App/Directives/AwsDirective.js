// Is not used.
angular.module("application").directive("awsfield", function () {
    return {
        restrict: 'AE',
        link: function(scope, element, attrs) {

            var optionsOrig = {
                'apikey': 'FCF3FC50-C9F6-4D89-9D7E-6E3706C1A0BD',
                'resource': 'addressaccess',
                'select' : function(data) {

                    scope.model = data.tekst;
                }
            };

            
            angular.element(element).spatialfind(optionsOrig);
        }
    };
});