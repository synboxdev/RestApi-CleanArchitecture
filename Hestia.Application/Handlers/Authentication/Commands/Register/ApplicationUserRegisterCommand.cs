using Hestia.Application.Models.Authentication.Inbound;
using Hestia.Application.Models.Authentication.Outbound;
using Hestia.Application.Models.Shared;
using MediatR;

namespace Hestia.Application.Handlers.Authentication.Commands.Register;

public sealed record ApplicationUserRegisterCommand(ApplicationUserRegisterDto Model) : IRequest<ApiResponse<ApplicationUserRegisterResponseDto>>;