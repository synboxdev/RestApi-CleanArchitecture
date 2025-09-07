using Hestia.Access.Requests.Product.Queries.GetExisting;
using Hestia.Persistence.Handlers.Product.Queries.GetExisting;
using Hestia.Persistence.Tests.Shared;
using Microsoft.Extensions.Logging;
using Moq;

namespace Hestia.Persistence.Tests.Handlers.Product.Queries.GetExisting;

public class GetExistingProductByUserQueryHandlerTests : RheaContextTestBase
{
    private readonly Mock<ILogger<GetExistingProductByUserQueryHandler>> _mockLogger;
    private readonly GetExistingProductByUserQueryHandler _handler;

    public GetExistingProductByUserQueryHandlerTests()
    {
        _mockLogger = new Mock<ILogger<GetExistingProductByUserQueryHandler>>();
        _handler = new GetExistingProductByUserQueryHandler(Context, _mockLogger.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnProduct_WhenProductExists()
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

        var query = new GetExistingProductByUserQuery(product.UserId, product.ExternalId);
        var result = await _handler.Handle(query, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(product.UserId, result?.UserId);
        Assert.Equal(product.ExternalId, result?.ExternalId);
    }

    [Fact]
    public async Task Handle_ShouldReturnNull_WhenProductDoesNotExist()
    {
        var query = new GetExistingProductByUserQuery("nonexistent-user-id", "nonexistent-external-id");
        var result = await _handler.Handle(query, CancellationToken.None);

        Assert.Null(result);
    }
}