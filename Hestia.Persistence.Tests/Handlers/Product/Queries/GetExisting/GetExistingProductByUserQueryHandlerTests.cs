using Hestia.Access.Requests.Product.Queries.GetExisting;
using Hestia.Persistence.Contexts;
using Hestia.Persistence.Handlers.Product.Queries.GetExisting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;

namespace Hestia.Persistence.Tests.Handlers.Product.Queries.GetExisting;

public class GetExistingProductByUserQueryHandlerTests : IDisposable
{
    private readonly RheaContext _context;
    private readonly Mock<ILogger<GetExistingProductByUserQueryHandler>> _mockLogger;
    private readonly GetExistingProductByUserQueryHandler _handler;

    public GetExistingProductByUserQueryHandlerTests()
    {
        var dbContextOptions = new DbContextOptionsBuilder<RheaContext>()
            .UseSqlite("DataSource=:memory:")
            .Options;

        _context = new RheaContext(dbContextOptions);
        _context.Database.OpenConnection();
        _context.Database.EnsureCreated();

        _mockLogger = new Mock<ILogger<GetExistingProductByUserQueryHandler>>();
        _handler = new GetExistingProductByUserQueryHandler(_context, _mockLogger.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnProduct_WhenProductExists()
    {
        // Arrange
        var product = new Access.Entities.Product.Product(Guid.NewGuid())
        {
            Name = "Test Product",
            UserId = "UserId",
            Description = "Test Description",
            Price = 99.99m,
            ExternalId = "ext-1234",
            DateCreated = DateTime.UtcNow,
            DateEdited = DateTime.UtcNow
        };

        await _context.Product.AddAsync(product);
        await _context.SaveChangesAsync();

        var query = new GetExistingProductByUserQuery(product.UserId, product.ExternalId);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(product.UserId, result?.UserId);
        Assert.Equal(product.ExternalId, result?.ExternalId);
    }

    [Fact]
    public async Task Handle_ShouldReturnNull_WhenProductDoesNotExist()
    {
        // Arrange
        var query = new GetExistingProductByUserQuery("nonexistent-user-id", "nonexistent-external-id");

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.Null(result);
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}