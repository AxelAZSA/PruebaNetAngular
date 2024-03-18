using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prueba.Domain.Entities.Tokens
{
    public class RefreshToken
    {
        [Key]
        public int id { get; set; }
        [Required]
        public string token { get; set; }
        [Required]
        public int idUser { get; set; }
        [Required]
        public string role { get; set; }
    }
}
