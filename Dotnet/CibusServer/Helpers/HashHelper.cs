using System.Text;
using XSystem.Security.Cryptography;

namespace CibusServer.Helpers
{
    public static class HashHelper
    {
        public static string CalculateHash(string clearTextPassword, string salt)
        {
            // Convert the salted password to a byte array
            var saltedHashBytes = Encoding.UTF8.GetBytes(clearTextPassword + salt);

            // Use the hash algorithm to calculate the hash
            var algorithm = new SHA256Managed();
            var hash = algorithm.ComputeHash(saltedHashBytes);

            // Return the hash as a base64 encoded string to be compared to the stored password
            return Convert.ToBase64String(hash);
        }
    }
}
