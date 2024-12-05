using Hestia.Domain.Models.Authentication;
using MediatR;

namespace Hestia.Access.Requests.User.Queries.UserExists;

public sealed record GetExistingUserQuery(string Username, string Email) : IRequest<ApplicationUser?>;