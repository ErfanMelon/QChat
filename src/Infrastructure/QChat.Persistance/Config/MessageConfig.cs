using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QChat.Domain.Entities;

namespace QChat.Persistance.Config;

public class MessageConfig : IEntityTypeConfiguration<Message>
{
    public void Configure(EntityTypeBuilder<Message> builder)
    {
        builder.HasOne(e => e.Chat).WithMany(e => e.Messages);
        builder.HasOne(e => e.User).WithMany();
    }
}
