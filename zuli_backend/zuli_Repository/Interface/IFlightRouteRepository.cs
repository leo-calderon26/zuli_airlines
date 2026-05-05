using System;
using System.Collections.Generic;
using System.Text;
using zuli_Data.Entities;

namespace zuli_Repository.Interface
{
    public interface IFlightRouteRepository
    {
        Task<int> CreateFlightRouter(FlightRouteEntity flightRouter);
        Task<bool> AlreadyExistFlightRoute(FlightRouteEntity flightRoute);
    }
}
