using Fare;
using System.Text.RegularExpressions;

namespace Lotus_Authentication.Models;

public class ApiKey
{
    private static Regex ApiRegex = new Regex(@"^[0-9]{4}[A-Fa-f]\-[A-Fa-f0-9]{8}\-[A-Fa-f0-9]{8}\-[A-Fa-f0-9]{8}\-[A-Fa-f0-9]{8}$");

    /// <summary>
    /// Generate a randomised API key
    /// </summary>
    /// <returns></returns>
    public static string GenerateApiKey()
    {
        Xeger xeger = new(ApiRegex.ToString());
        string apiKey = xeger.Generate();

        // TODO: Check if generated key already exists in the database. If it does, then recreate it until it's unique

        return apiKey;
    }

    public static bool IsValidApiKey(string apiKey)
    {
        return ApiRegex.IsMatch(apiKey);
    }
}