using Cinemory.Models;
using Microsoft.AspNetCore.Identity;

namespace Cinemory.Data
{
    // This class is used for seeding an admin user into the database during development.
    // Do not use this logic in production environments.

    public class SeedAdministrator
    {

        public static async Task InitializeAsync(IServiceProvider serviceProvider)
        {
            var userManager = serviceProvider.GetRequiredService<UserManager<AppUser>>();
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            string adminEmail = "soner@cinemory.com";
            string adminUsername = "soner";
            string adminPassword = "6565";  

            // rol yoksa oluştur
            if (!await roleManager.RoleExistsAsync("Admin"))
            {
                await roleManager.CreateAsync(new IdentityRole("Admin"));
            }

            // kullanıcı var mı kontrolü
            var adminUser = await userManager.FindByNameAsync(adminUsername);


            if (adminUser == null)
            {
                adminUser = new AppUser
                {
                    UserName = adminUsername,
                    Email = adminEmail
                };

                var result = await userManager.CreateAsync(adminUser, adminPassword);

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                }
            }
            else
            {
                // Varsa ama admin değilse, admin yap
                var roles = await userManager.GetRolesAsync(adminUser);
                if (!roles.Contains("Admin"))
                {
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                }
            }
        }
    }
}
