angular.module("application").controller("MainMenuController", [
   "$scope", "Person", "PersonalAddress", "HelpText", "$rootScope", "OrgUnit", "Person", function ($scope, Person, PersonalAddress, HelpText, $rootScope, OrgUnit, Person) {


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

        if ($rootScope.OrgUnits == undefined) {
            $rootScope.OrgUnits = OrgUnit.get({ query: "$select=Id, LongDescription, HasAccessToFourKmRule" }).$promise.then(function(res) {
                $rootScope.OrgUnits = res.value;
            });
        }

        if ($rootScope.People == undefined) {
            $rootScope.People = Person.getAll({ query: "$select=Id,FullName,IsActive" }).$promise.then(function (res) {
                $rootScope.People = res.value;
            });
        }


    }
]);