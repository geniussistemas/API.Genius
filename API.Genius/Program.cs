using API.Genius;
using API.Genius.Common.Api;
using API.Genius.Endpoints;

var builder = WebApplication.CreateBuilder(args);

// Permite rodar como Serviço Windows
builder.Host.UseWindowsService();

// Permite rodar sob IIS
builder.WebHost.UseIISIntegration();

// A adição dos serviços deve seguir a ordem abaixo
builder.AddLogging();
builder.AddConfiguration();
builder.AddDataContexts();
builder.AddCrossOrigin();
builder.AddDocumentation();
builder.AddServices();

/******************************************************************************
 * Habilitar o Worker apenas se for necessário executar código em paralelo 
 * enquanto atende as requisições REST como por exemplo, rotinas de limpeza, 
 * tratamento de filas, etc.
 * 
 * Caso o objetivo seja apenas atender as requisições, o Worker é desnecessário
 *****************************************************************************/ 
// Worker em paralelo
// builder.Services.AddHostedService<Worker>();

var app = builder.Build();
app.ConfigureDevEnvironment();
app.UseCors(ApiConfiguration.CorsPolicyName);
app.MapEndpoints();

app.Run();

