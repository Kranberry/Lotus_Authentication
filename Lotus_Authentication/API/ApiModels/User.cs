namespace Lotus_Authentication.API.ApiModels;

public class User
{
    public string? UserName { get; set; }
    public string Password { get; set; }    // Non nullable will be required for the API request body
    public string? Email { get; set; }  // Nullable will be optional for the API request body
}
