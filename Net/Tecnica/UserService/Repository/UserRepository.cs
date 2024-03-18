using Microsoft.EntityFrameworkCore;
using Prueba.Domain.DTOs;
using Prueba.Domain.Entities;
using Prueba.Domain.IRepository;
using Prueba.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserService.Entities.Request;

namespace Prueba.Infrastructure.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly DataContext _context;

        public UserRepository(DataContext context)
        {
            _context = context;
        }
        public async Task Create(RegisterRequest userTemp)
        {
            User user = new User()
            {
                correo = userTemp.correo,
                nombre = userTemp.nombre,
                password = userTemp.password,
                role = "user"
            };

            await _context.Users.AddAsync(user);

            try
            {
                await Save();
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }
        }


        public async Task<int> Delete(int id)
        {
            var Users = await _context.Users.FindAsync(id);

            if (Users == null)
                return 0;

            _context.Entry(Users).State = EntityState.Deleted;

            try
            {
                await Save();
                return Users.id;
            }
            catch (DbUpdateConcurrencyException)
            {
                return -1;
            }
        }

        public async Task<IEnumerable<UserDto>> GetAll()
        {
            List<UserDto> userDtos = new List<UserDto>();

            var Users = await _context.Users.Select(u => new { u.id, u.nombre, u.correo }).ToListAsync();
            foreach (var user in Users) 
            {
                UserDto dto = new UserDto()
                {
                    id = user.id,
                    nombre = user.nombre,
                    correo = user.correo
                };

                userDtos.Add(dto);
            }

            return userDtos;
        }

        public async Task<User> GetByCorreo(string correo)
        {
            return await _context.Users.FirstOrDefaultAsync(s => s.correo == correo);
        }

        public async Task<User> GetById(int id)
        {
            return await _context.Users.FindAsync(id);
        }

        public async Task Update(User user)
        {
            _context.Entry(user).State = EntityState.Modified;

            try
            {
                await Save();
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }
        }

        public async Task Save()
        {
            await _context.SaveChangesAsync();
        }
    }
}
