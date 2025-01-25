using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using FullWebApi.Application.Interfaces;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Configuration;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;

namespace FullWebApi.Application.Services;

public class TokenServices : ITokenServices
{
    private readonly IConfiguration _configuration;
    //IConfig DI
  public TokenServices(IConfiguration configuration)
  {
    _configuration = configuration;
  }

  public string GenerateToken(string username, string role)
  {
    //the ArgumentsNullExceptions are throw to avoid warnings about the chance of its value being null
    var jwtSettings = _configuration.GetSection("Jwt");
    var keyString = jwtSettings["Key"] ?? throw new ArgumentNullException("Jwt:Key", "JWT Key is not configured.");
    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(keyString));
    var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
    int expireMinutes = int.Parse(jwtSettings["ExpireMinutes"] ?? throw new ArgumentNullException("Jwt:Expire", "Expire time not configured."));
    
    var claims = new[]
    {
        new Claim(JwtRegisteredClaimNames.Sub, username),
        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        new Claim(ClaimTypes.Role, role)
    };

    var token = new JwtSecurityToken(
        issuer: jwtSettings["Issuer"],
        audience: jwtSettings["Audience"],
        claims: claims,
        expires: DateTime.Now.AddMinutes(expireMinutes),
        signingCredentials: creds
    );
    
    return new JwtSecurityTokenHandler().WriteToken(token);

  }
}
