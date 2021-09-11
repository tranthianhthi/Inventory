using COGInterfaceCommand.Common.COG;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Web.Script.Serialization;
using COGInterfaceCommand.Command;

namespace COGInterfaceCommand.Command
{
    public class APICommand
    {
        string Token { get; set; }
        public string AuthenticationURL { get; set; }
        public string ClientID { get; set; }
        public string ClientSecret { get; set; }
        public string Resource { get; set; }
        public string APIURL { get; set; }
        public string PrimarySubcriptionKey { get; set; }
        public string SecondarySubcriptionKey { get; set;}
        public string ResponseContent { get; set; }
        public ByteArrayContent RequestContent { get; set; }
        public string GrantType { get; set; }
        public string Licensee { get; set; }

        string RequestHeader = "{\"grant_type\":\"{0}\",\"client_id\":\"{1}\",\"client_secret\":\"{2}\",\"audience\":\"{3}\"}";
        string RequestBody = "grant_type={0}&client_id={1}&client_secret={2}&resource={3}";
        
        HttpClient client = new HttpClient();
        public APICommand(string URL, string id, string secret, string resource, string apiURL, string primarySub, string secondarySub, string apiParameter, string grantType = "client_credentials")
        {
            AuthenticationURL = URL;
            ClientID = id;
            ClientSecret = secret;
            Resource = resource;
            GrantType = grantType;
            APIURL = apiURL;
            PrimarySubcriptionKey = primarySub;
            SecondarySubcriptionKey = secondarySub;
            Licensee = apiParameter;
        }
        public void GetCOGToken()
        {
            string tokenString = string.Empty;

            string requestString = string.Format(RequestBody, GrantType, ClientID, ClientSecret, Resource);

            WebRequest request = WebRequest.Create(AuthenticationURL);
            // Set the Method property of the request to POST.
            request.Method = "POST";
            // Create POST data and convert it to a byte array.
            byte[] byteArray = Encoding.UTF8.GetBytes(requestString);
            // Set the ContentType property of the WebRequest.
            request.ContentType = "application/x-www-form-urlencoded";
            // Set the ContentLength property of the WebRequest.
            request.ContentLength = byteArray.Length;

            try
            {
                // Get the request stream.
                Stream dataStream = request.GetRequestStream();
                // Write the data to the request stream.
                dataStream.Write(byteArray, 0, byteArray.Length);
                // Close the Stream object.
                dataStream.Close();
                // Get the response.
                WebResponse response = request.GetResponse();

                dataStream = response.GetResponseStream();
                StreamReader reader = new StreamReader(dataStream);
                // Read the content.
                string responseFromServer = reader.ReadToEnd();
                JObject jObj = JObject.Parse(responseFromServer);

                Token = jObj["access_token"].ToString();
            }
            catch(Exception ex)
            {
                throw;
            }            
        }
        public string GetCOGTokenForSendFile()
        {
            string tokenString = string.Empty;

            string requestString = string.Format(RequestBody, GrantType, ClientID, ClientSecret, Resource);

            WebRequest request = WebRequest.Create(AuthenticationURL);
            // Set the Method property of the request to POST.
            request.Method = "POST";
            // Create POST data and convert it to a byte array.
            byte[] byteArray = Encoding.UTF8.GetBytes(requestString);
            // Set the ContentType property of the WebRequest.
            request.ContentType = "application/x-www-form-urlencoded";
            // Set the ContentLength property of the WebRequest.
            request.ContentLength = byteArray.Length;

            try
            {
                // Get the request stream.
                Stream dataStream = request.GetRequestStream();
                // Write the data to the request stream.
                dataStream.Write(byteArray, 0, byteArray.Length);
                // Close the Stream object.
                dataStream.Close();
                // Get the response.
                WebResponse response = request.GetResponse();

                dataStream = response.GetResponseStream();
                StreamReader reader = new StreamReader(dataStream);
                // Read the content.
                string responseFromServer = reader.ReadToEnd();
                JObject jObj = JObject.Parse(responseFromServer);

                Token = jObj["access_token"].ToString();
                return Token;
            }
            catch (Exception ex)
            {
               throw;
            }
        }
        public async void MakeRequest(string uri, string outputFile)
        {
            try
            {
                // Request headers     
                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + Token); // Bearer Token       
                client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", PrimarySubcriptionKey); // Subcription Key
                // Request parameters
                uri += Licensee;
                var response = client.GetAsync(APIURL + uri).Result;
                if (response.IsSuccessStatusCode)
                {
                    ResponseContent = await response.Content.ReadAsStringAsync();
                    string text = ResponseContent;

                    FileStream fstream = new FileStream(outputFile, FileMode.OpenOrCreate);
                    StreamWriter streamWriter = new StreamWriter(fstream);
                    streamWriter.Write(ResponseContent.Replace("},\"", "}," + Environment.NewLine + "\""));
                    streamWriter.Flush();
                    streamWriter.Close();
                    fstream.Close();
                }
            }
            catch { throw; }
        }
        public string GetJsonFromFile(string outputFile)
        {
            if (File.Exists(outputFile))
            {
                try
                {
                    FileStream fstream = new FileStream(outputFile, FileMode.Open);
                    StreamReader reader = new StreamReader(fstream);
                    string jsonString = reader.ReadToEnd();
                    reader.Close();
                    fstream.Close();

                    return jsonString;
                }
                catch { throw; }
            }
            else
            {
                throw new FileNotFoundException();
            }
        }
        public async void SubmitAcknowledgementMessage(string uri, AcknowledgeMessage message)
        {
            try
            {
                // Request headers if we call before no need call here.
                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + Token); // Bearer Token       
                client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", PrimarySubcriptionKey); // Subcription Key

                // Request parameters
                //uri += Licensee;

                string jsonString = new JavaScriptSerializer().Serialize(message);
                byte[] byteData = Encoding.UTF8.GetBytes(jsonString);

                using (var content = new ByteArrayContent(byteData))
                {
                    content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                    var response = client.PostAsync(uri, content).Result;

                    string responseMsg = await response.Content.ReadAsStringAsync();

                    if (response.IsSuccessStatusCode)
                    {                       
                        ResponseContent = "Acknowledgement message sent." + Environment.NewLine + responseMsg;
                    }
                    else
                    {
                        ResponseContent = "Acknowledgement message cannot sent." + Environment.NewLine + responseMsg;
                    }
                }
            }
            catch { throw; }
        }        
        public async void SubmitAPIToCOG(string uri, string message)
        {
            try
            {
                client.DefaultRequestHeaders.Clear();
                // Request headers if we call before no need call here.
                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + Token); // Bearer Token       
                client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", PrimarySubcriptionKey); // Subcription Key

                string jsonString = new JavaScriptSerializer().Serialize(message);
                byte[] byteData = Encoding.UTF8.GetBytes(jsonString);

                using (var content = new ByteArrayContent(byteData))
                {
                    content.Headers.ContentType = new MediaTypeHeaderValue("application/json");                    
                    var response = client.PostAsync(uri, content).Result;

                    string responseMsg = await response.Content.ReadAsStringAsync();

                    if (response.IsSuccessStatusCode)
                    {
                        ResponseContent = "Send API File." + Environment.NewLine + responseMsg;
                    }
                    else
                    {
                        ResponseContent = "Can not send API File." + Environment.NewLine + responseMsg;
                    }
                }
            }
            catch
            {
                { throw; }
            }
        }

        public async void PostDataToCOG(string uri, object obj, bool sendDCCode = false)
        {
            try
            {
                client.DefaultRequestHeaders.Clear();
                // Request headers if we call before no need call here.
                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + Token); // Bearer Token       
                client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", PrimarySubcriptionKey); // Subcription Key
                if (sendDCCode)
                {
                    client.DefaultRequestHeaders.Add("DC-Code", "0120");
                }

                string jsonString = new JavaScriptSerializer().Serialize(obj);
                byte[] byteData = Encoding.UTF8.GetBytes(jsonString);

                using (var content = new ByteArrayContent(byteData))
                {
                    content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                    
                    var response = client.PostAsync(uri, content).Result;

                    string responseMsg = await response.Content.ReadAsStringAsync();

                    if (response.IsSuccessStatusCode)
                    {
                        ResponseContent = "Post data completed." + Environment.NewLine + responseMsg;
                    }
                    else
                    {
                        ResponseContent = "Can not post data." + Environment.NewLine + responseMsg;
                    }
                    
                }           
            }
            catch
            {
                { throw; }
            }
        }

        public async void PatchDataToCOG(string uri, object obj, bool sendDCCode = false)
        {
            try
            {
                client.DefaultRequestHeaders.Clear();
                // Request headers if we call before no need call here.
                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + Token); // Bearer Token       
                client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", PrimarySubcriptionKey); // Subcription Key
                if (sendDCCode)
                {
                    client.DefaultRequestHeaders.Add("DC-Code", "0120");
                }

                string jsonString = new JavaScriptSerializer().Serialize(obj);
                byte[] byteData = Encoding.UTF8.GetBytes(jsonString);

                using (var content = new ByteArrayContent(byteData))
                {
                    content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                    var request = new HttpRequestMessage(new HttpMethod("PATCH"), uri);
                    request.Content = content;

                    // Set the Method property of the request to POST.


                    var response = client.SendAsync(request).Result;

                    string responseMsg = await response.Content.ReadAsStringAsync();

                    if (response.IsSuccessStatusCode)
                    {
                        ResponseContent = "Post data completed." + Environment.NewLine + responseMsg;
                    }
                    else
                    {
                        ResponseContent = "Can not post data." + Environment.NewLine + responseMsg;
                    }

                }
            }
            catch
            {
                { throw; }
            }
        }
    }
}

