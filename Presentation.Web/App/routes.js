angular.module('app').config(function ($stateProvider, $urlRouterProvider) {
    $stateProvider.
        state('default', {
            url: '/',
            template: '<div>Her goes the tile view</div>'
        }).
        state('/drive', {
            url: '/drive',
            template: '<div ui-view></div>'
        });
        
        $urlRouterProvider.otherwise('/');
})