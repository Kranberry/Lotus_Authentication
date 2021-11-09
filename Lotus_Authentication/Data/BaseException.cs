using System.Threading.Tasks;

namespace Lotus_Authentication.Data;

public abstract class BaseException : Exception
{
    public void SendSyslog(LogSeverity severity, string message, string page, Exception exception)
    {
        _ = DbHandler.AddNewSystemLog(severity, exception, message, page).GetAwaiter();
    }
}

public class UserNotFoundException : BaseException
{
    // public static async ValueTask<int> AddNewSystemLog(LogSeverity severity, Exception? exception, string message, string page)
    public UserNotFoundException(LogSeverity severity, string message, string page)
    {
        SendSyslog(severity, message, page, this);
    }
}

public class CountryNotFoundException : BaseException
{
    public CountryNotFoundException(LogSeverity severity, string message, string page)
    {
        SendSyslog(severity, message, page, this);
    }
}