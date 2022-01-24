using Lotus_Authentication.API.ApiModels;
using Microsoft.AspNetCore.Mvc;

namespace Lotus_Authentication.API.Controllers;

[ApiController]
public class ReportController
{
    /// <summary>
    /// An API endpoint to generate an array of users currently connected to the api key
    /// </summary>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    [HttpGet, Route("api/report/users")]
    public Task<IActionResult> GetUserReport([FromHeader] string apiKey)
    {
        throw new NotImplementedException();
    }
}
