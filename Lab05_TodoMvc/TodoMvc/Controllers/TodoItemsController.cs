using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TodoMvc.Data;
using TodoMvc.Models;

namespace TodoMvc.Controllers;

public class TodoItemsController : Controller
{
    private readonly AppDbContext _db;

    public TodoItemsController(AppDbContext db) => _db = db;

    // GET: /TodoItems?q=...&showDone=true&sort=due_asc
    public async Task<IActionResult> Index(string? q, bool? showDone, string? sort = "created_desc")
    {
        var query = _db.TodoItems.AsNoTracking();

        // Filter by status (done / not done) — in the database
        if (showDone.HasValue)
            query = query.Where(t => t.IsDone == showDone.Value);

        // Sorting — in the database
        query = sort switch
        {
            "due_asc" => query.OrderBy(t => t.DueAt),
            "due_desc" => query.OrderByDescending(t => t.DueAt),
            "prio_asc" => query.OrderBy(t => t.Priority),
            "prio_desc" => query.OrderByDescending(t => t.Priority),
            "created_asc" => query.OrderBy(t => t.CreatedAt),
            _ => query.OrderByDescending(t => t.CreatedAt) // created_desc
        };

        var items = await query.ToListAsync();

        // Search by title/description — in memory, case-insensitive.
        // SQLite compares Cyrillic letters case-sensitively, so "спробувати" would not find "Спробувати".
        if (!string.IsNullOrWhiteSpace(q))
        {
            items = items.Where(t =>
                    t.Title.Contains(q, StringComparison.CurrentCultureIgnoreCase) ||
                    (t.Description?.Contains(q, StringComparison.CurrentCultureIgnoreCase) ?? false))
                .ToList();
        }

        ViewBag.Q = q;
        ViewBag.ShowDone = showDone;
        ViewBag.Sort = sort;
        return View(items);
    }

    // GET: /TodoItems/Create
    public IActionResult Create() => View(new TodoItem());

    // POST: /TodoItems/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(TodoItem model)
    {
        if (!ModelState.IsValid) return View(model);

        var item = new TodoItem
        {
            Title = model.Title.Trim(),
            Description = string.IsNullOrWhiteSpace(model.Description) ? null : model.Description.Trim(),
            IsDone = model.IsDone,
            DueAt = model.DueAt,
            Priority = model.Priority,
            CreatedAt = DateTime.UtcNow
        };
        _db.TodoItems.Add(item);
        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    // GET: /TodoItems/Edit/5
    public async Task<IActionResult> Edit(int id)
    {
        var item = await _db.TodoItems.FindAsync(id);
        if (item == null) return NotFound();
        return View(item);
    }

    // POST: /TodoItems/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, TodoItem model)
    {
        if (id != model.Id) return BadRequest();
        if (!ModelState.IsValid) return View(model);

        var item = await _db.TodoItems.FindAsync(id);
        if (item == null) return NotFound();

        // Copy only editable fields; CreatedAt stays unchanged
        item.Title = model.Title.Trim();
        item.Description = string.IsNullOrWhiteSpace(model.Description) ? null : model.Description.Trim();
        item.IsDone = model.IsDone;
        item.DueAt = model.DueAt;
        item.Priority = model.Priority;

        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    // POST: /TodoItems/Delete/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var item = await _db.TodoItems.FindAsync(id);
        if (item != null)
        {
            _db.TodoItems.Remove(item);
            await _db.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Index));
    }

    // POST: /TodoItems/Toggle/5 — switches IsDone
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Toggle(int id)
    {
        var item = await _db.TodoItems.FindAsync(id);
        if (item == null) return NotFound();

        item.IsDone = !item.IsDone;
        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }
}