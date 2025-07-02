using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Genius.Core.Requests.Veiculos;

public class GetVeiculoPorPlacaRequest
{
    public string Placa { get; set; } = string.Empty;
}