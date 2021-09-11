using Newtonsoft.Json;
using System;
using System.Text;

namespace OnlineInventoryLib.Util
{
    public class APIUtil
    {
        public static string ConvertEncryptedBytes256ToString(byte[] bytes)
        {
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < bytes.Length; i++)
            {
                builder.Append(bytes[i].ToString("x2"));
            }
            return builder.ToString();
        }

        public static string ConvertToJson(Object request)
        {
            string json = JsonConvert.SerializeObject(request, new JsonSerializerSettings() { DefaultValueHandling = DefaultValueHandling.Ignore });
            return json;
        }
    }
}
