using System;
using System.Collections.Generic;
using System.Text;
using zuli_Data.Entities;

namespace zuli_Repository.Interface
{
    public interface IAircraftRepository
    {
        Task<AircraftEntity?> CreateAircraft(AircraftEntity aircraft);
    }
}
