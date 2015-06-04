using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web.OData;
using Core.ApplicationServices.Interfaces;
using Core.ApplicationServices.MailerService.Impl;
using Core.ApplicationServices.MailerService.Interface;
using Core.DomainModel;
using Core.DomainServices;
using Core.DomainServices.RoutingClasses;
using Infrastructure.AddressServices;
using Infrastructure.AddressServices.Routing;
using Infrastructure.DataAccess;
using log4net;
using Ninject;
using OS2Indberetning;


namespace Core.ApplicationServices
{
    public class PersonalRouteService : IPersonalRouteService
    {
        private readonly IAddressCoordinates _coordinates;
        private readonly IGenericRepository<PersonalRoute> _routeRepo;

        public PersonalRouteService(IAddressCoordinates coordinates, IGenericRepository<PersonalRoute> routeRepo)
        {
            _coordinates = coordinates;
            _routeRepo = routeRepo;
        }

        /// <summary>
        /// Creates and inserts a Personal Route into the database.
        /// </summary>
        /// <param name="route">The Route to be inserted.</param>
        /// <returns>The created Personal Route.</returns>
        public PersonalRoute Create(PersonalRoute route)
        {
            var pointsWithCoordinates =
                    route.Points.Select((t, i) => route.Points.ElementAt(i))
                        .Select(currentPoint => (Point)_coordinates.GetAddressCoordinates(currentPoint))
                        .ToList();

            route.Points = pointsWithCoordinates;

            var createdRoute = _routeRepo.Insert(route);
            _routeRepo.Save();

            for (var i = 0; i < createdRoute.Points.Count; i++)
            {
                var currentPoint = createdRoute.Points.ElementAt(i);

                if (i == route.Points.Count - 1)
                {
                    // last element   
                    currentPoint.PreviousPointId = createdRoute.Points.ElementAt(i - 1).Id;
                }
                else if (i == 0)
                {
                    // first element
                    currentPoint.NextPointId = createdRoute.Points.ElementAt(i + 1).Id;
                }
                else
                {
                    // between first and last
                    currentPoint.NextPointId = createdRoute.Points.ElementAt(i + 1).Id;
                    currentPoint.PreviousPointId = createdRoute.Points.ElementAt(i - 1).Id;
                }
            }
            _routeRepo.Save();
            return createdRoute;
        }
    }

}
