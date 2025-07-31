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

public class PutEntradaSaidaPorPlacaAlphadigiEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
        => app.MapPut("", HandleAsync)
            .WithName("Veículo: Entrada/saída por placa/LPR com câmera Alphadigi")
            .WithSummary("Entrada/saída por placa/LPR")
            .WithDescription("Entrada e saída do veículo no estacionamento por placa/LPR")
            .WithOrder(4)
            .Produces<Response<Veiculo?>>();

    private static async Task<IResult> HandleAsync(
        IVeiculoHandler handler,
        PutEntradaSaidaPorPlacaAlphadigiRequest request)
    {
        var internalRequest = new PutEntradaSaidaPorPlacaRequest
        {
            Placa = request.alarmInfoPlate.result.plateResult.license.Replace(" ", ""),
            IdCamera = request.alarmInfoPlate.channel,
            DataEvento = DateTime.Now,
            ArquivoImagem = request.alarmInfoPlate.result.plateResult.imageFile,
            Status = (request.alarmInfoPlate.result.plateResult.direction == AlphadigiCamDirection.Coming?
                        StatusEntradaSaidaPlaca.Entrada:
                        (request.alarmInfoPlate.result.plateResult.direction == AlphadigiCamDirection.Going?
                            StatusEntradaSaidaPlaca.Saida:
                            StatusEntradaSaidaPlaca.NaoInformado
                        )
                      )
        };

        var result = await handler.CreateEntradaSaidaPorPlacaAsync(internalRequest);

        return result.IsSuccess
            ? TypedResults.Ok(result)
            : TypedResults.BadRequest(result);
    }
    
}