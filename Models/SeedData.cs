﻿using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SD_340_W22SD_Final_Project_Group6.Data;

namespace SD_340_W22SD_Final_Project_Group6.Models
{
    public static class SeedData
    {
        public async static Task Initialize(IServiceProvider serviceProvider)
        {
            ApplicationDbContext context
                = new ApplicationDbContext(serviceProvider
                .GetRequiredService<DbContextOptions<ApplicationDbContext>>());

            RoleManager<IdentityRole> roleManager =
                serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            UserManager<ApplicationUser> userManager =
                serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            List<string> roles = new List<string>
            {
                "ProjectManager", "Developer", "Admin"
            };

            if (!context.Roles.Any())
            {
                foreach (string role in roles)
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
                await context.SaveChangesAsync();
            }

            if (!context.Users.Any())
            {
                ApplicationUser seedProjectManagerUser = new ApplicationUser
                {
                    Email = "pm@jello.com",
                    NormalizedEmail = "PM@JELLO.COM",
                    UserName = "pm@jello.com",
                    NormalizedUserName = "PM@JELLO.COM",
                    Name = "Gus Jello",
                    EmailConfirmed = true,
                };

                var password1 = new PasswordHasher<ApplicationUser>();
                var hashed1 = password1.HashPassword(seedProjectManagerUser, "P@ssW0rd");
                seedProjectManagerUser.PasswordHash = hashed1;

                await userManager.CreateAsync(seedProjectManagerUser);
                await userManager.AddToRoleAsync(seedProjectManagerUser, "ProjectManager");

                ApplicationUser seedDeveloperUser1 = new ApplicationUser
                {
                    Email = "dev1@jello.com",
                    NormalizedEmail = "DEV1@JELLO.COM",
                    UserName = "dev1@jello.com",
                    NormalizedUserName = "DEV1@JELLO.COM",
                    EmailConfirmed = true,
                    Name = "Pinkman Jello"
                };

                var password2 = new PasswordHasher<ApplicationUser>();
                var hashed2 = password2.HashPassword(seedDeveloperUser1, "P@ssW0rd");
                seedDeveloperUser1.PasswordHash = hashed2;

                await userManager.CreateAsync(seedDeveloperUser1);
                await userManager.AddToRoleAsync(seedDeveloperUser1, "Developer");

                await context.SaveChangesAsync();

                Project newProj1 = new Project();
                newProj1.ProjectName = "Jellow Shots";
                newProj1.CreatedBy = seedProjectManagerUser;

                Project newProj2 = new Project();
                newProj2.ProjectName = "You can't see me Jello";
                newProj2.CreatedBy = seedProjectManagerUser;

                await context.Projects.AddAsync(newProj1);
                await context.Projects.AddAsync(newProj2);

                await context.SaveChangesAsync();
            }


        }
    }
}
