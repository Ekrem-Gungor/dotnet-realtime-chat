using DevBudy.DOMAIN.Entities.Concretes;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevBudy.PERSISTANCE.Configurations
{
    public class ChatMessageConfiguration : BaseConfiguration<ChatMessage>
    {
        public override void Configure(EntityTypeBuilder<ChatMessage> builder)
        {
            base.Configure(builder);

            builder.Property(x => x.Message)
                .IsRequired()
                .HasMaxLength(1000);

            builder.Property(x => x.MessageType)
                .IsRequired();

            builder.Property(x => x.MetaData)
                .HasMaxLength(500)
                .IsRequired(false);

        }
    }
}
