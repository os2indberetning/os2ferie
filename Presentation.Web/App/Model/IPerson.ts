interface IPerson extends ng.resource.IResource<IPerson> {
    
}

interface IPersonResource extends ng.resource.IResourceClass<IPerson> {
    update(IPerson) : IPerson
}