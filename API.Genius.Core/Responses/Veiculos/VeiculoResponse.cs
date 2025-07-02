using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Genius.Core.Responses.Veiculos;

public class VeiculoResponse
{
    // TODO: Complementar com as demais informações do veículo
#pragma warning disable IDE1006 // Estilos de Nomenclatura
    public long id { get; set; }
    public string licensePlate { get; set; } = string.Empty;
    public string ticketId { get; set; } = string.Empty;
    public int cameraId { get; set; }
    public DateTime? entryDateTime { get; set; }
    public string entryImage { get; set; } = string.Empty;
#pragma warning restore IDE1006 // Estilos de Nomenclatura
}