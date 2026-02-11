using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ContentProcessor.Models;

public class ProcessedArticle
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = string.Empty;

    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    
    [BsonElement("url")]
    public string Url { get; set; } = string.Empty;
    
    public string UrlToImage { get; set; } = string.Empty;
    public DateTime PublishedAt { get; set; }
    public string Source { get; set; } = string.Empty;
    public string Author { get; set; } = string.Empty;
    
    public string Category { get; set; } = "General";
    public List<string> Tags { get; set; } = new();
    
    public DateTime IngestedAt { get; set; } = DateTime.UtcNow;
}
