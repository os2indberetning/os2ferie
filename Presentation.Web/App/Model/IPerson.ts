module ServiceModels {
    export interface IPerson extends ng.resource.IResource<IPerson> {
        id: number;
        cprNumber: string;
        personId: number;
        firstName: string;
        middleName: string;
        lastName: string;
        mail: string;
        workDistanceOverride: number;
    }

    export interface IPersonResource extends ng.resource.IResourceClass<IPerson> {
        update(IPerson): IPerson
    }
}

