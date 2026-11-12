using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RazorPagesYacht.Data;
using RazorPagesYacht.Models;

namespace RazorPagesYacht.Pages.Catalog;

[AllowAnonymous]
public class DetailsModel : PageModel
{
    private readonly RazorPagesYachtContext _context;
    private readonly UserManager<IdentityUser> _userManager;

    public DetailsModel(RazorPagesYachtContext context, UserManager<IdentityUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public Yacht? Yacht { get; private set; }
    public IList<Review> Reviews { get; private set; } = new List<Review>();
    public double AverageRating { get; private set; }
    public string? CurrentUserId { get; private set; }

    // Review form fields
    [BindProperty] public int? MyRating { get; set; }
    [BindProperty] public string? MyComment { get; set; }

    public async Task<IActionResult> OnGetAsync(int id)
    {
        if (!await LoadAsync(id)) return NotFound();

        // Pre-fill the form with the user's existing review
        var mine = Reviews.FirstOrDefault(r => r.UserId == CurrentUserId);
        if (mine != null)
        {
            MyRating = mine.Rating;
            MyComment = mine.Comment;
        }
        return Page();
    }

    // POST /Catalog/Details/{id}?handler=SaveReview
    public async Task<IActionResult> OnPostSaveReviewAsync(int id)
    {
        if (User.Identity?.IsAuthenticated != true) return Challenge(); // -> login page

        if (MyRating is null or < 1 or > 5)
            ModelState.AddModelError(nameof(MyRating), "Оберіть оцінку від 1 до 5.");
        if (MyComment?.Length > 2000)
            ModelState.AddModelError(nameof(MyComment), "Коментар задовгий (максимум 2000 символів).");

        if (!ModelState.IsValid)
        {
            if (!await LoadAsync(id)) return NotFound();
            return Page();
        }

        if (!await _context.Yacht.AnyAsync(y => y.Id == id)) return NotFound();

        var user = await _userManager.GetUserAsync(User);
        if (user == null) return Challenge();

        var comment = string.IsNullOrWhiteSpace(MyComment) ? null : MyComment.Trim();
        var existing = await _context.Reviews
            .FirstOrDefaultAsync(r => r.YachtId == id && r.UserId == user.Id);

        if (existing == null)
        {
            _context.Reviews.Add(new Review
            {
                YachtId = id,
                UserId = user.Id,
                ReviewerName = user.Email ?? user.UserName,
                Rating = MyRating!.Value,
                Comment = comment
            });
        }
        else
        {
            existing.Rating = MyRating!.Value;
            existing.Comment = comment;
            existing.UpdatedAt = DateTime.UtcNow;
        }

        await _context.SaveChangesAsync();
        return RedirectToPage(new { id });   // Post-Redirect-Get
    }

    // POST /Catalog/Details/{id}?handler=DeleteReview
    public async Task<IActionResult> OnPostDeleteReviewAsync(int id, int reviewId)
    {
        if (User.Identity?.IsAuthenticated != true) return Challenge();

        var review = await _context.Reviews
            .FirstOrDefaultAsync(r => r.Id == reviewId && r.YachtId == id);
        if (review == null) return NotFound();

        var isOwner = review.UserId == _userManager.GetUserId(User);
        if (!isOwner && !User.IsInRole("Admin")) return Forbid();   // 403

        _context.Reviews.Remove(review);
        await _context.SaveChangesAsync();
        return RedirectToPage(new { id });
    }

    private async Task<bool> LoadAsync(int id)
    {
        Yacht = await _context.Yacht.AsNoTracking()
            .Include(y => y.Reviews)                  // load reviews together with the yacht
            .FirstOrDefaultAsync(y => y.Id == id);
        if (Yacht == null) return false;

        Reviews = Yacht.Reviews.OrderByDescending(r => r.UpdatedAt ?? r.CreatedAt).ToList();
        AverageRating = Reviews.Count > 0 ? Reviews.Average(r => r.Rating) : 0;
        CurrentUserId = _userManager.GetUserId(User);
        return true;
    }
}