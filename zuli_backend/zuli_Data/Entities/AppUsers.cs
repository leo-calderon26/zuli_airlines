namespace zuli_Data.Entities
{
    public class AppUser
    {
        public Guid UserId { get; set; }
        public string BusinessEmail { get; set; } = string.Empty;
        public string BusinessId { get; set; } = string.Empty;
        public string UserRole { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public int FailedLoginAttempts { get; set; }
        public DateTime? LockoutEnd { get; set; }
        public Guid? ManagedByAdminId { get; set; }
    }
}