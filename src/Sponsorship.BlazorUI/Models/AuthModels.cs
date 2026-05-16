namespace Sponsorship.BlazorUI.Models;

public class LoginRequest
{
    public string Email { get; set; } = "";
    public string Password { get; set; } = "";
}

public class LoginResponse
{
    public string Token { get; set; } = "";
    public string FullName { get; set; } = "";
    public string Role { get; set; } = "";
    public string UserId { get; set; } = "";
}

public class CurrentUserDto
{
    public string Id { get; set; } = "";
    public string Email { get; set; } = "";
    public string FullName { get; set; } = "";
    public string Department { get; set; } = "";
    public string Role { get; set; } = "";
}
