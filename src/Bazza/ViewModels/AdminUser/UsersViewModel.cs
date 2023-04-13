using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bazza.Models.Database;
using Microsoft.EntityFrameworkCore;

namespace Bazza.ViewModels.AdminUser;

public class UsersViewModelFactory
{
    private readonly Db _db;

    public UsersViewModelFactory(Db db)
    {
        _db = db;
    }

    public async Task<UsersViewModel> Build()
    {
        return new UsersViewModel
        {
            Users = await _db.Users
                .OrderBy(x => x.Username)
                .Select(x => new UsersViewModel.User
                {
                    CanManagePersons = x.CanManagePersons,
                    Username = x.Username,
                    CanManageAdmin = x.CanManageAdmin,
                    CanManageSales = x.CanManageSales,
                    LastLoginUtc = x.LastLoginUtc,
                    RequirePasswordReset = x.RequiresPasswordReset
                }).ToListAsync()
        };
    }
}

public class UsersViewModel
{
    public IList<User> Users { get; set; } = new List<User>();

    public record User
    {
        public string Username { get; init; } = "";
        public bool RequirePasswordReset { get; init; }
        public bool CanManageAdmin { get; init; }
        public bool CanManagePersons { get; init; }
        public bool CanManageSales { get; init; }
        public DateTime? LastLoginUtc { get; init; }
    }
}