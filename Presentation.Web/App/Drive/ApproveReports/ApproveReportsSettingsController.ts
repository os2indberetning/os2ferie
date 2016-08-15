module app.drive {
    "use strict";

    import BaseApproveReportsSettingsController = app.core.controllers.BaseApproveReportsSettingsController;
    import ReportType = app.core.models.ReportType;

    class DriveApproveReportsSettingsController extends BaseApproveReportsSettingsController {

        static $inject: string[] = [
            "$scope",
            "Person",
            "$rootScope",
            "Autocomplete",
            "$modal",
            "moment"
        ];

        constructor(protected $scope, protected Person, protected $rootScope, protected Autocomplete, protected $modal, protected moment) {
            super($scope, Person, $rootScope, Autocomplete, $modal, moment, ReportType.Drive);
        }

    }

    angular.module("app.drive").controller("ApproveReportsSettingsController", DriveApproveReportsSettingsController);
}
