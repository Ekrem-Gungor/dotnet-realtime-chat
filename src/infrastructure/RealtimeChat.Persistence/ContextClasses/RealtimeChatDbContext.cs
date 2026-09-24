using RealtimeChat.Domain.Entities.Concretes;
using RealtimeChat.Persistence.Configurations;
using RealtimeChat.Persistence.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealtimeChat.Persistence.ContextClasses
{
    public class RealtimeChatDbContext : IdentityDbContext<AppUser, AppRole, int,
                                               IdentityUserClaim<int>, AppUserRole, IdentityUserLogin<int>,
                                               IdentityRoleClaim<int>, IdentityUserToken<int>>
    {
        public RealtimeChatDbContext(DbContextOptions<RealtimeChatDbContext> opt) : base(opt)
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
