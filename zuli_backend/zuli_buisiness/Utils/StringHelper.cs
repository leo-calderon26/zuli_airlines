using zuli_Business.DTO;

namespace zuli_Business
{
    public static class StringHelper
    {
        public static string BuildPassengerKey(PassengerTicketDTO passenger)
        {
            return string.Join(
                "|",
                Normalize(passenger.FirstName),
                Normalize(passenger.FirstLastName),
                Normalize(passenger.SecondLastName),
                NormalizeDate(passenger.BirthDate)
            );
        }

        public static string Normalize(string value)
        {
            return value
                .Trim()
                .ToLowerInvariant();
        }

        public static string NormalizeDate(string value)
        {
            if (!DateTime.TryParse(value, out var date))
            {
                return value.Trim();
            }

            return date
                .Date
                .ToString("yyyy-MM-dd");
        }
    }
}
