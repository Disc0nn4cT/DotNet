using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RazorPagesYacht.Data;

namespace RazorPagesYacht.Pages.Admin.Feedback;

public class IndexModel : PageModel
{
    private readonly RazorPagesYachtContext _context;

    public IndexModel(RazorPagesYachtContext context)
    {
        _context = context;
    }

    public record RowVM(int Id, string Name, string Email, string Subject, string Message,
        DateTime CreatedAt, DateTime? RepliedAt);

    public IList<RowVM> Items { get; private set; } = new List<RowVM>();

    [BindProperty(SupportsGet = true)]
    public string? Q { get; set; }

    [BindProperty(SupportsGet = true)]
    public bool OnlyUnreplied { get; set; }

    [TempData]
    public string? OkMessage { get; set; }   // set by the Reply page after sending

    public async Task OnGetAsync()
    {
        var query = _context.Feedbacks.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(Q))
        {
            var like = $"%{Q}%";
            query = query.Where(f =>
                EF.Functions.Like(f.Name, like) ||
                EF.Functions.Like(f.Email, like) ||
                EF.Functions.Like(f.Subject, like) ||
                EF.Functions.Like(f.Message, like));
        }

        if (OnlyUnreplied)
            query = query.Where(f => f.AdminRepliedAt == null);

        Items = await query
            .OrderByDescending(f => f.CreatedAt)
            .Select(f => new RowVM(f.Id, f.Name, f.Email, f.Subject, f.Message, f.CreatedAt, f.AdminRepliedAt))
            .ToListAsync();
    }
}