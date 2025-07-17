using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using API.Genius.Clients;
using API.Genius.Clients.DTOs;
using Refit;

namespace API.Genius.Controllers;

public class GarenAPIController : ControllerBase
{
    private readonly GarenApiClient _garenApiClient;

    public GarenAPIController(GarenApiClient garenApiClient)
    {
        _garenApiClient = garenApiClient;
    }

    /*
        [HttpGet("last_plate")]
        public async Task<IActionResult> GetLastPlate(string id)
        {
            try
            {
                var lastPlate = await _garenApiClient.GetLastPlateById(id);
                return Ok(lastPlate);
            }
            catch (Exception)
            {
                // Tratar o erro de forma apropriada para o consumidor da SUA API
                return StatusCode(503, "Serviço externo indisponível no momento."); // 503 Service Unavailable
            }
        }
    */

    [HttpPost("preferences")]
    public async Task<IActionResult> AcionamentoRemoto([FromBody] GarenApiAcionamentoRemotoRequestDto request)
    {
        try
        {
            var response = await _garenApiClient.AcionamentoRemoto(request);
            return Ok(response);

            // // Boa prática: Retornar 201 Created com a localização do novo recurso.
            // // O primeiro parâmetro é o nome da ação GET que busca um item por ID.
            // return CreatedAtAction(nameof(GetPreferenceById), new { id = preference.Id }, preference);
        }
        catch (ApiException ex) // Refit lança ApiException
        {
            // TODO: Inspecionar ex.StatusCode, ex.Content, etc.
            return StatusCode((int)ex.StatusCode, ex.Content);
        }
        catch (Exception)
        {
            // Tratar o erro de forma apropriada para o consumidor da SUA API
            return StatusCode(503, "Serviço externo indisponível no momento."); // 503 Service Unavailable
        }
    }
}