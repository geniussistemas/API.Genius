using API.Genius.Common.Api;
using API.Genius.Endpoints.Alphadigi;
using API.Genius.Endpoints.Veiculos;

namespace API.Genius.Endpoints;

public static class Endpoint
{
    public static void MapEndpoints(this WebApplication app)
    {
        var endpoints = app.MapGroup("");

        endpoints.MapGroup("/")
            .WithTags("Health Check")
            .MapGet("", () => new { message = "Genius.API is running", });

        // Endpoint originado do apiCamera (Robson)
        endpoints.MapGroup("/OnCarHandled/CameraAlphadigi")
            .WithTags("Veiculos")
            .MapEndpoint<PutEntradaCameraAlphadigiLegacyEndpoint>();

        // Endpoint que segue mesmo fluxo do /OnCarHandled/CameraAlphadigi
        endpoints.MapGroup("/v1/veiculos/entradacameraalphadigi")
            .WithTags("Veiculos")
            .MapEndpoint<PutEntradaCameraAlphadigiEndpoint>();

        // Entrada direta por placa
        endpoints.MapGroup("/v1/veiculos/entradaplaca")
            .WithTags("Veiculos")
            .MapEndpoint<PutEntradaPorPlacaEndpoint>();

        // Entrada direta por placa com LPR da Alphadigi
        endpoints.MapGroup("/v1/veiculos/entradaplacaalphadigi")
            .WithTags("Veiculos")
            .MapEndpoint<PutEntradaPorPlacaAlphadigiEndpoint>();

    }

    private static IEndpointRouteBuilder MapEndpoint<TEndpoint>(this IEndpointRouteBuilder app)
        where TEndpoint : IEndpoint
    {
        TEndpoint.Map(app);
        return app;
    }

}
