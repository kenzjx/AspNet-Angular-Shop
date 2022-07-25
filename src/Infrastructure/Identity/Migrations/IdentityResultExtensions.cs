using Application.Models;
using Microsoft.AspNetCore.Identity;

namespace ClassLibrary1.Identity.Migrations;

public static  class IdentityResultExtensions
{
    public static Result ToApplicationResult(this IdentityResult result)
    {
        return result.Succeeded
            ? Result.Success()
            : Result.Failure(result.Errors.Select(e => e.Description));
    }
}