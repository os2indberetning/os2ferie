angular.module("application").controller("RateController", [
    "$scope", "$modal", "Rate", "NotificationService", "RateType", function ($scope, $modal, Rate, NotificationService, RateType) {





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
                pageable: true,
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
            $scope.rateGrid.dataSource.read();
        }

        $scope.gridPageSize = 5;


      
        $scope.loadRates();


     

        $scope.addNewRateClick = function() {
            $scope.newRateYearError = "";
            $scope.newRateRateError = "";
            $scope.newRateTFCodeError = "";
            $scope.newRateRateTypeError = "";
            var error = false;
            if ($scope.newRateYear == "" || $scope.newRateYear == undefined) {
                $scope.newRateYearError = "* Du skal skrive et gyldigt år."
                error = true;
            }
            if ($scope.newRateRate == "" || $scope.newRateRate == undefined) {
                $scope.newRateRateError = "* Du skal skrive en gyldig takst."
                error = true;
            }
            if ($scope.newRateTFCode == "" || $scope.newRateTFCode == undefined) {
                $scope.newRateTFCodeError = "* Du skal skrive en gyldig TF kode."
                error = true;
            }
            if ($scope.newRateRateType == "" || $scope.newRateRateType == undefined) {
                $scope.newRateRateTypeError = "* Du skal vælge en gyldig taksttype."
                error = true;
            }

            if (!error) {
                Rate.post({ "Year": $scope.newRateYear, "TFCode": $scope.newRateTFCode, "KmRate": $scope.newRateRate, "TypeId" : $scope.newRateRateType, "Active" : true}, function () {
                    $scope.updateRatesGrid();
                    $scope.newRateYear = "";
                    $scope.newRateTFCode = "";
                    $scope.newRateRate = "";
                    $scope.newRateRateType = "";
                });
            }

        }

     

    }
]);
