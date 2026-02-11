using ContentProcessor.Models;
using MongoDB.Driver;

namespace ContentProcessor.Services;

public class MongoRepository
{
    private readonly IMongoCollection<ProcessedArticle> _collection;

    public MongoRepository(IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("MongoDb");
        var client = new MongoClient(connectionString);
        var database = client.GetDatabase("content_db");
        _collection = database.GetCollection<ProcessedArticle>("articles");

        // Create unique index on URL to prevent duplicates
        var indexKeys = Builders<ProcessedArticle>.IndexKeys.Ascending(a => a.Url);
        var indexOptions = new CreateIndexOptions { Unique = true };
        var indexModel = new CreateIndexModel<ProcessedArticle>(indexKeys, indexOptions);
        _collection.Indexes.CreateOne(indexModel);

        // Create text index for search
        var textIndexKeys = Builders<ProcessedArticle>.IndexKeys
            .Text(a => a.Title)
            .Text(a => a.Description);
        var textIndexModel = new CreateIndexModel<ProcessedArticle>(textIndexKeys);
        _collection.Indexes.CreateOne(textIndexModel);
    }

    public async Task SaveArticleAsync(ProcessedArticle article)
    {
        try
        {
            await _collection.InsertOneAsync(article);
        }
        catch (MongoWriteException ex) when (ex.WriteError.Category == ServerErrorCategory.DuplicateKey)
        {
            // Ignore duplicates
        }
    }

    public async Task<bool> ExistsAsync(string url)
    {
        return await _collection.Find(a => a.Url == url).AnyAsync();
    }
}
