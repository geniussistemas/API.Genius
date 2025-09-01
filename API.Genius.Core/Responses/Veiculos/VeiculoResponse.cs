using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace API.Genius.Core.Responses.Veiculos;

public class VeiculoResponse : Response<VeiculoResponse?>
{
    // TODO: Complementar com as demais informações do veículo
    [JsonPropertyName("id")]
    public long Id { get; set; }
    [JsonPropertyName("placa")]
    public string Placa { get; set; } = string.Empty;
    [JsonPropertyName("idTicket")]
    public string IdTicket { get; set; } = string.Empty;
    [JsonPropertyName("dataHoraEntrada")]
    public DateTime? DataHoraEntrada { get; set; }
}