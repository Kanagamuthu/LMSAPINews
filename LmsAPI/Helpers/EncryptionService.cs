using System.Security.Cryptography;
using System.Text;

namespace LMSAPI.Helpers
{
    public class EncryptionService
    {
        private readonly RSA _rsa;
        private byte[]? _aesKey;
        private byte[]? _iv;

        public EncryptionService()
        {
            _rsa = RSA.Create(2048);
        }

        public string GetPublicKey()
        {
            // Export as Base64 to simplify JSON response
            return Convert.ToBase64String(_rsa.ExportRSAPublicKey());
        }

        public void SetAesKey(string encryptedKeyB64, string ivB64)
        {
            var encryptedKey = Convert.FromBase64String(encryptedKeyB64);
            var decryptedKey = _rsa.Decrypt(encryptedKey, RSAEncryptionPadding.OaepSHA256);
            _aesKey = decryptedKey;
            _iv = Convert.FromBase64String(ivB64);
        }

        public string EncryptString(string plaintext)
        {
            if (_aesKey == null || _iv == null)
                throw new InvalidOperationException("AES key not initialized.");

            using var aes = Aes.Create();
            aes.Key = _aesKey;
            aes.IV = _iv;
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;

            using var encryptor = aes.CreateEncryptor();
            var inputBytes = Encoding.UTF8.GetBytes(plaintext);
            var encrypted = encryptor.TransformFinalBlock(inputBytes, 0, inputBytes.Length);
            return Convert.ToBase64String(encrypted);
        }

        public string DecryptString(string base64Cipher)
        {
            if (_aesKey == null || _iv == null)
                throw new InvalidOperationException("AES key not initialized.");

            var cipherBytes = Convert.FromBase64String(base64Cipher);
            using var aes = Aes.Create();
            aes.Key = _aesKey;
            aes.IV = _iv;
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;

            using var decryptor = aes.CreateDecryptor();
            var plainBytes = decryptor.TransformFinalBlock(cipherBytes, 0, cipherBytes.Length);
            return Encoding.UTF8.GetString(plainBytes);
        }

        public bool IsSessionActive => _aesKey != null && _iv != null;
    }
}
