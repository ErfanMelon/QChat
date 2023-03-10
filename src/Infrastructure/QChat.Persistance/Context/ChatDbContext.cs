using Microsoft.EntityFrameworkCore;
using QChat.Application.Interfaces;
using QChat.Domain.Entities;
using QChat.Persistance.Config;
using System.Reflection;

namespace QChat.Persistance.Context;

public class ChatDbContext : DbContext, IChatDbContext
{
	public ChatDbContext(DbContextOptions options) : base(options) { }
	public virtual DbSet<User> Users { get; set; }
	public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
	{
		BaseEntityConfig.QuantificationDefaultShadowProperties(ChangeTracker);
		return base.SaveChangesAsync(cancellationToken);
	}
	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		BaseEntityConfig.SetDefaultShadowProperties(ref modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly()); // set configuration for all entities
		base.OnModelCreating(modelBuilder);
	}
}
