using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.IO;
using System.Reflection;

namespace app1.fe.Pages;

public class IndexModel : PageModel
{
    private readonly IConfiguration Configuration;
    private readonly ILogger<IndexModel> _logger;
    public string ApiAddress => "https://catfact.ninja/fact";
    public CatFact MyCatFact;

    public AppInfo AppInfo;

    public string CatImagePath;

    public string MachineInfo;

    public string AppVersion;

    public IndexModel(ILogger<IndexModel> logger, IConfiguration config)
    {
        _logger = logger;
        Configuration = config;
    }

    public async Task OnGetAsync()
    {
        HttpClient client = new HttpClient();
        MyCatFact = await client.GetFromJsonAsync<CatFact>(ApiAddress);

        // load image
        if (Directory.Exists(Consts.ExternalFilePath))
        {
            var files = new DirectoryInfo(Consts.ExternalFilePath).GetFiles().ToList();
            if (files.Count > 0)
            {
                var selectedFile = files[new Random().Next(files.Count)];
                CatImagePath = Consts.ExternalFileWebPath + "/" + selectedFile.Name;
                _logger.LogInformation($"load cat image {selectedFile.Name} from azure files");
            }
        }

        if (CatImagePath == null)
        {
            CatImagePath = "img/black-cat-hi.png";
            _logger.LogWarning($"load default cat image from azure files");
        }

        // load title
        this.AppInfo = Configuration.GetSection("AppInfo").Get<AppInfo>();
        this.MachineInfo = this.AppInfo?.ShowMachineInfo == true ? "Host:" + Environment.MachineName : string.Empty;

        // load version info
        this.AppVersion = "Version:" + Assembly.GetExecutingAssembly().GetName().Version.ToString();
    }
}

public class AppInfo
{
    public string Title { set; get; }
    public string SiteName { set; get; }
    public bool ShowMachineInfo { set; get; }
}

public class CatFact
{
    public string fact { set; get; }
    public int length { set; get; }
}
