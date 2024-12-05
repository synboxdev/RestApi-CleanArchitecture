using Hestia.Application.Models.Shared;
using System.Net;

namespace Hestia.Application.Interfaces.Response;

public interface IResponseService
{
    Task<ApiResponse<T>> GetResponse<T>(HttpStatusCode statusCode, string message, T data, bool? success = null);
}