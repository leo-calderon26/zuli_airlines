using System.Threading.Channels;
using zuli_Business.DTO;
using zuli_Business.Interface;

namespace zuli_Business
{
    public class EmailJobQueue : IEmailJobQueue
    {
        private readonly Channel<EmailJobDTO> _queue;

        public EmailJobQueue()
        {
            _queue = Channel.CreateUnbounded<EmailJobDTO>(new UnboundedChannelOptions
            {
                SingleReader = true,
                SingleWriter = false
            });
        }

        public ValueTask QueueAsync(EmailJobDTO job, CancellationToken cancellationToken = default)
        {
            if (job == null)
            {
                throw new ArgumentNullException(nameof(job));
            }

            return _queue.Writer.WriteAsync(job, cancellationToken);
        }

        public ValueTask<EmailJobDTO> DequeueAsync(CancellationToken cancellationToken)
        {
            return _queue.Reader.ReadAsync(cancellationToken);
        }
    }
}
