namespace Lotus_Authentication.Models;

public class UserBanStatus
{
    public User User { get; init; }
    public string ApiKey { get; init; } = "";
    public BanStatus BanStatus { get; set; }
    public DateTime BanLiftDate { get; init; }
    public string Reason { get; init; } = "";

    public UserBanStatus(User user, string apiKey, BanStatus banStatus, DateTime banLiftDate, string reason)
    {
        User = user;
        ApiKey = apiKey;
        BanStatus = banStatus;
        BanLiftDate = banLiftDate;
        Reason = reason;
    }
}

public enum BanStatus
{
    UnBanned,
    Banned
}