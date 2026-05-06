namespace zuli_Business.DTO
{
    public class UserSearchResponseDTO
    {
        public int StatusCode { get; set; }
        public string Message { get; set; } = string.Empty;

        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalItems { get; set; }
        public int TotalPages { get; set; }

        public List<UserListItemDTO> Users { get; set; } = new();
    }
}