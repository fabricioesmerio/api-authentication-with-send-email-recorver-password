using System.ComponentModel.DataAnnotations;

namespace apiLogin.Dto
{
    public class ValidateCode
    {
        [Required]
        public int Codigo { get; set; }
        [Required]
        public string Email { get; set; } = string.Empty;
    }
}