/// <reference path="../../Scripts/typings/angularjs/angular.d.ts" />
/// <reference path="../App.ts" />
var Model;
(function (Model) {
    var Product = (function () {
        function Product() {
        }
        return Product;
    })();
    Model.Product = Product;
})(Model || (Model = {}));
var Example;
(function (Example) {
    'use strict';
    var Controller = (function () {
        function Controller($scope, $http) {
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
            };
        }
        Controller.prototype.getAllProducts = function (successCallback) {
            this.httpService.get('/api/example').success(function (data, status) {
                successCallback(data);
            });
        };
        Controller.prototype.addProduct = function (product, successCallback) {
            this.httpService.post('/api/example', product).success(function () {
                successCallback();
            });
        };
        Controller.prototype.deleteProduct = function (productId, successCallback) {
            this.httpService.delete('/api/example/' + productId).success(function () {
                successCallback();
            });
        };
        Controller.prototype.refreshProducts = function (scope) {
            this.getAllProducts(function (data) {
                scope.products = data;
            });
        };
        return Controller;
    })();
    Example.Controller = Controller;
})(Example || (Example = {}));
Application.AngularApp.Module.controller("ExampleController", Example.Controller);
//# sourceMappingURL=ExampleController.js.map