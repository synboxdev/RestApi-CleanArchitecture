using Hestia.Application.Interfaces.Response;
using Hestia.Application.Models.Shared;
using System.Net;

namespace Hestia.Application.Services;

public class ResponseService : IResponseService
{
    public Task<ApiResponse<T>> GetResponse<T>(HttpStatusCode statusCode, string message, T data, bool? success = null) =>
        Task.FromResult(new ApiResponse<T>(statusCode, message, data, success));
}