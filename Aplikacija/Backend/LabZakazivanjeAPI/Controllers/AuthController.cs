namespace LabZakazivanjeAPI.Controllers;
using System.Security.Claims;
using LabZakazivanjeAPI.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly AppDBContext Context;
    private readonly IConfiguration m_config;

    public AuthController(AppDBContext context, IConfiguration config)
    {
        Context = context;
        m_config = config;
    }

    [HttpGet("authenticate-google")]
    public IActionResult AuthenticateGoogle()
    {
        Console.WriteLine("***********************");
        Console.WriteLine(Url.Action("GoogleCallBack"));

        var props = new AuthenticationProperties
        {
            RedirectUri = Url.Action("GoogleCallback")
        };

        return Challenge(props, GoogleDefaults.AuthenticationScheme);
    }

    [HttpGet("google-callback")]
    public async Task<ActionResult> GoogleCallback([FromServices] IServiceScopeFactory scopeFactory)
    {
        Console.WriteLine("Uso sam u callback");

        var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);

        if (!result.Succeeded || result.Principal == null)
        {
            return Unauthorized();
        }

        var email = result.Principal.FindFirst(ClaimTypes.Email)?.Value;
        var ime = result.Principal.FindFirst(ClaimTypes.GivenName)?.Value;
        var prezime = result.Principal.FindFirst(ClaimTypes.Surname)?.Value;


        if (email == null || ime == null || prezime == null)
        {
            return BadRequest("Email nije pronadjen!");
        }

        using var scope = scopeFactory.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDBContext>();

        var korisnik = await db.Korisnik.FirstOrDefaultAsync(k => k.Email == email);

        if (korisnik == null)
        {
            return BadRequest("Email nije pronadjen!");
        }

        // postoji korisnik, samo saljemo token da bi se ulogovao
        var token = Helpers.JwtHelper.GenerisiJwtToken(korisnik.Id, m_config["Jwt:Kljuc"]!, m_config["Jwt:Issuer"]!, m_config["jwt:Audience"]!);
        var queryToken = $"?token={Uri.EscapeDataString(token)}";

        return Redirect("https://localhost:3000" + queryToken);
    }
}
