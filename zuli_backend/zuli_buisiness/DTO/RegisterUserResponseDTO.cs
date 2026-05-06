namespace zuli_Business.DTO
{
    public class RegisterUserResponseDTO
    {
        public int StatusCode { get; set; }
        public string Message { get; set; } = string.Empty;
        public Guid? UserId { get; set; }
        public int? PersonId { get; set; }
        public string? BusinessEmail { get; set; }
    }
}