using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace API.Genius.Core.Responses.VeiculosAlphadigi;

#pragma warning disable IDE1006 // Estilos de Nomenclatura
public class PutEntradaPorPlacaAlphadigiResponse
{
    [JsonPropertyName("Response_AlarmInfoPlate")]
    public ResponseAlarmInfoPlate responseAlarmInfoPlate { get; set; } = new ResponseAlarmInfoPlate();
}

public class ResponseAlarmInfoPlate
{
    public string info { get; set; } = string.Empty;
    public string content { get; set; } = string.Empty;
    [JsonPropertyName("is_pay")]
    public string isPay { get; set; } = string.Empty;
    public List<SerialData> serialData { get; set; } = new List<SerialData>();
}

public class SerialData
{
    public int serialChannel { get; set; }
    public string data { get; set; } = string.Empty;
    public int dataLen { get; set; }
}

#pragma warning restore IDE1006 // Estilos de Nomenclatura
