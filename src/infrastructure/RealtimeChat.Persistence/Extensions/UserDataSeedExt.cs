using RealtimeChat.Domain.Entities.Concretes;
using Microsoft.EntityFrameworkCore;

namespace RealtimeChat.Persistence.Extensions
{
    public static class UserDataSeedExt
    {
        // HasData tarihleri snapshot ile sabit kalmalı; çalışma anında üretilen değerler migration farkı oluşturur.
        private static readonly DateTime RoleDeveloperCreatedAt = SeedDate(276, 5318);
        private static readonly DateTime RoleAdminCreatedAt = SeedDate(276, 5402);
        private static readonly DateTime RoleMemberCreatedAt = SeedDate(276, 5406);
        private static readonly DateTime DeveloperProfileCreatedAt = SeedDate(276, 5578);
        private static readonly DateTime AdminProfileCreatedAt = SeedDate(276, 5580);
        private static readonly DateTime MemberProfileCreatedAt = SeedDate(276, 5581);
        private static readonly DateTime DeveloperUserCreatedAt = SeedDate(276, 5606);
        private static readonly DateTime AdminUserCreatedAt = SeedDate(340, 8462);
        private static readonly DateTime MemberUserCreatedAt = SeedDate(405, 1029);
        private static readonly DateTime DeveloperUserRoleCreatedAt = SeedDate(470, 4265);
        private static readonly DateTime AdminUserRoleCreatedAt = SeedDate(470, 4282);
        private static readonly DateTime MemberUserRoleCreatedAt = SeedDate(470, 4283);

        public static void SeedUsers(ModelBuilder builder)
        {
            AppRole roleDeveloper = new()
            {
                Id = 1,
                Name = "Developer",
                NormalizedName = "DEVELOPER",
                ConcurrencyStamp = "seed-role-developer",
                CreatedDate = RoleDeveloperCreatedAt
            };

            AppRole roleAdmin = new()
            {
                Id = 2,
                Name = "Admin",
                NormalizedName = "ADMIN",
                ConcurrencyStamp = "seed-role-admin",
                CreatedDate = RoleAdminCreatedAt
            };

            AppRole roleMember = new()
            {
                Id = 3,
                Name = "Member",
                NormalizedName = "MEMBER",
                ConcurrencyStamp = "seed-role-member",
                CreatedDate = RoleMemberCreatedAt
            };

            builder.Entity<AppRole>().HasData(roleDeveloper, roleAdmin, roleMember);

            AppUserProfile developerProfile = new()
            {
                ID = 1,
                FirstName = "Demo",
                LastName = "Developer",
                CreatedDate = DeveloperProfileCreatedAt
            };

            AppUserProfile adminProfile = new()
            {
                ID = 2,
                FirstName = "Demo",
                LastName = "Admin",
                CreatedDate = AdminProfileCreatedAt
            };

            AppUserProfile memberProfile = new()
            {
                ID = 3,
                FirstName = "Demo",
                LastName = "Member",
                CreatedDate = MemberProfileCreatedAt
            };

            builder.Entity<AppUserProfile>().HasData(
                developerProfile,
                adminProfile,
                memberProfile);

            AppUser userDeveloper = CreateDemoUser(
                id: 1,
                userName: "demo.developer",
                email: "developer@example.invalid",
                securityStamp: "seed-user-developer",
                createdDate: DeveloperUserCreatedAt);

            AppUser userAdmin = CreateDemoUser(
                id: 2,
                userName: "demo.admin",
                email: "admin@example.invalid",
                securityStamp: "seed-user-admin",
                createdDate: AdminUserCreatedAt);

            AppUser userMember = CreateDemoUser(
                id: 3,
                userName: "demo.member",
                email: "member@example.invalid",
                securityStamp: "seed-user-member",
                createdDate: MemberUserCreatedAt);

            builder.Entity<AppUser>().HasData(userDeveloper, userAdmin, userMember);

            builder.Entity<AppUserRole>().HasData(
                new AppUserRole
                {
                    RoleId = roleDeveloper.Id,
                    UserId = userDeveloper.Id,
                    CreatedDate = DeveloperUserRoleCreatedAt
                },
                new AppUserRole
                {
                    RoleId = roleAdmin.Id,
                    UserId = userAdmin.Id,
                    CreatedDate = AdminUserRoleCreatedAt
                },
                new AppUserRole
                {
                    RoleId = roleMember.Id,
                    UserId = userMember.Id,
                    CreatedDate = MemberUserRoleCreatedAt
                });
        }

        private static AppUser CreateDemoUser(
            int id,
            string userName,
            string email,
            string securityStamp,
            DateTime createdDate)
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
                ConcurrencyStamp = securityStamp,
                CreatedDate = createdDate,
                PasswordHash = null,
                PhoneNumber = null
            };
        }

        private static DateTime SeedDate(int millisecond, long ticks)
        {
            return new DateTime(
                2025,
                7,
                29,
                20,
                35,
                21,
                millisecond,
                DateTimeKind.Local).AddTicks(ticks);
        }
    }
}
