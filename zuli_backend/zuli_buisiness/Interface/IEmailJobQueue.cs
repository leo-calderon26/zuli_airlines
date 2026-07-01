using zuli_Business.DTO;

namespace zuli_Business.Interface
{
    public interface IEmailJobQueue
    {
        ValueTask QueueAsync(EmailJobDTO job, CancellationToken cancellationToken = default);
        ValueTask<EmailJobDTO> DequeueAsync(CancellationToken cancellationToken);
    }
}
