using zuli_Business.DTO;

namespace zuli_Business.Interface
{
    public interface IAircraftService
    {
        Task<BasicResponseDTO> CreateAircraft(AircraftDTO aircraft);
        Task<IEnumerable<AircraftDTO>> GetAll();
        Task<AircraftPaginatedResponseDTO<AircraftDTO>> GetAircraftsPaginated(int pageNumber, int pageSize);
        Task<BasicResponseDTO> UpdateAircraftAsync(Guid aircraftId, AircraftDTO aircraft);
    }
}
