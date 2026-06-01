
namespace zuli_Data.Entities
{
    public class PassportEntity
    {
        public int PassengerId { get; set; }
        public DateTime DueDate { get; set; }
        public string PassportCountry { get; set; } = string.Empty;
    }
}
