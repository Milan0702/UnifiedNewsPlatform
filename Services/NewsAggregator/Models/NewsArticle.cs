namespace NewsAggregator.Models;

public class NewsArticle
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public string UrlToImage { get; set; } = string.Empty;
    public DateTime PublishedAt { get; set; }
    public string Source { get; set; } = string.Empty;
    public string Author { get; set; } = string.Empty;
    public string Category { get; set; } = "General";
}

public class NewsApiResponse
{
    public string Status { get; set; } = string.Empty;
    public int TotalResults { get; set; }
    public List<NewsArticle> Articles { get; set; } = new();
}
