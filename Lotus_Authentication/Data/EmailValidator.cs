using System.Net.Mail;
using System.Text.RegularExpressions;

namespace Lotus_Authentication.Data;

public class EmailValidator
{
    public static bool IsValidEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return false;

        Regex emailRegex = new Regex(@"^[\w-\.+]+@([\w-]+\.)+[\w-]+[A-Za-z]+$");
        bool regexMatch = emailRegex.IsMatch(email);

        Regex dotMatrix = new Regex(@"(\.\.)+");
        bool tooManyDots = dotMatrix.Matches(email).Count > 0;

        Regex nothingFollowsDot = new Regex(@"[\.+\-^<>+]@|@+[\.\-_^<>\+]");
        bool atfollowsDot = nothingFollowsDot.Matches(email).Count > 0;

        try
        {
            MailAddress mailAddress = new(email);
            return true && regexMatch && !tooManyDots && !atfollowsDot;
        }
        catch (Exception)
        {
            return false;
        }
    }
}

