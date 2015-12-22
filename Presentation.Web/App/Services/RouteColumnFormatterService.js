angular.module("application").service("RouteColumnFormatter", [function () {
    return {
        format: function (data) {
            
            var tooltipContent = "";
            var gridContent = "";
            if (data.DriveReportPoints != null && data.DriveReportPoints != undefined && data.DriveReportPoints.length > 0) {
                angular.forEach(data.DriveReportPoints, function (point, key) {
                    if (key != data.DriveReportPoints.length - 1) {
                        tooltipContent += point.StreetName + " " + point.StreetNumber + ", " + point.ZipCode + " " + point.Town + "<br/>";
                        gridContent += point.Town + "<br/>";
                    } else {
                        tooltipContent += point.StreetName + " " + point.StreetNumber + ", " + point.ZipCode + " " + point.Town;
                        gridContent += point.Town;
                    }
                });
            } else {
                tooltipContent = data.UserComment;
            }
            gridContent = "<i class='fa fa-road fa-2x'></i>";
            var toolTip = "<div class='inline margin-left-5' kendo-tooltip k-content=\"'" + tooltipContent + "'\">" + gridContent + "</div>";
            var globe = "<div class='inline pull-right margin-right-5' kendo-tooltip k-content=\"'Se rute på kort'\"><a ng-click='showRouteModal(" + data.Id + ")'><i class='fa fa-globe fa-2x'></i></a></div>";
            if (data.IsOldMigratedReport) {
                globe = "<div class='inline pull-right margin-right-5' kendo-tooltip k-content=\"'Denne indberetning er overført fra eIndberetning og der kan ikke genereres en rute på et kort'\"><i class='fa fa-circle-thin fa-2x'></i></a></div>";
            }
            var roundTrip = "";
            if (data.IsRoundTrip) {
                roundTrip = "<div class='inline margin-left-5' kendo-tooltip k-content=\"'Ruten er tur/retur'\"><i class='fa fa-exchange fa-2x'></i></div>";
            }

            var edited = "";

            if (data.CreatedDateTimestamp < data.EditedDateTimestamp) {
                edited = "<div class='inline pull-right margin-right-5' kendo-tooltip k-content=\"'Denne indberetning er blevet redigeret'\"><i class='fa fa-pencil fa-2x'></i></div>";
            }

            var result = toolTip + roundTrip + globe;
            var comment = data.UserComment != null ? data.UserComment : "Ingen kommentar angivet";

            if (data.KilometerAllowance != "Read") {
                return result;
            } else {
                if (data.IsFromApp) {
                    var fromAppTooltip = "<div class='inline margin-left-5'>Indberettet fra mobil app</div><div class='inline margin-right-5 pull-right' kendo-tooltip k-content=\"'" + data.UserComment + "'\"><i class=\"fa fa-2x fa-comment-o\"></i></div>";
                    if (data.DriveReportPoints.length > 1) {
                        result = toolTip + roundTrip + fromAppTooltip + globe + edited;
                    } else {
                        // Set road tooltip to just contain "Aflæst manuelt"
                        toolTip = "<div class='inline margin-left-5' kendo-tooltip k-content=\"'" + "Aflæst manuelt" + "'\">" + gridContent + "</div>";
                        result = toolTip + roundTrip + fromAppTooltip + edited;
                    }
                    return result;
                } else {
                    return "<div class='inline' kendo-tooltip k-content=\"'" + comment + "'\">Aflæst manuelt</div>";
                }
            }

        }
    }
}])