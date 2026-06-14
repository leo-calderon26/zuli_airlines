using Mapster;
using zuli_Business.DTO;
using zuli_Data.Entities;

namespace zuli_Business.Mappings
{
    public class PassengerMappingConfig : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<PassengerTicketDTO, PersonEntity>();

            config.NewConfig<PassengerTicketDTO, PassportEntity>()
                .Map(dest => dest.DueDate, src => DateTime.Parse(src.PassportDueDate))
                .Ignore(dest => dest.PassengerId);
        }
    }
}
