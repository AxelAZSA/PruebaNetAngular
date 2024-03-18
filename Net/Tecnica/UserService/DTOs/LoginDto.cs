using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prueba.Domain.DTOs
{
    public class LoginDto
    {
        [Required]
        public string correo { get; set; }
        [Required]
        public string password { get; set; }
    }
}
