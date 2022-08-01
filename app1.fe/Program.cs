using Microsoft.Extensions.FileProviders;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("k8sconfig/app1.fe.appsettings.json", true, true);

// Add services to the container.
builder.Services.AddRazorPages();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}
app.UseStaticFiles();

try
{
    app.UseStaticFiles(new StaticFileOptions
    {
        RequestPath = "/azfiles",
        FileProvider = new PhysicalFileProvider("/mnt/azure/")
    });
}
catch (Exception ex)
{
    Console.WriteLine("Setup azfiles static web folder failed. error: " + ex);
}

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

using (var scope = app.Services.CreateScope())
{
    var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();
    var delayInSec = configuration.GetSection("AppInfo").GetValue<int>("DelayStartInSec");
    Console.WriteLine($"Will delay {delayInSec} sec for application warm up.");
    Thread.Sleep(TimeSpan.FromSeconds(delayInSec));
}

app.Run();
