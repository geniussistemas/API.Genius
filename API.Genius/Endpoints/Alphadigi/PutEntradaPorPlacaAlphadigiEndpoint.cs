using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Genius.Common.Api;
using API.Genius.Core.Handlers;
using API.Genius.Core.Models;
using API.Genius.Core.Requests.VeiculosAlphadigi;
using API.Genius.Core.Responses;
using Serilog;

namespace API.Genius.Endpoints.Alphadigi;

public class PutEntradaPorPlacaAlphadigiEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
        => app.MapPut("", HandleAsync)
            .WithName("Veiculo: Efetua entrada por placa/LPR (Alphadigi)")
            .WithSummary("Entrada por placa/LPR")
            .WithDescription("Entrada do veículo no estacionamento por placa/LPR")
            .WithOrder(4)
            .Produces<FlatResponse<VeiculoAlphadigi?>>();
    
    protected static async Task<IResult> HandleAsync(
        IVeiculoHandler handler, PutEntradaPorPlacaAlphadigiRequest request)
    {
        var result = await handler.CreateAsyncAlphadigi(request);

        return result.IsSuccess
            ? TypedResults.Ok(result)
            : TypedResults.BadRequest(result);
    }

}
