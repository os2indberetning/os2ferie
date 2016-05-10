module app.drive {
    "use strict";

    import BaseApproveReportsSettingsController = app.core.controllers.BaseApproveReportsSettingsController;
    import SubstituteType = app.core.models.SubstituteType;

    class ApproveVacationReportsSettingsController extends BaseApproveReportsSettingsController {

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

    angular.module("app.drive").controller("Vacation.ApproveVacationReportsSettingsController", ApproveVacationReportsSettingsController);
}
