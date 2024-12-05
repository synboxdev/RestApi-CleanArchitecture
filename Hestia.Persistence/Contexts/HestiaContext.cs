using Hestia.Access.Entities.Authentication;
using Hestia.Access.Entities.User;
using Hestia.Domain.Models.Authentication;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Reflection;

namespace Hestia.Persistence.Contexts;

public class HestiaContext : IdentityUserContext<ApplicationUser>
{
    private readonly IConfiguration configuration;

    public DbSet<TokenLog> TokenLog { get; set; } = null!;
    public DbSet<User> User { get; set; } = null!;

    public HestiaContext()
    {
    }

    public HestiaContext(IConfiguration configuration) => this.configuration = configuration;

    public HestiaContext(DbContextOptions<HestiaContext> options)
        : base(options)
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (optionsBuilder.IsConfigured)
            return;

        optionsBuilder.UseNpgsql(configuration.GetConnectionString("AuthServer"));
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }

    public async Task<TResult> ExecuteInTransactionAsync<T, TResult>(Func<HestiaContext, Task<TResult>> action, ILogger<T> logger)
    {
        using (var transaction = await Database.BeginTransactionAsync())
        {
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
}