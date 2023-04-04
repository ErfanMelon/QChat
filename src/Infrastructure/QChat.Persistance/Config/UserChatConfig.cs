using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QChat.Domain.Entities;

namespace QChat.Persistance.Config;

public class UserChatConfig : IEntityTypeConfiguration<UserChat>
{
    public void Configure(EntityTypeBuilder<UserChat> builder)
    {
        builder.HasKey(e => new { e.UserId, e.ChatId });

        builder.HasOne(e => e.User).WithMany(e => e.UserChats);
        builder.HasOne(e => e.Chat).WithMany(e => e.UserChats);
    }
}