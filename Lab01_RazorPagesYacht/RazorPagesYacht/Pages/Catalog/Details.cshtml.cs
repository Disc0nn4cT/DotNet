using Microsoft.AspNetCore.Authorization;
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

    public DetailsModel(RazorPagesYachtContext context)
    {
        _context = context;
    }

    public Yacht? Yacht { get; private set; }

    public async Task<IActionResult> OnGetAsync(int id)
    {
        Yacht = await _context.Yacht.AsNoTracking().FirstOrDefaultAsync(y => y.Id == id);
        if (Yacht == null) return NotFound();
        return Page();
    }
}