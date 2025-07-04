using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Genius.Core.Constants;

public class StatusEntradaSaidaPlaca
{
    #region Valores null e 1 são legados da API inicial desenvolvida pelo Robson
    public static readonly int? EntradaSaidaPlacaLegado = null;
    public static readonly int? EntradaSaidaPlacaProcessadaLegado = 1;
    #endregion
    public const int Entrada = 10;
    public const int Saida = 20;
}