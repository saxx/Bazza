using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Adliance.AspNetCore.Buddy;
using Bazza.Models.Database;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.ClearScript.Util.Web;
using Microsoft.EntityFrameworkCore;

namespace Bazza.ViewModels.User;

public class ResetPasswordViewModelFactory
{
    private readonly Db _db;

    public ResetPasswordViewModelFactory(Db db)
    {
        _db = db;
    }

    public async Task<ResetPasswordViewModel> ResetPassword(ResetPasswordViewModel viewModel)
    {
        var user = await _db.Users.SingleOrDefaultAsync(x => x.Username.ToLower() == viewModel.Username.ToLower());
        if (user == null) throw new Exception("User does not exist.");
        if (!user.RequiresPasswordReset) throw new Exception("User cannot reset password.");

        user.PasswordHash = Crypto.Hash(viewModel.Password, out var salt);
        user.PasswordSalt = salt;
        user.PasswordSetUtc = DateTime.UtcNow;
        user.RequiresPasswordReset = false;
        await _db.SaveChangesAsync();

        viewModel.Success = true;
        return viewModel;
    }
}

public class ResetPasswordViewModel
{
    [Required(ErrorMessage = "Bitte gib deinen Benutzernamen an.")] public string Username { get; set; } = "";
    [Required(ErrorMessage = "Bitte wähle dein neues Passwort.")] public string Password { get; set; } = "";
    [Compare(otherProperty: nameof(Password), ErrorMessage = "Die beiden Passwörter stimmen nicht überein.")] public string RepeatPassword { get; set; } = "";

    [BindNever] public bool Success { get; set; }
}