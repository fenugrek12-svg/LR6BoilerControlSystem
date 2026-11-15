using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ScadaWebUI.Data;
using ScadaWebUI.Models;
using ScadaWebUI.Services;

namespace ScadaWebUI.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ISensorDataService _sensorDataService;
        private readonly ApplicationDbContext _db;

        public HomeController(
            ILogger<HomeController> logger,
            ISensorDataService sensorDataService,
            ApplicationDbContext db)
        {
            _logger = logger;
            _sensorDataService = sensorDataService;
            _db = db;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult GetSensorData()
        {
            var data = _sensorDataService.GetAllValues();
            return Json(data);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "CanReadSavings")]
        public async Task<IActionResult> Save()
        {
            string userName = User.Identity?.Name ?? "unknown";
            var data = _sensorDataService.GetAllValues();

            foreach (var kvp in data)
            {
                string topic = kvp.Key;
                double val = kvp.Value;
                string unit = topic.Contains("pressure") ? "bar" : topic.Contains("temp") ? "°C" : "%";

                _db.SensorSamples.Add(new SensorSample
                {
                    Topic = topic,
                    Value = val,
                    Unit = unit,
                    MeasuredAtUtc = DateTimeOffset.UtcNow,
                    SavedBy = userName
                });
            }

            await _db.SaveChangesAsync();
            return RedirectToAction("History");
        }

        public async Task<IActionResult> History()
        {
            var samples = await _db.SensorSamples
                .OrderByDescending(s => s.Id)
                .Take(50)
                .ToListAsync();

            return View(samples);
        }

        [AllowAnonymous]
        public IActionResult Privacy()
        {
            return View();
        }

        [AllowAnonymous]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}