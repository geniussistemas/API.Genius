using System.Text.Json.Serialization;

namespace API.Genius.Core.Responses;

public class Response<TData>
{
    private readonly int _code;
    public TData? Data { get; set; }
    public string? Message { get; set; }

    [JsonConstructor]
    public Response()
    {
        _code = Configuration.DefaultStatusCode;
    }

    public Response(TData? data, int code = Configuration.DefaultStatusCode, string? message = null)
    {
        Data = data;
        Message = message;
        _code = code;
    }

    [JsonIgnore]
    public bool IsSuccess => _code is >= 200 and <= 299;
}
