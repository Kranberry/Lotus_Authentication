using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace Lotus_Authentication.Data
{
	public class SHA256Hash
	{
        public static (string, byte[]) HashAndSaltString(string psw)
        {
            byte[] salt = new byte[128 / 8];
            using (RandomNumberGenerator rngCsp = RandomNumberGenerator.Create())
            {
                rngCsp.GetNonZeroBytes(salt);
            }

            string hashedPassword = GenerateHash(psw, salt);
            return (hashedPassword, salt);
        }

        public static string HashString(string psw, byte[] salt) => GenerateHash(psw, salt);

        private static string GenerateHash(string str, byte[] salt)
            => Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: str,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 100000,
                numBytesRequested: 256 / 8));
    }
}
