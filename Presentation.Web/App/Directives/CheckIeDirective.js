(function () {
    'use strict';

    angular
        .module('checkie', [])
        .directive('checkie', ["$modal",
            function ($modal) {
                return {
                    restrict: 'AE',
                    replace: true,
                    scope: {
                        checkieMinIe: '@',
                        checkieMessage: '@',
                        checkieDebug: '@'
                    },
                    transclude: true,
                    template: '<div data-ng-class="{outdated: outdated}"><div data-ng-if="outdated === false" ng-transclude></div></div>',
                    link: link
                };

                function link($scope, elem, attr) {
                    var ieversion = ($scope.checkieDebug) ? $scope.checkieDebug : _getExplorerVersion();

                    // set the minimum ie to 9 if no value was passed from the attributes
                    $scope.checkieMinIe = ($scope.checkieMinIe && $scope.checkieMinIe.length) ? $scope.checkieMinIe : 9;

                    // determine if the browser is an outdated version of IE
                    $scope.outdated = (ieversion > -1 && ieversion < parseInt($scope.checkieMinIe));

                    // reset the message to blank by default
                    $scope.msg = "";

                    // check if the browser is any version of IE, and if so, if it is below the minimum version
                    if ($scope.outdated) {

                        // set the message to a generic message or the passed message via the checkie-message attribute
                        $scope.msg = ($scope.checkieMessage && $scope.checkieMessage.length > 0) ? $scope.checkieMessage : "Browseren kan ikke anvendes til OS2Indberetning. Din version af browseren Internet Explorer er for gammel. Du bedes opdatere Internet Explorer til version 11, alternativt bruge browserne Chrome eller Firefox.";

                        var modalInstance = $modal.open({
                            templateUrl: '/App/Services/Error/ServiceError.html',
                            controller: "ServiceErrorController",
                            backdrop: "static",
                            resolve: {
                                errorMsg: function () {
                                    return $scope.msg;
                                }
                            }
                        });
                    }
                }

                /**
                 * Private function to determine the exporer version
                 * @return {Number} The explorer version number, or -1 if it is not Internet Explorer
                 */
                function _getExplorerVersion() {
                    var rv = -1;

                    if (navigator.appName === 'Microsoft Internet Explorer') {
                        var ua = navigator.userAgent,
                            re = new RegExp("MSIE ([0-9]{1,}[.0-9]{0,})");

                        if (re.exec(ua) !== null) rv = parseFloat(RegExp.$1);
                    }

                    return rv;
                }
            }]);
})();
