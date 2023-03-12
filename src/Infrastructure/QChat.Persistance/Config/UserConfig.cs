using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QChat.Domain.Entities;

namespace QChat.Persistance.Config;

public class UserConfig : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasIndex(e => e.UserName)
            .IsUnique(true);

        builder.HasMany(e => e.UserChats).WithOne(e => e.User);
    }
}
public class ChatConfig : IEntityTypeConfiguration<Chat>
{
    public void Configure(EntityTypeBuilder<Chat> builder)
    {
        builder.HasMany(e => e.UserChats).WithOne(e => e.Chat);
        builder.HasMany(e => e.Messages).WithOne(e => e.Chat);
    }
}
public class MessageConfig : IEntityTypeConfiguration<Message>
{
    public void Configure(EntityTypeBuilder<Message> builder)
    {
        builder.HasOne(e => e.Chat).WithMany(e => e.Messages);
        builder.HasOne(e => e.User).WithMany();
    }
}
public class UserChatConfig : IEntityTypeConfiguration<UserChat>
{
    public void Configure(EntityTypeBuilder<UserChat> builder)
    {
        builder.HasKey(e => new { e.UserId, e.ChatId });

        builder.HasOne(e => e.User).WithMany(e => e.UserChats);
        builder.HasOne(e => e.Chat).WithMany(e => e.UserChats);
    }
}