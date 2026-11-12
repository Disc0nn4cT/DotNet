using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RazorPagesYacht.Data;
using RazorPagesYacht.Models;

namespace RazorPagesYacht.Pages.Contact;

[AllowAnonymous]
public class IndexModel : PageModel
{
    private readonly RazorPagesYachtContext _context;
    private readonly UserManager<IdentityUser> _userManager;

    public IndexModel(RazorPagesYachtContext context, UserManager<IdentityUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    // Contact info shown on the page
    public string SupportEmail => "support@rpyacht.example";
    public string Phone => "+380 (00) 000-00-00";

    public class ContactInput
    {
        [Required(ErrorMessage = "Вкажіть ім’я.")]
        [StringLength(80)]
        [Display(Name = "Ваше ім’я")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Вкажіть ел. пошту.")]
        [EmailAddress(ErrorMessage = "Некоректна адреса ел. пошти.")]
        [StringLength(120)]
        [Display(Name = "Ел. пошта")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Вкажіть тему.")]
        [StringLength(120)]
        [Display(Name = "Тема")]
        public string Subject { get; set; } = string.Empty;

        [Required(ErrorMessage = "Напишіть повідомлення.")]
        [StringLength(4000, ErrorMessage = "Максимум 4000 символів.")]
        [Display(Name = "Повідомлення")]
        public string Message { get; set; } = string.Empty;

        // Honeypot: hidden from people, bots fill it in
        public string? Website { get; set; }
    }

    [BindProperty]
    public ContactInput Input { get; set; } = new();

    [TempData]
    public string? OkMessage { get; set; }

    public async Task OnGetAsync()
    {
        // Pre-fill for logged-in users
        if (User.Identity?.IsAuthenticated == true)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user != null)
            {
                Input.Email = user.Email ?? "";
                Input.Name = user.UserName ?? "";
            }
        }
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
            return Page();

        // Simple anti-spam: a filled honeypot means a bot — pretend success, save nothing
        if (!string.IsNullOrWhiteSpace(Input.Website))
        {
            OkMessage = "Дякуємо! Ваше повідомлення надіслано.";
            return RedirectToPage();
        }

        _context.Feedbacks.Add(new Feedback
        {
            Name = Input.Name.Trim(),
            Email = Input.Email.Trim(),
            Subject = Input.Subject.Trim(),
            Message = Input.Message.Trim(),
            UserId = User.Identity?.IsAuthenticated == true ? _userManager.GetUserId(User) : null
        });
        await _context.SaveChangesAsync();

        OkMessage = "Дякуємо! Ваше повідомлення надіслано.";
        return RedirectToPage();   // Post-Redirect-Get
    }
}