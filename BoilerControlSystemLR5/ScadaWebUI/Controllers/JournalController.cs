using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ScadaWebUI.Data;

namespace ScadaWebUI.Controllers;

[Authorize]
public class JournalController : Controller
{
    private readonly ApplicationDbContext _db;

    public JournalController(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<IActionResult> Index()
    {
        var events = await _db.LoginEvents
            .OrderByDescending(e => e.Id)
            .Take(100)
            .ToListAsync();

        return View(events);
    }
}