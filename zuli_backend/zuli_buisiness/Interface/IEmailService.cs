namespace zuli_Business.Interface
{
    public interface IEmailService
    {
        Task SendActivationEmailAsync(string toEmail, string fullName, string activationLink);
    }
}