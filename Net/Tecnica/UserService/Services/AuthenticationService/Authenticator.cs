using Prueba.Application.Services.TokenService;
using Prueba.Domain.Entities.Response;
using Prueba.Domain.Entities.Tokens;
using Prueba.Domain.Interfaces;
using Prueba.Domain.IRepository;

namespace Prueba.Application.Services.AuthenticationService
{
    public class Authenticator
    {
        private readonly TokenGenerator _tokenGenerator;
        private readonly RefreshTokenGenerator _refreshTokenGenerator;
        private readonly IRefreshTokenRepository _refreshTokenRepository;

        public Authenticator(TokenGenerator tokenGenerator, RefreshTokenGenerator refreshTokenGenerator, IRefreshTokenRepository refreshTokenRepository)
        {
            _tokenGenerator = tokenGenerator;
            _refreshTokenGenerator = refreshTokenGenerator;
            _refreshTokenRepository = refreshTokenRepository;
        }

        public async Task<AuthenticationResponse> Authentication(IUser existingUser)
        {
            string token = null;
            string refreshToken = null;

            token = _tokenGenerator.GenerateToken(existingUser);
            refreshToken = _refreshTokenGenerator.GenerateToken();
            RefreshToken refreshClienteToken = new RefreshToken()
            {
                token = refreshToken,
                idUser = existingUser.id,
                role = existingUser.role,
            };
            await _refreshTokenRepository.CreateRefreshToken(refreshClienteToken);

            return new AuthenticationResponse()
            {
                token = token,
                refreshToken = refreshToken
            };

        }
    }
}
