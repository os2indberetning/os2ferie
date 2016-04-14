var __extends = (this && this.__extends) || function (d, b) {
    for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p];
    function __() { this.constructor = d; }
    d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
};
var app;
(function (app) {
    var drive;
    (function (drive) {
        "use strict";
        var BaseApproveReportsSettingsController = app.core.controllers.BaseApproveReportsSettingsController;
        var SubstituteType = app.core.models.SubstituteType;
        var DriveApproveReportsSettingsController = (function (_super) {
            __extends(DriveApproveReportsSettingsController, _super);
            function DriveApproveReportsSettingsController($scope, Person, $rootScope, Autocomplete, $modal) {
                _super.call(this, $scope, Person, $rootScope, Autocomplete, $modal, SubstituteType.Drive);
                this.$scope = $scope;
                this.Person = Person;
                this.$rootScope = $rootScope;
                this.Autocomplete = Autocomplete;
                this.$modal = $modal;
            }
            DriveApproveReportsSettingsController.$inject = [
                "$scope",
                "Person",
                "$rootScope",
                "Autocomplete",
                "$modal"
            ];
            return DriveApproveReportsSettingsController;
        })(BaseApproveReportsSettingsController);
        angular.module("app.drive").controller("ApproveReportsSettingsController", DriveApproveReportsSettingsController);
    })(drive = app.drive || (app.drive = {}));
})(app || (app = {}));
//# sourceMappingURL=ApproveReportsSettingsController.js.map