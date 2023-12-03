using System;

namespace HttpClient.Exceptions
{
    public class HttpClientException : Exception
    {
        public HttpClientException(string message) : base(message)
        {
        }
    }
}