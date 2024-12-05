namespace Hestia.Domain.Models.Product.Inbound.GetProduct;

public sealed class GetProductResponseDto
{
    public Guid Id { get; set; }
    public string? ExternalId { get; set; }
    public DateTime DateCreated { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public decimal? Price { get; set; }
    public DateTime DateEdited { get; set; }
}