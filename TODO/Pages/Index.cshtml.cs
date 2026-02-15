using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using TODO.Data;
using TODO.Models;

namespace TODO.Pages;

public class IndexModel : PageModel
{
    private readonly TodoDbContext _context;

    public IndexModel(TodoDbContext context)
    {
        _context = context;
    }

    [BindProperty]
    public TodoInput Input { get; set; } = new();

    public IList<TodoItem> Todos { get; private set; } = [];

    public async Task OnGetAsync(int? editId)
    {
        await LoadTodosAsync();

        if (editId.HasValue)
        {
            var existing = Todos.FirstOrDefault(t => t.Id == editId.Value);
            if (existing != null)
            {
                Input = new TodoInput
                {
                    Id = existing.Id,
                    Title = existing.Title,
                    Notes = existing.Notes
                };
            }
        }
    }

    public async Task<IActionResult> OnPostSaveAsync()
    {
        if (!ModelState.IsValid)
        {
            await LoadTodosAsync();
            return Page();
        }

        if (Input.Id.HasValue)
        {
            var existing = await _context.TodoItems.FindAsync(Input.Id.Value);
            if (existing == null)
            {
                return NotFound();
            }

            existing.Title = Input.Title.Trim();
            existing.Notes = NormalizeNotes(Input.Notes);
            existing.UpdatedAt = DateTime.UtcNow;
        }
        else
        {
            var todo = new TodoItem
            {
                Title = Input.Title.Trim(),
                Notes = NormalizeNotes(Input.Notes),
                CreatedAt = DateTime.UtcNow
            };

            _context.TodoItems.Add(todo);
        }

        await _context.SaveChangesAsync();
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostToggleAsync(int id)
    {
        var todo = await _context.TodoItems.FirstOrDefaultAsync(t => t.Id == id);
        if (todo == null)
        {
            return NotFound();
        }

        todo.IsCompleted = !todo.IsCompleted;
        todo.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostDeleteAsync(int id)
    {
        var todo = await _context.TodoItems.FirstOrDefaultAsync(t => t.Id == id);
        if (todo == null)
        {
            return NotFound();
        }

        _context.TodoItems.Remove(todo);
        await _context.SaveChangesAsync();
        return RedirectToPage();
    }

    private async Task LoadTodosAsync()
    {
        Todos = await _context.TodoItems
            .OrderByDescending(t => t.CreatedAt)
            .ThenBy(t => t.Id)
            .AsNoTracking()
            .ToListAsync();
    }

    private static string? NormalizeNotes(string? notes) =>
        string.IsNullOrWhiteSpace(notes) ? null : notes.Trim();

    public class TodoInput
    {
        public int? Id { get; set; }

        [Required]
        [StringLength(80)]
        public string Title { get; set; } = string.Empty;

        [StringLength(240)]
        public string? Notes { get; set; }
    }
}
