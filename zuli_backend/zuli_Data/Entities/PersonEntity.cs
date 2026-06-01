namespace zuli_Data.Entities
{
    public class PersonEntity
    {
        public int PersonId { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string FirstLastName { get; set; } = string.Empty;
        public string SecondLastName { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string BirthDate { get; set; } = string.Empty;
        public string Gender { get; set; } = string.Empty;
    }
}
