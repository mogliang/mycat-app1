using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace app1.fe.Pages;

public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;
    public string ApiAddress=>"https://catfact.ninja/fact";
    public CatFact MyCatFact;

    public IndexModel(ILogger<IndexModel> logger)
    {
        _logger = logger;
    }

    public async Task OnGetAsync()
    {
        HttpClient client =new HttpClient();
        MyCatFact = await client.GetFromJsonAsync<CatFact>(ApiAddress);
    }
}

public class CatFact
{
    public string fact { set; get; }
    public int length { set; get; }
}
