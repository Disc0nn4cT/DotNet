using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RazorPagesYacht.Data;
using RazorPagesYacht.Models;

namespace RazorPagesYacht.Pages.Yachts;

public class IndexModel : PageModel
{
	private readonly RazorPagesYachtContext _context;

	public IndexModel(RazorPagesYachtContext context)
	{
		_context = context;
	}

	public IList<Yacht> Yacht { get; set; } = default!;

	[BindProperty(SupportsGet = true)]
	public string? SearchString { get; set; }      // text from the search box

	public SelectList? Categories { get; set; }    // options for the dropdown

	[BindProperty(SupportsGet = true)]
	public string? YachtCategory { get; set; }     // selected category

	public async Task OnGetAsync()
	{
		// Distinct list of categories for the dropdown
		IQueryable<string> categoryQuery = from y in _context.Yacht
											orderby y.Category
											select y.Category;

		// The query is only built here, not executed yet
		var yachts = from y in _context.Yacht
					 select y;

		if (!string.IsNullOrEmpty(SearchString))
		{
			yachts = yachts.Where(y => y.Name.Contains(SearchString));
		}

		if (!string.IsNullOrEmpty(YachtCategory))
		{
            // yachts = yachts.Where(y => y.Category == YachtCategory); // SEEDED #4
        }

        Categories = new SelectList(await categoryQuery.Distinct().ToListAsync());
		Yacht = await yachts.ToListAsync();   // the SQL query runs here
	}
}