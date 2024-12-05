namespace Hestia.Access.Entities.User;

public class User(Guid id) : BaseEntity(id)
{
    public required string IdentityUserId { get; set; } = null!;
    public required DateTime DateCreated { get; set; }
    public required string Name { get; set; }
}