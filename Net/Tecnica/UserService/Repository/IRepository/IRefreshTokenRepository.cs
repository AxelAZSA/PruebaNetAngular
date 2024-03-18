using Prueba.Domain.Entities.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prueba.Domain.IRepository
{
    public interface IRefreshTokenRepository
    {
        Task CreateRefreshToken(RefreshToken refresh);
        Task<RefreshToken> GetByToken(string token);
        Task DeleteRefreshToken(int tokenId);
        Task DeleteAll(int UserId);
    }
}
