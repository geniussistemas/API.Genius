using System;
using API.Genius.Lib.Util;

namespace API.Genius.Core.Models;

public class ResponseFromServer<DataClass> : RequestResponseServer<DataClass>
    where DataClass : class, new()
{
#pragma warning disable IDE1006 // Estilos de Nomenclatura
    public int responseCode { get; set; } = ServerResponseCode.Success;
    public string responseMessage { get; set; } = string.Empty;
#pragma warning restore IDE1006 // Estilos de Nomenclatura
}
