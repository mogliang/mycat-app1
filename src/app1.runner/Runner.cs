using app1.common;
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
        TaskResultAzTableProvider _taskResultAzTableProvider;
        public Runner(IServiceProvider serviceProvider, ILogger<Runner> logger, IConfiguration configuration)
        {
            _serviceProvider = serviceProvider;
            _configuration = configuration;
            _logger = logger;

            var azTableConn = Environment.GetEnvironmentVariable("AZ_TABLE_CONN");
            if (azTableConn != null)
            {
                _logger.LogInformation("conn is " + azTableConn);
                _taskResultAzTableProvider = new TaskResultAzTableProvider(azTableConn);
                _taskResultAzTableProvider.Initalize();
            }
            else
            {
                _logger.LogWarning("Cannot get environment variable AZ_TABLE_CONN, will not write task result to az table.");
            }

        }

        public async Task Run()
        {
            var runId = $"RUN{DateTimeOffset.Now.TimeString()}-{Environment.MachineName}";
            var tasks = InitializeTasks();
            foreach (var task in tasks)
            {
                var result = await task.Run(runId);
                _logger.LogInformation($"[{runId}-{task.Name}] result: {result.Success}, message: {result.Message}");

                if (_taskResultAzTableProvider != null)
                {
                    try
                    {
                        await _taskResultAzTableProvider.AddTaskResult(new TaskResultEntity
                        {
                            PartitionKey = result.StartTime.TimeString(),
                            RowKey = result.Host,
                            Host = result.Host,
                            Message = result.Message,
                            RunId = result.RunId,
                            Success = result.Success,
                            TaskName = task.Name,
                            Timestamp = result.StartTime
                        });
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError($"Write az table failed. error: {ex}");
                    }
                }
            }
        }

        public List<TaskBase> InitializeTasks()
        {
            var tasks = new List<TaskBase>();
            var taskTypes = Assembly.GetExecutingAssembly().GetTypes()
                .Where(t => t.GetCustomAttribute<ScheduleAttribute>() != null && t.IsAssignableTo(typeof(TaskBase)))
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
