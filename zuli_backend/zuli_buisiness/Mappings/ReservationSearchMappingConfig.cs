using Mapster;
using System.Collections.Generic;
using System.Linq;
using zuli_Business.DTO.ReservationSearch;
using zuli_Data.Entities;

namespace zuli_Business.Mappings
{
    public class ReservationSearchMappingConfig : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<ReservationSearchFlightEntity, ReservationSearchSegmentDTO>()
                .Map(dest => dest.DurationMinutes, src => (int)(src.ArrivalDateTime - src.DepartureDateTime).TotalMinutes);

            config.NewConfig<List<ReservationSearchFlightEntity>, ReservationSearchJourneyDTO>()
                .Map(dest => dest.DepartureDateTime, src => src.First().DepartureDateTime)
                .Map(dest => dest.ArrivalDateTime, src => src.Last().ArrivalDateTime)
                .Map(dest => dest.OriginCity, src => src.First().OriginCity)
                .Map(dest => dest.OriginCode, src => src.First().OriginCode)
                .Map(dest => dest.DestinationCity, src => src.Last().DestinationCity)
                .Map(dest => dest.DestinationCode, src => src.Last().DestinationCode)
                .Map(dest => dest.FlightClass, src => src.First().FlightClass)
                .Map(dest => dest.Stops, src => src.Count - 1)
                .Map(dest => dest.TotalDurationMinutes, src => (int)(src.Last().ArrivalDateTime - src.First().DepartureDateTime).TotalMinutes)
                .Map(dest => dest.Segments, src => src)
                .Ignore(dest => dest.Layovers);

            config.NewConfig<List<ReservationSearchFlightEntity>, ReservationSearchResponseDTO>()
                .Map(dest => dest.DestinationCity, src => src.Last().DestinationCity)
                .Map(dest => dest.DestinationCode, src => src.Last().DestinationCode)
                .Map(dest => dest.Journey, src => src)
                .Ignore(dest => dest.ReservationCode)
                .Ignore(dest => dest.DaysRemaining)
                .Ignore(dest => dest.PassengerCount)
                .Ignore(dest => dest.Passengers);

            config.NewConfig<ReservationSearchPassengerEntity, ReservationSearchPassengerDTO>();
        }
    }
}