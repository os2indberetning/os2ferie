var myapp = angular.module("app", ["ngResource"]).factory("PersonResource",
[
    "$resource", ($resource: ng.resource.IResourceService): IPersonResource => {

        var updateAction: ng.resource.IActionDescriptor = {
            method: 'PUT',
            isArray: false
        };


        return <IPersonResource> $resource('', { id: "@id" }, {
            update: updateAction
        });


    }
]); 