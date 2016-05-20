module app.vacation.resources {
    import VacationBalance = app.core.models.VacationBalance;

    export interface IVacationBalance extends ng.resource.IResource<IVacationBalance>, VacationBalance {

    }

    export interface IVacationBalanceResource extends ng.resource.IResourceClass<IVacationBalance> {

    }

    angular.module('app.vacation')
        .factory('VacationBalanceResource', [
            '$resource', ($resource: ng.resource.IResourceService): IVacationBalanceResource => {
                var getAction: ng.resource.IActionDescriptor = {
                    method: "GET",
                    isArray: true,
                    transformResponse(res) {
                        return angular.fromJson(res).value;
                    }
                }

                var queryAction: ng.resource.IActionDescriptor = {
                    method: "GET",
                    isArray: true,
                    transformResponse(res) {
                        return angular.fromJson(res).value;
                    }
                }

                return <IVacationBalanceResource>$resource("/odata/VacationBalance(:id)?:query", { id: "@id", query: "@query" }, {
                    get: getAction,
                    query: queryAction
                });
            }
        ]);
}
