using Microsoft.EntityFrameworkCore;
using QChat.Domain.Entities;

namespace QChat.Application.Interfaces;

public interface IChatDbContext
{
    DbSet<User> Users { get; set; }
    DbSet<Chat> Chats { get; set; }
    DbSet<Message> Messages { get; set; }
    DbSet<UserChat> UserChats { get; set; }
    DbSet<PrivateChat> PrivateChats { get; set; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}