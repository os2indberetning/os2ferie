module app.drive {
    "use strict";

    import ReportType = core.models.ReportType;
    import BaseSubstitutesController = core.controllers.BaseSubstitutesController;

    class DriveSubstitutesController extends BaseSubstitutesController {

        static $inject: string[] = [
            "$scope",
            "Person",
            "$rootScope",
            "HelpText",
            "Autocomplete",
            "$modal",
            "$timeout",
            "moment"
        ];

        constructor(protected $scope, protected Person, protected $rootScope, protected HelpText, protected Autocomplete, protected $modal, protected $timeout, protected moment) {
            super($scope, Person, $rootScope, HelpText, Autocomplete, $modal, $timeout, moment, ReportType.Drive);
        }

    }

    angular.module("app.drive").controller("Drive.SubstitutesController", DriveSubstitutesController);
}