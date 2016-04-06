angular.module("app.vacation").controller("ReportVacationController", [
    "$scope", "Person", "$rootScope", "VacationReport", "NotificationService","VacationBalance", function ($scope, Person, $rootScope, VacationReport, NotificationService,VacationBalance) {

        var currentUser = $scope.CurrentUser;

        var vacationBalance = VacationBalance.get().$promise.then(function(data) {
            var first = data[0];
            $scope.VacationRemaining = (first.FreeVacationHours + first.TransferredHours + first.VacationHours);
            $scope.VacationHeld = first.TotalVacationHours - $scope.VacationRemaining;
            $scope.VacationYear = first.Year + "/" + (first.Year + 1);

            $scope.NetVacationRemaining = $scope.VacationRemaining;
            $scope.NetVacationRemainingIfApproved = $scope.VacationRemaining;
            $scope.NetHeldAndPlannedVacation = $scope.VacationHeld;

        });


        $scope.VacationDaysInPeriod = 0;


        $scope.startDate = new Date();
        $scope.endDate = new Date();
        $scope.maxEndDate = new Date();

        $scope.startTime = new Date(2000, 0, 1, 0, 0, 0, 0, 0);
        $scope.endTime = new Date(2000, 0, 1, 0, 0, 0, 0, 0);

        var calculateVacationPeriod = function (start, end) {
            $scope.VacationDaysInPeriod = Math.abs(end - start) / 1000 / 60 / 60;

            $scope.NetVacationRemainingIfApproved = $scope.NetVacationRemaining - $scope.VacationDaysInPeriod;

        }

        $scope.$watch("startDate", function (val) {
            if ($scope.startDate > $scope.endDate) {
                $scope.endDate = $scope.startDate;
            } else {
                calculateVacationPeriod($scope.startDate, $scope.endDate);
            }
        });

        $scope.$watch("endDate", function(val) {
            calculateVacationPeriod($scope.startDate, $scope.endDate);
        });

        

        $scope.timePickerOptions = {
            interval: 15,
            value: new Date(2000, 0, 1, 0, 0, 0, 0, 0)
        }

        $scope.Employments = [];

        angular.forEach(currentUser.Employments, function (value, key) {
            value.PresentationString = value.Position + " - " + value.OrgUnit.LongDescription + " (" + value.EmploymentId + ")";

            if (value.OrgUnit.HasAccessToVacation) $scope.Employments.push(value);

        });


        $scope.Save = function () {


            $scope.saveBtnDisabled = true;

            var report = new VacationReport();

            report.StartTimestamp = Math.floor($scope.startDate.getTime() / 1000);
            report.EndTimestamp = Math.floor($scope.endDate.getTime() / 1000);
            report.EmploymentId = $scope.Position;
            report.Comment = $scope.Comment;

            report.PersonId = currentUser.Id;
            report.Status = "Pending";
            report.CreatedDateTimestamp = Math.floor(Date.now() / 1000);
            report.VacationType = $scope.VacationType;

            report.$save(function (res) {
                $scope.latestDriveReport = res;
                NotificationService.AutoFadeNotification("success", "", "Din indberetning er sendt til godkendelse.");
                $scope.clearReport();
                $scope.saveBtnDisabled = false;
            }, function () {
                $scope.saveBtnDisabled = false;
                NotificationService.AutoFadeNotification("danger", "", "Der opstod en fejl under oprettelsen af din ferieindberetning.");
            });;
        }

        $scope.clearReport = function()
        {
            
        }

    }
]);