using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Linq;

namespace ASI.Basecode.Services.Manager
{
    /// <summary>
    /// Password Manager
    /// </summary>
    public class PasswordManager
    {
        /// <summary>
        /// Gets or sets the secret key.
        /// </summary>
        public static string secretKey { get; set; }

        /// <summary>
        /// Sets up.
        /// </summary>
        /// <param name="tokenAuthConfig">Token authentication configuration</param>
        public static void SetUp(IConfigurationSection tokenAuthConfig)
        {
            secretKey = tokenAuthConfig.GetValue<string>("SecretKey");
        }

        /// <summary>
        /// Encrypts the password.
        /// </summary>
        /// <param name="password">The password.</param>
        /// <returns></returns>
        public static string EncryptPassword(string password)
        {
            byte[] iv = new byte[16];
            byte[] aesKey = new byte[16];
            byte[] array;

            using (Aes aes = Aes.Create())
            {
                var keyBytes = Encoding.UTF8.GetBytes(secretKey);
                Array.Copy(keyBytes, aesKey, Math.Min(keyBytes.Length, aesKey.Length));
                aes.Key = aesKey;
                aes.IV = iv;

                ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

                using (MemoryStream memoryStream = new MemoryStream())
                {
                    using (CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter streamWriter = new StreamWriter((Stream)cryptoStream))
                        {
                            streamWriter.Write(password);
                        }

                        array = memoryStream.ToArray();
                    }
                }
            }

            return Convert.ToBase64String(array);
        }

        /// <summary>
        /// Decrypts the password.
        /// </summary>
        /// <param name="encryptedPassword">The encrypted password.</param>
        /// <returns></returns>
        public static string DecryptPassword(string encryptedPassword)
        {
            byte[] iv = new byte[16];
            byte[] aesKey = new byte[16];
            byte[] buffer = Convert.FromBase64String(encryptedPassword);

            using (Aes aes = Aes.Create())
            {
                var keyBytes = Encoding.UTF8.GetBytes(secretKey);
                Array.Copy(keyBytes, aesKey, Math.Min(keyBytes.Length, aesKey.Length));
                aes.Key = aesKey;
                aes.IV = iv;
                ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

                using (MemoryStream memoryStream = new MemoryStream(buffer))
                {
                    using (CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader streamReader = new StreamReader((Stream)cryptoStream))
                        {
                            return streamReader.ReadToEnd();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Validates password complexity and length requirements
        /// </summary>
        /// <param name="password">The password to validate</param>
        /// <returns>Validation result with success status and error messages</returns>
        public static PasswordValidationResult ValidatePassword(string password)
        {
            return ValidatePassword(password, null);
        }

        /// <summary>
        /// Validates password complexity, length requirements, and username similarity
        /// </summary>
        /// <param name="password">The password to validate</param>
        /// <param name="username">The username to compare against (optional)</param>
        /// <returns>Validation result with success status and error messages</returns>
        public static PasswordValidationResult ValidatePassword(string password, string username)
        {
            var result = new PasswordValidationResult();
            var errors = new List<string>();

            // Check length requirements (8-64 characters)
            if (string.IsNullOrEmpty(password) || password.Length < 8)
            {
                errors.Add("Password must be at least 8 characters long.");
            }
            else if (password.Length > 64)
            {
                errors.Add("Password must not exceed 64 characters.");
            }

            // Check for uppercase letters
            if (!password.Any(char.IsUpper))
            {
                errors.Add("Password must contain at least one uppercase letter.");
            }

            // Check for lowercase letters
            if (!password.Any(char.IsLower))
            {
                errors.Add("Password must contain at least one lowercase letter.");
            }

            // Check for numbers
            if (!password.Any(char.IsDigit))
            {
                errors.Add("Password must contain at least one number.");
            }

            // Check for special characters/symbols
            var specialChars = "!@#$%^&*()_+-=[]{}|;:,.<>?";
            if (!password.Any(c => specialChars.Contains(c)))
            {
                errors.Add("Password must contain at least one special character (!@#$%^&*()_+-=[]{}|;:,.<>?).");
            }

            // Check if password is the same as username (case-insensitive)
            if (!string.IsNullOrEmpty(username) &&
                string.Equals(password, username, StringComparison.OrdinalIgnoreCase))
            {
                errors.Add("Password cannot be the same as the username.");
            }

            // Check if password contains username (case-insensitive)
            if (!string.IsNullOrEmpty(username) && username.Length >= 3 &&
                password.ToLower().Contains(username.ToLower()))
            {
                errors.Add("Password cannot contain the username.");
            }

            // Check for common weak patterns
            if (ContainsCommonWeakPatterns(password))
            {
                errors.Add("Password contains common weak patterns. Please choose a more secure password.");
            }

            result.IsValid = errors.Count == 0;
            result.ErrorMessages = errors;
            return result;
        }

        /// <summary>
        /// Checks for common weak password patterns
        /// </summary>
        /// <param name="password">Password to check</param>
        /// <returns>True if contains weak patterns</returns>
        private static bool ContainsCommonWeakPatterns(string password)
        {
            var weakPatterns = new[]
            {
                @"(.)\1{3,}", // 4 or more consecutive same characters
                @"123456", // Sequential numbers
                @"abcdef", // Sequential letters
                @"qwerty", // Keyboard patterns
                @"password", // Common word
                @"admin", // Common word
                @"login" // Common word
            };

            return weakPatterns.Any(pattern =>
                Regex.IsMatch(password.ToLower(), pattern, RegexOptions.IgnoreCase));
        }

        /// <summary>
        /// Generates a strong password suggestion
        /// </summary>
        /// <param name="length">Desired password length (minimum 8, maximum 64)</param>
        /// <returns>Generated strong password</returns>
        public static string GenerateStrongPassword(int length = 12)
        {
            if (length < 8) length = 8;
            if (length > 64) length = 64;

            const string uppercase = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            const string lowercase = "abcdefghijklmnopqrstuvwxyz";
            const string numbers = "0123456789";
            const string symbols = "!@#$%^&*()_+-=[]{}|;:,.<>?";
            const string allChars = uppercase + lowercase + numbers + symbols;

            var random = new Random();
            var password = new StringBuilder();

            // Ensure at least one character from each category
            password.Append(uppercase[random.Next(uppercase.Length)]);
            password.Append(lowercase[random.Next(lowercase.Length)]);
            password.Append(numbers[random.Next(numbers.Length)]);
            password.Append(symbols[random.Next(symbols.Length)]);

            // Fill the rest randomly
            for (int i = 4; i < length; i++)
            {
                password.Append(allChars[random.Next(allChars.Length)]);
            }

            // Shuffle the password
            var passwordArray = password.ToString().ToCharArray();
            for (int i = 0; i < passwordArray.Length; i++)
            {
                int j = random.Next(i, passwordArray.Length);
                (passwordArray[i], passwordArray[j]) = (passwordArray[j], passwordArray[i]);
            }

            return new string(passwordArray);
        }
    }

    /// <summary>
    /// Password validation result
    /// </summary>
    public class PasswordValidationResult
    {
        public bool IsValid { get; set; }
        public List<string> ErrorMessages { get; set; } = new List<string>();
        public string ErrorMessage => string.Join(" ", ErrorMessages);
    }
}
