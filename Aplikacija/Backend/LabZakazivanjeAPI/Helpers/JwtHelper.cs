namespace LabZakazivanjeAPI.Helpers;

using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Reflection.Metadata.Ecma335;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore.Storage.Json;
using Microsoft.IdentityModel.Tokens;

public static class JwtHelper
{
    public static string GenerisiJwtToken(int korisnikId, string jwtKljuc, string jwtIssuer, string jwtAudience)
    {
        Console.WriteLine($"Kljuc: {jwtKljuc} issuer: {jwtIssuer}");

        var claims = new[]
        {
            new Claim("korisnikId", korisnikId.ToString())
        };

        var kljuc = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKljuc));
        var creds = new SigningCredentials(kljuc, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: jwtIssuer,
            audience: jwtAudience,
            claims: claims,
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}