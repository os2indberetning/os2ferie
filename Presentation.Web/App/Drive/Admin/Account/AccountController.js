angular.module("app.drive").controller("AccountController", [
    "$scope", "$modal", "BankAccount", "NotificationService", "$rootScope",
    function ($scope, $modal, BankAccount, NotificationService, $rootScope) {


        $scope.AccountHelpText = $rootScope.HelpTexts.AccountHelpText.text;

        $scope.$on('accountClicked', function (event, mass) {
            $scope.container.accountGrid.dataSource.read();
        });

        $scope.container = {};

        $scope.maskOptions = {
            //Omkostningssted
            mask: "0000000000"
        }

        $scope.PSPMaskOptions = {
            //PSP
            mask: "LL-0000000000-00000"
        }

        $scope.accountTypeChanged = function () {
            /// <summary>
            /// Clears the accountnumber field when account type is changed
            /// </summary>
            $scope.newAccountAccountNumber = "";
        }


        /// <summary>
        /// Loads BankAccounts from BackEnd to the Kendo Grid datasource
        /// </summary>
        $scope.accounts = {
            autoBind: false,
            dataSource: {
                type: "odata-v4",
                transport: {
                    read: {
                        beforeSend: function (req) {
                            req.setRequestHeader('Accept', 'application/json;odata=fullmetadata');
                        },
                        url: "/odata/BankAccounts",
                        dataType: "json",
                        cache: false
                    }
                },
                pageSize: 20,
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
                },
                pageSizes: [5, 10, 20, 30, 40, 50, 100, 150, 200]
            },
            scrollable: false,
            columns: [
                {
                    field: "Type",
                    title: "Type",
                    template: function (data) {
                        //if (data.Type == "PSPElement") {
                        //    return "PSP-element";
                        //}
                        //return data.Type;
                        return "Kontonummer";
                    }
                },
                {
                    field: "Number",
                    title: "Kontonummer"
                },
                {
                    field: "Description",
                    title: "Beskrivelse"
                },
                {
                    field: "Id",
                    template: "<a ng-click=deleteAccountClick(${Id})>Slet</a>",
                    title: "Muligheder"
                }
            ]
        };

        $scope.updateAccountGrid = function () {
            /// <summary>
            /// Refreshes the BankAccount grid
            /// </summary>
            $scope.container.accountGrid.dataSource.read();
        }


        $scope.addNewAccountClick = function () {
            /// <summary>
            /// Post new BankAccount to Backend
            /// </summary>
            $scope.accountNumberErrorMessage = "";
            $scope.accountDescriptionErrorMessage = "";
            var error = false;
            if ($scope.container.newAccountAccountNumber == "" || $scope.container.newAccountAccountNumber == undefined || $scope.container.newAccountAccountNumber.indexOf("_") > -1) {
                $scope.accountNumberErrorMessage = "* Du skal skrive et gyldigt kontonummer.";
                error = true;
            }
            if ($scope.container.newAccountDescription == "" || $scope.container.newAccountDescription == undefined) {
                $scope.accountDescriptionErrorMessage = "* Du skal skrive en beskrivelse.";
                error = true;
            }

            if (!error) {
                BankAccount.post({ "Description": $scope.container.newAccountDescription, "Number": $scope.container.newAccountAccountNumber, "Type": $scope.container.newAccountType }, function () {
                    $scope.updateAccountGrid();
                    $scope.container.newAccountDescription = "";
                    $scope.container.newAccountRegNumber = "";
                    $scope.container.newAccountAccountNumber = "";
                    NotificationService.AutoFadeNotification("success", "", "Ny konto oprettet!");
                });
            }

        }

        $scope.deleteAccountClick = function (id) {
            /// <summary>
            /// Sends DELETE request to backend
            /// </summary>
            /// <param name="id">Identifies BankAccount to be deleted</param>
            var modalInstance = $modal.open({
                templateUrl: '/App/Admin/Account/ConfirmDeleteAccountTemplate.html',
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
