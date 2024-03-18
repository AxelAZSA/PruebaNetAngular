using Microsoft.Extensions.Configuration;
using System;

namespace Prueba.Application.Services.TokenService
{
    public class RefreshTokenGenerator
    {
        private readonly IConfiguration _config;
        private readonly TokenGenerator _generator;

        public RefreshTokenGenerator(IConfiguration config, TokenGenerator generator)
        {
            _config = config;
            _generator = generator;
        }
        public string GenerateToken()
        {
            return _generator.ReturnToken(_config["Jwt:RefreshKey"], _config["Jwt:Issuer"], _config["Jwt:Audience"], Convert.ToDouble(_config["Jwt:RefreshTokenExpirationMinutes"]));
        }
    }
}
