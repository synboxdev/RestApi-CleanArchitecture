using Hestia.Api.Middleware;
using Hestia.Application.Interfaces.Response;
using Hestia.Application.Models.Shared;
using Microsoft.AspNetCore.Http;
using Moq;
using Newtonsoft.Json;
using System.Net;

namespace Hestia.Api.Tests.Middleware;

public class ExceptionMiddlewareTests
{
    private readonly Mock<IResponseService> _mockResponseService;
    private readonly RequestDelegate _next;
    private readonly ExceptionMiddleware _middleware;

    public ExceptionMiddlewareTests()
    {
        _mockResponseService = new Mock<IResponseService>();
        _next = (HttpContext _) => throw new FluentValidation.ValidationException("Validation error");
        _middleware = new ExceptionMiddleware(_next, _mockResponseService.Object);
    }

    [Fact]
    public async Task InvokeAsync_ShouldReturnBadRequest_WhenValidationExceptionIsThrown()
    {
        // Arrange
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();
        string responseMessage = "Validation error";

        var expectedResponse = new ApiResponse<string>(HttpStatusCode.BadRequest, "Validation errors!", responseMessage);

        _mockResponseService.Setup(service => service.GetResponse(It.IsAny<HttpStatusCode>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool?>()))
            .ReturnsAsync(expectedResponse);

        // Act
        await _middleware.InvokeAsync(context);

        // Assert
        _mockResponseService.Verify(service => service.GetResponse(HttpStatusCode.BadRequest, "Validation errors!", responseMessage, null), Times.Once);

        Assert.Equal((int)HttpStatusCode.BadRequest, context.Response.StatusCode);
        Assert.Equal("application/json", context.Response.ContentType);

        string responseBody = await ReadResponseBodyAsync(context.Response);
        var response = JsonConvert.DeserializeObject<ApiResponse<string>>(responseBody);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("Validation errors!", response.Message);
        Assert.Equal(responseMessage, response.Data);
    }

    private static async Task<string> ReadResponseBodyAsync(HttpResponse response)
    {
        response.Body.Seek(0, SeekOrigin.Begin);
        string result = await new StreamReader(response.Body).ReadToEndAsync();
        response.Body.Seek(0, SeekOrigin.Begin);

        return result;
    }
}