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

        endpoints.MapGroup("/OnCarHandled/CameraAlphadigi")
            .WithTags("Veiculos")
            .MapEndpoint<PutEntradaPorPlacaAlphadigiLegacyEndpoint>();

        endpoints.MapGroup("/v1/veiculos/entradaalphadigi")
            .WithTags("Veiculos")
            .MapEndpoint<PutEntradaPorPlacaAlphadigiEndpoint>();

        endpoints.MapGroup("/v1/veiculos")
            .WithTags("Veiculos")
            .MapEndpoint<PutEntradaPorPlacaEndpoint>();

    }

    private static IEndpointRouteBuilder MapEndpoint<TEndpoint>(this IEndpointRouteBuilder app)
        where TEndpoint : IEndpoint
    {
        TEndpoint.Map(app);
        return app;
    }

}
