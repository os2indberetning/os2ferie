using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.OData;
using Core.DomainModel;

namespace Core.ApplicationServices.Interfaces
{
    public interface IPersonalRouteService
    {
        PersonalRoute Create(PersonalRoute route);
    }
}
