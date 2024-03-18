using Hangfire;
using MailService.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace MailService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MailsController : ControllerBase
    {
        private readonly ISendMailService _service;
        public MailsController(ISendMailService service) 
        {
            _service = service;
        }

        [HttpPost("test")]
        public IActionResult SendTest(string destinatario) 
        {
            BackgroundJob.Enqueue(() =>  _service.SendEmail("Si", destinatario, "test", "prueba de mail"));
            return Ok();
        }

  
        [HttpPost("recuperar")]
        public IActionResult SendPassword(MailRequest request)
        {
            BackgroundJob.Enqueue(() => _service.SendEmail("Servicio", request.mail, "Recuperacion", "Accede a la ruta Users/restablecer/ con tu nueva contraseña y confirmación \n Utilizando el isguiente token: "+request.contenido));

            return Ok("Correo electrónico encolado para envío en segundo plano.");
        }
    }
}
