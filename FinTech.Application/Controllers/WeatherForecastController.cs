using FinTech.Domain.Entities;
using FinTech.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;


namespace FinTech.Application.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries =
        [
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        ];

        private readonly ILogger<WeatherForecastController> _logger;
        private readonly IUserRepository _userRepository;

        public WeatherForecastController(ILogger<WeatherForecastController> logger, IUserRepository userRepository)
        {
            _logger = logger;
            _userRepository = userRepository;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var user = new User("Jeremias", "jeremaios@gmail.com", "123456");

            var account = new Account("Cash", user);

            user.Accounts.Add(account);

            await _userRepository.CreateAsync(user);

            var users = _userRepository.GetAsync();

            var result = users.Result;

            var list = Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();

            return Ok(list);
        }
    }
}
