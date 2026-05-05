namespace zuli_Business.DTO
{
    public class RegisterUserResponseDTO
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public Guid? UserId { get; set; }
        public string? BusinessEmail { get; set; }
    }
}