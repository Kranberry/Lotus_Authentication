namespace Lotus_Authentication.Data;

public class EmailValidator
{
    public static bool IsValidEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return false;

        throw new NotImplementedException();
    }
}

