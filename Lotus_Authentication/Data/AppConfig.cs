namespace Lotus_Authentication.Data;

internal static class AppConfig
{
    private static IConfiguration _Configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json", true).Build();

    private static string GetValueOfKey(string key) => _Configuration[key];

    internal static string SkeletonKey => GetValueOfKey("secrets:skeleton-key");
    internal static string SkeletonKeyID => GetValueOfKey("secrets:skeleton-key-id");
    internal static string ActiveDatabaseCS => GetValueOfKey("authenticator");
}
