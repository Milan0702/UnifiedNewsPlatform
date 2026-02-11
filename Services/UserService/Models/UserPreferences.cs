using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UserService.Models;

public class UserPreferences
{
    [Key]
    public Guid UserId { get; set; }

    [ForeignKey("UserId")]
    public User? User { get; set; }

    public List<string> Categories { get; set; } = new();

    public List<string> Sources { get; set; } = new();
}
