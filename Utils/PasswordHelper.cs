using System.Security.Cryptography;
using System.Text;

namespace ImportadoraApi.Utils
{
    public static class PasswordHelper
    {
        public static string Hash(string input)
        {
            using var sha = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(input);
            var hash = sha.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }

        public static bool Verify(string input, string hash)
        {
            return Hash(input) == hash;
        }
    }
}
