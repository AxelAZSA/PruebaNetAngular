using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prueba.Domain.Interfaces
{
    public interface IUser
    {
        public int id { get; set; }
        public string correo { get; set; }
        public string nombre { get; set; }
        public string password { get; set; }
        public string role { get; set; }
    }
}
