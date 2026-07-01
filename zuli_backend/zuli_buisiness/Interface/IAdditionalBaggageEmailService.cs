using zuli_Business.DTO;

namespace zuli_Business.Interface
{
    public interface IAdditionalBaggageEmailService
    {
        Task SendAdditionalBaggagePurchaseEmailAsync(
            EmailJobDTO job, 
            CancellationToken cancellationToken = default
        );
    }
}
