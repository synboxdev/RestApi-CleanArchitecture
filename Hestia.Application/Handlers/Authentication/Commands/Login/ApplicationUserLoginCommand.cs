using Hestia.Application.Models.Authentication.Inbound;
using Hestia.Application.Models.Authentication.Outbound;
using Hestia.Application.Models.Shared;
using Hestia.Mediator.Infrastructure.Messaging;

namespace Hestia.Application.Handlers.Authentication.Commands.Login;

public sealed record ApplicationUserLoginCommand(ApplicationUserLoginDto Model) : IRequest<ApiResponse<ApplicationUserLoginResponseDto>>;