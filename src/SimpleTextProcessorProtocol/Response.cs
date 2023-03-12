using System.Text.Json;

namespace SimpleTextProcessorProtocol;

public sealed record Response
{
    /* Perfectly there should be packet length and checksum as well, but I chose to keep it simple */
    public ResponseStatusCode StatusCode { get; init; }
    public string Message { get; init; } = null!;

    public Response(ResponseStatusCode statusCode, string message)
    {
        StatusCode = statusCode;
        Message = message;
    }

    public static Response Deserialize(string responseStr)
    {
        try
        {
            return JsonSerializer.Deserialize<Response>(responseStr) ?? new(ResponseStatusCode.GENERAL_ERROR, "Failed to deserialize response");
        }
        catch
        {
            return new(ResponseStatusCode.GENERAL_ERROR, "Exception while deserializing response");
        }
    }

    public static byte[] Serialize(Response response) => JsonSerializer.SerializeToUtf8Bytes(response);
}
