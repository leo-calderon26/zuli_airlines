using Mapster;
using System;
using System.Collections.Generic;
using System.Linq;
using zuli_Business.DTO;
using zuli_Data.Entities;

namespace zuli_Business.Mappings
{
    public class FlightReservationMappingConfig : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<FlightRouteEntity, SummarizedFlightRouteDTO>()
                .Map(dest => dest.DepartureDate, src => src.departureDate.ToString("yyyy-MM-dd"));

            config.NewConfig<RequiredPurchaseInfoDTO, TicketPurchaseRequestDTO>()
                .Map(dest => dest.FlightIdList, src => new List<Guid> { (src.ReservationInfo.flightGUID) })
                .Map(dest => dest.FlightRoutes, src => new List<SummarizedFlightRouteDTO>
                {
                    new SummarizedFlightRouteDTO
                    {
                        FlightRouteId = src.SummarizedFlightRoute.FlightRouteId,
                        DepartureDate = src.SummarizedFlightRoute.DepartureDate
                    }
                })
                .Map(dest => dest.FlightClass, src => src.ReservationInfo.firstClass ? "Primera Clase" : "Turista")
                .Map(dest => dest.Passengers, src => src.ReservationInfo.passengers.Select(passenger => new PassengerTicketDTO
                {
                    FirstName = passenger.FirstName,
                    FirstLastName = passenger.LastName,
                    SecondLastName = passenger.LastName2,
                    BirthDate = passenger.BirthDate,
                    Gender = passenger.Gender,
                    PassportCountry = passenger.PassportCountry,
                    PassportDueDate = passenger.PassportExpirationDate,
                    CheckedBaggage = passenger.Checked,
                    BaggageItems = Enumerable.Range(0, passenger.Checked)
                        .Select(_ => new BaggageItemDTO
                        {
                            Weight = 23.0m,
                            Size = "Mediano",
                            Type = "Maleta"
                        })
                        .ToList(),
                    CarryOn = passenger.carryOn ? 1 : 0
                }).ToList())
                .Map(dest => dest.Buyer, src => new BuyerTicketDTO
                {
                    FirstName = src.ReservationInfo.buyer.FirstName,
                    FirstLastName = src.ReservationInfo.buyer.LastName,
                    SecondLastName = src.ReservationInfo.buyer.LastName2,
                    BirthDate = "2004-03-26",
                    Email = src.ReservationInfo.buyer.Email,
                    Phone = src.ReservationInfo.buyer.PhoneNumber
                })
                .Map(dest => dest.PaymentMethod, src => "Credit Card")
                .Map(dest => dest.ReservationOrigin, src => "External Request");

            config.NewConfig<ReservationResponseMappingContextDTO, ReservationResponseDTO>()
                .Map(dest => dest.reservationNumber, src => src.TicketPurchaseResponse.ConfirmationCode)
                .Map(dest => dest.firstClass, src => src.ReservationRequest.firstClass)
                .Map(dest => dest.flightBreakup, src => BuildFlightBreakup(src))
                .Map(dest => dest.paymentInfo, src => BuildPaymentBreakup(src))
                .Map(dest => dest.passengersInfoDTO, src => src.ReservationRequest.passengers)
                .Map(dest => dest.buyerInfo, src => new BuyerBreakupDTO
                {
                    FirstName = src.ReservationRequest.buyer.FirstName,
                    LastName = src.ReservationRequest.buyer.LastName,
                    LastName2 = src.ReservationRequest.buyer.LastName2,
                    Phone = src.ReservationRequest.buyer.PhoneNumber,
                    Email = src.ReservationRequest.buyer.Email
                });
        }

        private static FlightBreakupDTO BuildFlightBreakup(ReservationResponseMappingContextDTO context)
        {
            var firstFlight = context.TicketPurchaseResponse.Breakdown.Flights.FirstOrDefault();
            var passengerBreakdowns = GetPassengerBreakdowns(context.TicketPurchaseResponse);
            var ticketTotal = passengerBreakdowns.Sum(passenger => passenger.TicketPrice);

            return new FlightBreakupDTO
            {
                FlightGUID = context.ReservationRequest.flightGUID,
                DepartureTime = context.ReservedFlightData.RealDepartureTime.ToString(),
                ArrivalTime = context.ReservedFlightData.RealArrivalTime.ToString(),
                Duration = context.ReservedFlightData.Duration.ToString(),
                DepartureAirport = new AirportDTO
                {
                    code = context.ReservedFlightData.DepartureAirportCode,
                    name = context.ReservedFlightData.DepartureAirportName,
                    city = context.ReservedFlightData.DepartureCityName
                },
                ArrivalAirport = new AirportDTO
                {
                    code = context.ReservedFlightData.ArrivalAiportCode,
                    name = context.ReservedFlightData.ArrivalAirportName,
                    city = context.ReservedFlightData.ArrivalAirportCity
                },
                TouristPrice = context.ReservedFlightData.TouristPrice,
                FirstClassPrice = context.ReservedFlightData.FirstClassPrice,
                CarryOnPrice = context.ReservedFlightData.CarryOnPrice,
                CheckedPrice = context.ReservedFlightData.CheckedPrice
            };
        }

        private static PaymentBreakupDTO BuildPaymentBreakup(ReservationResponseMappingContextDTO context)
        {
            var passengerBreakdowns = GetPassengerBreakdowns(context.TicketPurchaseResponse);

            return new PaymentBreakupDTO
            {
                Luggage = passengerBreakdowns.Sum(passenger => passenger.CarryOnTotal + passenger.CheckedBags.Sum(bag => bag.Price)),
                Tickets = passengerBreakdowns.Sum(passenger => passenger.TicketPrice),
                Taxes = 0,
                Total = context.TicketPurchaseResponse.TotalPayment != 0
                    ? context.TicketPurchaseResponse.TotalPayment
                    : context.TicketPurchaseResponse.Breakdown.GrandTotal
            };
        }

        private static List<PassengerBreakdownDTO> GetPassengerBreakdowns(TicketPurchaseResponseDTO response)
        {
            return response.Breakdown.Flights.SelectMany(flight => flight.Passengers).ToList();
        }
    }
}
