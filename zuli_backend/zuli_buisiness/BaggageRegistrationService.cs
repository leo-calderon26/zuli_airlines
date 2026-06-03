using zuli_Business.DTO;
using zuli_Business.Interface;
using zuli_Data.Entities;
using zuli_Repository.Interface;

namespace zuli_Business
{
    public class BaggageRegistrationService : IBaggageRegistrationService
    {
        private readonly IBaggageRepository _baggageRepository;

        public BaggageRegistrationService(IBaggageRepository baggageRepository)
        {
            _baggageRepository = baggageRepository;
        }

        public async Task RegisterAllBaggage(
            List<PassengerTicketDTO> passengers,
            List<int> passengerIds,
            int reservationId)
        {
            for (int i = 0; i < passengers.Count; i++)
            {
                await RegisterCheckedBaggage(passengers[i], passengerIds[i], reservationId);
                await RegisterCarryOnBaggage(passengers[i], passengerIds[i], reservationId);
            }
        }

        private async Task RegisterCheckedBaggage(
            PassengerTicketDTO passenger,
            int passengerId,
            int reservationId)
        {
            var checkedBaggageCount = Math.Clamp(passenger.CheckedBaggage, 0, 10);

            for (int i = 0; i < checkedBaggageCount; i++)
            {
                var bag = passenger.BaggageItems.ElementAtOrDefault(i);

                await _baggageRepository.CreateBaggage(
                    new BaggageEntity
                    {
                        PassengerId = passengerId,
                        ReservationId = reservationId,
                        Weight = bag?.Weight > 0 ? bag.Weight : 23.0m,
                        Size = string.IsNullOrWhiteSpace(bag?.Size) ? "Mediano" : bag.Size,
                        Type = "Maleta"
                    }
                );
            }
        }

        private async Task RegisterCarryOnBaggage(
            PassengerTicketDTO passenger,
            int passengerId,
            int reservationId)
        {
            var carryOnCount = Math.Clamp(passenger.CarryOn, 0, 2);

            for (int i = 0; i < carryOnCount; i++)
            {
                await _baggageRepository.CreateBaggage(
                    new BaggageEntity
                    {
                        PassengerId = passengerId,
                        ReservationId = reservationId,
                        Weight = 7.0m,
                        Size = "Pequeño",
                        Type = "Mano"
                    }
                );
            }
        }
    }
}
