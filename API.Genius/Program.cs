using API.Genius;
using API.Genius.Common.Api;
using API.Genius.Endpoints;

var builder = WebApplication.CreateBuilder(args);
// A adição dos serviços deve seguir a ordem abaixo
builder.AddLogging();
builder.AddConfiguration();
builder.AddDataContexts();
builder.AddCrossOrigin();
builder.AddDocumentation();
builder.AddServices();

var app = builder.Build();
app.ConfigureDevEnvironment();
app.UseCors(ApiConfiguration.CorsPolicyName);
app.MapEndpoints();

app.Run();



