using ContentProcessor.Models;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace ContentProcessor.Services;

public class RedisPublisher
{
    private readonly IConnectionMultiplexer _redis;

    public RedisPublisher(IConfiguration configuration)
    {
        _redis = ConnectionMultiplexer.Connect(configuration.GetConnectionString("Redis") ?? "localhost");
    }

    public async Task CacheArticleAsync(ProcessedArticle article)
    {
        var db = _redis.GetDatabase();

        // Add to recent news sorted set (Score = Timestamp)
        await db.SortedSetAddAsync("news:recent", article.Id, new DateTimeOffset(article.PublishedAt).ToUnixTimeSeconds());

        // Add to category list
        await db.ListLeftPushAsync($"news:category:{article.Category}", article.Id);
        await db.ListTrimAsync($"news:category:{article.Category}", 0, 99); // Keep last 100

        // Cache article details
        var json = JsonConvert.SerializeObject(article);
        await db.StringSetAsync($"article:{article.Id}", json, TimeSpan.FromHours(24));
    }
}
