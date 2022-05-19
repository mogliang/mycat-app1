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
            _logger = logger;
            _configuration = configuration;

            var section = _configuration.GetSection("Runner");

            var azTableConn = Environment.GetEnvironmentVariable("AZ_TABLE_CONN");
            if (azTableConn != null)
            {
                var tableName = section.GetValue<string>("AzTableName");
                _logger.LogInformation("conn is " + azTableConn);
                _taskResultAzTableProvider = new TaskResultAzTableProvider(azTableConn, tableName);
                _taskResultAzTableProvider.Initalize();
            }
            else
            {
                _logger.LogWarning("Cannot get environment variable AZ_TABLE_CONN, will not write task result to az table.");
            }

        }

        public async Task Run()
        {
            var tasks = InitializeTasks();
            foreach (var task in tasks)
            {
                var runId = $"{DateTimeOffset.Now.TimeString()}-{task.Name}";
                var result = await task.Run(runId);
                _logger.LogInformation($"[{runId}-{task.Name}] result: {result.Success}, message:\n{result.Message}");

                if (_taskResultAzTableProvider != null)
                {
                    try
                    {
                        var partitionKey = $"{runId}-{Environment.MachineName}";
                        await _taskResultAzTableProvider.AddTaskResult(new TaskResultEntity
                        {
                            PartitionKey = partitionKey,
                            RowKey = partitionKey,
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
