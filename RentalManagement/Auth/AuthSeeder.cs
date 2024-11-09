using Microsoft.AspNetCore.Identity;
using RentalManagement.Entities;

namespace RentalManagement.Auth;
public class AuthSeeder
{
    private readonly UserManager<User> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public AuthSeeder(UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
    }
    public async Task SeedRoles(IServiceProvider serviceProvider)
    {
        await AddDefaultRolesAsync();
        await AddAdminUserAsync();

        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var roles = new[] { UserRoles.Tenant, UserRoles.Owner, UserRoles.Admin };

        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }
    }

    private async Task AddAdminUserAsync()
    {
        var newAdminUser = new User()
        {
            UserName = "admin",
            Email = "admin@admin.com",
        };

        var existingAdminUser = await _userManager.FindByNameAsync(newAdminUser.UserName);
        if (existingAdminUser == null)
        {
            // TODO: Get password from configuration
            var createAdminUserResult = await _userManager.CreateAsync(newAdminUser, "ChangeMePassword123!");
            if (createAdminUserResult.Succeeded)
            {
                await _userManager.AddToRolesAsync(newAdminUser, UserRoles.All);
            }
        }
    }

    private async Task AddDefaultRolesAsync()
    {
        foreach (var role in UserRoles.All)
        {
            var roleExists = await _roleManager.RoleExistsAsync(role);
            if (!roleExists)
            {
                await _roleManager.CreateAsync(new IdentityRole(role));
            }
        }
    }
}
