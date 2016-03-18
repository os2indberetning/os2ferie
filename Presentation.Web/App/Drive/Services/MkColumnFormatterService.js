angular.module("app.drive").service("MkColumnFormatter", [function () {
    return {
        format: function (data) {
            
            if (data.KilometerAllowance === "CalculatedWithoutExtraDistance") {
                if (data.StartsAtHome || data.EndsAtHome) {
                    return "<div class='inline margin-left-5' kendo-tooltip k-content=\"'Der er kørt til og/eller fra bopælsadressen,<br>men anvendt Beregnet uden merkørsel'\"><i class='fa fa-minus'></i></div>";
                }
            } else {

                if (data.StartsAtHome || data.EndsAtHome) {
                  return "<div class='inline margin-left-5' kendo-tooltip k-content=\"'Der er kørt til og/eller fra bopælsadressen.<br/> Der er fratrukket merkørsel.'\"><i class='fa fa-check'></i></div>";
                }
            }
            return "";

        }
    }
}])