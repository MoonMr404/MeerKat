using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using ServerBackend.Models;

namespace ServerBackend.Helpers;

public class JwtHelper
{
    private static string _jwtSecret = "MeerKat è un software di gestione aziendale";
    private static string[] _admins = new[] { "greenylie12@gmail.com" }; //Approccio errato solo per velocizzare
    
    public static string GenerateJwtToken(User user)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSecret));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Name, user.Name),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
        };

        var token = new JwtSecurityToken(
            issuer: "MeerKat",
            audience: "mircats",
            claims: claims,
            expires: DateTime.Now.AddMinutes(15), // Token expiration
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
    
    public static bool IsAdmin(ClaimsPrincipal user)
    {
        return _admins.Contains(user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value);
    }

    public static bool IsAmongSelf(ClaimsPrincipal user, Guid id)
    {
        return user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value == id.ToString();
    }
}