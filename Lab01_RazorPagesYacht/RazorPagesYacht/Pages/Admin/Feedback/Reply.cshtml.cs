using System.ComponentModel.DataAnnotations;
using System.Net;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RazorPagesYacht.Data;
using RazorPagesYacht.Services;

namespace RazorPagesYacht.Pages.Admin.Feedback;

public class ReplyModel : PageModel
{
    private readonly RazorPagesYachtContext _context;
    private readonly IEmailService _email;
    private readonly UserManager<IdentityUser> _userManager;
    private readonly ILogger<ReplyModel> _logger;

    public ReplyModel(RazorPagesYachtContext context, IEmailService email,
                      UserManager<IdentityUser> userManager, ILogger<ReplyModel> logger)
    {
        _context = context;
        _email = email;
        _userManager = userManager;
        _logger = logger;
    }

    // "Models.Feedback", because inside this namespace the name "Feedback" means the folder
    public Models.Feedback? Item { get; private set; }

    public class ReplyInput
    {
        [Required(ErrorMessage = "Вкажіть тему листа.")]
        [StringLength(200)]
        [Display(Name = "Тема листа")]
        public string Subject { get; set; } = string.Empty;

        [Required(ErrorMessage = "Напишіть текст відповіді.")]
        [StringLength(4000)]
        [Display(Name = "Текст відповіді")]
        public string Body { get; set; } = string.Empty;
    }

    [BindProperty]
    public ReplyInput Input { get; set; } = new();

    public async Task<IActionResult> OnGetAsync(int id)
    {
        Item = await _context.Feedbacks.AsNoTracking().FirstOrDefaultAsync(f => f.Id == id);
        if (Item == null) return NotFound();

        // Pre-fill: previous reply, or a "Re:" template
        Input.Subject = Item.AdminReplySubject ?? $"Re: {Item.Subject}";
        Input.Body = Item.AdminReplyBody ?? $"Вітаємо, {Item.Name}!\n\n";
        return Page();
    }

    public async Task<IActionResult> OnPostSendAsync(int id)
    {
        var f = await _context.Feedbacks.FirstOrDefaultAsync(x => x.Id == id);
        if (f == null) return NotFound();

        if (!ModelState.IsValid)
        {
            Item = f;
            return Page();
        }

        // Encode user-provided text before putting it into HTML
        var bodyHtml = WebUtility.HtmlEncode(Input.Body).Replace("\n", "<br/>");
        var html = $"<p>{bodyHtml}</p><hr/>" +
                   $"<p style=\"color:#666\">Ваше звернення: <b>{WebUtility.HtmlEncode(f.Subject)}</b></p>";

        try
        {
            await _email.SendAsync(f.Email, Input.Subject, html, Input.Body);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send reply to {Email}", f.Email);
            ModelState.AddModelError(string.Empty, "Не вдалося надіслати лист. Перевірте налаштування пошти.");
            Item = f;
            return Page();
        }

        // Mark as replied only after a successful send
        var admin = await _userManager.GetUserAsync(User);
        f.AdminReplySubject = Input.Subject;
        f.AdminReplyBody = Input.Body;
        f.AdminRepliedBy = admin?.Email ?? admin?.UserName ?? "admin";
        f.AdminRepliedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        TempData["OkMessage"] = $"Відповідь надіслано на {f.Email}.";
        return RedirectToPage("./Index");
    }
}