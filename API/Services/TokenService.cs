using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using API.Entities;
using API.Interfaces;
using Microsoft.IdentityModel.Tokens;

namespace API.Services;

public class TokenService(IConfiguration configuration) : ITokenServices
{
    public string CreateToken(AppUsers user)
    {
        string tokenKey = configuration["TokenKey"] ?? throw new InvalidOperationException("TokenKey is missing");

        if (tokenKey.Length < 64 )
            throw new InvalidOperationException("TokenKey must be at least 64 characters long");

        SymmetricSecurityKey key = new(Encoding.UTF8.GetBytes(tokenKey));

        List<Claim> claims =
        [
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.UserName)
        ];

        SigningCredentials credentials = new(key, SecurityAlgorithms.HmacSha512Signature);

        SecurityTokenDescriptor tokenDescriptor = new()
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.Now.AddDays(3),
            SigningCredentials = credentials
        };

        JwtSecurityTokenHandler tokenHandler = new();
        
        SecurityToken token = tokenHandler.CreateToken(tokenDescriptor);

        return tokenHandler.WriteToken(token);
    }
}
