using Azure;
using Azure.Data.Tables;

namespace app1.common;

public class TaskResultAzTableProvider
{
    string _taskResultTableName;
    string _connStr;
    TableClient _tableClient;

    public TaskResultAzTableProvider(string connStr, string taskResultTableName)
    {
        _connStr = connStr;
        _taskResultTableName = taskResultTableName;
    }

    public void Initalize()
    {
        var tableServiceClient = new TableServiceClient(_connStr);
        tableServiceClient.CreateTableIfNotExists(_taskResultTableName);
        _tableClient = tableServiceClient.GetTableClient(_taskResultTableName);
    }

    public async Task AddTaskResult(TaskResultEntity taskResultEntity)
    {
        await _tableClient.AddEntityAsync(taskResultEntity);
    }

    public IList<TaskResultEntity> GetTaskResults(DateTimeOffset start, DateTimeOffset end)
    {
        var query = _tableClient.Query<TaskResultEntity>(filter: $"PartitionKey ge '{start.TimeString()}' and PartitionKey lt '{end.TimeString()}'", 1000);
        return query.Take(1000).ToList();
    }
}

public class TaskResultEntity : ITableEntity
{
    public string Host { get; set; }
    public string RunId { get; set; }
    public string TaskName { get; set; }
    public bool Success { get; set; }
    public string Message { get; set; }

    public string PartitionKey { get; set; }
    public string RowKey { get; set; }
    public DateTimeOffset? Timestamp { get; set; }
    public ETag ETag { get; set; }
}
