using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RazorPagesYacht.Data;
using RazorPagesYacht.Models;
using RazorPagesYacht.Services;

namespace RazorPagesYacht.Pages.Admin.Yachts;

public class EditModel : PageModel
{
    private readonly RazorPagesYachtContext _context;
    private readonly IWebHostEnvironment _env;

    public EditModel(RazorPagesYachtContext context, IWebHostEnvironment env)
    {
        _context = context;
        _env = env;
    }

    [BindProperty]
    public Yacht Yacht { get; set; } = default!;

    [BindProperty]
    public IFormFile? PhotoFile { get; set; }

    public async Task<IActionResult> OnGetAsync(int? id)
    {
        if (id == null) return NotFound();
        var yacht = await _context.Yacht.FirstOrDefaultAsync(y => y.Id == id);
        if (yacht == null) return NotFound();
        Yacht = yacht;
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var photoError = PhotoHelper.Validate(PhotoFile);
        if (photoError != null)
            ModelState.AddModelError(nameof(PhotoFile), photoError);

        if (!ModelState.IsValid)
            return Page();

        var dbYacht = await _context.Yacht.FirstOrDefaultAsync(y => y.Id == Yacht.Id);
        if (dbYacht == null) return NotFound();   // deleted by someone else meanwhile

        // Copy only the fields the form is allowed to change
        dbYacht.Name = Yacht.Name;
        dbYacht.Shipyard = Yacht.Shipyard;
        dbYacht.Category = Yacht.Category;
        dbYacht.LaunchDate = Yacht.LaunchDate;
        dbYacht.Price = Yacht.Price;
        dbYacht.Length = Yacht.Length;

        if (PhotoFile is { Length: > 0 })
        {
            var newPath = await PhotoHelper.SaveAsync(PhotoFile, dbYacht.Id, _env.WebRootPath);
            PhotoHelper.TryDelete(dbYacht.PhotoPath, _env.WebRootPath);  // remove the old file
            dbYacht.PhotoPath = newPath;
        }

        await _context.SaveChangesAsync();
        return RedirectToPage("./Index");
    }
}