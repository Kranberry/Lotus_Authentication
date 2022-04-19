using Lotus_Authentication.API.ApiDocumentation;
using System.Text.Json;

namespace Lotus_Authentication.Components;

public partial class Endpoint
{
    private Dictionary<string, string?> Body = new();
    private string BodyAsJson = string.Empty;

    private Dictionary<string, string?> SucceedBody = new();
    private string SucceedBodyAsJson = string.Empty;

    protected override void OnParametersSet()
    {
        foreach(Parameter param in Method.BodyParameters)
        {
            Body[param.Name] = param.Sample;
        }
        JsonSerializerOptions options = new() { WriteIndented = true };
        BodyAsJson = JsonSerializer.Serialize(Body, options);

        Result result = Method.Results.Where(res => res.StatusCode >= 200 && res.StatusCode <= 300).Single();
        for (int i = 0; i < result.Parameters?.Length; i++)
        {
            SucceedBody[result.Parameters[i].Name] = result.Parameters[i].Sample;
        }
        SucceedBodyAsJson = JsonSerializer.Serialize(SucceedBody, options);

        base.OnParametersSet();
    }

    private string GetStatusClass(int statusCode) => statusCode switch
    {
        (>= 200 and < 300) => "alert alert-success",
        (>= 300 and < 400) => "alert alert-warning",
        (>= 400 and < 500) => "alert alert-danger",
        _ => "alert alert-info"
    };

    private string GetMethodClass(string method) => method switch
    {
        "POST" => "badge bg-primary",
        "DELETE" => "badge bg-danger",
        "GET" => "badge bg-info text-dark",
        "PUT" => "badge bg-primary",
        _ => "badge bg-dark",
    };
}