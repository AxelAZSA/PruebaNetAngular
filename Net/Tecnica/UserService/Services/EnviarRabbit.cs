using RabbitMQ.Client;
using System.Text;

namespace UserService.Services
{
    public class EnviarRabbit
    {

        public void enviar(string correo, string contenido, string protocolo)
        {
            // Conexión a RabbitMQ
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: "cola",
                                     durable: false,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);

                string message = "\"data\": {\"mail\": "+correo+", \"contenido\": "+contenido+ ", \"protocolo\": "+protocolo+"}}";
                var body = Encoding.UTF8.GetBytes(message);

                // Publicar el mensaje en la cola
                channel.BasicPublish(exchange: "",
                                     routingKey: "cola",
                                     basicProperties: null,
                                     body: body);
                Console.WriteLine("Mensaje enviado: {0}", message);
            }
        }
    }
}
