using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace app1.runner.Tasks
{
    [Schedule]
    public class PodCommTask : IOTask
    {
        public PodCommTask(ILogger<PodCommTask> logger, IConfiguration configuration) : base(logger, configuration)
        {
        }

        public override async Task<TaskResult> RunImpl()
        {
            string message = string.Empty;
            bool success = true;
            for (int i = 0; i < PodCount; i++)
            {
                var serviceUrl = new Uri(string.Format(PodServiceUrlFormat, i));
                try
                {
                    Stopwatch stopwatch = Stopwatch.StartNew();
                    WebClient client = new WebClient();
                    var content = client.DownloadData(serviceUrl);
                    stopwatch.Stop();
                    message += $"Api call to {serviceUrl} succeed. Took {stopwatch.ElapsedMilliseconds}ms, downloaded {content.Length} bytes\n";
                }
                catch (Exception ex)
                {
                    success = false;
                    message += $"Api call to {serviceUrl} failed. error: {ex}\n";
                }
            }

            return new TaskResult(success, message);
        }

        [Configuration]
        public string PodServiceUrlFormat { get; set; }

        [Configuration]
        public int PodCount { get; set; }
    }
}
