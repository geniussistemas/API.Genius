using API.Genius.Core.Models;
using API.Genius.Core.Requests.Veiculos;
using API.Genius.Core.Requests.VeiculosAlphadigi;
using API.Genius.Core.Responses;
using API.Genius.Core.Responses.Veiculos;
using API.Genius.Core.Responses.VeiculosAlphadigi;

namespace API.Genius.Core.Handlers;

public interface IVeiculoHandler
{
    //    Task<Response<Veiculo?>> CreateAsync(PutEntradaPorPlacaAlphadigiRequest request);
    Task<FlatResponse<PutEntradaPorPlacaAlphadigiResponse?>> CreateAsyncAlphadigi(PutEntradaPorPlacaAlphadigiRequest request);
    Task<Response<VeiculoResponse?>> GetVeiculoPorPlacaAsync(GetVeiculoPorPlacaRequest request);
}
