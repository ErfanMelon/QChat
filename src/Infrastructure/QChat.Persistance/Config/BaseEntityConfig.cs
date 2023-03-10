using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using QChat.Domain.Entities;
using System.Linq.Expressions;

namespace QChat.Persistance.Config;
public static class BaseEntityConfig
{
    /// <summary>
    /// Set Some Shadow Properties On Classes Which inherited BaseEntity
    /// </summary>
    /// <param name="builder"></param>
    public static void SetDefaultShadowProperties(ref ModelBuilder builder)
    {
        var entities = builder.Model.FindLeastDerivedEntityTypes(typeof(BaseEntityKeyLess));
        foreach (var item in entities)
        {
            builder.Entity(item.Name).Property<DateTime>("CreateDate");
            builder.Entity(item.Name).Property<DateTime?>("RemoveDate")
                .IsRequired(false);
            builder.Entity(item.Name).Property<DateTime>("UpdateDate");
            builder.Entity(item.Name).Property<bool>("IsRemoved");

            var parameter = Expression.Parameter(item.ClrType, "e");
            var body = Expression.Equal(
                Expression.Call(typeof(EF), nameof(EF.Property), new[] { typeof(bool) }, parameter, Expression.Constant("IsRemoved")),
            Expression.Constant(false));
            builder.Entity(item.ClrType).HasQueryFilter(Expression.Lambda(body, parameter));

        }
    }
    /// <summary>
    /// Quantification Default Shadow Properties
    /// </summary>
    /// <param name="changeTracker"></param>
    public static void QuantificationDefaultShadowProperties(ChangeTracker changeTracker)
    {
        changeTracker.DetectChanges();
        var timestamp = DateTime.Now;

        foreach (var entry in changeTracker.Entries<BaseEntityKeyLess>())
        {
            switch (entry.State)
            {
                case EntityState.Deleted:
                    entry.Property("IsRemoved").CurrentValue = true;
                    entry.Property("RemoveDate").CurrentValue = timestamp;
                    entry.State = EntityState.Modified;
                    return;
                case EntityState.Modified:
                    entry.Property("UpdateDate").CurrentValue = timestamp;
                    return;
                case EntityState.Added:
                    entry.Property("CreateDate").CurrentValue = timestamp;
                    entry.Property("IsRemoved").CurrentValue = false;
                    return;
                case EntityState.Detached:
                case EntityState.Unchanged:
                default:
                    return;
            }
        }
    }
}