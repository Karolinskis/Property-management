using Microsoft.AspNetCore.Identity;

namespace RentalManagement.Auth;

public class User : IdentityUser
{

}

public class RegisterUserDTO
{
    /// <example>JohnDoe</example>
    public string UserName { get; set; }
    /// <example>JohnDoe@email.com</example>
    public string Email { get; set; }
    /// <example>["Tennant", "Owner"]</example>
    public List<string> Roles { get; set; }
    /// <example>password123</example>
    public string Password { get; set; }
}

public class LoginUserDTO
{
    /// <example>JohnDoe</example>
    public string UserName { get; set; }
    /// <example>password123</example>
    public string Password { get; set; }
}

public class SuccessfulLoginDTO
{
    /// <example>eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJKb2huRG9lIiwianRpIjoi</example>
    public string AccessToken { get; set; }
}
