using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Adliance.AspNetCore.Buddy;
using Bazza.Models.Database;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;

namespace Bazza.ViewModels.AdminUsers;

public class EditUserViewModelFactory
{
    private readonly Db _db;

    public EditUserViewModelFactory(Db db)
    {
        _db = db;
    }

    public async Task<EditUserViewModel> Build(string? username)
    {
        if (string.IsNullOrWhiteSpace(username))
        {
            return new EditUserViewModel
            {
                RequirePasswordReset = true,
                CanManageSales = true,
                CanManagePersons = true
            };
        }

        var user = await _db.Users.AsNoTracking().SingleOrDefaultAsync(x => x.Username.ToLower() == username.ToLower()) ?? throw new EntityNotFoundException();
        var result = new EditUserViewModel
        {
            CanManageAdmin = user.CanManageAdmin,
            CanManagePersons = user.CanManagePersons,
            CanManageSales = user.CanManageSales,
            Username = user.Username,
            RequirePasswordReset = user.RequiresPasswordReset
        };
        return result;
    }

    public async Task Validate(ModelStateDictionary modelState, EditUserViewModel viewModel, string? username)
    {
        if (string.IsNullOrWhiteSpace(username) && await _db.Users.AnyAsync(x => x.Username.ToLower() == viewModel.Username.ToLower()))
        {
            modelState.AddModelError(nameof(EditUserViewModel.Username), "Es existiert bereits ein Benutzer mit diesem Benutzernamen");
        }
    }

    public async Task UpdateDatabase(EditUserViewModel viewModel, string? username)
    {
        Models.Database.User user = null!;
        if (!string.IsNullOrWhiteSpace(username))
        {
            user = await _db.Users.SingleOrDefaultAsync(x => x.Username.ToLower() == username.ToLower()) ?? throw new EntityNotFoundException();
        }
        else
        {
            user = new Models.Database.User
            {
                Username = viewModel.Username
            };
            await _db.Users.AddAsync(user);
        }

        user.CanManagePersons = viewModel.CanManagePersons;
        user.CanManageSales = viewModel.CanManageSales;
        user.CanManageAdmin = viewModel.CanManageAdmin;
        user.RequiresPasswordReset = viewModel.RequirePasswordReset;

        if (!string.IsNullOrWhiteSpace(viewModel.Password))
        {
            user.PasswordHash = Crypto.Hash(viewModel.Password, out var salt);
            user.PasswordSalt = salt;
        }
        else if (string.IsNullOrWhiteSpace(user.PasswordHash))
        {
            user.PasswordHash = Crypto.Hash(Crypto.RandomString(50), out var salt);
            user.PasswordSalt = salt;
        }

        await _db.SaveChangesAsync();
    }
}

public class EditUserViewModel
{
    [Required(ErrorMessage = "Bitte angeben.")] public string Username { get; init; } = "";
    [MinLength(16, ErrorMessage = "Mindestens 16 Zeichen.")] public string? Password { get; init; }
    public bool RequirePasswordReset { get; set; }
    public bool CanManageAdmin { get; set; }
    public bool CanManagePersons { get; set; }
    public bool CanManageSales { get; set; }
}