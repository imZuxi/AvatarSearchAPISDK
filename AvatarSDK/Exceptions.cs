using System.Text.Json.Serialization;

namespace AvatarSDK;

/// <summary>
/// Error payload returned by the API on non-success responses.
/// </summary>
public class ApiError
{
    [JsonPropertyName("error")]
    public string? Error { get; set; }

    [JsonPropertyName("message")]
    public string? Message { get; set; }

    [JsonPropertyName("status_code")]
    public int StatusCode { get; set; }

    public override string ToString() => Error ?? Message ?? $"HTTP {StatusCode}";
}

/// <summary>
/// Thrown when the API returns a non-success HTTP status code.
/// </summary>
public class AvatarApiException(ApiError error) : Exception(error.ToString())
{
    /// <summary>The structured error returned by the API.</summary>
    public ApiError ApiError  { get; } = error;

    /// <summary>The HTTP status code of the failed response.</summary>
    public int      StatusCode => ApiError.StatusCode;
}
