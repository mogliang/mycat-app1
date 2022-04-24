using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.IO;

namespace app1.fe.Pages;

public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;
    public string ApiAddress => "https://catfact.ninja/fact";
    public CatFact MyCatFact;

    public string CatImagePath;

    public IndexModel(ILogger<IndexModel> logger)
    {
        _logger = logger;
    }

    public async Task OnGetAsync()
    {
        HttpClient client = new HttpClient();
        MyCatFact = await client.GetFromJsonAsync<CatFact>(ApiAddress);

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
        
        if(CatImagePath == null)
        {
            CatImagePath = "img/black-cat-hi.png";
            _logger.LogWarning($"load default cat image from azure files");
        }
    }
}

public class CatFact
{
    public string fact { set; get; }
    public int length { set; get; }
}
