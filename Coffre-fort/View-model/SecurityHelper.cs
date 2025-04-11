using System;
using System.Security.Cryptography;
using System.Text;

namespace Coffre_fort.View_model
{
    public static class SecurityHelper
    {
        // Clé secrète utilisée pour chiffrer/déchiffrer (32 caractères = 256 bits)
        private static readonly string SecretKey = "MaSuperCléUltraSecrète12345678!";

        private static byte[] GetKey()
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                return sha256.ComputeHash(Encoding.UTF8.GetBytes(SecretKey));
            }
        }

        public static string Encrypt(string plainText)
        {
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = GetKey();
                aesAlg.IV = new byte[16];

                using (var encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV))
                {
                    byte[] plainBytes = Encoding.UTF8.GetBytes(plainText);
                    byte[] encryptedBytes = encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);
                    return Convert.ToBase64String(encryptedBytes);
                }
            }
        }

        public static string Decrypt(string encryptedText)
        {
            if (string.IsNullOrWhiteSpace(encryptedText) || !IsBase64(encryptedText))
                return "🔒";

            try
            {
                using (Aes aesAlg = Aes.Create())
                {
                    aesAlg.Key = GetKey();
                    aesAlg.IV = new byte[16];

                    byte[] encryptedBytes = Convert.FromBase64String(encryptedText);

                    using (var decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV))
                    {
                        byte[] decryptedBytes = decryptor.TransformFinalBlock(encryptedBytes, 0, encryptedBytes.Length);
                        return Encoding.UTF8.GetString(decryptedBytes);
                    }
                }
            }
            catch
            {
                return "🔒";
            }
        }


        public static bool IsBase64(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return false;

            if (input.Length % 4 != 0)
                return false;

            try
            {
                Convert.FromBase64String(input);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
