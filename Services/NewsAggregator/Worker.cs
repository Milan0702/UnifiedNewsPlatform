using NewsAggregator.Services;

namespace NewsAggregator;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly NewsFetcher _newsFetcher;
    private readonly RabbitMqProducer _producer;

    public Worker(ILogger<Worker> logger, NewsFetcher newsFetcher, RabbitMqProducer producer)
    {
        _logger = logger;
        _newsFetcher = newsFetcher;
        _producer = producer;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);

            try
            {
                var articles = await _newsFetcher.FetchNewsAsync();
                _logger.LogInformation($"Fetched {articles.Count} articles");

                foreach (var article in articles)
                {
                    await _producer.PublishNewsAsync(article);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in worker loop");
            }

            await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
        }
    }
}