//public string GetToken()
//{
//    IRestResponse response;
//    string tokenString = string.Empty;

//    // fill authentication info
//    authenticationURL = "https://dev-acfc.auth0.com/oauth/token"; //Should we use https://login.microsoftonline.com/ab872c56-a5b0-452c-85fb-a87fa1350088/oauth2/token
//    clientID = "bs3yAUSin5jnsRevsgNFmjAHUsITMQpt"; // COG Document said that COG IT will provide this ID in the first time
//    clientSecret = "tL8G7pJd_1j5n2w7N7k2Pn0oC8ujuhoAAnqoJy_dfbfnFH-0AqXOFQ5AEI3IcWMz"; // COG Document said that COG IT will provide this ID in the first time
//    audience = "http://localhost:5000/api"; //Change to COG API url? https://cogapimanagementuat01.azure-api.net/Licensee/v1/

//    string header = "{\"client_id\":\"" + clientID + "\",\"client_secret\":\"" + clientSecret + "\",\"audience\":\"" + audience + "\",\"grant_type\":\"client_credentials\"}";


//    var AuthClient = new RestClient(authenticationURL); 
//    var authRequest = new RestRequest(Method.POST);
//    authRequest.AddHeader("content-type", "application/json");
//    authRequest.AddParameter("application/json", header , ParameterType.RequestBody);

//    response = AuthClient.Execute(authRequest);

//    if (response.ContentType == "application/json")
//    {
//        JObject obj = JObject.Parse(response.Content);
//        tokenString = obj["access_token"].ToString();
//    }

//    Token = tokenString;

//    return tokenString;
//}
