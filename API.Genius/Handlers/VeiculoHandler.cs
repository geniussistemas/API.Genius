using Microsoft.EntityFrameworkCore;
using Serilog;
using API.Genius.Data;
using API.Genius.Core.Handlers;
using API.Genius.Core.Models;
using API.Genius.Core.Responses;
using API.Genius.Core.Requests.VeiculosAlphadigi;
using API.Genius.Core.Requests.Veiculos;
using API.Genius.Core.Responses.Veiculos;
using API.Genius.Core.Responses.VeiculosAlphadigi;

namespace API.Genius.Handlers;

public class VeiculoHandler(AppDbContext context) : IVeiculoHandler
{
    public async Task<Response<VeiculoResponse?>> CreateEntradaPorPlacaAsync(PutEntradaPorPlacaRequest request)
    {
        var entradaSaidaPlaca = new EntradaSaidaPlaca
        {
            Placa = request.Placa?.Replace(" ", "").Replace("-", "") ?? string.Empty,
            IdCamera = request.IdCamera,
            Data = request.DataEntrada,
            ArquivoImagem = request.ArquivoImagem,
            Status = request.Status
            // TODO: Verificar se o campo status deve ser criado com NULL deve ser criado com algum valor
        };

        try
        {
            // No trecho abaixo, para fazer rollback das alterações, basta não 
            // chamar SaveChengesAsync
            await context.EntradaSaidaPlaca.AddAsync(entradaSaidaPlaca);    // Em memória
            await context.SaveChangesAsync();   // Efetiva alterações na base

            var response = new VeiculoResponse
            {
                Id = entradaSaidaPlaca.Id,
                Placa = entradaSaidaPlaca.Placa,    // Interrompa a transmissão
                IdTicket = "",
                DataHoraEntrada = entradaSaidaPlaca.Data
            };

            Log.Information($"Entrada do veículo: Placa - {entradaSaidaPlaca.Placa} Câmera - {entradaSaidaPlaca.IdCamera}");

            return new Response<VeiculoResponse?>(response, 201, "Entrada criada com sucesso");
        }
        catch (Exception e)
        {
            // *** Nunca é recomendado simplesmente silenciar a exceção

            Log.Error(e, $"Erro ao criar entrada do veículo: Placa - {entradaSaidaPlaca.Placa} Câmera - {entradaSaidaPlaca.IdCamera}");

            return new Response<VeiculoResponse?>(null, 500, "Não foi possível efetuar a entrada do veículo");
        }
    }

    public async Task<FlatResponse<PutEntradaPorPlacaAlphadigiResponse?>> CreateEntradaCameraAlphadigiAsync(PutEntradaPorPlacaAlphadigiRequest request)
    {
        var internalRequest = new PutEntradaPorPlacaRequest
        {
            Placa = request.alarmInfoPlate.result.plateResult.license.Replace(" ", "") ?? string.Empty,
            IdCamera = request.alarmInfoPlate.channel,
            DataEntrada = DateTime.Now,
            ArquivoImagem = request.alarmInfoPlate.result.plateResult.imageFile ?? string.Empty
        };

        var result = await CreateEntradaPorPlacaAsync(internalRequest);

        // if (!result.IsSuccess)
        if (result.Data == null)
        {
            return new FlatResponse<PutEntradaPorPlacaAlphadigiResponse?>(null, result.Code, result.Message ?? "Não foi possível efetuar a entrada do veículo");
        }

        var internalResponse = new PutEntradaPorPlacaAlphadigiResponse
        {
            responseAlarmInfoPlate = new ResponseAlarmInfoPlate
            {
                info = "ok",
                content = "retransfer_stop",    // Interrompa a transmissão
                isPay = true.ToString(),
            }
        };

        return new FlatResponse<PutEntradaPorPlacaAlphadigiResponse?>(internalResponse, result.Code, result.Message);
    }
    
    public async Task<Response<VeiculoResponse?>> GetVeiculoPorPlacaAsync(GetVeiculoPorPlacaRequest request)
    {
        try
        {
            // Reidratação (buscar dados do banco de dados)
            // Usar AsNoTracking quando quiser apenas buscar dados para 
            // exibição (é mais otimizada)
            var veiculo = await context
                .EntradaSaidaPlaca
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Placa.CompareTo(request.Placa) == 0);

            if (veiculo is null)
            {
                Log.Warning($"Veículo não encontrado: Placa - {request.Placa}");
                return new Response<VeiculoResponse?>(null, 404, "Veículo não encontrado");
            }

            Log.Information($"Veículo encontrado: Placa - {veiculo.Placa}");
            
            var response = new VeiculoResponse
            {
                // TODO: Revisar informações vindas de veiculo
                Id = veiculo.Id,
                Placa = veiculo.Placa,
                IdTicket = "",
                DataHoraEntrada = veiculo.Data
            };

            return new Response<VeiculoResponse?>(response);
        }
        catch (Exception e)
        {
            Log.Error(e, $"Não foi possível recuperar o veículo com a placa: {request.Placa}");
            return new Response<VeiculoResponse?>(null, 500, "Não foi possível recuperar o veículo");
        }
    }

}
