using Hestia.Application.Interfaces.Response;
using Newtonsoft.Json;
using System.Net;

namespace Hestia.Api.Middleware;

public class ExceptionMiddleware(RequestDelegate next, IResponseService responseService)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (FluentValidation.ValidationException ex)
        {
            await HandleValidationExceptionAsync(context, ex);
        }
    }

    private async Task HandleValidationExceptionAsync(
        HttpContext context,
        FluentValidation.ValidationException exception,
        CancellationToken cancellationToken = default)
    {
        var result = await responseService.GetResponse(HttpStatusCode.BadRequest, "Validation errors!", exception.Message);
        string response = JsonConvert.SerializeObject(result);

        context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
        context.Response.ContentType = "application/json";

        await context.Response.WriteAsync(response, cancellationToken);
    }
}