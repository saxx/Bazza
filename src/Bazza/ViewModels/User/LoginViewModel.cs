using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Adliance.AspNetCore.Buddy;
using Adliance.Buddy.Crypto;
using Bazza.Models.Database;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;

namespace Bazza.ViewModels.User;

public class LoginViewModelFactory(Db db)
{
    public async Task<LoginViewModel> Login(LoginViewModel viewModel)
    {
        var user = await db.Users.SingleOrDefaultAsync(x => x.Username.ToLower() == viewModel.Username.ToLower());
        if (user != null)
        {
            if (string.IsNullOrWhiteSpace(user.PasswordHash) && string.IsNullOrWhiteSpace(user.PasswordSalt) && user.RequiresPasswordReset)
            {
                user.LastLoginUtc = DateTime.UtcNow;
                await db.SaveChangesAsync();

                viewModel.RequiresPasswordReset = true;
                return viewModel;
            }

            if (user.PasswordHash == Crypto.Hash(viewModel.Password, user.PasswordSalt))
            {
                user.LastLoginUtc = DateTime.UtcNow;
                await db.SaveChangesAsync();
                return viewModel;
            }
        }

        viewModel.Error = true;
        return viewModel;
    }
}

public class LoginViewModel
{
    [Required(ErrorMessage = "Bitte gib deinen Benutzernamen an.")] public string Username { get; set; } = "";
    [Required(ErrorMessage = "Bitte gib dein Passwort an.")] public string Password { get; set; } = "";

    [BindNever] public bool Error { get; set; }
    [BindNever] public bool RequiresPasswordReset { get; set; }
}