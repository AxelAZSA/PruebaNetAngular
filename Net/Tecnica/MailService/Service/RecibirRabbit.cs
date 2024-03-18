using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using System.Text;
using Docker.DotNet.Models;
using System.Text.Json;
using Hangfire;

namespace MailService.Service
{
    public class RecibirRabbit
    {
        private readonly ISendMailService _service;
        public RecibirRabbit(ISendMailService service)
        {
            _service = service;
        }
        public void Recibir()
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: "cola",
                                     durable: false,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);

                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += (model, ea) =>
                {
                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);
                    Console.WriteLine("Mensaje recibido: {0}", message);

                    // Aquí analizas el mensaje para determinar qué método o endpoint llamar y cualquier otro dato necesario

                    // Llamar al método o endpoint correspondiente en el microservicio receptor
                    // Método ficticio
                    ProcesarMensaje(message);
                };
                channel.BasicConsume(queue: "mi_cola",
                                     autoAck: true,
                                     consumer: consumer);

                Console.WriteLine("Presione cualquier tecla para salir.");
                Console.ReadLine();
            }
        }
        public async Task ProcesarMensaje(string message)
        {
            // Convertir el mensaje JSON en un objeto
            var mensajeObjeto = JsonSerializer.Deserialize<Mensaje>(message);

            // Verificar que el mensaje tenga todos los campos necesarios
            if (mensajeObjeto != null && !string.IsNullOrEmpty(mensajeObjeto.correo) && !string.IsNullOrEmpty(mensajeObjeto.contenido) && !string.IsNullOrEmpty(mensajeObjeto.protocolo))
            {
                switch (mensajeObjeto.protocolo)
                {
                    case "recuperar":
                BackgroundJob.Enqueue(() => _service.SendEmail("Servicio", mensajeObjeto.correo, "Recuperacion", "Accede a la ruta Users/restablecer/ con tu nueva contraseña y confirmación \n Utilizando el isguiente token: " + mensajeObjeto.contenido));
                        break;
                    case "test":
                BackgroundJob.Enqueue(() => _service.SendEmail("Servicio", mensajeObjeto.correo, "Test", "Este es un test de envio de correo:" + mensajeObjeto.contenido));
                        break;
                    case "notificacion":
                        BackgroundJob.Enqueue(() => _service.SendEmail("Servicio", mensajeObjeto.correo, "Nuevo", "Notificacion:" + mensajeObjeto.contenido));
                        break;
                }
            }
            else
            {
                Console.WriteLine("El mensaje no contiene todos los campos necesarios.");
            }
        }

        // Clase para deserializar el mensaje JSON
        public class Mensaje
        {
            public string correo { get; set; } 
            public string contenido { get; set; } 
            public string protocolo {  get; set; }
        }
    }
}
