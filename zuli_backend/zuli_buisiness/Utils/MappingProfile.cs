using AutoMapper;
using zuli_Business.DTO;
using zuli_Data.Entities;

namespace zuli_Business.Utils
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<FlightRouteDTO, FlightRouteEntity>()
                .ForMember(dest => dest.flightRouteId, opt => opt.Ignore())
                .ForMember(dest => dest.adminId, opt => opt.Ignore());
        }
    }
}
