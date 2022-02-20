using Microsoft.AspNetCore.Components;
using System.Text.Json;

namespace Lotus_Authentication.Data;

public class Enviourment
{
    private readonly IWebHostEnvironment Environment;
    private readonly NavigationManager NavigationManager;

    public string WebUrl => NavigationManager.BaseUri;

    public Enviourment(IWebHostEnvironment env, NavigationManager nav)
    {
        Environment = env;
        NavigationManager = nav;
    }

    public string GetWWWRootPath()
    {
        if (Environment.IsDevelopment())
            return Environment.WebRootPath + "/";

        return Environment.ContentRootPath + "/wwwroot/";
    }
}
