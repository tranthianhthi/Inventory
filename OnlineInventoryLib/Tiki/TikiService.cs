using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
//using System.Web.Services.Description;
using Newtonsoft.Json;
using OnlineInventoryLib.Interfaces;
using OnlineInventoryLib.Lazada.Responses;
using OnlineInventoryLib.Prism.Models;
using System.Runtime.Serialization.Json;
using OnlineInventoryLib.Tiki.Response;
using System.Threading.Tasks;

namespace OnlineInventoryLib.Tiki
{
    public class TikiService : ITikiService
    {
        public string Url {get; set;}
        public string ApplicationID { get; set; }
        public string Secret { get; set; }
        public DateTime AuthenticateDate { get; set; }
        public string AccessToken { get; set; }

        public string Token_type { get; set;  }

        public TikiService(string url, string applicationid, string secret)
        {
            this.Url = url;
            this.ApplicationID = applicationid;
            this.Secret = secret;
        }


        public bool RefreshToken(Online_store_Tiki tikiAuth)
        {

            string responseText = "";
            long dtNow = long.Parse(DateTime.Now.ToString("yyyyMMddHHmmss"));
            long dtCreate = long.Parse(tikiAuth.AuthenticateDate.ToString("yyyyMMddHHmmss"));
            int numSec = 0;
            int.TryParse((dtNow - dtCreate).ToString(), out numSec); // Find out how many seconds have elapsed.

            int expires_in = tikiAuth.ExpiresIn;

            if (numSec < (expires_in - 3600))
            {
                this.AccessToken = tikiAuth.AccessToken;
                this.Token_type = "Bearer";
                return false;
            }

            try
            {
                var basicAuth = "Basic " + Convert.ToBase64String(System.Text.Encoding.Default.GetBytes(tikiAuth.ApplicationID + ":" + tikiAuth.AccessTokenSecret));


                HttpWebRequest refreshtokenRequest = (HttpWebRequest)WebRequest.Create(tikiAuth.AppURL + "/sc/oauth2/token");
                refreshtokenRequest.Method = "POST";
                refreshtokenRequest.ContentType = "application/x-www-form-urlencoded";
                refreshtokenRequest.Accept = "application/json";
                //Adding Authorization header
                refreshtokenRequest.Headers[HttpRequestHeader.Authorization] = basicAuth;               

                string refreshtokenRequestBody = string.Format("grant_type=client_credentials&client_id={0}",
                    tikiAuth.ApplicationID);

                byte[] _byteVersion = Encoding.ASCII.GetBytes(refreshtokenRequestBody);
                refreshtokenRequest.ContentLength = _byteVersion.Length;
                Stream stream = refreshtokenRequest.GetRequestStream();
                stream.Write(_byteVersion, 0, _byteVersion.Length);
                stream.Close();

                HttpWebResponse refreshtokenResponse = (HttpWebResponse)refreshtokenRequest.GetResponse();

                using (var refreshTokenReader = new StreamReader(refreshtokenResponse.GetResponseStream()))
                {
                    //read response
                    responseText = refreshTokenReader.ReadToEnd();
                }

                if (responseText != "")
                {
                    Dictionary<string, string> refreshtokenEndpointDecoded = JsonConvert.DeserializeObject<Dictionary<string, string>>(responseText);

                    tikiAuth.AccessToken = refreshtokenEndpointDecoded["access_token"];

                    tikiAuth.ExpiresIn = int.Parse(refreshtokenEndpointDecoded["expires_in"]);
                    this.AccessToken = refreshtokenEndpointDecoded["access_token"];                    
                    this.Token_type = refreshtokenEndpointDecoded["token_type"];

                }

                return true;

            }
            catch (Exception ex) { throw; }
        }

        public Root GetProducts(string lazadaAccessToken, int v1, string v2, string lazFilter = "all", DateTime? create_after = null)
        {
            try
            {
                //var basicAuth = "Basic " + Convert.ToBase64String(System.Text.Encoding.Default.GetBytes(tikiAuth.ApplicationID + ":" + tikiAuth.AccessTokenSecret));



                string sUrl  = this.Url + "/integration/v2/products?page={0}";

#if DEBUG
                sUrl = "https://api-sandbox.tiki.vn" + "/integration/v1/products?updated_from_date=2021-08-05 16:29:21&updated_to_date=2021-09-07 16:29:21&page={0}";
#endif
                sUrl = string.Format(sUrl, v1);
                string responseText;

                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(sUrl);
                httpWebRequest.Method = "Get";
                //httpWebRequest.ContentType = "application/x-www-form-urlencoded";
                //httpWebRequest.Accept = "application/json";
                //Adding Authorization header
                string token = string.Concat("Bearer ", this.AccessToken);
                httpWebRequest.Headers[HttpRequestHeader.Authorization] = token;
#if DEBUG
                httpWebRequest.Headers.Add("tiki-api", "55f438d1-3438-409e-b5a4-9d16e764c5b8");
#endif

                HttpWebResponse refreshtokenResponse = (HttpWebResponse)httpWebRequest.GetResponse();

                using (var refreshTokenReader = new StreamReader(refreshtokenResponse.GetResponseStream()))
                {
                    //read response
                    responseText = refreshTokenReader.ReadToEnd();
                }

                if (responseText != "")
                {
                    Root refreshtokenEndpointDecoded = JsonConvert.DeserializeObject<Root>(responseText);
                    return refreshtokenEndpointDecoded;
                }

                return null;
            }
            catch (Exception ex) {
                throw ex;
            }
        }

        public void UpdateStock(TikiProduct prod)
        {
            try
            {
                string sUrl = this.Url + "/integration/v2/products/updateSku";

#if DEBUG
                sUrl = "https://api-sandbox.tiki.vn" + "/integration/v2/products/updateSku";
#endif

                string responseText;

                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(sUrl);
                httpWebRequest.Method = "Post";
                httpWebRequest.ContentType = "application/json";
                //Adding Authorization header
                httpWebRequest.Headers[HttpRequestHeader.Authorization] = string.Concat("Bearer ", this.AccessToken);
#if DEBUG
                httpWebRequest.Headers.Add("tiki-api", "55f438d1-3438-409e-b5a4-9d16e764c5b8");
#endif

                var refreshtokenRequestBody = string.Concat(@"{""original_sku"": """,prod.original_sku, @""",""quantity"":", prod.NewQuantity,"}");

                //string refreshtokenRequestBody = string.Format(body,
                //       prod.original_sku, 200);

                byte[] _byteVersion = Encoding.ASCII.GetBytes(refreshtokenRequestBody);
                httpWebRequest.ContentLength = _byteVersion.Length;
                Stream stream = httpWebRequest.GetRequestStream();
                stream.Write(_byteVersion, 0, _byteVersion.Length);
                stream.Close();

                HttpWebResponse refreshtokenResponse = (HttpWebResponse)httpWebRequest.GetResponse();

                using (var refreshTokenReader = new StreamReader(refreshtokenResponse.GetResponseStream()))
                {
                    //read response
                    responseText = refreshTokenReader.ReadToEnd();
                }

                if (responseText != "")
                {
                    return;
                }

            }catch (Exception ex) { throw ex;  }

            return;//throw new NotImplementedException();
        }
    }

    

}
