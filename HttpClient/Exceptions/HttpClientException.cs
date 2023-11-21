using System;
using System.Collections.Generic;
using System.Text;

namespace HttpClient.Exceptions
{
    public class HttpClientException : Exception
    {
        public HttpClientException(string message) : base(message)
        {
        }
    }
}
