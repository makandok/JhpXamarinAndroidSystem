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
}