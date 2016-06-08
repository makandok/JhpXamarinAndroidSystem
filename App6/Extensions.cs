using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace JhpDataSystem
{
    internal static class Extensions
    {
        internal static string toText(this System.IO.Stream stream)
        {
            var mstream = new System.IO.MemoryStream();
            stream.CopyTo(mstream);
            var bytes = mstream.ToArray();
            return System.Text.Encoding.Default.GetString(bytes);
        }

        internal static byte[] toByteArray(this System.IO.Stream stream)
        {
            var mstream = new System.IO.MemoryStream();
            stream.CopyTo(mstream);
            var bytes = mstream.ToArray();
            return bytes;
        }

        internal static string decryptAndGetApiSetting(this System.Json.JsonValue jsonObject, string settingString)
        {
            var value = jsonObject[settingString];
            if (Constants.ENCRYPTED_ASSETS.Contains(settingString)){
                //we decrypt
                return JhpSecurity.Decrypt(value);
            }
            return value;
        }
    }
}