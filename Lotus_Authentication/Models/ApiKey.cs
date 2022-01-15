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
    private static int SkeletonKeyID = int.Parse(AppConfig.SkeletonKeyID);

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
        bool isValid = _ApiRegex.IsMatch(apiKey);
        if (!isValid)
        {
            try
            {
                ApiKey key = DbHandler.GetApiKeyByApiKey(apiKey);
                if (key.Alias == "skeleton-key" && key.Key.Length == 40 && key.ApiKeyID == SkeletonKeyID)
                {
                    return true;
                }
            }
            catch (BadApiKeyReferenceException)
            {

            }
            return false;
        }

        return isValid;
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