using zuli_Data.Entities;

namespace zuli_business.DTO
{
    public class AuthResultDto
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public AppUser? User { get; set; }
    }
}