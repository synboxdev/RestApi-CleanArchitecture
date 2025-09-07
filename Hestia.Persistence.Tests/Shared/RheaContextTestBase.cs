using Hestia.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Hestia.Persistence.Tests.Shared;

public abstract class RheaContextTestBase : IDisposable
{
    protected readonly RheaContext Context;

    protected RheaContextTestBase()
    {
        var dbContextOptions = new DbContextOptionsBuilder<RheaContext>()
            .UseSqlite("DataSource=:memory:")
            .Options;

        Context = new RheaContext(dbContextOptions);
        Context.Database.OpenConnection();
        Context.Database.EnsureCreated();
    }

    public void Dispose() => Context.Dispose();
}