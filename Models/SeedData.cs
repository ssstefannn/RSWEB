using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RSWEB.Areas.Identity.Data;
using RSWEB.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RSWEB.Models
{
    public class SeedData
    {
        public static async Task CreateUserRoles(IServiceProvider serviceProvider)
        {
            var RoleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var UserManager = serviceProvider.GetRequiredService<UserManager<RSWEBUser>>();
            IdentityResult roleResult;
            //Add Admin Role
            var roleCheck = await RoleManager.RoleExistsAsync("Admin");
            if (!roleCheck) { roleResult = await RoleManager.CreateAsync(new IdentityRole("Admin")); }
            roleCheck = await RoleManager.RoleExistsAsync("Teacher");
            if (!roleCheck) { roleResult = await RoleManager.CreateAsync(new IdentityRole("Teacher")); }
            roleCheck = await RoleManager.RoleExistsAsync("Student");
            if (!roleCheck) { roleResult = await RoleManager.CreateAsync(new IdentityRole("Student")); }
            RSWEBUser user = await UserManager.FindByEmailAsync("admin@rsweb.com");
            if (user == null)
            {
                var User = new RSWEBUser();
                User.Email = "admin@rsweb.com";
                User.UserName = "admin@rsweb.com";
                string userPWD = "Admin123";
                IdentityResult chkUser = await UserManager.CreateAsync(User, userPWD);
                //Add default User to Role Admin
                if (chkUser.Succeeded) { var result1 = await UserManager.AddToRoleAsync(User, "Admin"); }
            }
        }

        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new ApplicationDbContext(
serviceProvider.GetRequiredService<
DbContextOptions<ApplicationDbContext>>()))
            {
                CreateUserRoles(serviceProvider).Wait();
                if (context.Teachers.Any() || context.Students.Any() || context.Courses.Any())
                {
                    return; // DB has been seeded
                }
            }
        }
    }
}
