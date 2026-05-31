namespace zuli_Data.Entities
{
    public class CityEntity
    {
        public int Id { get; set; }
        public int CountryId { get; set; }
        public string CityName { get; set; } = string.Empty;
    }
}