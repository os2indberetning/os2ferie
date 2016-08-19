angular.module("app.drive").controller("EmailNotificationController", [
    "$scope", "$modal", "EmailNotification", "$rootScope", function ($scope, $modal, EmailNotification, $rootScope) {


        $scope.$on('emailClicked', function (event, mass) {
            $scope.gridContainer.notificationGrid.dataSource.read();
        });

        $scope.EmailHelpText = $rootScope.HelpTexts.EmailHelpText.text;

        $scope.gridContainer = {};


        /// <summary>
        /// Loads existing MailNotifications from backend to kendo grid datasource.
        /// </summary>
        $scope.notifications = {
            autoBind: false,
            dataSource: {
                type: "odata-v4",
                transport: {
                    read: {
                        beforeSend: function(req) {
                            req.setRequestHeader('Accept', 'application/json;odata=fullmetadata');
                        },
                        url: "/odata/MailNotifications",
                        dataType: "json",
                        cache: false
                    }
                },
                pageSize: 20,
                serverPaging: false,
                serverSorting: true
            },
            sortable: true,
            pageable: {
                messages: {
                    display: "{0} - {1} af {2} adviseringer", //{0} is the index of the first record on the page, {1} - index of the last record on the page, {2} is the total amount of records
                    empty: "Ingen adviseringer at vise",
                    page: "Side",
                    of: "af {0}", //{0} is total amount of pages
                    itemsPerPage: "adviseringer pr. side",
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
                    field: "DateTimestamp",
                    title: "Adviseringsdato",
                    template: function(data) {
                        var m = moment.unix(data.DateTimestamp);
                        return m._d.getDate() + "/" +
                            (m._d.getMonth() + 1) + "/" + // +1 because getMonth is zero indexed.
                            m._d.getFullYear();
                    }
                },
                {
                    field: "PayRoleTimestamp",
                    title: "Lønkørselsdato",
                    template: function(data) {
                        var m = moment.unix(data.PayRoleTimestamp);
                        return m._d.getDate() + "/" +
                            (m._d.getMonth() + 1) + "/" + // +1 because getMonth is zero indexed.
                            m._d.getFullYear();
                    }
                },
                {
                    field: "Repeat",
                    title: "Gentag månedligt",
                    template: function(data) {
                        if (data.Repeat) {
                            return "Ja";
                        }
                        return "Nej";
                    }
                },
                {
                    field: "Notified",
                    title: "Er kørt",
                    template: function(data) {
                        if (data.Notified) {
                            return "<i class='fa fa-check'></i>";
                        }
                        return "";
                    }
                },
                {
                    field: "Id",
                    template: "<a ng-click=editClick(${Id})>Redigér</a> | <a ng-click=deleteClick(${Id})>Slet</a>",
                    title: "Muligheder"
                }
            ]
        };

        $scope.updateNotificationGrid = function () {
            /// <summary>
            /// Refreshes kendo grid datasource.
            /// </summary>
            $scope.gridContainer.notificationGrid.dataSource.read();
        }

        $scope.editClick = function (id) {
            /// <summary>
            /// Opens Edit MailNotification modal
            /// </summary>
            /// <param name="id">Id of MailNotification to edit</param>
            var modalInstance = $modal.open({
                templateUrl: '/App/Drive/Admin/Email/EditMailNotificationTemplate.html',
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
                    "PayRoleTimestamp": result.payDate,
                }, function () {
                    $scope.updateNotificationGrid();
                });
            });
        }

        //$scope.pageSizeChanged = function () {
        //    $scope.gridContainer.notificationGrid.dataSource.pageSize(Number($scope.gridContainer.gridPageSize));
        //}

        $scope.deleteClick = function (id) {
            /// <summary>
            /// Opens delete MailNotification modal
            /// </summary>
            /// <param name="id">Id of MailNotification to delete</param>
            var modalInstance = $modal.open({
                templateUrl: '/App/Drive/Admin/Email/ConfirmDeleteMailNotificationTemplate.html',
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
            /// <summary>
            /// Opens add new MailNotification modal
            /// </summary>
            var modalInstance = $modal.open({
                templateUrl: '/App/Drive/Admin/Email/AddNewMailNotificationTemplate.html',
                controller: 'AddNewMailNotificationController',
                backdrop: "static",
            });

            modalInstance.result.then(function (result) {
                EmailNotification.post({
                    "DateTimestamp": result.notificationDate,
                    "Repeat": result.repeatMonthly,
                    "Notified": false,
                    "PayRoleTimestamp": result.payDate,
                }, function () {
                    $scope.updateNotificationGrid();
                });
            });
        }


    }
]);
