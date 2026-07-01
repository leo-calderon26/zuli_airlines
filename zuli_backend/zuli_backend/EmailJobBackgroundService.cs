using zuli_Business.Interface;

namespace zuli_backend
{
    public class EmailJobBackgroundService : BackgroundService
    {
        private readonly IEmailJobQueue _emailJobQueue;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly ILogger<EmailJobBackgroundService> _logger;

        public EmailJobBackgroundService(
            IEmailJobQueue emailJobQueue,
            IServiceScopeFactory serviceScopeFactory,
            ILogger<EmailJobBackgroundService> logger)
        {
            _emailJobQueue = emailJobQueue;
            _serviceScopeFactory = serviceScopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var job = await _emailJobQueue.DequeueAsync(stoppingToken);

                try
                {
                    using var scope = _serviceScopeFactory.CreateScope();
                    var processor = scope.ServiceProvider.GetRequiredService<IEmailJobProcessor>();
                    await processor.ProcessAsync(job, stoppingToken);
                }
                catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
                {
                    throw;
                }
                catch (Exception exception)
                {
                    _logger.LogError(
                        exception,
                        "Error processing email job {EmailJobType} for reservation {ReservationCode}",
                        job.Type,
                        job.ReservationCode
                    );
                }
            }
        }
    }
}
