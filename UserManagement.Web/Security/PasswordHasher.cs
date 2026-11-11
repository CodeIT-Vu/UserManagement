using System;
using System.Security.Cryptography;

namespace UserManagement.Web.Security
{
    public static class PasswordHasher
    {
        public const int Iterations = 600000;

        public static PasswordHashResult HashPassword(string password)
        {
            if (string.IsNullOrEmpty(password))
            {
                throw new ArgumentException("Password cannot be empty.");
            }

            byte[] salt = new byte[16];
            byte[] hash;

            using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }

            using (Rfc2898DeriveBytes pbkdf2 = new Rfc2898DeriveBytes(
                password,
                salt,
                Iterations,
                HashAlgorithmName.SHA256))
            {
                hash = pbkdf2.GetBytes(32);
            }

            PasswordHashResult result = new PasswordHashResult();
            result.Hash = hash;
            result.Salt = salt;
            return result;
        }

        public static bool VerifyPassword(
            string password,
            byte[] storedHash,
            byte[] salt,
            int iterations)
        {
            if (string.IsNullOrEmpty(password))
            {
                return false;
            }

            if (storedHash == null)
            {
                return false;
            }

            if (storedHash.Length != 32)
            {
                return false;
            }

            if (salt == null)
            {
                return false;
            }

            if (salt.Length != 16)
            {
                return false;
            }

            if (iterations < 100000)
            {
                return false;
            }

            byte[] newHash;

            using (Rfc2898DeriveBytes pbkdf2 = new Rfc2898DeriveBytes(
                password,
                salt,
                iterations,
                HashAlgorithmName.SHA256))
            {
                newHash = pbkdf2.GetBytes(32);
            }

            int difference = 0;

            for (int i = 0; i < storedHash.Length; i++)
            {
                // XOR gives 0 when the two bytes are equal.
                int byteDifference = storedHash[i] ^ newHash[i];

                // Keep any difference found; compare every byte before returning.
                difference = difference | byteDifference;
            }

            return difference == 0;
        }
    }
}
