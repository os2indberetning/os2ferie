module app {
    "use strict";

    export class routes {
        static $inject = ["$stateProvider", "$urlRouterProvider"];

        static init($stateProvider: ng.ui.IStateProvider, $urlRouterProvider: ng.ui.IUrlRouterProvider) {

            $stateProvider.
                state('default', {
                    url: '/',
                    templateUrl: '/App/app.html',
                    resolve: {
                        authorize: ['Authorization',
                            Authorization => Authorization.authorize()
                        ]
                    }
                });

            $stateProvider.state("drive", {
                url: "/drive",
                templateUrl: "/App/Drive/app.drive.html",
                abstract: true,
                resolve: {
                    authorize: ['Authorization',
                        Authorization => Authorization.authorize()
                    ]
                }
            } as ng.ui.IState);

            $stateProvider.state("vacation", {
                url: "/vacation",
                templateUrl: "/App/Vacation/app.vacation.html",
                abstract: true,
                resolve: {
                    authorize: ['Authorization',
                        Authorization => Authorization.authorize()
                    ]
                }
            } as ng.ui.IState);

            $urlRouterProvider.when("/drive", "/drive/driving");
            $urlRouterProvider.when("/vacation", "/vacation/report");
            $urlRouterProvider.otherwise("/");

            $urlRouterProvider.rule(($injector, $location) => {

                var path = $location.path();
                var hasTrailingSlash = path[path.length - 1] === "/";

                if (hasTrailingSlash) {
                    var newPath = path.substr(0, path.length - 1);
                    return newPath;
                }
                return path;
            });
        }
    }

    angular.module("app").config(routes.init);
}
