using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prueba.Domain.Entities.Tokens
{
    public class RefreshRequest
    {
        [Required]
        public string RefreshToken { get; set; }
    }
}
