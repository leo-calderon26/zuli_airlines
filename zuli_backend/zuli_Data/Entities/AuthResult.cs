namespace zuli_Data.Entites
{
	public class AuthResult
	{
		public bool Success { get; set; }
		public string Message { get; set; } = string.Empty;
		public AppUser? User { get; set; }
	}
}