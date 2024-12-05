using Hestia.Domain.Enumerations;
using Microsoft.AspNetCore.Identity;

namespace Hestia.Domain.Models.Authentication;

public class ApplicationUser : IdentityUser
{
    public Role Role { get; set; }
}