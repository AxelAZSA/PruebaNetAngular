using System.ComponentModel.DataAnnotations;

namespace UserService.Entities.Request
{
    public class passwordRequest
    {
        [Required]
        public string password { get; set; }
        [Required]
        public string confirmPassword { get; set; }
    }
}
