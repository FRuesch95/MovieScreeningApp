using System.Net;

namespace MovieScreeningApp.Api.Exceptions;

public class StreamingAvailabilityApiException : Exception
{
    /// <summary>
    /// Status code returned by the Streaming Availability API. Null if no response was received (connection error, timeout).
    /// </summary>
    public HttpStatusCode? StatusCode { get; }

    public StreamingAvailabilityApiException(string message, HttpStatusCode? statusCode = null, Exception? innerException = null)
        : base(message, innerException)
    {
        StatusCode = statusCode;
    }
}
