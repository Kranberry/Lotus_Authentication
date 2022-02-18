using System.Text.Json;

namespace Lotus_Authentication.Data;

public class Enviourment
{
    private readonly IWebHostEnvironment Environment;

    public Enviourment(IWebHostEnvironment env)
    {
        Environment = env;
    }

    public string GetContentPath()
    {
        if (Environment.IsDevelopment())
            return Environment.ContentRootPath + "/";

        string[] dirs = Directory.GetDirectories(Environment.ContentRootPath);
        DbHandler.AddNewSystemLog(LogSeverity.Error, null, JsonSerializer.Serialize(dirs), "").GetAwaiter();

        string[] files = Directory.GetFiles(Environment.ContentRootPath);
        DbHandler.AddNewSystemLog(LogSeverity.Error, null, JsonSerializer.Serialize(files), "").GetAwaiter();

        return Environment.ContentRootPath + "/";
    }
}
