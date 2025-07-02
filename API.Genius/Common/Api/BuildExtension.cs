using System;
using System.Drawing;
using Microsoft.EntityFrameworkCore;
using API.Genius.Data;
using API.Genius.Handlers;
using API.Genius.Core;
using API.Genius.Core.Handlers;
using Serilog;

namespace API.Genius.Common.Api;

    public static class BuildExtension
{
    public static void AddConfiguration(this WebApplicationBuilder builder)
    {
        ApiConfiguration.ConnectionString = builder.Configuration.GetConnectionString("DefaultConnectionString") ?? string.Empty;
        ApiConfiguration.GerenciadorHost = builder.Configuration.GetValue<string>("GerenciadorHost") ?? string.Empty;
        ApiConfiguration.GerenciadorPort = builder.Configuration.GetValue<int>("GerenciadorPort", 0);
        Configuration.BackendUrl = builder.Configuration.GetValue<string>("BackendUrl") ?? string.Empty;
        Configuration.FrontendUrl = builder.Configuration.GetValue<string>("FrontendUrl") ?? string.Empty;

        Log.Information($"Configuration.BackendUrl: \"{Configuration.BackendUrl}\"");
        Log.Information($"Configuration.FrontendUrl: \"{Configuration.FrontendUrl}\"");
        Log.Information($"ApiConfiguration.GerenciadorHost: \"{ApiConfiguration.GerenciadorHost}\"");
        Log.Information($"ApiConfiguration.GerenciadorPort: \"{ApiConfiguration.GerenciadorPort}\"");
    }

    // Adiciona serviços de documentação para a API usando o Swagger
    public static void AddDocumentation(this WebApplicationBuilder builder)
    {
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(x =>
        {
            // Garante que o Swagger usará os namespaces completos
            // No lugar de "Category", ele usará "Fina.Core.Models.Category"
            // Ajuda quando há classes com mesmo nome em namespaces diferentes
            x.CustomSchemaIds(n => n.FullName);
        });
    }

    // Adiciona serviços de banco de dados
    public static void AddDataContexts(this WebApplicationBuilder builder)
    {
        builder.Services
            .AddDbContext<AppDbContext>(
                options =>
                {
                    options.UseSqlServer(ApiConfiguration.ConnectionString);
                });
    }

    // CORS -> Cross Oriins Resource Sharing
    //      -> Compartilhamento de recursos entre domínios diferentes
    // Esta aplicação tem dois domínios/portas diferentes e precisa do CORS
    // Impede que a API receba requisições de domínios não autorizados
    public static void AddCrossOrigin(this WebApplicationBuilder builder)
    {
        builder.Services.AddCors(
            options => options.AddPolicy(
                        ApiConfiguration.CorsPolicyName,
                        policy =>
                            policy.WithOrigins([
                                Configuration.FrontendUrl,
                                Configuration.BackendUrl
                                ])
                               .AllowAnyMethod()
                               .AllowAnyHeader()
                               .AllowCredentials()
                        )
                );
    }

    public static void AddServices(this WebApplicationBuilder builder)
    {
        builder.Services
            .AddTransient<IVeiculoHandler, VeiculoHandler>();
    }

    public static void AddLogging(this WebApplicationBuilder builder)
    {
        var logFilePath = $"logs\\{System.AppDomain.CurrentDomain.FriendlyName}.log";

        Log.Logger = new LoggerConfiguration()
            .WriteTo.File(
                logFilePath,
                fileSizeLimitBytes: 1_000_000, // 1 MB
                rollOnFileSizeLimit: true, // cria um novo arquivo quando o tamanho atinge o limite  
                rollingInterval: RollingInterval.Day, // cria um novo arquivo a cada dia
                retainedFileCountLimit: 10 // mantém até 10 arquivos antigos
            )
            .WriteTo.Console(/*outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level:u3}] {Message:lj}{NewLine}{Exception}"*/)
            .MinimumLevel.Debug()
            .MinimumLevel.Override("Microsoft", Serilog.Events.LogEventLevel.Warning)
            .CreateLogger();

        Log.Information($"Logger configurado com sucesso (arquivo inicial {logFilePath}).");
    }

}
