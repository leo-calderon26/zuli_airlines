namespace zuli_Data.Entities
{
    public class AppUser
    {
        public Guid UserId { get; set; }
        public int PersonId { get; set; }

        public string NationalId { get; set; } = string.Empty;

        public string FirstName { get; set; } = string.Empty;
        public string FirstLastName { get; set; } = string.Empty;
        public string SecondLastName { get; set; } = string.Empty;
        public string? Email { get; set; }

        public string BusinessEmail { get; set; } = string.Empty;
        public string BusinessId { get; set; } = string.Empty;
        public string UserRole { get; set; } = string.Empty;
        public string? PasswordHash { get; set; }

        public bool IsActive { get; set; }
        public int FailedLoginAttempts { get; set; }
        public DateTime? LockoutEnd { get; set; }

        public Guid? ManagedByAdminId { get; set; }
        public string? ActivationTokenHash { get; set; }
    }
}