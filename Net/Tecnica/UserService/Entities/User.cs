using Prueba.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prueba.Domain.Entities
{
    public class User : IUser
    {
        [Key]
        public int id { get; set; }
        [Required]
        public string correo { get; set; }
        [Required]
        public string nombre { get; set; }
        [Required]
        public string password { get; set; }
        [Required]
        public string role { get; set; }
    }
}
