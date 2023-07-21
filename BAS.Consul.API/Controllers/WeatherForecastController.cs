using BAS.Consul.API.Helpers;
using BAS.Consul.API.Models;

using Consul;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace BAS.Consul.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;
        public IConsulClient ConsulClient { get; }

        private readonly IConfiguration _configuration;
        private readonly DemoAppSettings _options;
        private readonly DemoAppSettings _optionsSnapshot;

        public WeatherForecastController(
            ILogger<WeatherForecastController> logger,
            IConsulClient consulClient,
            IConfiguration configuration,
            IOptions<DemoAppSettings> options,
            IOptionsSnapshot<DemoAppSettings> optionsSnapshot)
        {
            _logger = logger;
            ConsulClient = consulClient;
            _configuration = configuration;
            _options = options.Value;
            _optionsSnapshot = optionsSnapshot.Value;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<WeatherForecast>>> GetAsync(string key = "Sinjul")
        {
            {
                var data = new string[] { _configuration["DemoAppSettings:Key1"]!, _options.Key1, _optionsSnapshot.Key1 };
            }

            {
                var consulDemoKey = await ConsulKeyValueProvider.GetValueAsync<ConsulDemoKey>(key);

                if (consulDemoKey != null && consulDemoKey.IsEnabled)
                {
                    //return Ok(consulDemoKey);

                    //return Ok("ConsulDemoKey is null");}
                }
            }

            {
                var str = string.Empty;
                //query the value  
                var res = await ConsulClient.KV.Get(key);

                if (res.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    //convert byte[] to string  
                    str = System.Text.Encoding.UTF8.GetString(res.Response.Value);
                }

                //return $"value-{str}";
            }

            var datas = Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
                .ToArray();

            return datas;
        }
    }
}