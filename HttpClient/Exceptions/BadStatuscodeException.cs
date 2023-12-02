using System;
using System.Net;

namespace HttpClient.Exceptions;

public class BadStatuscodeException : Exception
{
    public BadStatuscodeException(HttpStatusCode statusCode, string message) : base(message)
    {
        StatusCode = statusCode;
    }

    public HttpStatusCode? StatusCode { get; set; }
}