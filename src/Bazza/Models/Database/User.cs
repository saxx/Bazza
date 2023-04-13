using System;

namespace Bazza.Models.Database;

public class User
{
    public string Username { get; set; } = "";
    public string? PasswordHash { get; set; }
    public string? PasswordSalt { get; set; }
    public bool RequiresPasswordReset { get; set; }

    public bool CanManageAdmin { get; set; }
    public bool CanManagePersons { get; set; }
    public bool CanManageSales { get; set; }
    
    public DateTime? LastLoginUtc { get; set; }
    public DateTime? PasswordSetUtc { get; set; }
}