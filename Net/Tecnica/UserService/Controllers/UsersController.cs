using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Prueba.Application.Services;
using Prueba.Application.Services.AuthenticationService;
using Prueba.Application.Services.TokenService;
using Prueba.Domain.DTOs;
using Prueba.Domain.Entities;
using Prueba.Domain.Entities.Response;
using Prueba.Domain.Entities.Tokens;
using Prueba.Domain.Interfaces;
using Prueba.Domain.IRepository;
using System.ComponentModel;
using System.Security.Claims;
using UserService.Entities.Request;
using UserService.Services;

namespace Prueba.Application.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {

        private readonly IUserRepository _UserRepository;
        private readonly BcryptPassword _passwordHasher;
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly RefreshTokenValidator _refreshTokenValidator;
        private readonly Authenticator _authenticator;
        private readonly EnviarRabbit _enviar;
        private readonly RegistroHub _hub;
        public UsersController(IUserRepository UserRepository, BcryptPassword passwordHasher, IRefreshTokenRepository refreshTokenRepository, Authenticator authenticator, RefreshTokenValidator refreshTokenValidator, EnviarRabbit enviar, RegistroHub hub)
        {
            _UserRepository = UserRepository;
            _passwordHasher = passwordHasher;
            _authenticator = authenticator;
            _refreshTokenValidator = refreshTokenValidator;
            _refreshTokenRepository = refreshTokenRepository;
            _enviar = enviar;
            _hub = hub;
        }


        [Authorize(Roles = "Admin")]
        [HttpGet("all")]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _UserRepository.GetAll());
        }

        [Authorize]
        [HttpGet("MyUser")]
        public async Task<IActionResult> GetUser()
        {

            string rawUserId = HttpContext.User.FindFirstValue("id");
            if (!int.TryParse(rawUserId, out int userId))
            {
                return Unauthorized();
            }
            var user = await _UserRepository.GetById(userId);

            if (user == null)
                return NotFound();

            UserDto userDto = new UserDto()
            {
                id = user.id,
                nombre = user.nombre,
                correo = user.correo
            };

            return Ok(userDto);
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest userTemp)
        {
            if (!ModelState.IsValid)
            {
                return BadRequestModelState();
            }

            var existingUser = await _UserRepository.GetByCorreo(userTemp.correo);

            if (existingUser != null)
            {
                return Conflict(new ErrorResponse("Ya hay una cuenta con ese correo"));
            }

            if (userTemp.password != userTemp.confirmPassword)
                return BadRequest(new ErrorResponse("Las contraseñas no coinciden"));

            LoginDto login = new LoginDto()
            {
                correo = userTemp.correo,
                password = userTemp.password
            };

            userTemp.password = _passwordHasher.Hash(userTemp.password);

            await _UserRepository.Create(userTemp);
            await _hub.NotificarRegistro(userTemp.nombre);
            return await Login(login);
        }


        [Authorize]
        [HttpPut("edit/{id}")]
        public async Task<IActionResult> Put(int id, User User)
        {
            if (id != User.id)
            {
                return BadRequest();
            }

            await _UserRepository.Update(User);

            return Ok("Actualizado con exito");
        }


        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto login)
        {
            if (!ModelState.IsValid)
            {
                return BadRequestModelState();
            }

            var existingUser = await _UserRepository.GetByCorreo(login.correo);

            if (existingUser == null)
            {
                return Unauthorized();
            }

            return Ok(await _authenticator.Authentication(existingUser));
        }

        [Authorize]
        [HttpDelete("logout")]
        public async Task<IActionResult> Logout()
        {
            string rawUserId = HttpContext.User.FindFirstValue("id");
            if (!int.TryParse(rawUserId, out int userId))
            {
                return Unauthorized();
            }
            await _refreshTokenRepository.DeleteAll(userId);
            return NoContent();
        }

        [AllowAnonymous]
        [HttpPost("recuperar")]
        public async Task<IActionResult> Recuperar([FromBody] string correo)
        {

            var existingUser = await _UserRepository.GetByCorreo(correo);

            if (existingUser == null)
            {
                return Unauthorized();
            }
            var token = await _authenticator.Authentication(existingUser);
            //Consumo de api
            _enviar.enviar(correo,token.token, "recuperar");

            return Ok("Se ha enviado la confirmación al correo");
        }

        [AllowAnonymous]
        [HttpPost("test")]
        public async Task<IActionResult> test([FromBody] string correo)
        {
            //Consumo de api
            _enviar.enviar(correo, "Prueba", "test");

            return Ok("Se ha enviado la prueba al correo");
        }

        [Authorize]
        [HttpPost("restablecer")]
        public async Task<IActionResult> Restablecer([FromBody] passwordRequest passwords)
        {
            if (!ModelState.IsValid)
            {
                return BadRequestModelState();
            }

            string rawUserId = HttpContext.User.FindFirstValue("id");

            if (!int.TryParse(rawUserId, out int userId))
            {
                return Unauthorized();
            }

            var existingUser = await _UserRepository.GetById(userId);

            if (existingUser == null)
            {
                return Unauthorized();
            }

            if (passwords.password != passwords.confirmPassword)
                return BadRequest(new ErrorResponse("Las contraseñas no coinciden"));

            existingUser.password = _passwordHasher.Hash(passwords.password);

            await _UserRepository.Update(existingUser);

            return Ok("Se ha actualizado la contraseña");
        }

        [AllowAnonymous]
        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh([FromBody] RefreshRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequestModelState();
            }
            bool validateToken = _refreshTokenValidator.Validate(request.RefreshToken);
            if (!validateToken)
            {
                return BadRequest(new ErrorResponse("Invalid Refresh Token"));
            }
            RefreshToken refreshTokenDTO = await _refreshTokenRepository.GetByToken(request.RefreshToken);
            if (refreshTokenDTO == null)
            {
                return NotFound(new ErrorResponse("Invalid refresh token"));
            }
            await _refreshTokenRepository.DeleteRefreshToken(refreshTokenDTO.id);
            User user = await _UserRepository.GetById(refreshTokenDTO.idUser);
            if (User == null)
            {
                return NotFound(new ErrorResponse("User doesn´t exist"));
            }

            return Ok(await _authenticator.Authentication(user));
        }

        private IActionResult BadRequestModelState()
        {
            IEnumerable<string> errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage));
            return BadRequest(new ErrorResponse(errors));
        }
    }
}
