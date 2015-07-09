angular.module("application").controller("MainMenuController", [
   "$scope", "Person", "PersonalAddress", "HelpText", "$rootScope", function ($scope, Person, PersonalAddress, HelpText, $rootScope) {


       HelpText.getAll().$promise.then(function (res) {
           $scope.helpLink = res.InformationHelpLink;
           $rootScope.HelpTexts = res;
       });

       if ($rootScope.CurrentUser == undefined) {
           $rootScope.CurrentUser = Person.GetCurrentUser().$promise.then(function (res) {
               $rootScope.CurrentUser = res;
               $scope.showAdministration = res.IsAdmin;
               $scope.showApproveReports = res.IsLeader || res.IsSubstitute;
               $scope.UserName = res.FullName;
           });
       }


       var reloadPageIfNewCacheIsAvailable = function() {
           // Check if a new cache is available on page load.
           window.addEventListener('load', function (e) {

               window.applicationCache.addEventListener('updateready', function (e) {
                   if (window.applicationCache.status == window.applicationCache.UPDATEREADY) {
                       // Browser downloaded a new app cache. Refresh the page to load the new cache.
                           window.location.reload();
                   } else {
                       // Manifest didn't change. Nothing new to server.
                   }
               }, false);

           }, false);
       }

        reloadPageIfNewCacheIsAvailable();

    }
]);