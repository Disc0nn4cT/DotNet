using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RazorPagesYacht.Data;
using Microsoft.EntityFrameworkCore;
using RazorPagesYacht.Models;

namespace RazorPagesYacht.Pages.YachtPages;

public class CreateModel : PageModel
{
    private readonly RazorPagesYachtContext _context;

    public CreateModel(RazorPagesYachtContext context)
    {
        _context = context;
    }

    public IActionResult OnGet()
    {
        return Page();
    }

    [BindProperty]
    public Yacht Yacht { get; set; } = default!;

    // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD.
    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        _context.Yacht.Add(Yacht);
        await _context.SaveChangesAsync();

        return RedirectToPage("./Index");
    }
}
