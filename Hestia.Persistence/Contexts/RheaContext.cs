using Hestia.Access.Entities.Product;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Reflection;

namespace Hestia.Persistence.Contexts;

public class RheaContext : DbContext
{
    private readonly IConfiguration configuration;
    public DbSet<Product> Product { get; set; } = null!;

    public RheaContext()
    {
    }

    public RheaContext(IConfiguration configuration) => this.configuration = configuration;

    public RheaContext(DbContextOptions<RheaContext> options)
    : base(options)
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (optionsBuilder.IsConfigured)
            return;

        optionsBuilder.UseNpgsql(configuration.GetConnectionString("DataServer"));
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }

    public async Task<TResult> ExecuteInTransactionAsync<T, TResult>(Func<RheaContext, Task<TResult>> action, ILogger<T> logger)
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