using Mapster;
using zuli_Business.DTO;
using zuli_Data.Entities;

namespace zuli_Business.Mappings
{
    public class AirportMappingConfig : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<AirportDTO, AirportEntity>()
                .Ignore(dest => dest.AirportCode)
                .Ignore(dest => dest.AdminId)
                .Map(dest => dest.Name, src => src.name.Trim())
                .Map(dest => dest.Country, src => src.country.Trim())
                .Map(dest => dest.City, src => src.city.Trim());

            config.NewConfig<AirportEntity, AirportDTO>()
                .Map(dest => dest.airportCode, src => src.AirportCode)
                .Map(dest => dest.name, src => src.Name)
                .Map(dest => dest.country, src => src.Country)
                .Map(dest => dest.city, src => src.City)
                .Map(dest => dest.businessId, src => src.AdminId.ToString());
        }
    }
}