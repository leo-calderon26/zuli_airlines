using Microsoft.Extensions.Logging;
using zuli_Business.Interface;

namespace zuli_Business
{
    public class DevEmailService : IEmailService
    {
        private readonly ILogger<DevEmailService> _logger;

        public DevEmailService(ILogger<DevEmailService> logger)
        {
            _logger = logger;
        }

        public Task SendActivationEmailAsync(string toEmail, string fullName, string activationLink)
        {
            _logger.LogInformation(
                "Correo de activación para {Email}. Usuario: {FullName}. Link: {ActivationLink}",
                toEmail,
                fullName,
                activationLink
            );

            return Task.CompletedTask;
        }
    }
}