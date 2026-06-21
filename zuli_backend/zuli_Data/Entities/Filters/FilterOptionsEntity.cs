
namespace zuli_Data.Entities.Filters
{
    public class FilterOptionsEntity
    {
        public List<int> Years { get; set; }
        public List<AirportOptionEntity> Origins { get; set; }
        public List<AirportOptionEntity> Destinations { get; set; }
        public List<AirlineOptionEntity> Airlines { get; set; }
        public List<FlightClassOptionEntity> Classes { get; set; }
    }
}

