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
    public async Task<FlatResponse<PutEntradaPorPlacaAlphadigiResponse?>> CreateAsyncAlphadigi(PutEntradaPorPlacaAlphadigiRequest request)
    {
        var entradaSaidaPlaca = new EntradaSaidaPlaca
        {
            Placa = request.alarmInfoPlate.result.plateResult.license.Replace(" ", "") ?? string.Empty,
            IdCamera = request.alarmInfoPlate.channel,
            Data = DateTime.Now,
            ArquivoImagem = request.alarmInfoPlate.result.plateResult.imageFile ?? string.Empty
            // TODO: Verificar se o campo status deve ser criado com NULL deve ser criado com algum valor
        };

        try
        {
            // No trecho abaixo, para fazer rollback das alterações, basta não 
            // chamar SaveChengesAsync
            await context.EntradaSaidaPlaca.AddAsync(entradaSaidaPlaca);    // Em memória
            await context.SaveChangesAsync();   // Efetiva alterações na base

            var response = new PutEntradaPorPlacaAlphadigiResponse
            {
                responseAlarmInfoPlate = new ResponseAlarmInfoPlate
                {
                    info = "ok",
                    content = "retransfer_stop",    // Interrompa a transmissão
                    isPay = true.ToString(),
                }
            };

            Log.Information($"Entrada do veículo: Placa - {entradaSaidaPlaca.Placa} Câmera - {entradaSaidaPlaca.IdCamera}");

            return new FlatResponse<PutEntradaPorPlacaAlphadigiResponse?>(response, 201, "Entrada criada com sucesso");
        }
        catch (Exception e)
        {
            // *** Nunca é recomendado simplesmente silenciar a exceção

            Log.Error(e, $"Erro ao criar entrada do veículo: Placa - {entradaSaidaPlaca.Placa} Câmera - {entradaSaidaPlaca.IdCamera}");

            return new FlatResponse<PutEntradaPorPlacaAlphadigiResponse?>(null, 500, "Não foi possível efetuar a entrada do veículo");
        }
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
                id = veiculo.Id,
                licensePlate = veiculo.Placa,
                ticketId = "",  //veiculo.TicketId,
                cameraId = 0, //veiculo.CameraId,
                entryDateTime = veiculo.Data,
                entryImage = veiculo.ArquivoImagem
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
