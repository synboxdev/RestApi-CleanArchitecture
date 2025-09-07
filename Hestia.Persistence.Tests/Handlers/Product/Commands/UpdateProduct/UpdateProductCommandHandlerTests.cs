using Hestia.Access.Requests.Product.Commands.UpdateProduct;
using Hestia.Persistence.Handlers.Product.Commands.UpdateProduct;
using Hestia.Persistence.Tests.Shared;
using Microsoft.Extensions.Logging;
using Moq;

namespace Hestia.Persistence.Tests.Handlers.Product.Commands.UpdateProduct;

public class UpdateProductCommandHandlerTests : RheaContextTestBase
{
    private readonly Mock<ILogger<UpdateProductCommandHandler>> _mockLogger;
    private readonly UpdateProductCommandHandler _handler;

    public UpdateProductCommandHandlerTests()
    {
        _mockLogger = new Mock<ILogger<UpdateProductCommandHandler>>();
        _handler = new UpdateProductCommandHandler(Context, _mockLogger.Object);
    }

    [Fact]
    public async Task Handle_ShouldUpdateProductInDatabase_WhenCalled()
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

        product.Name = "Updated Product";
        product.Description = "Updated Description";
        product.Price = 79.99m;

        var command = new UpdateProductCommand(product);
        bool result = await _handler.Handle(command, CancellationToken.None);

        Assert.True(result);

        var updatedProduct = await Context.Product.FindAsync(product.Id);
        Assert.NotNull(updatedProduct);
        Assert.Equal(product.Name, updatedProduct.Name);
        Assert.Equal(product.Description, updatedProduct.Description);
        Assert.Equal(product.Price, updatedProduct.Price);
        Assert.Equal(product.ExternalId, updatedProduct.ExternalId);
    }
}