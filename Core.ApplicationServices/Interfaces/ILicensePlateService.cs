using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.DomainModel;
using Core.DomainServices;

namespace Core.ApplicationServices.Interfaces
{
    public interface ILicensePlateService
    {
        bool MakeLicensePlatePrimary(int plateId);
        LicensePlate HandlePost(LicensePlate plate);
        void HandleDelete(LicensePlate plate);
    }
}
