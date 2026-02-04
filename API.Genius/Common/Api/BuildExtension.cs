using System;
using System.Drawing;
using System.Runtime.InteropServices.JavaScript;
using Microsoft.EntityFrameworkCore;
using API.Genius.Data;
using API.Genius.Handlers;
using API.Genius.Core;
using API.Genius.Core.Handlers;
using Serilog;
using Serilog.Core;
using Serilog.Events;

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
        Configuration.CorsAllowedOrigins = builder.Configuration.GetSection("CorsSettings:AllowedOrigins").Get<string[]>() ?? [];
        Configuration.DefaultLogLevel = builder.Configuration.GetSection("Serilog:MinimumLevel:Default").Value ?? string.Empty;

        Log.Information(@"Configuration.BackendUrl: ""{backendUrl}""", Configuration.BackendUrl);
        Log.Information(@"Configuration.FrontendUrl: ""{frontendUrl}""", Configuration.FrontendUrl);
        Log.Information(@"Configuration.CorsAllowedOrigins: ""{allowedOrigins}""", string.Join(", ", Configuration.CorsAllowedOrigins));
        Log.Information(@"Configuration.DefaultLogLevel: ""{logLevel}""", Configuration.DefaultLogLevel);
        Log.Information(@"ApiConfiguration.GerenciadorHost: ""{managerHost}""", ApiConfiguration.GerenciadorHost);
        Log.Information(@"ApiConfiguration.GerenciadorPort: ""{managerPort}""", ApiConfiguration.GerenciadorPort);
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
                            policy
                                .WithOrigins(Configuration.CorsAllowedOrigins)
                                .AllowAnyHeader()
                                .AllowAnyMethod()
                               //
                               // OBSERVAÇÃO:
                               // O ideal é ser restritivo nas políticas do CORS
                               // Por enquanto libera acesso a todos os domínios
                               // considerando que está sendo executado em rede interna
                               //.WithOrigins([
                               //     Configuration.FrontendUrl,
                               //     Configuration.BackendUrl,
                               //     ])
                               //.AllowCredentials()
                               //
                               // Possível abordagem para permitir IPs de uma determinada faixa
                               //    .SetIsOriginAllowed(origin =>
                               //    {
                               //        // Verifica se a origem começa com "http://10.0.0."
                               //        return origin.StartsWith("http://10.0.0.");
                               //    })
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
        var logFilePath = Path.Combine(
            AppContext.BaseDirectory,
            "logs",
            $"{AppDomain.CurrentDomain.FriendlyName}.log");
        
        var defaultLogLevel = builder.Configuration.GetSection("Serilog:MinimumLevel:Default").Value ?? string.Empty;

        var levelSwitch = new LoggingLevelSwitch(defaultLogLevel switch
        {
            "Verbose" => LogEventLevel.Verbose,
            "Debug" => LogEventLevel.Debug,
            "Information" => LogEventLevel.Information,
            "Warning" => LogEventLevel.Warning,
            "Error" => LogEventLevel.Error,
            "Fatal" => LogEventLevel.Fatal,
            _ => LogEventLevel.Information
        });

        Directory.CreateDirectory(Path.Combine(AppContext.BaseDirectory, "logs"));

        // Adicionar as linhas abaixo caso queira que o host não utilize o EventLogLogger do Windows
        //builder.Logging.ClearProviders();
        //builder.Logging.AddConsole();

        Log.Logger = new LoggerConfiguration()
            .WriteTo.File(
                logFilePath,
                fileSizeLimitBytes: 1_000_000, // 1 MB
                rollOnFileSizeLimit: true, // cria um novo arquivo quando o tamanho atinge o limite  
                rollingInterval: RollingInterval.Day, // cria um novo arquivo a cada dia
                retainedFileCountLimit: 10 // mantém até 10 arquivos antigos
            )
            .WriteTo.Console(/*outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level:u3}] {Message:lj}{NewLine}{Exception}"*/)
            .MinimumLevel.ControlledBy(levelSwitch)
            .MinimumLevel.Override("Microsoft", Serilog.Events.LogEventLevel.Warning)
            .CreateLogger();

        Log.Information("Logger configurado com sucesso com nível {logLevel} (arquivo inicial {logFilePath}).",defaultLogLevel,  logFilePath);
    }

}
