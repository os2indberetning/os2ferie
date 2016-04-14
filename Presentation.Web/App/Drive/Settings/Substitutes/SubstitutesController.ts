module app.drive {
    "use strict";

    import GridService = core.services.SubstitutesGridService;
    import SubstituteType = core.models.SubstituteType;
    import BaseSubstitutesController = core.controllers.BaseSubstitutesController;

    class DriveSubstitutesController extends BaseSubstitutesController {

        static $inject = [
            "$scope",
            "Person",
            "$rootScope",
            "HelpText",
            "Autocomplete",
            "SubstitutesGridService",
            "$modal",
            "$timeout"
        ];

        constructor(protected $scope, protected Person, protected $rootScope, protected HelpText, protected Autocomplete, protected SubstitutesGridService: GridService, protected $modal, protected $timeout) {
            super($scope, Person, $rootScope, HelpText, Autocomplete, SubstitutesGridService, $modal, $timeout, SubstituteType.Drive);
        }

    }

    angular.module("app.drive").controller("Drive.SubstitutesController", DriveSubstitutesController);
}