using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RazorPagesYacht.Models;
using RazorPagesYacht.Data;
namespace RazorPagesYacht.Pages.YachtPages;

public class IndexModel : PageModel
{
    private readonly RazorPagesYachtContext _context;

    public IndexModel(RazorPagesYachtContext context)
    {
        _context = context;
    }

    public IList<Yacht> Yacht { get; set; } = default!;

    public async Task OnGetAsync()
    {
        Yacht = await _context.Yacht.ToListAsync();
    }
}
