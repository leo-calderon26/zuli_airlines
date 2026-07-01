using zuli_Business.DTO;

namespace zuli_Business.Interface
{
    public interface IEmailJobProcessor
    {
        Task ProcessAsync(EmailJobDTO job, CancellationToken cancellationToken = default);
    }
}
