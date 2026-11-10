using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RazorPagesYacht.Data;
using RazorPagesYacht.Models;

namespace RazorPagesYacht.Pages.Catalog;

[AllowAnonymous] // public page
public class IndexModel : PageModel
{
    private readonly RazorPagesYachtContext _context;

    public IndexModel(RazorPagesYachtContext context)
    {
        _context = context;
    }

    public IList<Yacht> Yachts { get; private set; } = new List<Yacht>();

    [BindProperty(SupportsGet = true)]
    public string? Q { get; set; }              // search text

    [BindProperty(SupportsGet = true)]
    public string? Category { get; set; }       // selected category

    public SelectList CategoryOptions { get; private set; } = default!;

    public async Task OnGetAsync()
    {
        // 1) Unique, sorted categories for the dropdown
        var categories = await _context.Yacht.AsNoTracking()
            .Select(y => y.Category)
            .Distinct()
            .OrderBy(c => c)
            .ToListAsync();
        CategoryOptions = new SelectList(categories);

        // 2) Base query + optional filters
        var query = _context.Yacht.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(Q))
            query = query.Where(y => EF.Functions.Like(y.Name, $"%{Q}%"));

        if (!string.IsNullOrWhiteSpace(Category))
            query = query.Where(y => y.Category == Category);

        Yachts = await query.OrderBy(y => y.Name).ToListAsync();
    }
}