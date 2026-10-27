using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RazorPagesYacht.Models;
using RazorPagesYacht.Data;
namespace RazorPagesYacht.Pages.Yachts;

public class DetailsModel : PageModel
{
    private readonly RazorPagesYachtContext _context;
    public DetailsModel(RazorPagesYachtContext context)
    {
        _context = context;
    }

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
}
