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
    public class AzDiskTask : IOTask
    {
        public AzDiskTask(ILogger<AzDiskTask> logger, IConfiguration configuration) : base(logger, configuration)
        {
            this.DirectoryPath = DiskMountPath;
        }

        [Configuration]
        public string DiskMountPath { get; set; }

    }
}
