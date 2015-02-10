


    //[
    //"$resource", ($resource: ng.resource.IResourceService): ServiceModels.IPersonResource => {

    //    var updateAction: ng.resource.IActionDescriptor = {
    //        method: 'PUT',
    //        isArray: false
    //    };

    //    var getAction: ng.resource.IActionDescriptor = {
    //        method: 'GET',
    //        isArray: true,
    //        q: '*'
    //    };

    //    return <ServiceModels.IPersonResource> $resource('/odata/Person(:id)', { id: "@id" }, {
    //        update: updateAction,
    //        get: getAction
    //    });


    //}
    //]);

module Services {
    export class PersonResource {

        constructor($resource: ng.resource.IResourceService) {
            //updateAction: ng.resource.IActionDescriptor = {
            //    method: 'PUT',
            //    isArray: false
            //};

            //getAction: ng.resource.IActionDescriptor = {
            //    method: 'GET',
            //    isArray: true,
            //    q: '*'
            //};

            //return resource('/odata/Person(:id)', { id: "@id" }, {
            //    update: updateAction,
            //    get: getAction
            //});

            var updateAction: ng.resource.IActionDescriptor = {
                method: 'PUT',
                isArray: false
            };

            var getAction: ng.resource.IActionDescriptor = {
                method: 'GET',
                isArray: true,
                q: '*'
            };

            return <ServiceModels.IPersonResource> $resource('/odata/Person(:id)', { id: "@id" }, {
                update: updateAction,
                get: getAction
            });
            

            
        }
    }
}

Application.AngularApp.Module.service("PersonResource", Services.PersonResource);