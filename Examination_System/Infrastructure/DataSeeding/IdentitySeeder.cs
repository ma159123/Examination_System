using Domain.Entites;
using Domain.Enums;
using Microsoft.AspNetCore.Identity;

namespace Infrastructure.DataSeeding
{
    public static class IdentitySeeder
    {
        public static async Task SeedRolesAndAdminAsync(
            RoleManager<IdentityRole<string>> roleManager,
            UserManager<AppUser> userManager,
            string adminEmail)
        {

            foreach (var roleName in Enum.GetNames<UserRoles>())
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    await roleManager.CreateAsync(new IdentityRole<string>
                    {
                        Id = Guid.NewGuid().ToString(),
                        Name = roleName,
                        NormalizedName = roleName.ToUpper()
                    });
                }
            }

            var adminUser = await userManager.FindByEmailAsync(adminEmail);
            if (adminUser != null)
            {
                var adminRoleName = UserRoles.Admin.ToString();

                var isInRole = await userManager.IsInRoleAsync(adminUser, adminRoleName);
                if (!isInRole)
                {
                    await userManager.AddToRoleAsync(adminUser, adminRoleName);
                }
            }
        }
    }
}
