﻿using Microsoft.Extensions.Configuration;
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
    public class InternetCommTask : IOTask
    {
        public InternetCommTask(ILogger<PodCommTask> logger, IConfiguration configuration) : base(logger, configuration)
        {
        }

        public override async Task<TaskResult> RunImpl(string RunId)
        {
            bool success = false;
            string message = string.Empty;
            var urlList = ServiceUrls.Split(";");

            foreach (var serviceUrl in urlList)
            {
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
        public string ServiceUrls { get; set; }
    }
}