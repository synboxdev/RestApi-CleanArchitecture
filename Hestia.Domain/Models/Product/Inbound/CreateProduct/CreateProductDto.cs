namespace Hestia.Domain.Models.Product.Inbound.CreateProduct;

public sealed record CreateProductDto
{
    private string? UserId { get; set; } = null!;

    /// <summary>
    /// This should represent a unique product code from the user (e.g. Supplier company), for example barcode or SKU (Stock keeping unit), which should be unique.
    ///     Within our system, this will simply help identifying existing products user-by-user basis, since internally we'll use unique 'Id' field.
    /// </summary>
    public required string ExternalId { get; set; } = string.Empty;
    public required string Name { get; set; } = string.Empty;
    public required string Description { get; set; } = string.Empty;
    public required decimal? Price { get; set; }

    public void SetUserId(string userId) => UserId = userId;
    public string? GetUserId() => UserId;
}