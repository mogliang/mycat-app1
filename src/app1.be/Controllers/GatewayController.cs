using Microsoft.AspNetCore.Mvc;

namespace app1.be.Controllers;

[ApiController]
[Route("[controller]")]
public class GatewayController : ControllerBase
{
    private static readonly string[] Summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    private readonly ILogger<GatewayController> _logger;

    public GatewayController(ILogger<GatewayController> logger)
    {
        _logger = logger;
    }

    [HttpGet(Name = "Invoke")]
    public IEnumerable<WeatherForecast> Invoke(string test)
    {
        return Enumerable.Range(1, 5).Select(index => new WeatherForecast
        {
            Date = DateTime.Now.AddDays(index),
            TemperatureC = Random.Shared.Next(-20, 55),
            Summary = Summaries[Random.Shared.Next(Summaries.Length)]
        })
        .ToArray();
    }
}

public class WorkerInfo
{
    public int WorkerNumber { set; get; }
    public string SiteName { set; get; }
    public bool ShowMachineInfo { set; get; }
}
