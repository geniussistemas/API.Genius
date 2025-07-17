using System.Text.Json.Serialization;

namespace API.Genius.Clients.DTOs
{
  public class GarenApiAcionamentoRemotoResponseDto
  {
    [JsonPropertyName("status")]
    public string? Status { get; set; }

    [JsonPropertyName("codigo")]
    public int? Codigo { get; set; }
  }
}