using System.Text;
using System.Text.RegularExpressions;
using System.Security.Cryptography;

namespace Lotus_Authentication.Data;

public class SHA1Hash
{
    public static string Hash(string input)
    {
		//using SHA1Managed sha1 = new();
		//byte[] hash = sha1.ComputeHash(Encoding.UTF8.GetBytes(input));

		byte[] hash = SHA1.Create().ComputeHash(Encoding.UTF8.GetBytes(input));
		StringBuilder sb = new(hash.Length * 2);

		foreach (byte b in hash)
		{
			sb.Append(b.ToString("X2"));
		}

		return sb.ToString();
	}

	public static bool IsValidSHA1(string str)
	{
		Regex regex = new(@"^[a-fA-F0-9]{40}$");
		return regex.IsMatch(str);
	}
}

/// <summary>
/// Throw whenever a string must be a valid SHA1 checksum but is not
/// </summary>
public class BadSHA1ReferenceException : BaseException
{
	public BadSHA1ReferenceException(LogSeverity severity, string message, string page)
	{
		SendSyslog(severity, message, page, this);
	}
}