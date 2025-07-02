using System;

namespace API.Genius.Core.Models;

public class RequestResponseServer<DataClass> where DataClass : class, new()
{
    private DataClass _data = new();
    
#pragma warning disable IDE1006 // Estilos de Nomenclatura
    // TODO: Usar constantes para inicializar os valores
    public int protocolVersion { get; set; } = 1;
    public string senderName { get; set; } = string.Empty;
    public string messageType { get; set; } = string.Empty;
    public DataClass data { get => _data; set => _data = value; }
#pragma warning restore IDE1006 // Estilos de Nomenclatura

}
