using app1.runner.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace app1.runner
{
    public class Runner
    {
        IConfiguration _configuration;
        ILogger _logger;
        IServiceProvider _serviceProvider;
        public Runner(IServiceProvider serviceProvider, ILogger<Runner> logger, IConfiguration configuration)
        {
            _serviceProvider = serviceProvider;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task Run()
        {
            var runId = $"RUN{DateTimeOffset.Now.ToString("yyyyMMddTHHmmss")}-{Environment.MachineName}";
            var tasks = InitializeTasks();
            foreach (var task in tasks)
            {
                var result = await task.Run();
                _logger.LogInformation($"[{runId}-{task.Name}] result: {result.Success}, message: {result.Message}");
            }
        }

        public List<TaskBase> InitializeTasks()
        {
            var tasks = new List<TaskBase>();
            var taskTypes = Assembly.GetExecutingAssembly().GetTypes()
                .Where(t=>t.GetCustomAttribute<ScheduleAttribute>() != null && t.IsAssignableTo(typeof(TaskBase)))
                .ToList();

            foreach (var taskType in taskTypes)
            {
                var task = _serviceProvider.GetService(taskType) as TaskBase;
                tasks.Add(task);
            }

            return tasks;
        }
    }
}
