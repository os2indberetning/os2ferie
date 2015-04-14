using System.Collections.Generic;
using Core.DomainModel;

namespace Presentation.Web.Test.Controllers.LicensePlates
{
    class LicensePlateRepositoryMock : GenericRepositoryMock<Core.DomainModel.LicensePlate>
    {
        public static List<Core.DomainModel.LicensePlate> RepoList = new List<LicensePlate>();

        protected override List<Core.DomainModel.LicensePlate> Seed()
        {
            RepoList = new List<LicensePlate>();
            return RepoList;
        }
    }
}
