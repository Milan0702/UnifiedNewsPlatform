using ContentProcessor.Models;
using System.Text.RegularExpressions;

namespace ContentProcessor.Services;

public class ProcessorService
{
    private readonly MongoRepository _mongoRepository;
    private readonly RedisPublisher _redisPublisher;
    private readonly ILogger<ProcessorService> _logger;

    public ProcessorService(MongoRepository mongoRepository, RedisPublisher redisPublisher, ILogger<ProcessorService> logger)
    {
        _mongoRepository = mongoRepository;
        _redisPublisher = redisPublisher;
        _logger = logger;
    }

    public async Task ProcessContentAsync(dynamic rawContent)
    {
        try
        {
            string url = rawContent.Url ?? rawContent.link; // Handle both NewsAPI and RSS fields
            if (string.IsNullOrEmpty(url)) return;

            // Deduplication
            if (await _mongoRepository.ExistsAsync(url))
            {
                _logger.LogInformation($"Duplicate content found: {url}");
                return;
            }

            DateTime publishedAt = DateTime.UtcNow;
            try
            {
                var dateStr = (string?)(rawContent.PublishedAt ?? rawContent.publishedDate);
                if (!string.IsNullOrEmpty(dateStr) && DateTime.TryParse(dateStr, out var parsedDate))
                {
                    publishedAt = parsedDate;
                }
            }
            catch
            {
                // Fallback to UtcNow
            }

            // Extract category - try both PascalCase and camelCase, and handle potential JSON property
            string category = "General";
            try
            {
                // Try to get Category as a property
                var categoryValue = rawContent.Category ?? rawContent.category;
                if (categoryValue != null)
                {
                    category = (string)categoryValue;
                }
            }
            catch
            {
                // If dynamic access fails, use DetermineCategory
                category = DetermineCategory((string?)(rawContent.Title ?? rawContent.title) ?? "");
            }

            var article = new ProcessedArticle
            {
                Title = (string?)(rawContent.Title ?? rawContent.title) ?? "No Title",
                Description = CleanHtml((string?)(rawContent.Description ?? rawContent.description) ?? ""),
                Url = url,
                UrlToImage = (string?)(rawContent.UrlToImage ?? "") ?? "",
                PublishedAt = publishedAt,
                Source = (string?)(rawContent.Source ?? rawContent.source) ?? "Unknown",
                Author = (string?)(rawContent.Author ?? rawContent.author) ?? "Unknown",
                Category = category
            };

            await _mongoRepository.SaveArticleAsync(article);
            await _redisPublisher.CacheArticleAsync(article);
            
            _logger.LogInformation($"Processed and saved article: {article.Title}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing content");
        }
    }

    private string CleanHtml(string input)
    {
        if (string.IsNullOrEmpty(input)) return string.Empty;
        return Regex.Replace(input, "<.*?>", string.Empty);
    }

    private string DetermineCategory(string title)
    {
        if (string.IsNullOrEmpty(title)) return "General";
        title = title.ToLower();
        
        if (title.Contains("tech") || title.Contains("code") || title.Contains("ai")) return "Technology";
        if (title.Contains("business") || title.Contains("market") || title.Contains("stock")) return "Business";
        if (title.Contains("sport") || title.Contains("game")) return "Sports";
        
        return "General";
    }
}
