using DevBudy.DOMAIN.Entities.Concretes;
using DevBudy.PERSISTANCE.Configurations;
using DevBudy.PERSISTANCE.Extentions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevBudy.PERSISTANCE.ContextClasses
{
    public class DevBudyContext : IdentityDbContext<AppUser, AppRole, int,
                                               IdentityUserClaim<int>, AppUserRole, IdentityUserLogin<int>,
                                               IdentityRoleClaim<int>, IdentityUserToken<int>>
    {
        public DevBudyContext(DbContextOptions<DevBudyContext> opt) : base(opt)
        {

        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.ApplyConfigurationsFromAssembly(typeof(BaseConfiguration<>).Assembly);

            UserDataSeedExt.SeedUsers(builder);
        }

        public virtual DbSet<AppUser> AppUsers { get; set; }
        public virtual DbSet<AppRole> AppRoles { get; set; }
        public virtual DbSet<AppUserRole> AppUserRoles { get; set; }
        public virtual DbSet<AppUserProfile> AppUserProfiles { get; set; }
        public virtual DbSet<ChatMessage> ChatMessages { get; set; }
    }
}
