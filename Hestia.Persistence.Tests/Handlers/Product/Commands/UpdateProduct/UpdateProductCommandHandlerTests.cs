using Hestia.Access.Requests.Product.Commands.UpdateProduct;
using Hestia.Persistence.Contexts;
using Hestia.Persistence.Handlers.Product.Commands.UpdateProduct;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;

namespace Hestia.Persistence.Tests.Handlers.Product.Commands.UpdateProduct;

public class UpdateProductCommandHandlerTests : IDisposable
{
    private readonly RheaContext _context;
    private readonly Mock<ILogger<UpdateProductCommandHandler>> _mockLogger;
    private readonly UpdateProductCommandHandler _handler;

    public UpdateProductCommandHandlerTests()
    {
        var dbContextOptions = new DbContextOptionsBuilder<RheaContext>()
            .UseSqlite("DataSource=:memory:")
            .Options;

        _context = new RheaContext(dbContextOptions);
        _context.Database.OpenConnection();
        _context.Database.EnsureCreated();

        _mockLogger = new Mock<ILogger<UpdateProductCommandHandler>>();
        _handler = new UpdateProductCommandHandler(_context, _mockLogger.Object);
    }

    [Fact]
    public async Task Handle_ShouldUpdateProductInDatabase_WhenCalled()
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

        // Update product details
        product.Name = "Updated Product";
        product.Description = "Updated Description";
        product.Price = 79.99m;

        var command = new UpdateProductCommand(product);

        // Act
        bool result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result);

        var updatedProduct = await _context.Product.FindAsync(product.Id);
        Assert.NotNull(updatedProduct);
        Assert.Equal(product.Name, updatedProduct.Name);
        Assert.Equal(product.Description, updatedProduct.Description);
        Assert.Equal(product.Price, updatedProduct.Price);
        Assert.Equal(product.ExternalId, updatedProduct.ExternalId);
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}