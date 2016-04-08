var app;
(function (app) {
    var vacation;
    (function (vacation) {
        "use strict";
        var ReportVacationController = (function () {
            function ReportVacationController($scope, Person, $rootScope, VacationReport, NotificationService, VacationBalance) {
                var _this = this;
                this.$scope = $scope;
                this.Person = Person;
                this.$rootScope = $rootScope;
                this.VacationReport = VacationReport;
                this.NotificationService = NotificationService;
                this.VacationBalance = VacationBalance;
                this.VacationRemaining = 0;
                this.VacationHeld = 0;
                this.VacationYear = "";
                this.NetVacationRemaining = 0;
                this.NetVacationRemainingIfApproved = 0;
                this.NetHeldAndPlannedVacation = 0;
                this.VacationDaysInPeriod = 0;
                this.SaveButtenDisabled = true;
                this.TimePickerOptions = {
                    interval: 15,
                    value: new Date(2000, 0, 1, 0, 0, 0, 0)
                };
                this._currentUser = $scope.CurrentUser;
                VacationBalance.get().$promise.then(function (data) {
                    var first = data[0];
                    _this.VacationRemaining = (first.FreeVacationHours + first.TransferredHours + first.VacationHours);
                    _this.VacationHeld = first.TotalVacationHours - _this.VacationRemaining;
                    _this.VacationYear = first.Year + "/" + (first.Year + 1);
                    _this.NetVacationRemaining = _this.VacationRemaining;
                    _this.NetVacationRemainingIfApproved = _this.VacationRemaining;
                    _this.NetHeldAndPlannedVacation = _this.VacationHeld;
                });
                this.VacationDaysInPeriod = 0;
                this.StartDate = new Date();
                this.EndDate = new Date();
                this._maxEndDate = new Date();
                this.StartTime = new Date(2000, 0, 1, 0, 0, 0, 0);
                this.EndTime = new Date(2000, 0, 1, 0, 0, 0, 0);
                this.$scope.$watch(function () { return _this.StartDate; }, function (newValue, oldValue) {
                    if (_this.StartDate > _this.EndDate) {
                        _this.EndDate = _this.StartDate;
                    }
                    else {
                        _this._calculateVacationPeriod(_this.StartDate, _this.EndDate);
                    }
                });
                this.$scope.$watch(function () { return _this.EndDate; }, function (newValue, oldValue) {
                    _this._calculateVacationPeriod(_this.StartDate, _this.EndDate);
                });
                this.Employments = [];
                angular.forEach(this._currentUser.Employments, function (value, key) {
                    value.PresentationString = value.Position + " - " + value.OrgUnit.LongDescription + " (" + value.EmploymentId + ")";
                    if (value.OrgUnit.HasAccessToVacation)
                        _this.Employments.push(value);
                });
            }
            ReportVacationController.prototype._calculateVacationPeriod = function (start, end) {
                this.VacationDaysInPeriod = Math.abs(end - start) / 1000 / 60 / 60;
                this.NetVacationRemainingIfApproved = this.NetVacationRemaining - this.VacationDaysInPeriod;
            };
            ReportVacationController.prototype.SaveReport = function () {
                var _this = this;
                var report = new this.VacationReport();
                report.StartTimestamp = Math.floor(this.StartDate.getTime() / 1000);
                report.EndTimestamp = Math.floor(this.EndDate.getTime() / 1000);
                report.EmploymentId = this.Position;
                report.Comment = this.Comment;
                report.PersonId = this._currentUser.Id;
                report.Status = "Pending";
                report.CreatedDateTimestamp = Math.floor(Date.now() / 1000);
                report.VacationType = this.VacationType;
                report.$save(function (res) {
                    _this.$scope.latestDriveReport = res;
                    _this.NotificationService.AutoFadeNotification("success", "", "Din indberetning er sendt til godkendelse.");
                    _this.ClearReport();
                    _this.SaveButtenDisabled = false;
                }, function () {
                    _this.SaveButtenDisabled = false;
                    _this.NotificationService.AutoFadeNotification("danger", "", "Der opstod en fejl under oprettelsen af din ferieindberetning.");
                });
                ;
            };
            ReportVacationController.prototype.ClearReport = function () {
            };
            ReportVacationController.$inject = [
                "$scope",
                "Person",
                "$rootScope",
                "VacationReport",
                "NotificationService",
                "VacationBalanceResource"
            ];
            return ReportVacationController;
        })();
        angular.module("app.vacation").controller("ReportVacationController", ReportVacationController);
    })(vacation = app.vacation || (app.vacation = {}));
})(app || (app = {}));
