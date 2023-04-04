using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QChat.Domain.Entities;

namespace QChat.Persistance.Config;

public class ChatConfig : IEntityTypeConfiguration<Chat>
{
    public void Configure(EntityTypeBuilder<Chat> builder)
    {
        builder.HasMany(e => e.UserChats).WithOne(e => e.Chat);
        builder.HasMany(e => e.Messages).WithOne(e => e.Chat);

        builder.HasDiscriminator("ChatType", typeof(char))
            .HasValue('g') // Default is group
            .HasValue<PrivateChat>('p');
    }
}
public class PrivateChatConfig:IEntityTypeConfiguration<PrivateChat>
{
    public void Configure(EntityTypeBuilder<PrivateChat> builder)
    {
        builder.HasIndex(e => e.UserId1);
        builder.HasIndex(e => e.UserId2);
    }
}
