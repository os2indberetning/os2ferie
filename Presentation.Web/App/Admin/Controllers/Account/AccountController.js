angular.module("application").controller("AccountController", [
    "$scope", "$modal", "BankAccount", "NotificationService", function ($scope, $modal, BankAccount, NotificationService) {




        $scope.loadAccounts = function () {
            $scope.accounts = {
                dataSource: {
                    type: "odata",
                    transport: {
                        read: {
                            beforeSend: function (req) {
                                req.setRequestHeader('Accept', 'application/json;odata=fullmetadata');
                            },
                            url: "/odata/BankAccounts",
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
                        display: "{0} - {1} af {2} konti", //{0} is the index of the first record on the page, {1} - index of the last record on the page, {2} is the total amount of records
                        empty: "Ingen konti at vise",
                        page: "Side",
                        of: "af {0}", //{0} is total amount of pages
                        itemsPerPage: "konti pr. side",
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
                        field: "Number",
                        title: "Registreringsnummer",
                        template: function (data) {
                            var split = data.Number.split("-");
                            // Return registreringsnummer
                            return split[0];

                        }
                    }, {
                        field: "Number",
                        title: "Kontonummer",
                        template: function (data) {
                            var split = data.Number.split("-");
                            // return account number
                            return split[1];
                        }
                    }, {
                        field: "Description",
                        title: "Beskrivelse",
                    }, {
                        field: "Id",
                        template: "<a ng-click=deleteAccountClick(${Id})>Slet</a>",
                        title: "Muligheder",
                    }
                ],
            };
        }

        $scope.updateAccountGrid = function () {
            $scope.accountGrid.dataSource.read();
        }

        $scope.gridPageSize = 5;

        $scope.loadAccounts();
 
        $scope.addNewAccountClick = function () {
            $scope.accountNumberErrorMessage = "";
            $scope.accountDescriptionErrorMessage = "";
            $scope.accountNumberRegNumberErrorMessage = "";
            var error = false;
            if ($scope.newAccountAccountNumber == "" || $scope.newAccountAccountNumber == undefined) {
                $scope.accountNumberErrorMessage = "* Du skal skrive et gyldigt kontonummer."
                error = true;
            }
            if ($scope.newAccountRegNumber == "" || $scope.newAccountRegNumber == undefined) {
                $scope.accountNumberRegNumberErrorMessage = "* Du skal skrive et gyldigt registreringsnummer.";
                error = true;
            }
            if ($scope.newAccountDescription == "" || $scope.newAccountDescription == undefined) {
                $scope.accountDescriptionErrorMessage = "* Du skal skrive en beskrivelse.";
                error = true;
            }


            if (!error) {
                BankAccount.post({ "Description": $scope.newAccountDescription, "Number": $scope.newAccountRegNumber + "-" + $scope.newAccountAccountNumber }, function () {
                    $scope.updateAccountGrid();
                    $scope.newAccountDescription = "";
                    $scope.newAccountRegNumber = "";
                    $scope.newAccountAccountNumber = "";
                });
            }

        }

        $scope.deleteAccountClick = function (id) {
            var modalInstance = $modal.open({
                templateUrl: '/App/Admin/HTML/Account/ConfirmDeleteAccountTemplate.html',
                controller: 'DeleteAccountController',
                backdrop: "static",
                resolve: {
                    itemId: function () {
                        return -1;
                    }
                }
            });

            modalInstance.result.then(function () {
                BankAccount.delete({ id: id }, function () {
                    $scope.updateAccountGrid();
                });
            });
        }

    }
]);
