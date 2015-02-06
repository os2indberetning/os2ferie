/// <reference path="../../Scripts/typings/angularjs/angular.d.ts" />
/// <reference path="../App.ts" />

module Model {
    export class Product {
        Id: string;
        Name: string;
        Price: number;
    }
}

module Example {
    'use strict';
    interface Scope extends ng.IScope {
        newProductName: string;
        newProductPrice: number;
        products: Model.Product[];
        addNewProduct: Function;
        deleteProduct: Function;
    }

    export class Controller {
        private httpService: any;

        constructor($scope: Scope, $http: any) {
            this.httpService = $http;

            this.refreshProducts($scope);

            var controller = this;

            $scope.addNewProduct = function () {
                var newProduct = new Model.Product();
                newProduct.Name = $scope.newProductName;
                newProduct.Price = $scope.newProductPrice;

                controller.addProduct(newProduct, function () {
                    controller.getAllProducts(function (data) {
                        $scope.products = data;
                    });
                });
            };

            $scope.deleteProduct = function (productId) {
                controller.deleteProduct(productId, function () {
                    controller.getAllProducts(function (data) {
                        $scope.products = data;
                    });
                });
            }
        }

        getAllProducts(successCallback: Function): void {
            this.httpService.get('/api/example').success(function (data, status) {
                successCallback(data);
            });
        }

        addProduct(product: Model.Product, successCallback: Function): void {
            this.httpService.post('/api/example', product).success(function () {
                successCallback();
            });
        }

        deleteProduct(productId: string, successCallback: Function): void {
            this.httpService.delete('/api/example/' + productId).success(function () {
                successCallback();
            });
        }

        refreshProducts(scope: Scope) {
            this.getAllProducts(function (data) {
                scope.products = data;
            });
        }

    }
}

Application.AngularApp.Module.controller("ExampleController", Example.Controller);




