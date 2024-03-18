using Microsoft.IdentityModel.Tokens;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System;
using System.Reflection.Emit;
using Microsoft.Extensions.Configuration;
using Prueba.Domain.Interfaces;

namespace Prueba.Application.Services.TokenService
{
    public class TokenGenerator
    {
        private readonly IConfiguration _config;

        public TokenGenerator(IConfiguration config)
        {
            _config = config;   
        }
        public string ReturnToken(string secretKey, string Issuer, string Audience, double date, List<Claim> claims = null)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            DateTime dateTimeLocal = DateTime.Now;
            DateTime dateExp = dateTimeLocal.AddMinutes(date);
            JwtSecurityToken token = new JwtSecurityToken(Issuer, Audience, claims, dateTimeLocal, dateExp, credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public string GenerateToken(IUser user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            List<Claim> claims = new List<Claim>()
            {
                 new Claim("id",user.id.ToString()),
                 new Claim(ClaimTypes.Name,user.nombre),
                 new Claim(ClaimTypes.Email,user.correo),
                 new Claim(ClaimTypes.Role, user.role)
            };

            return ReturnToken(_config["Jwt:Key"], _config["Jwt:Issuer"], _config["Jwt:Audience"], Convert.ToDouble(_config["Jwt:AccessTokenExpirationMinutes"]), claims);
        }
    }
}
