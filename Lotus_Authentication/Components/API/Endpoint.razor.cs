namespace Lotus_Authentication.Components;

public partial class Endpoint
{
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