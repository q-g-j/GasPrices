using System;
using System.Net;

namespace HttpClient.Exceptions;

public class BadStatuscodeException(HttpStatusCode statusCode, string message) : Exception(message)
{
    public HttpStatusCode? StatusCode { get; set; } = statusCode;
}