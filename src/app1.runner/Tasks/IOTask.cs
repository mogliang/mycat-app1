using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace app1.runner.Tasks
{
    public class IOTask : TaskBase
    {
        public IOTask(ILogger logger, IConfiguration configuration) : base(logger, configuration)
        {
        }

        public string DirectoryPath { get; internal set; }

        public override async Task<TaskResult> RunImpl()
        {
            var workPath = Path.Combine(DirectoryPath, $"{Name}-{Environment.MachineName}-{DateTimeOffset.Now.ToString("yyyyMMddTHHmmss")}");
            var workDir = new DirectoryInfo(workPath);
            workDir.Create();

            var stopWatch = Stopwatch.StartNew();
            try
            {
                for (int i = 0; i < 1000; i++)
                {
                    var filePath = Path.Combine(workPath, Guid.NewGuid().ToString());
                    await File.WriteAllTextAsync(filePath, filePath);
                    var readContent = await File.ReadAllTextAsync(filePath);
                    if (readContent != filePath)
                    {
                        var message = $"write/read conteant inconsistent!, expected:{filePath}, actual:{readContent}";
                        return new TaskResult(false, message);
                    }
                }

                workDir.Delete(true);
            }
            catch (Exception ex)
            {
                var message = $"io test failed with exception: {ex}";
                return new TaskResult(false, message);
            }

            stopWatch.Stop();
            return new TaskResult(true, $"1000 read/write took {stopWatch.ElapsedMilliseconds}ms");
        }
    }
}
