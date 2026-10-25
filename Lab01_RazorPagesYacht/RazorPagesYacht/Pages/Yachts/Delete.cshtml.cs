using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RazorPagesYacht.Models;
using RazorPagesYacht.Data;
namespace RazorPagesYacht.Pages.YachtPages;

public class DeleteModel : PageModel
{
    private readonly RazorPagesYachtContext _context;

    public DeleteModel(RazorPagesYachtContext context)
    {
        _context = context;
    }

    [BindProperty]
    public Yacht Yacht { get; set; } = default!;

    public async Task<IActionResult> OnGetAsync(int? id)
    {
        if (id is null)
        {
            return NotFound();
        }

        var yacht = await _context.Yacht.FirstOrDefaultAsync(m => m.Id == id);
        if (yacht is null)
        {
            return NotFound();
        }
        else
        {
            Yacht = yacht;
        }

        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int? id)
    {
        if (id is null)
        {
            return NotFound();
        }

        var yacht = await _context.Yacht.FindAsync(id);
        if (yacht != null)
        {
            Yacht = yacht;
            _context.Yacht.Remove(Yacht);
            await _context.SaveChangesAsync();
        }

        return RedirectToPage("./Index");
    }
}
