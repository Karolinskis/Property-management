using Microsoft.AspNetCore.Identity;
using RentalManagement.Entities;

namespace RentalManagement.Auth;
public class AuthSeeder
{
    private readonly UserManager<User> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly IConfiguration _configuration;

    public AuthSeeder(
        UserManager<User> userManager,
        RoleManager<IdentityRole> roleManager,
        IConfiguration configuration)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _configuration = configuration;
    }
    public async Task SeedRoles(IServiceProvider serviceProvider)
    {
        await AddDefaultRolesAsync();
        await AddAdminUserAsync();

        RoleManager<IdentityRole> roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        string[] roles = new[] { UserRoles.Tenant, UserRoles.Owner, UserRoles.Admin };

        foreach (string role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }
    }

    private async Task AddAdminUserAsync()
    {
        User newAdminUser = new User()
        {
            UserName = _configuration["AdminSettings:AdminUserName"]!,
            Email = _configuration["AdminSettings:AdminEmail"]!,
        };

        User? existingAdminUser = await _userManager.FindByNameAsync(newAdminUser.UserName);
        if (existingAdminUser == null)
        {
            string adminPassword = _configuration["AdminSettings:AdminPassword"]!;
            IdentityResult createAdminUserResult = await _userManager.CreateAsync(newAdminUser, adminPassword);
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
