using Mapster;
using zuli_Business.DTO;
using zuli_Data.Entities;

namespace zuli_Business.Mappings
{
    public class AircraftMappingConfig : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<AircraftDTO, AircraftEntity>()
                .Ignore(dest => dest.aircraftId)
                .Ignore(dest => dest.AdminId)
                .Map(dest => dest.model, src => src.model ?? string.Empty)
                .Map(dest => dest.weight, src => src.weight)
                .Map(dest => dest.numberEconomyClassRows, src => src.numberEconomyClassRows)
                .Map(dest => dest.numberSeatingRowsEconomy, src => src.numberSeatingRowsEconomy)
                .Map(dest => dest.numberFirstClassRows, src => src.numberFirstClassRows)
                .Map(dest => dest.numberSeatingRowsFirst, src => src.numberSeatingRowsFirst)
                .Map(dest => dest.baggageCapacity, src => src.baggageCapacity);

            config.NewConfig<AircraftEntity, AircraftDTO>()
                .Map(dest => dest.businessId, _ => (string?)null);
        }
    }
}