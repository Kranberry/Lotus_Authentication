namespace Lotus_Authentication.Data;

public class Enviourment
{
    private readonly IWebHostEnvironment Environment;

    public Enviourment(IWebHostEnvironment env)
    {
        Environment = env;
    }

    public string GetContentPath => Environment.ContentRootPath;
}
