using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Genius.Clients.DTOs;
using Refit;

namespace API.Genius.Clients
{
    public class GarenApiClient : IGarenApiClient
    {
        private readonly HttpClient _httpClient;

        // O HttpClient é injetado aqui!
        public GarenApiClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        /*
                public async Task<GarenApiAcionamentoRemotoResponseDto> GetLastPlateById(string id)
                {
                    try
                    {
                        // O BaseUrl já foi configurado na injeção de dependência.
                        var response = await _httpClient.GetFromJsonAsync<GarenApiAcionamentoRemotoResponseDto>($"api/lpr/1/last_plate");

                        // Se a API retornar um 404, GetFromJsonAsync pode retornar null ou lançar exceção
                        // dependendo da versão do .NET. É bom verificar.
                        if (response == null)
                        {
                            // Lançar uma exceção customizada ou retornar null, dependendo da sua estratégia.
                            throw new Exception("Preferência não encontrada na API externa.");
                        }

                        return response;
                    }
                    catch (HttpRequestException ex)
                    {
                        // TODO: Logar o erro (ex.StatusCode, ex.Message)
                        // TODO: Lançar uma exceção mais específica para a camada de cima tratar.
                        throw new Exception("Erro ao comunicar com a API externa.", ex);
                    }
                }
        */

        public async Task<GarenApiAcionamentoRemotoResponseDto> AcionamentoRemoto([Body] GarenApiAcionamentoRemotoRequestDto request)
        {
            try
            {
                // O BaseUrl já foi configurado na injeção de dependência.
                HttpResponseMessage response = await _httpClient.PostAsJsonAsync("/acionamento_remoto", request);

                // Lança exceção se o status code não for sucesso (2xx)
                response.EnsureSuccessStatusCode();

                // Desserializa o corpo da resposta para um DTO ou lança uma exceção.
                var responseJson = await response.Content.ReadFromJsonAsync<GarenApiAcionamentoRemotoResponseDto>()
                                   ?? throw new Exception("A API da Garen retornou uma resposta vazia após a criação da preferência.");

                return responseJson;
            }
            catch (HttpRequestException ex)
            {
                // TODO: Logar o erro (ex.StatusCode, ex.Message)
                // TODO: Lançar uma exceção mais específica para a camada de cima tratar.
                throw new Exception("Erro ao comunicar com a API da Garen.", ex);
            }
        }


    }
}