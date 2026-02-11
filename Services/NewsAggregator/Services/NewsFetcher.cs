using NewsAggregator.Models;
using Newtonsoft.Json;
using Polly;
using Polly.Retry;

namespace NewsAggregator.Services;

public class NewsFetcher
{
    private readonly HttpClient _httpClient;
    private readonly AsyncRetryPolicy<HttpResponseMessage> _retryPolicy;
    private readonly ILogger<NewsFetcher> _logger;
    private readonly string _apiKey;

    public NewsFetcher(HttpClient httpClient, IConfiguration configuration, ILogger<NewsFetcher> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
        _apiKey = configuration["NewsApi:Key"] ?? "mock-key";

        _retryPolicy = Policy
            .HandleResult<HttpResponseMessage>(r => !r.IsSuccessStatusCode)
            .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                (result, timeSpan, retryCount, context) =>
                {
                    _logger.LogWarning($"Request failed with {result.Result.StatusCode}. Waiting {timeSpan} before next retry. Retry attempt {retryCount}");
                });
    }

    public async Task<List<NewsArticle>> FetchNewsAsync()
    {
        // Mock data if no API key
        if (_apiKey == "mock-key" || string.IsNullOrEmpty(_apiKey))
        {
            return GenerateMockNews();
        }

        var allArticles = new List<NewsArticle>();
        var categories = new[] { "technology", "business", "sports", "general" };

        foreach (var category in categories)
        {
            try
            {
                // Add delay to respect rate limits (free tier is limited)
                await Task.Delay(1000);

                var response = await _retryPolicy.ExecuteAsync(() => 
                    _httpClient.GetAsync($"https://newsapi.org/v2/top-headlines?country=us&category={category}&apiKey={_apiKey}"));

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var newsResponse = JsonConvert.DeserializeObject<NewsApiResponseDto>(content);
                    
                    if (newsResponse?.Articles != null)
                    {
                        foreach (var dto in newsResponse.Articles)
                        {
                            var article = new NewsArticle
                            {
                                Title = dto.Title,
                                Description = dto.Description,
                                Url = dto.Url,
                                UrlToImage = dto.UrlToImage,
                                PublishedAt = dto.PublishedAt ?? DateTime.UtcNow,
                                Source = dto.Source?.Name ?? "Unknown",
                                Author = dto.Author,
                                Category = char.ToUpper(category[0]) + category.Substring(1)
                            };
                            allArticles.Add(article);
                        }
                    }
                }
                else
                {
                    _logger.LogWarning($"Failed to fetch {category} news: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error fetching {category} news");
            }
        }

        return allArticles;
    }

    private List<NewsArticle> GenerateMockNews()
    {
        var articles = new List<NewsArticle>();
        var random = new Random();
        var categories = new[] { "Tech", "Business", "Sports", "General" };
        
        for (int i = 0; i < 10; i++)
        {
            var category = categories[random.Next(categories.Length)];
            articles.Add(new NewsArticle
            {
                Title = $"Mock {category} News {random.Next(1000)}",
                Description = $"This is a generated mock {category} news article description.",
                Url = $"https://example.com/news/{category.ToLower()}/{random.Next(10000)}",
                Source = "Mock Source",
                PublishedAt = DateTime.UtcNow,
                Author = "Mock Author",
                Category = category
            });
        }
        return articles;
    }

    // DTOs for NewsAPI response
    private class NewsApiResponseDto
    {
        public string Status { get; set; } = string.Empty;
        public int TotalResults { get; set; }
        public List<NewsApiArticleDto> Articles { get; set; } = new();
    }

    private class NewsApiArticleDto
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
        public string UrlToImage { get; set; } = string.Empty;
        public DateTime? PublishedAt { get; set; }
        public NewsApiSourceDto? Source { get; set; }
        public string Author { get; set; } = string.Empty;
    }

    private class NewsApiSourceDto
    {
        public string? Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}
