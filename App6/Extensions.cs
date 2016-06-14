namespace JhpDataSystem
{
    internal static class Extensions
    {
        //internal static void Click()
        //{

        //}

        internal static string toText(this System.IO.Stream stream)
        {
            //var buffer = new byte[length];
            //prepexFieldsStream.Read(buffer, 0, Convert.ToInt32(length));
            //var asString = Convert.ToString(buffer);

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