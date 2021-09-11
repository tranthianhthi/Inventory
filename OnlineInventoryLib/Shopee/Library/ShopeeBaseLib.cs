using OnlineInventoryLib.Util;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace OnlineInventoryLib.Shopee
{
    public class ShopeeBaseLib
    {
        public ShopeeBaseLib(string secretKey)
        {
            //this.shopeeDomain = shopeeDomain;
            this.secretKey = secretKey;
        }

        public string secretKey { get; set; }
        public string shopeeDomain { get; set; }

        public string CalculateToken(string url, string id, string key, string redirect)
        {
            //url = @"https://partner.shopeemobile.com/api/v1/shop/auth_partner?id={0}&token={1}&redirect={2}";
            url += @"/shop/auth_partner?id={0}&token={1}&redirect=https%3A%2F%2F{2}";
            string token = "";
            string baseString = key + @"https://" + redirect;

            using (SHA256 sha256Hash = SHA256.Create())
            {
                // ComputeHash - returns byte array  
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(baseString));

                // Convert byte array to a string   
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                token = builder.ToString();
            }

            return string.Format(url, id, token, redirect);
        }

        public async Task<string> PostData(string uri, string jsonData)
        {
            // tạo authenticate signature
            string authSignature = CreateAuthSignature(uri, jsonData);

            var reqContent = new StringContent(jsonData, Encoding.UTF8, "application/json");
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add("Authorization", authSignature);

            var response = await client.PostAsync(uri, reqContent);
            if (response.IsSuccessStatusCode)
            {
                string result = response.Content.ReadAsStringAsync().Result;
                return result;
            }
            return "ERROR";
        }

        private string CreateAuthSignature(string uri, string jsonData)
        {
            string baseString = uri + "|" + jsonData;
            HMACSHA256 hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secretKey));
            byte[] bytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(baseString));

            string authSignature = APIUtil.ConvertEncryptedBytes256ToString(bytes);
            return authSignature;
        }
    }
}
