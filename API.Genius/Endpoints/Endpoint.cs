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
            .WithTags("OnCarHandled")
            .MapEndpoint<PutEntradaSaidaCameraAlphadigiLegacyEndpoint>();

        // Endpoint que segue mesmo fluxo do /OnCarHandled/CameraAlphadigi
        endpoints.MapGroup("/v1/veiculos")
            .WithTags("Veiculos")
            .MapEndpoint<PutEntradaSaidaCameraAlphadigiEndpoint>()
            .MapEndpoint<PutEntradaSaidaPorPlacaEndpoint>()
            .MapEndpoint<PutEntradaSaidaPorPlacaAlphadigiEndpoint>();

    }

    private static IEndpointRouteBuilder MapEndpoint<TEndpoint>(this IEndpointRouteBuilder app)
        where TEndpoint : IEndpoint
    {
        TEndpoint.Map(app);
        return app;
    }

}
