using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RazorPagesYacht.Data;

namespace RazorPagesYacht.Pages.Admin.Reviews;

public class IndexModel : PageModel
{
    private readonly RazorPagesYachtContext _context;

    public IndexModel(RazorPagesYachtContext context)
    {
        _context = context;
    }

    // One table row: only the data the page needs
    public record RowVM(
        int ReviewId,
        int YachtId,
        string YachtName,
        string ReviewerName,
        int Rating,
        string? Comment,
        DateTime CreatedAt,
        string? AdminReply,
        DateTime? AdminRepliedAt);

    public IList<RowVM> Items { get; private set; } = new List<RowVM>();

    [BindProperty(SupportsGet = true)]
    public string? Q { get; set; }                 // search by yacht / author / text

    [BindProperty(SupportsGet = true)]
    public bool OnlyUnreplied { get; set; }        // show only reviews without a reply

    public async Task OnGetAsync()
    {
        var query = _context.Reviews.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(Q))
        {
            var like = $"%{Q}%";
            query = query.Where(r =>
                EF.Functions.Like(r.Yacht!.Name, like) ||
                EF.Functions.Like(r.ReviewerName!, like) ||
                EF.Functions.Like(r.Comment!, like));
        }

        if (OnlyUnreplied)
            query = query.Where(r => r.AdminReply == null || r.AdminReply == "");

        Items = await query
            .OrderByDescending(r => r.UpdatedAt ?? r.CreatedAt)
            .Select(r => new RowVM(
                r.Id, r.YachtId, r.Yacht!.Name,
                r.ReviewerName ?? "Користувач",
                r.Rating, r.Comment, r.CreatedAt,
                r.AdminReply, r.AdminRepliedAt))
            .ToListAsync();
    }
}