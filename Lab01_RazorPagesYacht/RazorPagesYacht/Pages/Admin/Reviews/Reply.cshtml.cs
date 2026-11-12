using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RazorPagesYacht.Data;
using RazorPagesYacht.Models;

namespace RazorPagesYacht.Pages.Admin.Reviews;

public class ReplyModel : PageModel
{
    private readonly RazorPagesYachtContext _context;
    private readonly UserManager<IdentityUser> _userManager;

    public ReplyModel(RazorPagesYachtContext context, UserManager<IdentityUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public Review? Review { get; private set; }

    public class ReplyInput
    {
        [Display(Name = "Відповідь адміністратора")]
        [StringLength(2000, ErrorMessage = "Максимум 2000 символів.")]
        public string? AdminReply { get; set; }
    }

    [BindProperty]
    public ReplyInput Input { get; set; } = new();

    public async Task<IActionResult> OnGetAsync(int id)
    {
        Review = await _context.Reviews.AsNoTracking()
            .Include(r => r.Yacht)
            .FirstOrDefaultAsync(r => r.Id == id);
        if (Review == null) return NotFound();

        Input.AdminReply = Review.AdminReply;   // pre-fill when editing
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int id)
    {
        var review = await _context.Reviews
            .Include(r => r.Yacht)
            .FirstOrDefaultAsync(r => r.Id == id);
        if (review == null) return NotFound();

        if (!ModelState.IsValid)
        {
            Review = review;
            return Page();
        }

        var text = string.IsNullOrWhiteSpace(Input.AdminReply) ? null : Input.AdminReply.Trim();
        var admin = await _userManager.GetUserAsync(User);

        // Empty text = remove the reply
        review.AdminReply = text;
        review.AdminReplyBy = text == null ? null : (admin?.Email ?? admin?.UserName ?? "admin");
        review.AdminRepliedAt = text == null ? null : DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return RedirectToPage("./Index");
    }
}