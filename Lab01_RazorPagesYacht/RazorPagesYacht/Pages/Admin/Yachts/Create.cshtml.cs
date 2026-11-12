using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RazorPagesYacht.Data;
using RazorPagesYacht.Models;
using RazorPagesYacht.Services;

namespace RazorPagesYacht.Pages.Admin.Yachts;

public class CreateModel : PageModel
{
    private readonly RazorPagesYachtContext _context;
    private readonly IWebHostEnvironment _env;

    public CreateModel(RazorPagesYachtContext context, IWebHostEnvironment env)
    {
        _context = context;
        _env = env;
    }

    [BindProperty]
    public Yacht Yacht { get; set; } = default!;

    [BindProperty]
    public IFormFile? PhotoFile { get; set; }

    public IActionResult OnGet() => Page();

    public async Task<IActionResult> OnPostAsync()
    {
        // Check the file BEFORE saving anything
        var photoError = PhotoHelper.Validate(PhotoFile);
        if (photoError != null)
            ModelState.AddModelError(nameof(PhotoFile), photoError);

        if (!ModelState.IsValid)
            return Page();

        _context.Yacht.Add(Yacht);
        await _context.SaveChangesAsync();          // now Yacht.Id is known

        if (PhotoFile is { Length: > 0 })
        {
            Yacht.PhotoPath = await PhotoHelper.SaveAsync(PhotoFile, Yacht.Id, _env.WebRootPath);
            await _context.SaveChangesAsync();
        }

        return RedirectToPage("./Index");
    }
}