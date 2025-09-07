using Hestia.Access.Requests.Product.Commands.CreateProduct;
using Hestia.Persistence.Handlers.Product.Commands.CreateProduct;
using Hestia.Persistence.Handlers.Product.Queries.GetExisting;
using Hestia.Persistence.Tests.Shared;
using Microsoft.Extensions.Logging;
using Moq;

namespace Hestia.Persistence.Tests.Handlers.Product.Commands.CreateProduct;

public class CreateProductCommandHandlerTests : RheaContextTestBase
{
    private readonly Mock<ILogger<GetExistingProductByUserQueryHandler>> _mockLogger;
    private readonly CreateProductCommandHandler _handler;

    public CreateProductCommandHandlerTests()
    {
        _mockLogger = new Mock<ILogger<GetExistingProductByUserQueryHandler>>();
        _handler = new CreateProductCommandHandler(Context, _mockLogger.Object);
    }

    [Fact]
    public async Task Handle_ShouldAddProductToDatabase_WhenCalled()
    {
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
        bool result = await _handler.Handle(command, CancellationToken.None);

        Assert.True(result);

        var savedProduct = await Context.Product.FindAsync(product.Id);
        Assert.NotNull(savedProduct);
    }
}