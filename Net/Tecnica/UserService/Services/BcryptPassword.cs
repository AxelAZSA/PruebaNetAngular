using Microsoft.AspNetCore.Identity;
using Prueba.Domain.Interfaces;

namespace Prueba.Application.Services
{
    public class BcryptPassword
    {
        //Encripta la contraseña
        public string Hash(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        //Verifica la contraseña
        public bool Verify(string password, string passwordHasher)
        {
            return BCrypt.Net.BCrypt.Verify(password, passwordHasher);
        }
    }
}
