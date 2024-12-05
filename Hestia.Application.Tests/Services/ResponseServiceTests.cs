using Hestia.Application.Interfaces.Response;
using Hestia.Application.Services;
using System.Net;

namespace Hestia.Application.Tests.Services;

public class ResponseServiceTests
{
    private readonly IResponseService _responseService;

    public ResponseServiceTests()
    {
        _responseService = new ResponseService();
    }

    [Fact]
    public async Task GetResponse_ShouldReturnApiResponse_WithCorrectStatusCodeAndMessage()
    {
        // Arrange
        var statusCode = HttpStatusCode.OK;
        string message = "Success";
        string data = "TestData";

        // Act
        var result = await _responseService.GetResponse(statusCode, message, data);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(statusCode, result.StatusCode);
        Assert.Equal(message, result.Message);
        Assert.Equal(data, result.Data);
        Assert.True(result.Success);
    }

    [Fact]
    public async Task GetResponse_ShouldReturnApiResponse_WithErrorStatusCodeAndMessage()
    {
        // Arrange
        var statusCode = HttpStatusCode.BadRequest;
        string message = "Error";
        string data = "ErrorData";

        // Act
        var result = await _responseService.GetResponse(statusCode, message, data);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(statusCode, result.StatusCode);
        Assert.Equal(message, result.Message);
        Assert.Equal(data, result.Data);
        Assert.False(result.Success);
    }

    [Fact]
    public async Task GetResponse_ShouldReturnApiResponse_WithSpecifiedSuccessValue()
    {
        // Arrange
        var statusCode = HttpStatusCode.OK;
        string message = "Success";
        string data = "TestData";
        bool success = false;

        // Act
        var result = await _responseService.GetResponse(statusCode, message, data, success);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(statusCode, result.StatusCode);
        Assert.Equal(message, result.Message);
        Assert.Equal(data, result.Data);
        Assert.Equal(success, result.Success);
    }
}