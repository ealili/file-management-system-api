using FileSystemManagementApi.Models;
using FileSystemManagementApi.Utilities.Helpers;

namespace FileSystemManagementApi.Data;

using Microsoft.AspNetCore.Identity;

public class AppDbInitializer
{
    public static async Task SeedRolesToDb(IApplicationBuilder applicationBuilder)
    {
        using (var serviceScope = applicationBuilder.ApplicationServices.CreateScope())
        {
            var roleManager = serviceScope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceScope.ServiceProvider.GetRequiredService<UserManager<User>>();

            if (!await roleManager.RoleExistsAsync(UserRoles.Admin))
            {
                await roleManager.CreateAsync(new IdentityRole(UserRoles.Admin));
            }

            if (!await roleManager.RoleExistsAsync(UserRoles.User))
            {
                await roleManager.CreateAsync(new IdentityRole(UserRoles.User));
            }

            const string adminUserEmail = "admin@mail.com";

            var adminUser = await userManager.FindByEmailAsync(adminUserEmail);
            if (adminUser == null)
            {
                adminUser = new User
                {
                    FirstName = "Admin",
                    LastName = "Role",
                    Email = adminUserEmail,
                    UserName = "user",
                    SecurityStamp = Guid.NewGuid().ToString(),
                };
                await userManager.CreateAsync(adminUser, "Password100%");
                await userManager.AddToRoleAsync(adminUser, UserRoles.Admin);
            }
        }
    }
}