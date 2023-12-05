using System;

namespace HttpClient.Exceptions;

public class HttpClientException(string message) : Exception(message);