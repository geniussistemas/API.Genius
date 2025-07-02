namespace API.Genius.Common.Api;

public static class AppExtension
{
    public static void ConfigureDevEnvironment(this WebApplication app)
    {
        // Documentação da API (Swagger) apenas em desenvolvimento
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();

            // Usado para quando há autenticação
            // app.MapSwagger().RequireAuthorization();
        }
    }
}
