namespace Hestia.Access.Entities.Product;

public class Product(Guid id) : BaseEntity(id)
{
    public required string ExternalId { get; set; } = null!;
    public required string UserId { get; set; } = null!;
    public required DateTime DateCreated { get; set; }
    public required string Name { get; set; } = null!;
    public required string Description { get; set; } = null!;
    public required decimal? Price { get; set; }

    public DateTime DateEdited { get; set; }
}