using System;

namespace API.Genius.Core;

public static class Configuration
{

    public const int DefaultStatusCode = 200;
    public const int DefaultPageNumber = 1;
    public const int DefaultPageSize = 25;

    public const int MaxLocationIdLength = 15;
    public const int MaxLocationNameLength = 50;
    public const int MaxTicketIdLength = 50;
    public const int MaxDescriptionLength = 50;
    public const int MaxEntryLaneLength = 20;

    public static string BackendUrl { get; set; } = "http://localhost:5250";
    public static string FrontendUrl { get; set; } = "http://localhost:5200";
}
