using Hestia.Application.Models.Authentication.Inbound;
using Hestia.Application.Models.Authentication.Outbound;
using Hestia.Application.Models.Shared;
using Hestia.Mediator.Infrastructure.Messaging;

namespace Hestia.Application.Handlers.Authentication.Commands.Register;

public sealed record ApplicationUserRegisterCommand(ApplicationUserRegisterDto Model) : IRequest<ApiResponse<ApplicationUserRegisterResponseDto>>;