using System.Net;

namespace Hestia.Application.Models.Shared;

[Serializable]
public class ApiResponse<T>(HttpStatusCode statusCode, string message, T data, bool? success = null)
{
    public bool Success { get; private set; } = success ?? (((int)statusCode >= 200) && ((int)statusCode <= 299));
    public HttpStatusCode StatusCode { get; private set; } = statusCode;
    public string Message { get; private set; } = message;
    public T Data { get; private set; } = data;
}