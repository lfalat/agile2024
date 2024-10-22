using System.Security.Cryptography;
using System.Text;

namespace AGILE2024_BE.Helpers
{
    public static class PasswordHasher
    {
        // Method to hash a password with a generated salt
        public static (string HashedPassword, byte[] Salt) HashPassword(string plainPassword)
        {
            // Generate a random salt
            byte[] salt = new byte[16];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }

            // Hash the password with the salt
            using (var sha256 = SHA256.Create())
            {
                byte[] passwordBytes = Encoding.UTF8.GetBytes(plainPassword);
                byte[] saltedPassword = new byte[salt.Length + passwordBytes.Length];

                // Combine salt and password
                Buffer.BlockCopy(salt, 0, saltedPassword, 0, salt.Length);
                Buffer.BlockCopy(passwordBytes, 0, saltedPassword, salt.Length, passwordBytes.Length);

                byte[] hash = sha256.ComputeHash(saltedPassword);

                // Combine salt and hash for storage
                byte[] hashBytes = new byte[salt.Length + hash.Length];
                Buffer.BlockCopy(salt, 0, hashBytes, 0, salt.Length);
                Buffer.BlockCopy(hash, 0, hashBytes, salt.Length, hash.Length);

                // Convert to a base64 string for storage
                return (Convert.ToBase64String(hashBytes), salt);
            }
        }

        // Method to verify a password against the stored hash
        public static bool VerifyPassword(string hashedPassword, byte[] storedSalt, string plainPassword)
        {
            // Convert the base64 string back to byte array
            byte[] hashBytes = Convert.FromBase64String(hashedPassword);

            // Hash the input password with the extracted salt
            using (var sha256 = SHA256.Create())
            {
                byte[] passwordBytes = Encoding.UTF8.GetBytes(plainPassword);
                byte[] saltedPassword = new byte[storedSalt.Length + passwordBytes.Length];

                // Combine stored salt and password
                Buffer.BlockCopy(storedSalt, 0, saltedPassword, 0, storedSalt.Length);
                Buffer.BlockCopy(passwordBytes, 0, saltedPassword, storedSalt.Length, passwordBytes.Length);

                byte[] hash = sha256.ComputeHash(saltedPassword);

                // Compare the computed hash with the stored hash
                for (int i = 0; i < hash.Length; i++)
                {
                    if (hashBytes[i + storedSalt.Length] != hash[i])
                    {
                        return false; // Passwords do not match
                    }
                }
            }

            return true; // Passwords match
        }
    }
}
