using System;
using API.Genius.Common.Api;
using API.Genius.Core.Constants;
using API.Genius.Core.Handlers;
using API.Genius.Core.Models;
using API.Genius.Core.Requests.Veiculos;
using API.Genius.Core.Responses;

namespace API.Genius.Endpoints.Veiculos;

public class PutEntradaSaidaPorPlacaEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
        => app.MapPut("", HandleAsync)
            .WithName("Veículo: Efetua entrada por placa/LPR")
            .WithSummary("Entrada por placa/LPR")
            .WithDescription("Entrada do veículo no estacionamento por placa/LPR")
            .WithOrder(4)
            .Produces<Response<Veiculo?>>();

    private static async Task<IResult> HandleAsync(
        IVeiculoHandler handler,
        PutEntradaSaidaPorPlacaRequest request)
    {
        request.Status = StatusEntradaSaidaPlaca.NaoInformado;

        var result = await handler.CreateEntradaSaidaPorPlacaAsync(request);

        return result.IsSuccess
            ? TypedResults.Ok(result)
            : TypedResults.BadRequest(result);
    }

}
