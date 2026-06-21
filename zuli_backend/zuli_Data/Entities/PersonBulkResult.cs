namespace zuli_Data.Entities
{
    public class PersonBulkResult
    {
        public int RowIndex { get; set; }
        public int PersonId { get; set; }
        public int BuyerId { get; set; }
        public bool IsBuyer { get; set; }
    }
}
