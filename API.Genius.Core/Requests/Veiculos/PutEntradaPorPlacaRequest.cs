using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace API.Genius.Core.Requests.Veiculos;

public class PutEntradaPorPlacaRequest : Request
{
    [JsonPropertyName("placa")]
    public string Placa { get; set; } = string.Empty;
    [JsonPropertyName("idCamera")]
    public int IdCamera { get; set; }
    [JsonPropertyName("dataEntrada")]
    public DateTime DataEntrada { get; set; }
    [JsonPropertyName("arquivoImagem")]
    public string? ArquivoImagem { get; set; } = string.Empty;
    [JsonIgnore]
    public int? Status { get; set; } = null;
}