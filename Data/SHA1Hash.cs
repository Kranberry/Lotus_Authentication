using System.Text;
using System.Text.RegularExpressions;
using XSystem.Security.Cryptography;

namespace Lotus_Authentication.Data
{
	public class SHA1Hash
	{
        public static string Hash(string input)
        {
			using SHA1Managed sha1 = new();

			byte[] hash = sha1.ComputeHash(Encoding.UTF8.GetBytes(input));
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
			return regex.Matches(str).Any();
		}
	}
}
