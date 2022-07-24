using System;

namespace Bazza.Models.Database;

public class User
{
    public string Username { get; set; } = "";
    public string? PasswordHash { get; set; }
    public string? PasswordSalt { get; set; }
    public bool RequiresPasswordReset { get; set; }

    public DateTime? LastLoginUtc { get; set; }
    public DateTime? PasswordSetUtc { get; set; }
}