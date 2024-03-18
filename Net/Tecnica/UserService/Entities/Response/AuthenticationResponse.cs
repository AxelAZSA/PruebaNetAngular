using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prueba.Domain.Entities.Response
{
    public class AuthenticationResponse
    {
        public string token { get; set; }
        public string refreshToken { get; set; }
    }
}
