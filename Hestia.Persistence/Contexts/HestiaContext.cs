using Hestia.Access.Entities.Authentication;
using Hestia.Access.Entities.User;
using Hestia.Domain.Models.Authentication;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Reflection;

namespace Hestia.Persistence.Contexts;

public class HestiaContext(DbContextOptions<HestiaContext> options) : IdentityUserContext<ApplicationUser>(options)
{
    public DbSet<TokenLog> TokenLog { get; set; } = null!;
    public DbSet<User> User { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }

    public async Task<TResult> ExecuteInTransactionAsync<T, TResult>(Func<HestiaContext, Task<TResult>> action, ILogger<T> logger)
    {
        using var transaction = await Database.BeginTransactionAsync();
        try
        {
            var result = await action(this);
            await transaction.CommitAsync();
            return result;
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            logger.LogCritical(ex, "An error occurred while executing the database action: {Message} | Context: {Context}", ex.Message, typeof(T).Name);
            throw;
        }
    }
}