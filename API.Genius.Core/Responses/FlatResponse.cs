using System.Text.Json;
using System.Text.Json.Serialization;

namespace API.Genius.Core.Responses;

[JsonConverter(typeof(FlatResponseJsonConverterFactory))]
public class FlatResponse<TData>
{
    private readonly int _code;
    public TData? Data { get; set; }
    public string? Message { get; set; }

    public FlatResponse() { }

    public FlatResponse(TData? data, int code = Configuration.DefaultStatusCode, string? message = null)
    {
        _code = code;
        Data = data;
        Message = message;
    }

    [JsonIgnore]
    public bool IsSuccess => _code is >= 200 and <= 299;
}

// Novo conversor:
public class FlatResponseJsonConverterFactory : JsonConverterFactory
{
    public override bool CanConvert(Type typeToConvert)
        => typeToConvert.IsGenericType && typeToConvert.GetGenericTypeDefinition() == typeof(FlatResponse<>);

    public override JsonConverter CreateConverter(Type type, JsonSerializerOptions options)
    {
        var dataType = type.GetGenericArguments()[0];
        var converterType = typeof(FlatResponseJsonConverter<>).MakeGenericType(dataType);
        return (JsonConverter)Activator.CreateInstance(converterType)!;
    }
}

public class FlatResponseJsonConverter<T> : JsonConverter<FlatResponse<T>>
{
    public override FlatResponse<T>? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }

    public override void Write(Utf8JsonWriter writer, FlatResponse<T> value, JsonSerializerOptions options)
    {
        if (value.Data == null)
        {
            writer.WriteStartObject();
            if (value.Message != null)
                writer.WriteString("message", value.Message);
            writer.WriteEndObject();
            return;
        }

        using var doc = JsonDocument.Parse(JsonSerializer.Serialize(value.Data, options));
        writer.WriteStartObject();

        foreach (var prop in doc.RootElement.EnumerateObject())
        {
            prop.WriteTo(writer);
        }

        if (value.Message != null)
            writer.WriteString("message", value.Message);

        writer.WriteEndObject();
    }
}
