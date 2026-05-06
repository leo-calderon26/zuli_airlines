using System;
using System.Collections.Generic;
using System.Text;
using zuli_Data.Entities;

namespace zuli_Repository.Interface
{
        public interface IAirportRepository
        {
                Task<int> CreateAirport(AirportEntity Airport);
                Task<bool> AlreadyExist(string airportCode);
                Task<IEnumerable<AirportEntity>> GetAll();
                Task<IEnumerable<AirportEntity>> SearchAirportsByTerm(string searchTerm);
        }
}
