using System.Security.Cryptography;
using System.Text;

namespace QuickTools.Shared.Helpers
{
    public static class AES256Helper
    {
        private const int KeySize = 32; // 256 bit
        private const int NonceSize = 12; // 96 bit
        private const int TagSize = 16; // 128 bit

        private static byte[] GetKey(string key)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentException("Key must be not emtpy.", nameof(key));

            // convert to AES 256-bit key
            return SHA256.HashData(Encoding.UTF8.GetBytes(key));
        }

        public static string Encrypt(string key, string text)
        {
            if (text == null)
                throw new ArgumentNullException(nameof(text));

            var aesKey = GetKey(key);
            var nonce = RandomNumberGenerator.GetBytes(NonceSize);
            var plaintext = Encoding.UTF8.GetBytes(text);
            var ciphertext = new byte[plaintext.Length];
            var tag = new byte[TagSize];

            using (var aes = new AesGcm(aesKey, TagSize))
            {
                aes.Encrypt(nonce, plaintext, ciphertext, tag);
            }

            // Format: nonce (12 bytes) + tag (16 bytes) + ciphertext
            var result = new byte[NonceSize + TagSize + ciphertext.Length];

            Buffer.BlockCopy(nonce, 0, result, 0, NonceSize);
            Buffer.BlockCopy(tag, 0, result, NonceSize, TagSize);
            Buffer.BlockCopy(
                ciphertext, 0, result, NonceSize + TagSize,
                ciphertext.Length
            );

            return Convert.ToBase64String(result);
        }

        public static string Decrypt(string key, string encryptedText)
        {
            if (encryptedText == null)
                throw new ArgumentNullException(nameof(encryptedText));

            var aesKey = GetKey(key);
            var data = Convert.FromBase64String(encryptedText);

            if (data.Length < NonceSize + TagSize)
                throw new CryptographicException("Dữ liệu mã hóa không hợp lệ.");

            var nonce = new byte[NonceSize];
            var tag = new byte[TagSize];
            var ciphertextLength = data.Length - NonceSize - TagSize;

            var ciphertext = new byte[ciphertextLength];
            var plaintext = new byte[ciphertextLength];

            Buffer.BlockCopy(data, 0, nonce, 0, NonceSize);
            Buffer.BlockCopy(data, NonceSize, tag, 0, TagSize);
            Buffer.BlockCopy(
                data, NonceSize + TagSize,
                ciphertext, 0, ciphertextLength
            );

            using (var aes = new AesGcm(aesKey, TagSize))
            {
                // Nếu key sai hoặc dữ liệu bị chỉnh sửa,
                // hàm sẽ ném CryptographicException.
                aes.Decrypt(nonce, ciphertext, tag, plaintext);
            }

            return Encoding.UTF8.GetString(plaintext);
        }
    }
}