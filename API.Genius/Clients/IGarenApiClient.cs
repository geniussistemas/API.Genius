using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Genius.Clients.DTOs;
using Refit;

namespace API.Genius.Clients;

public interface IGarenApiClient
{
    [Post("/api/comando")]
    Task<GarenApiAcionamentoRemotoResponseDto> AcionamentoRemoto([Body] GarenApiAcionamentoRemotoRequestDto request);
    // [Get("/api/lpr/1")]
    // Task<GarenApiAcionamentoRemotoResponseDto> GetLastPlateById(string id);
}