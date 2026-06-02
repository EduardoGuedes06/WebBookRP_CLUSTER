using System.ComponentModel.DataAnnotations;

namespace WebBookRP_API.DTOs
{
    public class GoogleLoginRequestDto
    {
        [Required]
        public string Credential { get; set; } = string.Empty;
    }
}
