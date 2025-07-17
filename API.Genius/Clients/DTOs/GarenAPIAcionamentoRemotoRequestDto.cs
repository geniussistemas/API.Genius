using System.Text.Json.Serialization;

namespace API.Genius.Clients.DTOs
{
  public class GarenApiAcionamentoRemotoRequestDto
  {
    [JsonPropertyName("porta")]
    public int? Porta { get; set; }

    [JsonPropertyName("tempo")]
    public int? Tempo { get; set; }
  }
}