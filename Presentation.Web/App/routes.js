angular.module('app').config(["$stateProvider", "$urlRouterProvider", function ($stateProvider, $urlRouterProvider) {
    
    $stateProvider.
        state('default', {
            url: '/',
            templateUrl: '/App/app.html'
        }).
        state('drive', {
            url: '/drive',
            templateUrl: '/App/Drive/app.drive.html'
        }).
        state('vacation', {
            url: '/vacation',
            template: '<div>Vacation has not yet been implemented</div>'
        });
        
        $urlRouterProvider.otherwise('/');
}]);