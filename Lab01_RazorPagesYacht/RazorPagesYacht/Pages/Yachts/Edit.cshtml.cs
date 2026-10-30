using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RazorPagesYacht.Models;
using RazorPagesYacht.Data;
namespace RazorPagesYacht.Pages.Yachts;

public class EditModel : PageModel
{
    private readonly RazorPagesYachtContext _context;

    public EditModel(RazorPagesYachtContext context)
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
        Yacht = yacht;
        return Page();
    }

    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see https://aka.ms/RazorPagesCRUD.
    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        _context.Attach(Yacht).State = EntityState.Unchanged; // SEEDED #6

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!YachtExists(Yacht.Id))
            {
                return NotFound();
            }
            else
            {
                throw;
            }
        }

        return RedirectToPage("./Index");
    }

    private bool YachtExists(int id)
    {
        return _context.Yacht.Any(e => e.Id == id);
    }
}
