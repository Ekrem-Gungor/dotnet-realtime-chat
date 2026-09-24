using DevBudy.DOMAIN.Entities.Concretes;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevBudy.PERSISTANCE.Configurations
{
    public class AppUserConfiguration : BaseConfiguration<AppUser>
    {
        public override void Configure(EntityTypeBuilder<AppUser> builder)
        {
            base.Configure(builder);
            builder.Ignore(u => u.ID);
            builder.HasMany(u => u.UserRoles).WithOne(ur => ur.User).HasForeignKey(ur => ur.UserId).IsRequired();
            builder.HasMany(u => u.ChatMessages).WithOne(cm => cm.Sender).HasForeignKey(cm => cm.SenderID).IsRequired();
            builder.HasOne(u => u.Profile).WithOne(p => p.AppUser).HasForeignKey<AppUserProfile>(p => p.ID);
        }
    }
}
