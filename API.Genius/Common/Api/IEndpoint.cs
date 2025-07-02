using System;

namespace API.Genius.Common.Api;

public interface IEndpoint
{
    public abstract static void Map(IEndpointRouteBuilder app);
}
