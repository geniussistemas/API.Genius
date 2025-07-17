using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace API.Genius.Clients;

public class GarenAuthHeaderHandler(IHttpContextAccessor httpContextAccessor) : DelegatingHandler
{
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        string? token = ApiConfiguration.GarenApi.AccessToken;

        // Se não tem token configurado, tenta obter o token do contexto HTTP da 
        // requisição atual que chega na API.
        // Isso é útil quando a API está repassando o token de um usuário logado.
        if (!string.IsNullOrEmpty(token))
        {
            token = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"].ToString();
        }

        // Se encontrou um token, o adiciona ao header da requisição que VAI SAIR para a API externa.
        // É importante verificar se o token não é nulo ou vazio e se já não existe um header de autorização.
        if (!string.IsNullOrEmpty(token) && request.Headers.Authorization == null)
        {
            // O token vindo do HttpContext geralmente já vem com "Bearer ", mas é bom garantir.
            // Se ele vier sem, você deve adicionar o prefixo.
            if (AuthenticationHeaderValue.TryParse(token, out var headerValue))
            {
                request.Headers.Authorization = headerValue;
            }
        }

        // Chama o próximo handler na cadeia para que a requisição continue seu fluxo.
        // ESTA LINHA NÃO PODE SER ALTERADA OU EXCLUÍDA
        return await base.SendAsync(request, cancellationToken);
    }
}
