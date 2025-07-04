using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Genius.Common.Api;
using API.Genius.Core.Constants;
using API.Genius.Core.Handlers;
using API.Genius.Core.Models;
using API.Genius.Core.Requests.Veiculos;
using API.Genius.Core.Requests.VeiculosAlphadigi;
using API.Genius.Core.Responses;
using API.Genius.Endpoints.Veiculos;

namespace API.Genius.Endpoints.Alphadigi;

public class PutEntradaPorPlacaAlphadigiEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
        => app.MapPut("", HandleAsync)
            .WithName("Veículo: Entrada por placa/LPR com câmera Alphadigi")
            .WithSummary("Entrada por placa/LPR")
            .WithDescription("Entrada do veículo no estacionamento por placa/LPR")
            .WithOrder(4)
            .Produces<Response<Veiculo?>>();

    private static async Task<IResult> HandleAsync(
        IVeiculoHandler handler,
        PutEntradaPorPlacaAlphadigiRequest request)
    {
        var internalRequest = new PutEntradaPorPlacaRequest
        {
            Placa = request.alarmInfoPlate.result.plateResult.license.Replace(" ", ""),
            IdCamera = request.alarmInfoPlate.channel,
            DataEntrada = DateTime.Now,
            ArquivoImagem = request.alarmInfoPlate.result.plateResult.imageFile,
            Status = StatusEntradaSaidaPlaca.Entrada
        };

        var result = await handler.CreateEntradaPorPlacaAsync(internalRequest);

        return result.IsSuccess
            ? TypedResults.Ok(result)
            : TypedResults.BadRequest(result);
    }
    
}