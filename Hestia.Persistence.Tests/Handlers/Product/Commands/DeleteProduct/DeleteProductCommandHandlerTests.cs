using Hestia.Access.Requests.Product.Commands.DeleteProduct;
using Hestia.Persistence.Contexts;
using Hestia.Persistence.Handlers.Product.Commands.DeleteProduct;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;

namespace Hestia.Persistence.Tests.Handlers.Product.Commands.DeleteProduct;

public class DeleteProductCommandHandlerTests : IDisposable
{
    private readonly RheaContext _context;
    private readonly Mock<ILogger<DeleteProductCommandHandler>> _mockLogger;
    private readonly DeleteProductCommandHandler _handler;

    public DeleteProductCommandHandlerTests()
    {
        var dbContextOptions = new DbContextOptionsBuilder<RheaContext>()
            .UseSqlite("DataSource=:memory:")
            .Options;

        _context = new RheaContext(dbContextOptions);
        _context.Database.OpenConnection();
        _context.Database.EnsureCreated();

        _mockLogger = new Mock<ILogger<DeleteProductCommandHandler>>();
        _handler = new DeleteProductCommandHandler(_context, _mockLogger.Object);
    }

    [Fact]
    public async Task Handle_ShouldRemoveProductFromDatabase_WhenCalled()
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

        var command = new DeleteProductCommand(product);

        // Act
        bool result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result);

        var deletedProduct = await _context.Product.FindAsync(product.Id);
        Assert.Null(deletedProduct); // Ensure the product is deleted
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}