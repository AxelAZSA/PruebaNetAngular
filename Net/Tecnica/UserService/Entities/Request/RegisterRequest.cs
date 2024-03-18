using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserService.Entities.Request
{
    public class RegisterRequest
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
        public string confirmPassword { get; set; }
    }
}
