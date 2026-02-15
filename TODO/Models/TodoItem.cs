using System.ComponentModel.DataAnnotations;

namespace TODO.Models;

public class TodoItem
{
    public int Id { get; set; }

    [Required]
    [StringLength(80)]
    public string Title { get; set; } = string.Empty;

    [StringLength(240)]
    public string? Notes { get; set; }

    public bool IsCompleted { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }
}
