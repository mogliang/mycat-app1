﻿using app1.common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Buffers.Text;
using System.IO;
using System.Reflection;
using System.Text;

namespace app1.fe.Pages;

public class IndexModel : PageModel
{
    private readonly IConfiguration Configuration;
    private readonly ILogger<IndexModel> _logger;

    public AppInfo AppInfo;
    public string MachineInfo;
    public string AppVersion;
    public List<TaskResultEntity> TaskResults;

    public IndexModel(ILogger<IndexModel> logger, IConfiguration config)
    {
        _logger = logger;
        Configuration = config;
    }

    public async Task OnGetAsync()
    {
        try
        {
            var azTableConn = Environment.GetEnvironmentVariable("AZ_TABLE_CONN");
            var taskResultTableProvider = new TaskResultAzTableProvider(azTableConn);
            taskResultTableProvider.Initalize();
            var now = DateTimeOffset.Now;
            TaskResults = taskResultTableProvider.GetTaskResults(now.AddDays(-1), now).ToList();
        }
        catch (Exception ex)
        {
            var message = $"Unable to load task result from az table, error: {ex}";
            _logger.LogError(message);
            throw new ArgumentException(message, ex);
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