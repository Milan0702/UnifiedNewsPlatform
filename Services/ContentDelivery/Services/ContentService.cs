using ContentDeliveryService.Models;
using MongoDB.Driver;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace ContentDeliveryService.Services;

public class ContentService
{
    private readonly IMongoCollection<Article> _collection;
    private readonly IConnectionMultiplexer _redis;

    public ContentService(IConfiguration configuration)
    {
        var mongoClient = new MongoClient(configuration.GetConnectionString("MongoDb"));
        var database = mongoClient.GetDatabase("content_db");
        _collection = database.GetCollection<Article>("articles");

        _redis = ConnectionMultiplexer.Connect(configuration.GetConnectionString("Redis") ?? "localhost");
    }

    public async Task<List<Article>> GetRecentArticlesAsync(int limit = 20)
    {
        var db = _redis.GetDatabase();
        var cachedIds = await db.SortedSetRangeByRankAsync("news:recent", 0, limit - 1, Order.Descending);
        
        var articles = new List<Article>();
        var missingIds = new List<string>();

        foreach (var id in cachedIds)
        {
            var cachedArticle = await db.StringGetAsync($"article:{id}");
            if (cachedArticle.HasValue)
            {
                articles.Add(JsonConvert.DeserializeObject<Article>(cachedArticle!));
            }
            else
            {
                missingIds.Add(id.ToString());
            }
        }

        if (missingIds.Any())
        {
            var filter = Builders<Article>.Filter.In(a => a.Id, missingIds);
            var dbArticles = await _collection.Find(filter).ToListAsync();
            articles.AddRange(dbArticles);
            
            // Backfill cache (optional, or rely on processor)
        }

        // Fallback if cache empty
        if (!articles.Any())
        {
            articles = await _collection.Find(_ => true)
                .SortByDescending(a => a.PublishedAt)
                .Limit(limit)
                .ToListAsync();
        }

        return articles;
    }

    public async Task<List<Article>> GetArticlesByCategoryAsync(string category, int limit = 20)
    {
        try
        {
            // Simplified: Direct DB query for now, can implement list caching later
            var articles = await _collection.Find(a => a.Category == category)
                .SortByDescending(a => a.PublishedAt)
                .Limit(limit)
                .ToListAsync();
                
            Console.WriteLine($"[DEBUG] Querying Category: '{category}'. Found {articles.Count} articles.");
            return articles;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ERROR] Failed to get articles by category: {ex.Message}");
            Console.WriteLine(ex.StackTrace);
            return new List<Article>();
        }
    }

    public async Task<List<Article>> SearchArticlesAsync(string query)
    {
        var db = _redis.GetDatabase();
        var cacheKey = $"search:{query}";
        var cachedResults = await db.StringGetAsync(cacheKey);

        if (cachedResults.HasValue)
        {
            return JsonConvert.DeserializeObject<List<Article>>(cachedResults!) ?? new List<Article>();
        }

        var filter = Builders<Article>.Filter.Text(query);
        var articles = await _collection.Find(filter).Limit(20).ToListAsync();

        if (articles.Any())
        {
            await db.StringSetAsync(cacheKey, JsonConvert.SerializeObject(articles), TimeSpan.FromMinutes(15));
        }

        return articles;
    }

    public async Task<Article?> GetArticleByIdAsync(string id)
    {
        var db = _redis.GetDatabase();
        var cachedArticle = await db.StringGetAsync($"article:{id}");
        
        if (cachedArticle.HasValue)
        {
            return JsonConvert.DeserializeObject<Article>(cachedArticle!);
        }

        return await _collection.Find(a => a.Id == id).FirstOrDefaultAsync();
    }
}
