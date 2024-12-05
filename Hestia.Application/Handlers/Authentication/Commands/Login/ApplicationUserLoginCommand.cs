using Hestia.Application.Models.Authentication.Inbound;
using Hestia.Application.Models.Authentication.Outbound;
using Hestia.Application.Models.Shared;
using MediatR;

namespace Hestia.Application.Handlers.Authentication.Commands.Login;

public sealed record ApplicationUserLoginCommand(ApplicationUserLoginDto Model) : IRequest<ApiResponse<ApplicationUserLoginResponseDto>>;