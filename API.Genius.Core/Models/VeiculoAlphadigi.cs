using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Genius.Core.Models;

public class VeiculoAlphadigi
{
    public string? Placa { get; set; } = string.Empty;
    public int IdCamera { get; set; } = 0;
    public string? Imagem { get; set; } = string.Empty;
}
