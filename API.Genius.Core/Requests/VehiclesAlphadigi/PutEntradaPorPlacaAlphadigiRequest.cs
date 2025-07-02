using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace API.Genius.Core.Requests.VeiculosAlphadigi;

#pragma warning disable IDE1006 // Estilos de Nomenclatura
public class PutEntradaPorPlacaAlphadigiRequest
{
    [JsonPropertyName("AlarmInfoPlate")]
    public AlarmInfoPlate alarmInfoPlate { get; set; } = new AlarmInfoPlate();
}

public class AlarmInfoPlate
{
    public int channel { get; set; }
    public string deviceName { get; set; } = string.Empty;
    public string ipaddr { get; set; } = string.Empty;
    public int openflag { get; set; }
    public string entityName { get; set; } = string.Empty;
    public string TaxID { get; set; } = string.Empty;
    public Result result { get; set; } = new Result();
    public string serialno { get; set; } = string.Empty;
}

public class Result
{
    [JsonPropertyName("PlateResult")]
    public PlateResult plateResult { get; set; } = new PlateResult();
}

public class PlateResult
{
    public int bright { get; set; }
    public int carBright { get; set; }
    public int carColor { get; set; }
    public int colorType { get; set; }
    public int colorValue { get; set; }
    public int confidence { get; set; }
    public int direction { get; set; }
    public string imageFile { get; set; } = string.Empty;
    public int imageFileLen { get; set; }
    public string imageFragmentFile { get; set; } = string.Empty;
    public int imageFragmentFileLen { get; set; }
    public string license { get; set; } = string.Empty;
    public int Whitelist { get; set; }
    public Location location { get; set; } = new Location();
    public Timestamp timeStamp { get; set; } = new Timestamp();
    public int timeUsed { get; set; }
    public int triggerType { get; set; }
    public int type { get; set; }
    public int speed { get; set; }
    public Radarspeed radarSpeed { get; set; } = new Radarspeed();
    public int vehicleId { get; set; }
    public bool realplate { get; set; }
    public int retryflag { get; set; }
}

public class Location
{
    [JsonPropertyName("RECT")]
    public RECT rect { get; set; } = new RECT();
}

public class RECT
{
    public int left { get; set; }
    public int top { get; set; }
    public int right { get; set; }
    public int bottom { get; set; }
}

public class Timestamp
{
    public TimeVal Timeval { get; set; } = new TimeVal();
}

public class TimeVal
{
    public int sec { get; set; }
    public int usec { get; set; }
}

public class Radarspeed
{
    [JsonPropertyName("Speed")]
    public Speed speed { get; set; } = new Speed();
}

public class Speed
{
    public int PerHour { get; set; }
    public int Direction { get; set; }
}

#pragma warning restore IDE1006 // Estilos de Nomenclatura
