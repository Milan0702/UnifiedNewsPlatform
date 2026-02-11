using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UserService.Data;
using UserService.DTOs;
using UserService.Models;

namespace UserService.Controllers;

[Authorize]
[ApiController]
[Route("api/users")]
public class UserController : ControllerBase
{
    private readonly UserDbContext _context;

    public UserController(UserDbContext context)
    {
        _context = context;
    }

    [HttpGet("profile")]
    public async Task<IActionResult> GetProfile()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userId == null) return Unauthorized();

        var user = await _context.Users
            .Select(u => new { u.Id, u.Email, u.FullName, u.Role, u.CreatedAt })
            .FirstOrDefaultAsync(u => u.Id == Guid.Parse(userId));

        return Ok(user);
    }

    [HttpGet("preferences")]
    public async Task<IActionResult> GetPreferences()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userId == null) return Unauthorized();

        var prefs = await _context.UserPreferences.FindAsync(Guid.Parse(userId));
        return Ok(prefs);
    }

    [HttpPut("preferences")]
    public async Task<IActionResult> UpdatePreferences([FromBody] UpdatePreferencesDto dto)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userId == null) return Unauthorized();

        var prefs = await _context.UserPreferences.FindAsync(Guid.Parse(userId));
        if (prefs == null) return NotFound();

        prefs.Categories = dto.Categories;
        prefs.Sources = dto.Sources;

        await _context.SaveChangesAsync();
        return Ok(prefs);
    }
}

public class UpdatePreferencesDto
{
    public List<string> Categories { get; set; } = new();
    public List<string> Sources { get; set; } = new();
}
