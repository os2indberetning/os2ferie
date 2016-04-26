module app.drive {
    "use strict";

    import BaseApproveReportsSettingsController = app.core.controllers.BaseApproveReportsSettingsController;
    import SubstituteType = app.core.models.SubstituteType;

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
            super($scope, Person, $rootScope, Autocomplete, $modal, moment, SubstituteType.Drive);
        }

    }

    angular.module("app.drive").controller("ApproveReportsSettingsController", DriveApproveReportsSettingsController);
}
