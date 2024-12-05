namespace Hestia.Access.Entities.Authentication;

public class TokenLog(Guid id) : BaseEntity(id)
{
    public required string IdentityUserId { get; set; } = null!;
    public required DateTime DateCreated { get; set; }
    public required DateTime TokenExpirationDate { get; set; }
    public required string Token { get; set; } = null!;
}