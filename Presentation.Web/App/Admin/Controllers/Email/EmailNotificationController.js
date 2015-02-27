angular.module("application").controller("EmailNotificationController", [
    "$scope", "$modal", "EmailNotification", function ($scope, $modal, EmailNotification) {




        $scope.loadNotifications = function () {
            $scope.notifications = {
                dataSource: {
                    type: "odata",
                    transport: {
                        read: {
                            beforeSend: function (req) {
                                req.setRequestHeader('Accept', 'application/json;odata=fullmetadata');
                            },
                            url: "/odata/MailNotifications",
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
                        field: "DateTimestamp",
                        title: "Adviseringsdato",
                        template: function (data) {
                            var m = moment.unix(data.DateTimestamp);
                            return m._d.getDate() + "/" +
                                (m._d.getMonth() + 1) + "/" + // +1 because getMonth is zero indexed.
                                m._d.getFullYear();
                        }
                    }, {
                        field: "Id",
                        title: "Lønkørselsdato"
                    }, {
                        field: "Repeat",
                        title: "Gentag Månedligt",
                        template: function (data) {
                            if (data.Repeat) {
                                return "Ja";
                            }
                            return "Nej";
                        }
                    }, {
                        field: "Id",
                        template: "<a ng-click=editClick(${Id})>Redigér</a> | <a ng-click=deleteClick(${Id})>Slet</a>",
                        title: "Muligheder",
                    }
                ],
            };
        }

        $scope.updateNotificationGrid = function () {
            $scope.notificationGrid.dataSource.read();
        }

        $scope.gridPageSize = 5;


        $scope.loadNotifications();



        $scope.editClick = function (id) {
            var modalInstance = $modal.open({
                templateUrl: '/App/Admin/HTML/Email/EditMailNotificationTemplate.html',
                controller: 'EditMailNotificationController',
                backdrop: "static",
                resolve: {
                    itemId: function () {
                        return id;
                    }
                }
            });

            modalInstance.result.then(function (result) {
                EmailNotification.patch({ id: id }, {
                    "DateTimestamp": result.notificationDate,
                    "Repeat": result.repeatMonthly,
                }, function () {
                    $scope.updateNotificationGrid();
                });
            });
        }

        $scope.pageSizeChanged = function () {
            $scope.notificationGrid.dataSource.pageSize(Number($scope.gridPageSize));
        }

        $scope.deleteClick = function (id) {
            var modalInstance = $modal.open({
                templateUrl: '/App/Admin/HTML/Email/ConfirmDeleteMailNotificationTemplate.html',
                controller: 'DeleteMailNotificationController',
                backdrop: "static",
                resolve: {
                    itemId: function () {
                        return id;
                    }
                }
            });

            modalInstance.result.then(function () {
                EmailNotification.delete({ id: id }, function () {
                    $scope.updateNotificationGrid();
                });
            });
        }

        $scope.addNewClick = function () {
            var modalInstance = $modal.open({
                templateUrl: '/App/Admin/HTML/Email/AddNewMailNotificationTemplate.html',
                controller: 'AddNewMailNotificationController',
                backdrop: "static",
            });

            modalInstance.result.then(function (result) {
                EmailNotification.post({
                    "DateTimestamp": result.notificationDate,
                    "Repeat": result.repeatMonthly,
                    "Notified": false
                }, function () {
                    $scope.updateNotificationGrid();
                });
            });
        }

        
    }
]);
