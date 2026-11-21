using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ScadaWebUI.Services;

namespace ScadaWebUI.Controllers;

[Authorize(Policy = "CanConfigure")]
public class SettingsController : Controller
{
    private readonly ISensorDataService _sensorService;

    public SettingsController(ISensorDataService sensorService)
    {
        _sensorService = sensorService;
    }

    public IActionResult Index()
    {
        return View(_sensorService.GetThresholds());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult UpdateMax(string topic, double max)
    {
        if (max < -50 || max > 150)
        {
            TempData["Message"] = "Threshold must be between -50 and 150.";
            return RedirectToAction("Index");
        }

        _sensorService.UpdateMax(topic, max);
        TempData["Message"] = "Threshold saved by " + User.Identity?.Name;
        return RedirectToAction("Index");
    }
}