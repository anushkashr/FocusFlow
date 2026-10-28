namespace FocusFlow.Api.Data.Entities;

public class User : BaseEntity
{
    public string Username { get; set; } = string.Empty;

    // Never the plain password: a salted hash made by PasswordHasher
    public string PasswordHash { get; set; } = string.Empty;
}
