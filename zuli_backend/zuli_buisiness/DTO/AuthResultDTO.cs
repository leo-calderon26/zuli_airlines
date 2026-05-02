using zuli_Data.Entities;

namespace zuli_buisiness.DTO
{
    public class AuthResultDTO
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public AppUser? User { get; set; }
    }
}