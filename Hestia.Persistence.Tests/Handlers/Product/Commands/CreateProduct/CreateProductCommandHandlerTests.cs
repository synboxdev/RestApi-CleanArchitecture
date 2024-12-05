using Hestia.Access.Requests.Product.Commands.CreateProduct;
using Hestia.Persistence.Contexts;
using Hestia.Persistence.Handlers.Product.Commands.CreateProduct;
using Hestia.Persistence.Handlers.Product.Queries.GetExisting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;

namespace Hestia.Persistence.Tests.Handlers.Product.Commands.CreateProduct;

public class CreateProductCommandHandlerTests : IDisposable
{
    private readonly RheaContext _context;
    private readonly Mock<ILogger<GetExistingProductByUserQueryHandler>> _mockLogger;
    private readonly CreateProductCommandHandler _handler;

    public CreateProductCommandHandlerTests()
    {
        var dbContextOptions = new DbContextOptionsBuilder<RheaContext>()
            .UseSqlite("DataSource=:memory:")
            .Options;

        _context = new RheaContext(dbContextOptions);
        _context.Database.OpenConnection();
        _context.Database.EnsureCreated();

        _mockLogger = new Mock<ILogger<GetExistingProductByUserQueryHandler>>();
        _handler = new CreateProductCommandHandler(_context, _mockLogger.Object);
    }

    [Fact]
    public async Task Handle_ShouldAddProductToDatabase_WhenCalled()
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

        var command = new CreateProductCommand(product);

        // Act
        bool result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result);

        var savedProduct = await _context.Product.FindAsync(product.Id);
        Assert.NotNull(savedProduct);
        Assert.Equal(product.Name, savedProduct.Name);
        Assert.Equal(product.Description, savedProduct.Description);
        Assert.Equal(product.Price, savedProduct.Price);
        Assert.Equal(product.ExternalId, savedProduct.ExternalId);
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}