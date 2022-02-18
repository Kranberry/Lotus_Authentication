using System.Text.Json;

namespace Lotus_Authentication.Data;

public class Enviourment
{
    private readonly IWebHostEnvironment Environment;

    public Enviourment(IWebHostEnvironment env)
    {
        Environment = env;
    }

    public string GetWWWRootPath()
    {
        if (Environment.IsDevelopment())
            return Environment.WebRootPath + "/";

        return Environment.ContentRootPath + "/wwwroot/";
    }
}
