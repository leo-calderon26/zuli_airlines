namespace zuli_Data.Entites
{
    public class AppUser
    {
        public int UserId { get; set; }
        public string BusinessEmail { get; set; } = string.Empty;
        public string BusinessId { get; set; } = string.Empty;
        public string UserRole { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public int FailedLoginAttempts { get; set; }
        public DateTime? LockoutEnd { get; set; }
    }
}