using System.ComponentModel.DataAnnotations;

public class AuthResponse
{
    [Required]
    public string Token { get; set; }
    public DateTime Expiration { get; set; }
}