using Microsoft.AspNetCore.Mvc;
using ScadaWebUI.Services;

namespace ScadaWebUI.Controllers
{
    public class HomeController : Controller
    {
        private readonly ISensorDataService _sensorDataService;

        public HomeController(ISensorDataService sensorDataService)
        {
            _sensorDataService = sensorDataService;
        }

        // Открытие страницы с мнемосхемой
        public IActionResult Index()
        {
            return View();
        }

        // Метод, который опрашивает JavaScript для получения актуальных показаний
        [HttpGet]
        public IActionResult GetSensorData()
        {
            var data = _sensorDataService.GetAllValues();
            return Json(data);
        }
    }
}