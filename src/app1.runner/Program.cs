using app1.runner;
using app1.runner.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration(app =>
    {
        app.AddJsonFile("appsettings.json");
        app.AddJsonFile("k8sconfig/app1.runner.appsettings.json", true);
    })
    .ConfigureServices(services =>
    {
        services.AddSingleton<Runner>();
        services.AddScoped<AzFileTask>();
        services.AddScoped<AzDiskTask>();
        services.AddScoped<WebTask>();
        services.AddScoped<PodPingTask>();
    })
    .ConfigureLogging(b=>b.AddConsole())
    .Build();

using (var serviceScope = host.Services.CreateScope())
{
    var provider = serviceScope.ServiceProvider;
    var runner = provider.GetRequiredService<Runner>();

    while (true)
    {
        runner.Run().Wait();
        Thread.Sleep(TimeSpan.FromSeconds(60));
    }

    //host.Run();
}