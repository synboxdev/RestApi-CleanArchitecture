using System.ComponentModel.DataAnnotations;

namespace Hestia.Access.Entities;

public abstract class BaseEntity(Guid id)
{
    [Key]
    public Guid Id { get; private set; } = id;
}