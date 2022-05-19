namespace app1.common;

public static class TimeExtension
{
    public static string TimeString(this DateTimeOffset time)
    {
        return time.ToString("yyyyMMddTHHmmss");
    }
}
