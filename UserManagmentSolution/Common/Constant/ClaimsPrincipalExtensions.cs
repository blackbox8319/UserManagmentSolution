using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using UserManagmentSolution.Models;

public static class ClaimsPrincipalExtensions
{
    public static async Task<string> GetFirstNameAsync(this ClaimsPrincipal user, UserManager<ApplicationUser> userManager)
    {
        if (!user.Identity.IsAuthenticated)
            return "";

        var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null)
            return "";

        var appUser = await userManager.FindByIdAsync(userId);
        if (appUser == null)
            return "";

        return string.IsNullOrWhiteSpace(appUser.FirstName) ? appUser.UserName : appUser.FirstName;
    }
}
