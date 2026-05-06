namespace zuli_Business.DTO
{
    public class ActivateAccountRequestDTO
    {
        public string Token { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}