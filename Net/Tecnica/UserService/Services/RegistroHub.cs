using Microsoft.AspNetCore.SignalR;
using Prueba.Domain.IRepository;

namespace UserService.Services
{
    public class RegistroHub : Hub
    {
        private readonly EnviarRabbit _enviar;
        private readonly IUserRepository _repository;
        public RegistroHub(EnviarRabbit enviar, IUserRepository repository)
        {
            _enviar = enviar;
            _repository = repository;
        }
        public async Task NotificarRegistro(string nombreUsuario)
        {
            // Notificar a todos los clientes conectados
            await Clients.All.SendAsync("UsuarioRegistrado", nombreUsuario);

            await EnvioMultiple(nombreUsuario);
        }
        private async Task EnvioMultiple(string nombreUsuario)
        {
            string contenido = $"Nuevo usuario registrado {nombreUsuario}";
            var users = await _repository.GetAll();

            foreach (var user in users)
            {
                _enviar.enviar(user.correo, contenido, "notificacion");
            }
        }
    }
}
