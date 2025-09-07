using Hestia.Access.Requests.Product.Commands.DeleteProduct;
using Hestia.Persistence.Handlers.Product.Commands.DeleteProduct;
using Hestia.Persistence.Tests.Shared;
using Microsoft.Extensions.Logging;
using Moq;

namespace Hestia.Persistence.Tests.Handlers.Product.Commands.DeleteProduct;

public class DeleteProductCommandHandlerTests : RheaContextTestBase
{
    private readonly Mock<ILogger<DeleteProductCommandHandler>> _mockLogger;
    private readonly DeleteProductCommandHandler _handler;

    public DeleteProductCommandHandlerTests()
    {
        _mockLogger = new Mock<ILogger<DeleteProductCommandHandler>>();
        _handler = new DeleteProductCommandHandler(Context, _mockLogger.Object);
    }

    [Fact]
    public async Task Handle_ShouldRemoveProductFromDatabase_WhenCalled()
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

        await Context.Product.AddAsync(product);
        await Context.SaveChangesAsync();

        var command = new DeleteProductCommand(product);
        bool result = await _handler.Handle(command, CancellationToken.None);

        Assert.True(result);

        var deletedProduct = await Context.Product.FindAsync(product.Id);
        Assert.Null(deletedProduct);
    }
}
