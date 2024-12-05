namespace Hestia.Domain.Models.Product.Inbound.UpdateProduct;

public sealed record UpdateProductDto
{
    public required Guid Id { get; set; }
    public required string ExternalId { get; set; } = null!;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal? Price { get; set; }
}