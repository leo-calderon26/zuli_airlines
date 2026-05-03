namespace zuli_Business.DTO
{
    public class LoginResponseDTO
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public string? BusinessEmail { get; set; }
        public string? BusinessId { get; set; }
        public string? UserRole { get; set; }
    }
}