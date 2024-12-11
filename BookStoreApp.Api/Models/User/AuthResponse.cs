namespace BookStoreApp.Api.Models.User;

public class AuthResponse
{
    public string UserId { get; set; } = string.Empty;
    public string Token { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}
