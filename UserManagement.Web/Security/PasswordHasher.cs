using System;
using System.Security.Cryptography;
using UserManagement.Web.Constants;

namespace UserManagement.Web.Security
{
    /// <summary>
    /// Tạo và kiểm tra hash mật khẩu bằng PBKDF2-SHA256.
    /// </summary>
    public static class PasswordHasher
    {
        public const int Iterations = 600000;
        private const int SaltSizeInBytes = 16;
        private const int HashSizeInBytes = 32;
        private const int MinimumAcceptedIterations = 100000;

        /// <summary>
        /// Tạo salt ngẫu nhiên và băm mật khẩu bằng PBKDF2-SHA256.
        /// </summary>
        public static PasswordHashResult HashPassword(string password)
        {
            if (string.IsNullOrEmpty(password))
            {
                throw new ArgumentException(ErrorMessages.PasswordRequired);
            }

            byte[] salt = new byte[SaltSizeInBytes];
            byte[] hash;

            // Mỗi lần băm dùng salt ngẫu nhiên riêng.
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

        /// <summary>
        /// Băm lại mật khẩu với tham số đã lưu rồi so sánh toàn bộ hash.
        /// </summary>
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

            // So sánh đủ các byte, không dừng sớm khi gặp byte khác nhau.
            for (int i = 0; i < storedHash.Length; i++)
            {
                difference |= storedHash[i] ^ newHash[i];
            }

            return difference == 0;
        }
    }
}
