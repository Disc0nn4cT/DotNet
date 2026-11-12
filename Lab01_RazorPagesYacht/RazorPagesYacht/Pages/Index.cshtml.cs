using System.Linq.Expressions;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RazorPagesYacht.Data;
using RazorPagesYacht.Models;

namespace RazorPagesYacht.Pages;

public class IndexModel : PageModel
{
    private readonly RazorPagesYachtContext _context;

    public IndexModel(RazorPagesYachtContext context)
    {
        _context = context;
    }

    public record CardVM(int Id, string Name, string Category, string? PhotoPath,
                         decimal Length, double? AvgRating, int ReviewCount);

    public int YachtCount { get; private set; }
    public int ReviewCount { get; private set; }
    public int CategoryCount { get; private set; }
    public IList<CardVM> Latest { get; private set; } = new List<CardVM>();
    public IList<CardVM> TopRated { get; private set; } = new List<CardVM>();

    // One projection reused by both queries (translated to SQL by EF)
    private static readonly Expression<Func<Yacht, CardVM>> ToCard = y => new CardVM(
        y.Id, y.Name, y.Category, y.PhotoPath, y.Length,
        y.Reviews.Any() ? (double?)y.Reviews.Average(r => r.Rating) : null,
        y.Reviews.Count());

    public async Task OnGetAsync()
    {
        YachtCount = await _context.Yacht.CountAsync();
        ReviewCount = await _context.Reviews.CountAsync();
        CategoryCount = await _context.Yacht.Select(y => y.Category).Distinct().CountAsync();

        // Newest additions: the highest Id was added last
        Latest = await _context.Yacht.AsNoTracking()
            .OrderByDescending(y => y.Id)
            .Take(3)
            .Select(ToCard)
            .ToListAsync();

        // Best rated: only yachts that have reviews
        TopRated = await _context.Yacht.AsNoTracking()
            .Where(y => y.Reviews.Any())
            .OrderByDescending(y => y.Reviews.Average(r => (double)r.Rating))
            .ThenByDescending(y => y.Reviews.Count())
            .Take(3)
            .Select(ToCard)
            .ToListAsync();
    }
}