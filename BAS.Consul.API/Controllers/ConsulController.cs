using System.Text.Json;

using Microsoft.AspNetCore.Mvc;

namespace BAS.Consul.API.Controllers;

[ApiController]
[Route("[controller]")]
public class ConsulController : ControllerBase
{
    private readonly ILogger<ConsulController> _logger;
    private readonly IConfiguration _configuration;

    public ConsulController(ILogger<ConsulController> logger, IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;
    }

    [HttpGet]
    public ActionResult<List<string>> GetValues()
    {
        List<string> values = new() {
            _configuration!["key1"]!,
            _configuration!["key2"]!,
            _configuration!["key3"]!,
            _configuration!["key4"]!
        };

        _logger.LogInformation("{Values}", JsonSerializer.Serialize(values));

        return values;
    }
}