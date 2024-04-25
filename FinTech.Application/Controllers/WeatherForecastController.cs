using FinTech.Domain.Entities;
using FinTech.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;


namespace FinTech.Application.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase
{
    private readonly ILogger<WeatherForecastController> _logger;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly UserManager<ApplicationUser> _userManager;

    public WeatherForecastController(
        ILogger<WeatherForecastController> logger, 
        SignInManager<ApplicationUser> signInManager,
        UserManager<ApplicationUser> userManager
    )
    {
        _logger = logger;
        _signInManager = signInManager;
        _userManager = userManager;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> Get()
    {
        var user = new ApplicationUser { Email = "jereleao@gmail.com", UserName = "Jere" };

        user.PasswordHash = _userManager.PasswordHasher.HashPassword(user, "passS2");

        await _userManager.CreateAsync(user);

        await _signInManager.SignInAsync(user, isPersistent: false);

        //var account = new Account("Cash", user);

        //user.Accounts.Add(account);

        //await _userService.CreateAsync(user);

        //var users = _userService.GetAsync();

        //var result = users.Result;

        return Ok("OK");
    }

    [HttpGet]
    [Authorize]
    [Route("au")]
    public async Task<IActionResult> GetAuthorized()
    {
        var loggedUser = await _userManager.GetUserAsync(User);

        var account = new Account("Cash", loggedUser);

        //var saved = await _accountRepository.CreateAsync(account);

        //await _userService.CreateAsync(user);

        //var users = _userService.GetAsync();

        //var result = users.Result;

        return Ok("OK");
    }
}
