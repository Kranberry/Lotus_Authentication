using Fare;
using System.Text.RegularExpressions;

namespace Lotus_Authentication.Data;

public class ApiKey
{
    public int ApiKeyID { get; init; }
    public string Key { get; set; }
    public string? Alias { get; set; }
    public DateTime InsertDate { get; init; }
    public DateTime? UpdateDate { get; init; }

    private static Regex _ApiRegex = new Regex(@"^[0-9]{4}[A-Fa-f]\-[A-Fa-f0-9]{8}\-[A-Fa-f0-9]{8}\-[A-Fa-f0-9]{8}\-[A-Fa-f0-9]{8}$");

    /// <summary>
    /// Generate a randomised API key
    /// </summary>
    /// <returns>A string containing your new api key</returns>
    public static string GenerateApiKey()
    {
        Xeger xeger = new(_ApiRegex.ToString());
        string apiKey = xeger.Generate();

        while (true)
        {
            try
            {
                DbHandler.GetApiKeyByApiKey(apiKey);
            }
            catch ( BadApiKeyReferenceException )
            {
                return apiKey;
            }
        }
    }

    public static bool IsValidApiKey(string apiKey)
    {
        return _ApiRegex.IsMatch(apiKey);
    }
}

/// <summary>
/// Thrown when api key is not found in database
/// </summary>
public class BadApiKeyReferenceException : BaseException
{
    public BadApiKeyReferenceException(LogSeverity severity, string message, string page)
    {
        SendSyslog(severity, message, page, this);
    }
}