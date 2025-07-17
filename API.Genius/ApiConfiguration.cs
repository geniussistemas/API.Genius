using System;

namespace API.Genius;

public static class ApiConfiguration
{
    public class GarenApiSettings
    {
        public string BaseUrl { get; set; } = string.Empty;
        public string AccessToken { get; set; } = string.Empty;
    }

    public const string UserId = "desenvolvimento1@geniussistemas.com.br";
    public static string ConnectionString { get; set; } = string.Empty;
    public static string CorsPolicyName = "wasm";   // O nome é indiferente
    public static string GerenciadorHost { get; set; } = string.Empty;
    public static int GerenciadorPort { get; set; } = 0;
    public static GarenApiSettings GarenApi { get; set; } = new GarenApiSettings();
}
