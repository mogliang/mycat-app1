using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace app1.runner.Tasks
{
    [Schedule]
    public class AzFileTask : IOTask
    {
        public AzFileTask(ILogger<AzFileTask> logger, IConfiguration configuration) : base(logger, configuration)
        {
            this.DirectoryPath = FileMountPath;
        }

        [Configuration]
        public string FileMountPath { get; set; }

    }
}
