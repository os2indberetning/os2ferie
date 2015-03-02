angular.module("application").controller("RateController", [
    "$scope", "$modal", "Rate", "NotificationService", "RateType", function ($scope, $modal, Rate, NotificationService, RateType) {


        $scope.container = {};



        $scope.loadRates = function () {
            $scope.rates = {
                dataSource: {
                    type: "odata",
                    transport: {
                        read: {
                            beforeSend: function (req) {
                                req.setRequestHeader('Accept', 'application/json;odata=fullmetadata');
                            },
                            url: "/odata/Rates?$expand=Type&$filter=Active eq true",
                            dataType: "json",
                            cache: false
                        },
                        parameterMap: function (options, type) {
                            var d = kendo.data.transports.odata.parameterMap(options);
                            delete d.$inlinecount; // <-- remove inlinecount parameter
                            d.$count = true;
                            return d;
                        }
                    },
                    schema: {
                        data: function (data) {
                            return data.value;
                        },
                        total: function (data) {
                            return data['@odata.count']; // <-- The total items count is the data length, there is no .Count to unpack.
                        }
                    },
                    pageSize: 5,
                    serverPaging: false,
                    serverSorting: true,
                },
                sortable: true,
                pageable: {
                    messages: {
                        display: "{0} - {1} af {2} takster", //{0} is the index of the first record on the page, {1} - index of the last record on the page, {2} is the total amount of records
                        empty: "Ingen takster at vise",
                        page: "Side",
                        of: "af {0}", //{0} is total amount of pages
                        itemsPerPage: "takster pr. side",
                        first: "Gå til første side",
                        previous: "Gå til forrige side",
                        next: "Gå til næste side",
                        last: "Gå til sidste side",
                        refresh: "Genopfrisk"
                    }
                },
                scrollable: false,
                columns: [
                    {
                        field: "Year",
                        title: "År"
                    }, {
                        field: "KmRate",
                        title: "Takst",
                        template: "${KmRate} ører pr/km"
                    }, {
                        field: "TFCode",
                        title: "TF kode",
                    },
                    {
                        field: "Type",
                        title: "Type",
                        template: function(data) {
                            return data.Type.Description;
                        }
                    }
                ],
            };
        }

    

        $scope.rateTypes = RateType.get();

        $scope.updateRatesGrid = function () {
            $scope.container.rateGrid.dataSource.read();
        }




      
        $scope.loadRates();


     

        $scope.addNewRateClick = function() {
            $scope.newRateYearError = "";
            $scope.newRateRateError = "";
            $scope.newRateTFCodeError = "";
            $scope.newRateRateTypeError = "";
            var error = false;
            if ($scope.container.newRateYear == "" || $scope.container.newRateYear == undefined || $scope.container.newRateYear.toString().length != 4) {
                $scope.newRateYearError = "* Du skal skrive et gyldigt år."
                error = true;
            }
            if ($scope.container.newRateRate == "" || $scope.container.newRateRate == undefined) {
                $scope.newRateRateError = "* Du skal skrive en gyldig takst."
                error = true;
            }
            if ($scope.container.newRateTFCode == "" || $scope.container.newRateTFCode == undefined) {
                $scope.newRateTFCodeError = "* Du skal skrive en gyldig TF kode."
                error = true;
            }
            if ($scope.container.newRateRateType == "" || $scope.container.newRateRateType == undefined) {
                $scope.newRateRateTypeError = "* Du skal vælge en gyldig taksttype."
                error = true;
            }

            if (!error) {
                Rate.post({ "Year": $scope.container.newRateYear, "TFCode": $scope.container.newRateTFCode, "KmRate": $scope.container.newRateRate, "TypeId": $scope.container.newRateRateType, "Active": true }, function () {
                    $scope.updateRatesGrid();
                    $scope.container.newRateYear = "";
                    $scope.container.newRateTFCode = "";
                    $scope.container.newRateRate = "";
                    $scope.container.newRateRateType = "";
                });
            }

        }

     

    }
]);
