using System;
using System.Security.Cryptography;
using UserManagement.Web.Constants;

namespace UserManagement.Web.Security
{
    public static class PasswordHasher
    {
        public const int Iterations = 600000;
        private const int SaltSizeInBytes = 16;
        private const int HashSizeInBytes = 32;
        private const int MinimumAcceptedIterations = 100000;

        public static PasswordHashResult HashPassword(string password)
        {
            if (string.IsNullOrEmpty(password))
            {
                throw new ArgumentException(ErrorMessages.PasswordRequired);
            }

            byte[] salt = new byte[SaltSizeInBytes];
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
                hash = pbkdf2.GetBytes(HashSizeInBytes);
            }

            PasswordHashResult result = new PasswordHashResult
            {
                Hash = hash,
                Salt = salt
            };

            return result;
        }

        public static bool VerifyPassword(
            string password,
            byte[] storedHash,
            byte[] salt,
            int iterations)
        {
            if (string.IsNullOrEmpty(password) || storedHash == null || storedHash.Length != HashSizeInBytes || salt == null || salt.Length != SaltSizeInBytes || iterations < MinimumAcceptedIterations)
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
                newHash = pbkdf2.GetBytes(HashSizeInBytes);
            }

            int difference = 0;

            for (int i = 0; i < storedHash.Length; i++)
            {
                difference |= storedHash[i] ^ newHash[i];
            }

            return difference == 0;
        }
    }
}
