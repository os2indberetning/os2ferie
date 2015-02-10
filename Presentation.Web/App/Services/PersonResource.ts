module Service {

    export class PersonResource {
        public static Person($resource: ng.resource.IResourceService): Resource.IPersonResource {
            
            var updateAction: ng.resource.IActionDescriptor = {
                method: 'PUT',
                isArray: false
            };

            var getAction: ng.resource.IActionDescriptor = {
                method: 'GET',
                isArray: true
            };

            var resource =  $resource('/odata/Person(:id)', { id: "@id" }, {
                update: updateAction,
                get: getAction
            });

            return <Resource.IPersonResource> resource;

        }
    }
}

Application.AngularApp.Module.factory('Person', ["$resource", Service.PersonResource.Person]);