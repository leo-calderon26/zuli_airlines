using System;
using System.Collections.Generic;
using System.Text;
using zuli_Data.Entities;

namespace zuli_Repository.Interface
{
    public interface IAircraftRepository
    {
        Task<int> CreateAircraft(AircraftEntity aircraft);
        Task<bool> AlreadyExist(int aircraftId);
        Task<bool> AlreadyExistByModel(string model);
    }
}
