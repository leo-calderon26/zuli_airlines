using System;
using System.Collections.Generic;
using System.Text;
using zuli_Data.Entities;

namespace zuli_Repository.Interface
{
    public interface IAircraftRepository
    {
        Task<int> CreateAircraft(AircraftEntity aircraft);
        Task<bool> AlreadyExistByModel(string model);
        Task<bool> AlreadyExistByModelExcludingId(string model, Guid aircraftId);
        Task<IEnumerable<AircraftEntity>> GetAll();
        Task<(IEnumerable<AircraftEntity> aircrafts, int totalCount)> GetAircraftsPaginated(int pageNumber, int pageSize);
        Task<AircraftEntity?> GetById(Guid aircraftId);
        Task UpdateAircraft(AircraftEntity aircraft);
        Task<bool> IsAdmin(Guid userId);
    }
}
