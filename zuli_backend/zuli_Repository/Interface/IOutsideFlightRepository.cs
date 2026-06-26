using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using zuli_Data.Entities;

namespace zuli_Repository.Interface
{
    public interface IOutsideFlightRepository
    {
        Task CreateOutsideFlightBulk(List<OutsideFlightEntity> outsideFlights);
    }
}