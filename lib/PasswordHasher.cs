using System;
using System.Security.Cryptography;
using System.Text;

namespace FlightManagement.lib
{
    public static class PasswordHasher
    {
        private const int SaltSize = 12; // 128-bit salt
        private const int KeySize = 24; // 256-bit key
        private const int Iterations = 10000; // Number of iterations for PBKDF2

        public static string HashPassword(string password)
        {
            // Generate a random salt
            byte[] salt = RandomNumberGenerator.GetBytes(SaltSize);

            // Hash the password with the salt
            using (var algorithm = new Rfc2898DeriveBytes(password, salt, Iterations, HashAlgorithmName.SHA256))
            {
                var key = algorithm.GetBytes(KeySize); // Generate the hashed password

                // Combine the salt and key into a single string for storage
                var hashBytes = new byte[SaltSize + KeySize];
                Buffer.BlockCopy(salt, 0, hashBytes, 0, SaltSize);
                Buffer.BlockCopy(key, 0, hashBytes, SaltSize, KeySize);

                // Convert to base64 for storage
                return Convert.ToBase64String(hashBytes);
            }
        }

        public static bool VerifyPassword(string password, string hashedPassword)
        {
            // Decode the hashed password from base64
            byte[] hashBytes = Convert.FromBase64String(hashedPassword);

            // Extract the salt from the hash
            byte[] salt = new byte[SaltSize];
            Buffer.BlockCopy(hashBytes, 0, salt, 0, SaltSize);

            // Extract the stored hash from the hashBytes
            byte[] storedKey = new byte[KeySize];
            Buffer.BlockCopy(hashBytes, SaltSize, storedKey, 0, KeySize);

            // Hash the input password using the same salt
            using (var algorithm = new Rfc2898DeriveBytes(password, salt, Iterations, HashAlgorithmName.SHA256))
            {
                var computedKey = algorithm.GetBytes(KeySize);

                // Compare the computed hash with the stored hash
                return CryptographicOperations.FixedTimeEquals(computedKey, storedKey);
            }
        }
    }

}
