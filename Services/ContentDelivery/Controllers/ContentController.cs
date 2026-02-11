using ContentDeliveryService.Services;
using Microsoft.AspNetCore.Mvc;

using Microsoft.AspNetCore.Authorization;

namespace ContentDeliveryService.Controllers;

[Authorize]
[ApiController]
[Route("api/content")]
public class ContentController : ControllerBase
{
    private readonly ContentService _contentService;

    public ContentController(ContentService contentService)
    {
        _contentService = contentService;
    }

    [HttpGet("feed")]
    public async Task<IActionResult> GetFeed([FromQuery] int limit = 20)
    {
        var articles = await _contentService.GetRecentArticlesAsync(limit);
        return Ok(articles);
    }

    [HttpGet("category/{category}")]
    public async Task<IActionResult> GetByCategory(string category, [FromQuery] int limit = 20)
    {
        var articles = await _contentService.GetArticlesByCategoryAsync(category, limit);
        return Ok(articles);
    }

    [HttpGet("search")]
    public async Task<IActionResult> Search([FromQuery] string q)
    {
        if (string.IsNullOrWhiteSpace(q)) return BadRequest("Query cannot be empty");
        var articles = await _contentService.SearchArticlesAsync(q);
        return Ok(articles);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        var article = await _contentService.GetArticleByIdAsync(id);
        if (article == null) return NotFound();
        return Ok(article);
    }
}
