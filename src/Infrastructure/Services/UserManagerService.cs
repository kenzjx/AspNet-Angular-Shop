using Application.Abtractions;
using Application.Models;
using ClassLibrary1.Identity.Entities;
using ClassLibrary1.Identity.Migrations;
using Microsoft.AspNetCore.Identity;

namespace ClassLibrary1.Services;

public class UserManagerService : IUserManager
{
    private readonly UserManager<ApplicationUser> _userManager;

    public UserManagerService(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<(Result Result, Guid UserId)> CreateUserAsync(string userName, string password)
    {
        var user = new ApplicationUser()
        {
            UserName = userName,
            Email = userName
        };

        var result = await _userManager.CreateAsync(user, password);

        return (result.ToApplicationResult(), user.Id);

    }

    public Task<Result> DeleteUserAsync(Guid userId)
    {
        throw new NotImplementedException();
    }
}