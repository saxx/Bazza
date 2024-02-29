using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Adliance.Buddy.Crypto;
using Bazza.Models.Database;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;

namespace Bazza.ViewModels.User;

public class ResetPasswordViewModelFactory(Db db)
{
    public async Task<ResetPasswordViewModel> FillReadonly(ResetPasswordViewModel viewModel)
    {
        var user = await db.Users.AsNoTracking().SingleOrDefaultAsync(x => x.Username.ToLower() == viewModel.Username.ToLower());
        if (user == null) viewModel.HasInvalidUser = true;
        else if (!user.RequiresPasswordReset) viewModel.CannotResetPassword = true;
        return viewModel;
    }

    public async Task<ResetPasswordViewModel> ResetPassword(ResetPasswordViewModel viewModel)
    {
        var user = await db.Users.SingleOrDefaultAsync(x => x.Username.ToLower() == viewModel.Username.ToLower());
        if (user == null) viewModel.HasInvalidUser = true;
        else if (!user.RequiresPasswordReset) viewModel.CannotResetPassword = true;
        else
        {
            user.PasswordHash = Crypto.Hash(viewModel.Password, out var salt);
            user.PasswordSalt = salt;
            user.PasswordSetUtc = DateTime.UtcNow;
            user.RequiresPasswordReset = false;
            await db.SaveChangesAsync();

            viewModel.Success = true;
        }

        return viewModel;
    }
}

public class ResetPasswordViewModel
{
    [Required(ErrorMessage = "Bitte gib deinen Benutzernamen an.")]
    public string Username { get; set; } = "";

    [Required(ErrorMessage = "Bitte wähle dein neues Passwort.")]
    public string Password { get; set; } = "";

    [Compare(otherProperty: nameof(Password), ErrorMessage = "Die beiden Passwörter stimmen nicht überein.")]
    public string RepeatPassword { get; set; } = "";

    [BindNever] public bool Success { get; set; }

    [BindNever] public bool HasInvalidUser { get; set; }
    [BindNever] public bool CannotResetPassword { get; set; }
}
