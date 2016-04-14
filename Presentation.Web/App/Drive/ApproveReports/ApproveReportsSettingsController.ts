module app.drive {
    "use strict";
    
    import BaseApproveReportsSettingsController = app.core.controllers.BaseApproveReportsSettingsController;
    import SubstituteType = app.core.models.SubstituteType;

    class DriveApproveReportsSettingsController extends BaseApproveReportsSettingsController {

        static $inject = [
            "$scope",
            "Person",
            "$rootScope",
            "Autocomplete",
            "$modal"
        ];

        constructor(protected $scope, protected Person, protected $rootScope, protected Autocomplete, protected $modal) {
            super($scope, Person, $rootScope, Autocomplete, $modal, SubstituteType.Drive);
        }

    }

    angular.module("app.drive").controller("ApproveReportsSettingsController", DriveApproveReportsSettingsController);
}