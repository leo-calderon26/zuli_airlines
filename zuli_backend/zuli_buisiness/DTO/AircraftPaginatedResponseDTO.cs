namespace zuli_Business.DTO
{
    public class AircraftPaginatedResponseDTO<T>
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalRecords { get; set; }
        public int TotalPages { get; set; }
        public IEnumerable<AircraftDTO> Data { get; set; }
    }
}
