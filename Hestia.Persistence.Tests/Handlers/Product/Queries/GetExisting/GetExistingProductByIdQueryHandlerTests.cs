using Hestia.Access.Requests.Product.Queries.GetExisting;
using Hestia.Persistence.Handlers.Product.Queries.GetExisting;
using Hestia.Persistence.Tests.Shared;
using Microsoft.Extensions.Logging;
using Moq;

namespace Hestia.Persistence.Tests.Handlers.Product.Queries.GetExisting;

public class GetExistingProductByIdQueryHandlerTests : RheaContextTestBase
{
    private readonly Mock<ILogger<GetExistingProductByIdQueryHandler>> _mockLogger;
    private readonly GetExistingProductByIdQueryHandler _handler;

    public GetExistingProductByIdQueryHandlerTests()
    {
        _mockLogger = new Mock<ILogger<GetExistingProductByIdQueryHandler>>();
        _handler = new GetExistingProductByIdQueryHandler(Context, _mockLogger.Object);
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

        var query = new GetExistingProductByIdQuery(product.Id);
        var result = await _handler.Handle(query, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(product.Id, result?.Id);
    }

    [Fact]
    public async Task Handle_ShouldReturnNull_WhenProductDoesNotExist()
    {
        var query = new GetExistingProductByIdQuery(Guid.NewGuid());
        var result = await _handler.Handle(query, CancellationToken.None);

        Assert.Null(result);
    }
}