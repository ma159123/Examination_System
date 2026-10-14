using Application.Interfaces;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace Infrastructure.Services
{
    public class OutboxProcessorBackgroundService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<OutboxProcessorBackgroundService> _logger;

        public OutboxProcessorBackgroundService(
            IServiceProvider serviceProvider,
            ILogger<OutboxProcessorBackgroundService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Outbox Processor Service is starting.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var outboxRepo = scope.ServiceProvider.GetRequiredService<IOutboxRepository>();
                        var publisher = scope.ServiceProvider.GetRequiredService<IPublisher>(); // MediatR Publisher
                        // get unprocessed messages from the outbox
                        var messages = await outboxRepo.GetUnprocessedMessagesAsync(20, stoppingToken);

                        foreach (var message in messages)
                        {
                            try
                            {
                                var eventType = Type.GetType(message.Type);
                                if (eventType != null)
                                {
                                    // 2.get the domain event type from the message and deserialize it
                                    var domainEvent = JsonSerializer.Deserialize(message.Payload, eventType) as INotification;

                                    if (domainEvent != null)
                                    {
                                        // 3. publish the domain event using MediatR
                                        await publisher.Publish(domainEvent, stoppingToken);
                                    }
                                }

                                message.ProcessedOn = DateTime.UtcNow;
                                message.Error = null;

                            }
                            catch (Exception ex)
                            {
                                message.Error = ex.Message;
                                _logger.LogError(ex, "Failed to process outbox message {MessageId}", message.Id);
                            }

                            await outboxRepo.SaveChangesAsync(stoppingToken);
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occurred while processing outbox messages.");
                }

                // wait for a short period before checking for new messages again
                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
            }
        }
    }
}