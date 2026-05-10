using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NasaApp.Services;

namespace NasaApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NasaController : ControllerBase
    {
        private readonly NasaApiService _nasaService;

        public NasaController(NasaApiService nasaService)
        {
            _nasaService = nasaService;
        }

        [HttpGet("apod-today")]
        public async Task<IActionResult> GetApodToday()
        {
            var data = await _nasaService.GetPhotoOfTheDayAsync();
            if (data == null) return NotFound(new { Message = "Помилка зв'язку з NASA" });
            return Ok(data);
        }

        [HttpGet("apod-random")]
        public async Task<IActionResult> GetApodRandom()
        {
            var data = await _nasaService.GetRandomPhotoAsync();
            if (data == null) return NotFound(new { Message = "Помилка зв'язку з NASA" });
            return Ok(data);
        }

        [HttpGet("asteroids-today")]
        public async Task<IActionResult> GetAsteroids()
        {
            var data = await _nasaService.GetAsteroidsTodayAsync();
            if (data == null) return NotFound(new { Message = "Не вдалося отримати дані про астероїди" });
            return Ok(data);
        }
    }
}