namespace zuli_Business.DTO.Filters
{
    public class FilterOptionsDTO
    {
        public List<int> Years { get; set; }
        public List<AirportOptionDTO> Origins { get; set; }
        public List<AirportOptionDTO> Destinations { get; set; }
        public List<AirlineOptionDTO> Airlines { get; set; }
        public List<FlightClassOptionDTO> Classes { get; set; }
    }
}
