using zuli_Business.DTO;
using zuli_Business.Interface;
using zuli_Data.Entities;
using zuli_Data.Exceptions;
using zuli_Repository.Interface;

namespace zuli_Business
{
    public class ReservationCreationService : IReservationCreationService
    {
        private const int MAX_RESERVATION_CODE_GENERATION_ATTEMPTS = 10;
        private readonly IReservationRepository _reservationRepository;

        public ReservationCreationService(IReservationRepository reservationRepository)
        {
            _reservationRepository = reservationRepository;
        }

        public async Task<(int ReservationId, string ReservationCode)> CreateReservation(
            TicketPurchaseRequestDTO request,
            decimal total,
            int buyerId)
        {
            var reservationCode = await GenerateUniqueReservationCodeAsync();
            var reservation = BuildReservationEntity(reservationCode, request, total, buyerId);
            var reservationId = await _reservationRepository.CreateReservation(reservation);
            return (reservationId, reservationCode);
        }

        private async Task<string> GenerateUniqueReservationCodeAsync()
        {
            var existingCodes = await _reservationRepository.GetAllReservationCodes();

            for (int attempt = 0; attempt < MAX_RESERVATION_CODE_GENERATION_ATTEMPTS; attempt++)
            {
                var code = ReservationCodeGenerator.Generate();

                if (!existingCodes.Contains(code))
                {
                    return code;
                }
            }

            throw new ZuliNotFoundException($"No se pudo generar un código de reservación único después de {MAX_RESERVATION_CODE_GENERATION_ATTEMPTS} intentos.");
        }

        private static ReservationEntity BuildReservationEntity(
            string code, 
            TicketPurchaseRequestDTO request,
            decimal total,
            int buyerId)
        {
            return new ReservationEntity
            {
                ReservationCode = code,
                ReservationOrigin = request.ReservationOrigin,
                TotalPayment = total,
                PurchaseDate = DateTime.Now.Date,
                BuyerId = buyerId,
                FlightClass = request.FlightClass,
                PaymentMethod = request.PaymentMethod
            };
        }
    }
}
