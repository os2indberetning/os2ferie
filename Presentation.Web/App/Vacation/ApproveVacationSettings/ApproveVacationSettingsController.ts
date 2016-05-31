module app.drive {
    "use strict";

    import BaseApproveReportsSettingsController = core.controllers.BaseApproveReportsSettingsController;
    import SubstituteType = core.models.SubstituteType;

    class ApproveVacationSettingsController extends BaseApproveReportsSettingsController {

        static $inject: string[] = [
            "$scope",
            "Person",
            "$rootScope",
            "Autocomplete",
            "$modal",
            "moment"
        ];

        constructor(protected $scope, protected Person, protected $rootScope, protected Autocomplete, protected $modal, protected moment) {
            super($scope, Person, $rootScope, Autocomplete, $modal, moment, SubstituteType.Vacation);
        }

    }

    angular.module("app.drive").controller("ApproveVacationSettingsController", ApproveVacationSettingsController);
}
