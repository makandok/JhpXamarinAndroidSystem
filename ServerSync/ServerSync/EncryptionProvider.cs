using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ServerSync
{
    public class PlainText
    {
        public PlainText(string plainText) { Value = plainText; }
        public string Value { get; set; }
    }

    public class EncryptedText
    {
        public EncryptedText(string encryptedText) { Value = encryptedText; }
        public string Value { get; set; }
    }

    public static class EncryptionHelper
    {
        public static EncryptedText Encrypt(this PlainText plainText, string encryptionKey)
        {
            var encryptedText = Crypto.Encrypt(plainText.Value, encryptionKey);
            return new EncryptedText(encryptedText);
        }

        public static PlainText Decrypt(this EncryptedText encryptedText, string encryptionKey)
        {
            var decryptedText = Crypto.Decrypt(encryptedText.Value, encryptionKey);
            return new PlainText(decryptedText);
        }
    }
}
