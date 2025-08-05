using System;

namespace API.Genius;

public static class ApiConfiguration
{
    public static string ConnectionString { get; set; } = string.Empty;
    public static string CorsPolicyName = "wasm";   // O nome é indiferente
    public static string GerenciadorHost { get; set; } = string.Empty;
    public static int GerenciadorPort { get; set; } = 0;
}
