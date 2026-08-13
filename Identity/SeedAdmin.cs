using HostelManagementSystem.Data;
using HostelManagementSystem.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace HostelManagementSystem.Identity
{
    public static class SeedAdmin
    {
        public static async Task Initialize(
            IServiceProvider serviceProvider)
        {
            var userManager = serviceProvider
                .GetRequiredService<UserManager<IdentityUser>>();

            var roleManager = serviceProvider
                .GetRequiredService<RoleManager<IdentityRole>>();

            var dbContext = serviceProvider
                .GetRequiredService<ApplicationDbContext>();


            // ==========================================
            // ADMIN DETAILS
            // ==========================================

            string adminEmail = "admin@hostel.com";
            string adminPassword = "Admin123@";


            // ==========================================
            // CREATE ADMIN ROLE
            // ==========================================

            if (!await roleManager.RoleExistsAsync("Admin"))
            {
                var roleResult = await roleManager
                    .CreateAsync(new IdentityRole("Admin"));

                if (!roleResult.Succeeded)
                {
                    throw new Exception(
                        "Unable to create Admin role.");
                }
            }


            // ==========================================
            // FIND ADMIN IDENTITY ACCOUNT
            // ==========================================

            var adminUser = await userManager
                .FindByEmailAsync(adminEmail);


            // ==========================================
            // CREATE ADMIN IDENTITY ACCOUNT
            // ==========================================

            if (adminUser == null)
            {
                adminUser = new IdentityUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true
                };

                var result = await userManager
                    .CreateAsync(adminUser, adminPassword);

                if (!result.Succeeded)
                {
                    var errors = string.Join(
                        ", ",
                        result.Errors.Select(e => e.Description));

                    throw new Exception(
                        $"Unable to create admin account: {errors}");
                }
            }


            // ==========================================
            // MAKE SURE ADMIN HAS ADMIN ROLE
            // ==========================================

            if (!await userManager.IsInRoleAsync(
                    adminUser,
                    "Admin"))
            {
                await userManager
                    .AddToRoleAsync(adminUser, "Admin");
            }


            // ==========================================
            // CREATE ADMIN PROFILE
            // ==========================================

            var adminProfile = await dbContext.AdminProfiles
                .FirstOrDefaultAsync(
                    a => a.IdentityUserId == adminUser.Id);


            if (adminProfile == null)
            {
                adminProfile = new AdminProfile
                {
                    IdentityUserId = adminUser.Id,

                    FullName = "System Administrator",

                    Email = adminUser.Email,

                    ProfilePicture = "/images/default-user.png"
                };

                dbContext.AdminProfiles.Add(adminProfile);

                await dbContext.SaveChangesAsync();
            }
        }
    }
}