using Microsoft.EntityFrameworkCore;
using QChat.Domain.Entities;

namespace QChat.Application.Interfaces;

public interface IChatDbContext
{
    DbSet<User> Users { get; set; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}