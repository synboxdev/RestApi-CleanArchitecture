namespace Hestia.Domain.Models.Product.Inbound.GetProduct;

public sealed record GetProductDto
{
    public Guid? Id { get; set; }
    public string? ExternalId { get; set; }
}