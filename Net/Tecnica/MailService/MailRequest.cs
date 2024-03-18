using System.ComponentModel.DataAnnotations;

namespace MailService
{
    public class MailRequest
    {
        [Required]
        [EmailAddress]
        public string mail { get; set; }
        [Required]
        public string contenido { get; set; } 
    }
}
