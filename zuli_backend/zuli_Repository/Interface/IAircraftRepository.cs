using System;
using System.Collections.Generic;
using System.Text;
using zuli_Data.Entities;

namespace zuli_Repository.Interface
{
    public interface IAircraftRepository
    {
        Task<int> CreateAircraft(AircraftEntity aircraft);
        Task<IEnumerable<AircraftEntity>> GetAll();
        Task<(IEnumerable<AircraftEntity> aircrafts, int totalCount)> GetAircraftsPaginated(int pageNumber, int pageSize);
        Task<AircraftEntity?> GetById(Guid aircraftId);
        Task UpdateAircraftAsync(AircraftEntity aircraft);
        Task DeleteAircraft(Guid aircraftId);
        Task<bool> IsAdmin(Guid userId);
    }
}
