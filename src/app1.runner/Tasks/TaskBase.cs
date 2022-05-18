using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace app1.runner.Tasks
{
    public abstract class TaskBase
    {
        [Configuration]
        public bool Enabled { get; set; }

        public string Name => this.GetType().Name;
        public bool IsRunning { get; set; }
        protected ILogger Logger { get; }
        protected IConfiguration Configuration { get; }
        public TaskBase(ILogger logger, IConfiguration configuration)
        {
            Logger = logger;
            Configuration = configuration;
            LoadConfiguration(configuration);
        }

        private void LoadConfiguration(IConfiguration configuration)
        {
            var section = configuration.GetSection(Name);

            var confProperties = this.GetType().GetProperties().Where(p => p.GetCustomAttribute<ConfigurationAttribute>() != null);
            foreach (var prop in confProperties)
            {
                var allowedType = new[] { typeof(bool), typeof(int), typeof(double), typeof(string) };
                if (allowedType.Contains(prop.PropertyType) || prop.PropertyType.IsEnum)
                {
                    var value = section.GetValue(prop.PropertyType, prop.Name);
                    prop.SetValue(this, value);
                }
                else
                {
                    throw new ArgumentException($"Not supported property type {prop.PropertyType}, property name: {prop.Name}");
                }
            }
        }

        public async Task<TaskResult> Run()
        {
            if (Enabled)
            {
                IsRunning = true;
                Logger.LogInformation($"Begin to run task {Name}");
                var result = await RunImpl();
                IsRunning = false;
                Logger.LogInformation($"Run task {Name} Ends.");
                return result;
            }
            else
            {
                Logger.LogInformation($"Skip task {Name}");
                return new TaskResult(true);
            }
        }

        public abstract Task<TaskResult> RunImpl();
    }

    public class TaskResult
    {
        public TaskResult(bool success, string message = null)
        {
            Success = success;
            Message = message;
        }

        public bool Success { get; set; }
        public string Message { get; set; }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class ConfigurationAttribute : Attribute
    {

    }

    [AttributeUsage(AttributeTargets.Class)]
    public class ScheduleAttribute : Attribute
    {

    }
}
