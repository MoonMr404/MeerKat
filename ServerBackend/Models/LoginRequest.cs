using System.ComponentModel.DataAnnotations;

namespace ServerBackend.Models;

public class LoginRequest
{
    [Required]
    public string Email { get; set; }
    [Required]
    public string Password { get; set; }
    
    public LoginRequest() {}
}