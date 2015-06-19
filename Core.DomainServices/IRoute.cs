using System.Collections.Generic;
using Core.DomainModel;

namespace Core.DomainServices
{
    public interface IRoute<T>
    {
        /// <summary>
        /// Returns a route for a set of addresses.
        /// </summary>
        /// <param name="transportType">Type of transport. Car or bike</param>
        /// <param name="addresses"></param>
        /// <returns></returns>
        T GetRoute(DriveReportTransportType transportType, IEnumerable<Address> addresses);
    }
}