using System;
using System.Collections.Generic;
using System.Json;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System.Security.Cryptography;
using System.Linq;

namespace JhpDataSystem
{
    class JhpSecurity
    {
        internal static string Decrypt(string jsonValue)
        {
            return jsonValue;
        }

        internal static string Encrypt(string textToEncrypt)
        {
            return ComputeSha512Hash(textToEncrypt);
        }

        public static string ComputeSha512Hash(string textToEncrypt)
        {
            var sha512 = new SHA512Managed();
            var hashBytes = sha512.ComputeHash(Encoding.UTF8.GetBytes(textToEncrypt));
            var builder = new StringBuilder();

            for (int i = 0; i < hashBytes.Length; i++)
            {
                builder.Append(hashBytes[i].ToString("x2"));
            }            
            return builder.ToString().Substring(6, 30);
        }
    }

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
        public static PlainText Decrypt(this string textToEncrypt)
        {
            var encryptionKey = AppInstance.Instance.ApiAssets[Constants.ASSET_ADMIN_ENCRYPTIONKEY];
            var encryptedText = new EncryptedText(textToEncrypt);
            return encryptedText.Decrypt(encryptionKey);
        }

        public static EncryptedText Encrypt(this string textToEncrypt)
        {
            var encryptionKey = AppInstance.Instance.ApiAssets[Constants.ASSET_ADMIN_ENCRYPTIONKEY];
            var plainText = new PlainText(textToEncrypt);
            return plainText.Encrypt(encryptionKey);
        }

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