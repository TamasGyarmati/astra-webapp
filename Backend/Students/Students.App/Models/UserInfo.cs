namespace Students.App.Models;

public class UserInfo
{
    public string UserName { get; set; } = string.Empty;
    public string Id { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string[] Roles { get; set; } = [];
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
}
