using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RazorPagesYacht.Data;
using RazorPagesYacht.Models;
using RazorPagesYacht.Services;

namespace RazorPagesYacht.Pages.Admin.Yachts;

public class DeleteModel : PageModel
{
    private readonly RazorPagesYachtContext _context;
    private readonly IWebHostEnvironment _env;

    public DeleteModel(RazorPagesYachtContext context, IWebHostEnvironment env)
    {
        _context = context;
        _env = env;
    }

    [BindProperty]
    public Yacht Yacht { get; set; } = default!;

    public async Task<IActionResult> OnGetAsync(int? id)
    {
        if (id == null) return NotFound();
        var yacht = await _context.Yacht.FirstOrDefaultAsync(y => y.Id == id);
        if (yacht == null) return NotFound();
        Yacht = yacht;
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int? id)
    {
        if (id == null) return NotFound();

        var yacht = await _context.Yacht.FindAsync(id);
        if (yacht != null)
        {
            _context.Yacht.Remove(yacht);
            await _context.SaveChangesAsync();
            PhotoHelper.TryDeleteFolder(yacht.Id, _env.WebRootPath);  // remove its photos too
        }

        return RedirectToPage("./Index");
    }
}