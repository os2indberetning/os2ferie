module app.vacation {
    "use strict";

    import VacationBalanceResource = app.vacation.resources.IVacationBalanceResource;
    import IVacationBalance = app.vacation.resources.IVacationBalance;

    class ReportVacationController {
        
        static $inject = [
            "$scope",
            "Person",
            "$rootScope",
            "VacationReport",
            "NotificationService",
            "VacationBalanceResource"
        ];

        VacationRemaining = 0;
        VacationHeld = 0;
        VacationYear = "";
        NetVacationRemaining = 0;
        NetVacationRemainingIfApproved = 0;
        NetHeldAndPlannedVacation = 0;
        VacationDaysInPeriod = 0;
        StartDate: Date;
        EndDate: Date;
        StartTime: Date;
        EndTime: Date;
        Employments;
        VacationType;
        Comment;
        Position;
        SaveButtenDisabled = true;

        private _maxEndDate: Date;
        private _currentUser;

        constructor(private $scope, private Person, private $rootScope, private VacationReport, private NotificationService, private VacationBalance: VacationBalanceResource) {

            this._currentUser = $scope.CurrentUser;

            VacationBalance.get().$promise.then(data => {
                var first: IVacationBalance = data[0];
                this.VacationRemaining = (first.FreeVacationHours + first.TransferredHours + first.VacationHours);
                this.VacationHeld = first.TotalVacationHours - this.VacationRemaining;
                this.VacationYear = first.Year + "/" + (first.Year + 1);

                this.NetVacationRemaining = this.VacationRemaining;
                this.NetVacationRemainingIfApproved = this.VacationRemaining;
                this.NetHeldAndPlannedVacation = this.VacationHeld;

            });


            this.VacationDaysInPeriod = 0;


            this.StartDate = new Date();
            this.EndDate = new Date();
            this._maxEndDate = new Date();

            this.StartTime = new Date(2000, 0, 1, 0, 0, 0, 0);
            this.EndTime = new Date(2000, 0, 1, 0, 0, 0, 0);


            this.$scope.$watch(() => { return this.StartDate }, (newValue, oldValue) => {
                if (this.StartDate > this.EndDate) {
                    this.EndDate = this.StartDate;
                } else {
                    this._calculateVacationPeriod(this.StartDate, this.EndDate);
                }
            });

            this.$scope.$watch(() => { return this.EndDate }, (newValue, oldValue) => {
                this._calculateVacationPeriod(this.StartDate, this.EndDate);
            });


            

            this.Employments = [];

            angular.forEach(this._currentUser.Employments, (value, key) => {
                value.PresentationString = value.Position + " - " + value.OrgUnit.LongDescription + " (" + value.EmploymentId + ")";

                if (value.OrgUnit.HasAccessToVacation) this.Employments.push(value);

            });
        }

        private _calculateVacationPeriod(start, end) {
            this.VacationDaysInPeriod = Math.abs(end - start) / 1000 / 60 / 60;

            this.NetVacationRemainingIfApproved = this.NetVacationRemaining - this.VacationDaysInPeriod;

        }

        TimePickerOptions = {
            
            interval: 15,
            value: new Date(2000, 0, 1, 0, 0, 0, 0)
            
        }

        SaveReport() {
            var report = new this.VacationReport();

            report.StartTimestamp = Math.floor(this.StartDate.getTime() / 1000);
            report.EndTimestamp = Math.floor(this.EndDate.getTime() / 1000);
            report.EmploymentId = this.Position;
            report.Comment = this.Comment;

            report.PersonId = this._currentUser.Id;
            report.Status = "Pending";
            report.CreatedDateTimestamp = Math.floor(Date.now() / 1000);
            report.VacationType = this.VacationType;

            report.$save(res => {
                this.$scope.latestDriveReport = res;

                this.NotificationService.AutoFadeNotification("success", "", "Din indberetning er sendt til godkendelse.");

                this.ClearReport();
                this.SaveButtenDisabled = false;
            }, () => {
                this.SaveButtenDisabled = false;
                this.NotificationService.AutoFadeNotification("danger", "", "Der opstod en fejl under oprettelsen af din ferieindberetning.");
            });;
        }

        ClearReport() {
            
        }

    }

    angular.module("app.vacation").controller("ReportVacationController", ReportVacationController);
}