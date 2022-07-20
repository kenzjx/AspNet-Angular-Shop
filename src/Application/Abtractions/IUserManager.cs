using Application.Models;

namespace Application.Abtractions;

public interface IUserManager
{
    Task<(Result Result, Guid UserId)> CreateUserAsync(string userName, string password);

    Task<Result> DeleteUserAsync(Guid userId);
}