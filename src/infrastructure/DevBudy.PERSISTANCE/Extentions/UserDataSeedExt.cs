using DevBudy.DOMAIN.Entities.Concretes;
using Microsoft.EntityFrameworkCore;

namespace DevBudy.PERSISTANCE.Extentions
{
    public static class UserDataSeedExt
    {
        public static void SeedUsers(ModelBuilder builder)
        {
            AppRole roleDeveloper = new()
            {
                Id = 1,
                Name = "Developer",
                NormalizedName = "DEVELOPER",
                ConcurrencyStamp = "seed-role-developer"
            };

            AppRole roleAdmin = new()
            {
                Id = 2,
                Name = "Admin",
                NormalizedName = "ADMIN",
                ConcurrencyStamp = "seed-role-admin"
            };

            AppRole roleMember = new()
            {
                Id = 3,
                Name = "Member",
                NormalizedName = "MEMBER",
                ConcurrencyStamp = "seed-role-member"
            };

            builder.Entity<AppRole>().HasData(roleDeveloper, roleAdmin, roleMember);

            AppUserProfile developerProfile = new()
            {
                ID = 1,
                FirstName = "Demo",
                LastName = "Developer"
            };

            AppUserProfile adminProfile = new()
            {
                ID = 2,
                FirstName = "Demo",
                LastName = "Admin"
            };

            AppUserProfile memberProfile = new()
            {
                ID = 3,
                FirstName = "Demo",
                LastName = "Member"
            };

            builder.Entity<AppUserProfile>().HasData(
                developerProfile,
                adminProfile,
                memberProfile);

            AppUser userDeveloper = CreateDemoUser(
                id: 1,
                userName: "demo.developer",
                email: "developer@example.invalid",
                securityStamp: "seed-user-developer");

            AppUser userAdmin = CreateDemoUser(
                id: 2,
                userName: "demo.admin",
                email: "admin@example.invalid",
                securityStamp: "seed-user-admin");

            AppUser userMember = CreateDemoUser(
                id: 3,
                userName: "demo.member",
                email: "member@example.invalid",
                securityStamp: "seed-user-member");

            builder.Entity<AppUser>().HasData(userDeveloper, userAdmin, userMember);

            builder.Entity<AppUserRole>().HasData(
                new AppUserRole { RoleId = roleDeveloper.Id, UserId = userDeveloper.Id },
                new AppUserRole { RoleId = roleAdmin.Id, UserId = userAdmin.Id },
                new AppUserRole { RoleId = roleMember.Id, UserId = userMember.Id });
        }

        private static AppUser CreateDemoUser(
            int id,
            string userName,
            string email,
            string securityStamp)
        {
            return new AppUser
            {
                Id = id,
                UserName = userName,
                NormalizedUserName = userName.ToUpperInvariant(),
                Email = email,
                NormalizedEmail = email.ToUpperInvariant(),
                EmailConfirmed = true,
                SecurityStamp = securityStamp,
                PasswordHash = null,
                PhoneNumber = null
            };
        }
    }
}
