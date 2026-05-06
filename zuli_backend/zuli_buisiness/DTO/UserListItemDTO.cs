namespace zuli_Business.DTO
{
    public class UserListItemDTO
    {
        public Guid UserId { get; set; }
        public int PersonId { get; set; }

        public string NationalId { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string FirstLastName { get; set; } = string.Empty;
        public string SecondLastName { get; set; } = string.Empty;

        public string BusinessEmail { get; set; } = string.Empty;
        public string UserRole { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }
}