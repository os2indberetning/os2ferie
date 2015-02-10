///<reference path="../../../Core.DomainModel/Person.cs.d.ts"/>

module Setting {
    'use strict';
    interface Scope extends ng.IScope {
        personalAddresses: any;
        personalRoutes: any;
        mailAdvice: any;
        licensePlates: any;
        isCollapsed: any;
        setHomeWorkOverride: any;

    }

    class Person {
        public id: number;
        public cprNumber: string;
        public personId: number;
        public firstName: string;
        public middleName: string;
        public lastName: string;
        public mail: string;
        public workDistanceOverride: number;
        //public licenseplates: LicensePlate[];
    }

    class LicensePlate {
        public id: number;
        public plate: string;
        public description: string;
    }

    export class Controller {

        private http: any;
        private person: Person;
        private licenseplates: LicensePlate[];

        constructor(private $scope: Scope, private $modal, private $http) {
            this.http = $http;

            this.person = this.getPerson(1);

            

            $scope.personalAddresses = {
                dataSource: {
                    type: "odata",
                    transport: {
                        read: "http://demos.telerik.com/kendo-ui/service/Northwind.svc/Employees"
                    },
                    pageSize: 5,
                    serverPaging: true,
                    serverSorting: true
                },
                sortable: true,
                pageable: true,
                dataBound: function () {
                    this.expandRow(this.tbody.find("tr.k-master-row").first());
                },
                columns: [
                    {
                        field: "FirstName",
                        title: "First Name",
                        width: "120px"
                    }, {
                        field: "LastName",
                        title: "Last Name",
                        width: "120px"
                    }, {
                        field: "Country",
                        width: "120px"
                    }, {
                        field: "City",
                        width: "120px"
                    }, {
                        field: "Title"
                    }
                ]
            };
            $scope.personalRoutes = {
                dataSource: {
                    type: "odata",
                    transport: {
                        read: "http://demos.telerik.com/kendo-ui/service/Northwind.svc/Employees"
                    },
                    pageSize: 5,
                    serverPaging: true,
                    serverSorting: true
                },
                sortable: true,
                pageable: true,
                dataBound: function () {
                    this.expandRow(this.tbody.find("tr.k-master-row").first());
                },
                columns: [
                    {
                        field: "FirstName",
                        title: "First Name",
                        width: "120px"
                    }, {
                        field: "LastName",
                        title: "Last Name",
                        width: "120px"
                    }, {
                        field: "Country",
                        width: "120px"
                    }, {
                        field: "City",
                        width: "120px"
                    }, {
                        field: "Title"
                    }
                ]
            };

            $scope.isCollapsed = true;

            $scope.mailAdvice = 'No';

            //$scope.licensePlates = ['AB 12 345', 'HG 56 987', ' TI 53 456'];

            var person = new Person();
            person.workDistanceOverride = 42;

            $scope.setHomeWorkOverride = () => {
                $http({ method: 'PATCH', url: "odata/Person(1)", data: { workDistanceOverride: 42} }).success(
                        (data, status) => console.log(data)
                    );
            }
        }

        getPerson(id: number) {
            return this.http({
                method: 'GET',
                url: 'odata/Person(1)'
            }).success(
                (data) => {
                        console.log(data);
                        this.licenseplates = this.getLicensePlates(data[0].Id);
                        console.log(this.licenseplates);
                    return data;
                }
            );
        }

        getLicensePlates(personId: number) {
            return this.http({
                method: 'GET',
                url: 'odata/LicensePlates(' + personId + ')'
            }).success(
                (data) => {
                    console.log(data); return data; }
            );
        }


    }
}

Application.AngularApp.Module.controller("SettingController", Setting.Controller);

///Ren javescript/angular controllere. Undskyld.
Application.AngularApp.Module.controller('TokenController', function ($scope, $modal, $log) {

    $scope.items = ['Nr. 1: 123456', 'Nr. 1: 234567', 'Nr. 1: 345678'];

    $scope.openTokenModal = function (size) {

        var modalInstance = $modal.open({
            templateUrl: '/App/Settings/tokenModal.html',
            controller: 'TokenInstanceController',
            size: size,
            resolve: {
                items: function () {
                    return $scope.items;
                }
            }
        });

        modalInstance.result.then(function (selectedItem) {
            $scope.selected = selectedItem;
        });
    };
});

Application.AngularApp.Module.controller('TokenInstanceController', function ($scope, $modalInstance, items) {

    $scope.items = items;
    $scope.selected = {
        item: $scope.items[0]
    };

    $scope.closeTokenModal = function () {
        $modalInstance.dismiss('Luk');
    };
});