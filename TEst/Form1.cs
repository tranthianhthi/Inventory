using Newtonsoft.Json.Linq;
using QRCoder;
using RestSharp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TEst
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        readonly HttpClient client = new HttpClient();

        private void Form1_Load(object sender, EventArgs e)
        {
            //QRCodeGenerator qrGenerator = new QRCodeGenerator();
            //QRCodeData qrCodeData = qrGenerator.CreateQrCode("http://www.google.com/search?q=abc", QRCodeGenerator.ECCLevel.Q);
            //QRCode qrCode = new QRCode(qrCodeData);
            //Bitmap qrCodeImage = qrCode.GetGraphic(20);
            //img.Image = qrCodeImage;

            IRestResponse response;

            var Authlient = new RestClient("https://cogapimanagementuat01.portal.azure-api.net/");//("https://dev-acfc.auth0.com/oauth/token");
            var authRequest = new RestRequest(Method.POST);
            authRequest.AddHeader("content-type", "application/json");
            authRequest.AddParameter("application/json", "{\"client_id\":\"bs3yAUSin5jnsRevsgNFmjAHUsITMQpt\",\"client_secret\":\"tL8G7pJd_1j5n2w7N7k2Pn0oC8ujuhoAAnqoJy_dfbfnFH-0AqXOFQ5AEI3IcWMz\",\"audience\":\"http://localhost:5000/api\",\"grant_type\":\"client_credentials\"}", ParameterType.RequestBody);
            response = Authlient.Execute(authRequest);
            if (response.ContentType == "application/json")
            {
                JObject obj = JObject.Parse(response.Content);
            }
            
            


            var client = new RestClient("http://192.168.10.109:5000/api/transferasn/private");
            var request = new RestRequest(Method.GET);
            request.AddHeader("authorization", "Bearer eyJ0eXAiOiJKV1QiLCJhbGciOiJSUzI1NiIsImtpZCI6Ik1EY3pNa0kzTlVGR05qVXhPVGhHTUVReFFqUTVOREZDUVRVelJUVkJRakJCTVRsRVJrTTBRdyJ9.eyJpc3MiOiJodHRwczovL2Rldi1hY2ZjLmF1dGgwLmNvbS8iLCJzdWIiOiJiczN5QVVTaW41am5zUmV2c2dORm1qQUhVc0lUTVFwdEBjbGllbnRzIiwiYXVkIjoiaHR0cDovL2xvY2FsaG9zdDo1MDAwL2FwaSIsImlhdCI6MTU2NjE4MzA1NiwiZXhwIjoxNTY2MjY5NDU2LCJhenAiOiJiczN5QVVTaW41am5zUmV2c2dORm1qQUhVc0lUTVFwdCIsImd0eSI6ImNsaWVudC1jcmVkZW50aWFscyJ9.pe0Xz_E222wVZ4FfvDjxq3L6JaC7YvncrIQJeBn5q-mHL8lynVsHOtGYQYZGUQofIws0uBSYx1au_F9Vf__nqdZDw33Zw0lJSOOkuc3Nk6D24Fi-DXoqLVPejFLby5jRPCEu_WtLbbI-jxbOWdVAcCVxNht5kToO4cXhdBzYyuLYDQPqJQFQoxSJtvRhR_NWCl6_RqdW2oB0HMawIw9VZ9mCNflZCcJ6ZbNgCWAyl9Pf4RnYiidujo-zeoy2G8qoKFHURX-urDlQm5D_hQdVNLyEf2xV6Ri0M8aDTrXhol_Kv7HyI_Nij6WBV9pcyaE822nQYmCiRBnRW3QhT1Q2PQ");
            response = client.Execute(request);

            MessageBox.Show(response.ToString());
        }

        //private async Task<Product> GetProductAsync(string path)
        //{
        //    Product product = null;
        //    HttpResponseMessage response = await client.GetAsync(path);
        //    if (response.IsSuccessStatusCode)
        //    {
        //        product = await response.Content.ReadAsAsync<Product>();
        //    }
        //    return product;
        //}


        private async Task RunAsync()
        {
            // Update port # in the following line.
            client.BaseAddress = new Uri("http://localhost:5000/");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));

            try
            {
                // Create a new product
                //Product product = new Product
                //{
                //    Name = "Gizmo",
                //    Price = 100,
                //    Category = "Widgets"
                //};

                //var url = await CreateProductAsync(product);
                //Console.WriteLine($"Created at {url}");

                // Get the product
                //product = await GetProductAsync(url.PathAndQuery);
                //ShowProduct(product);

                // Update the product
                //Console.WriteLine("Updating price...");
                //product.Price = 80;
                //await UpdateProductAsync(product);

                //// Get the updated product
                //product = await GetProductAsync(url.PathAndQuery);
                //ShowProduct(product);

                //// Delete the product
                //var statusCode = await DeleteProductAsync(product.Id);
                //Console.WriteLine($"Deleted (HTTP Status = {(int)statusCode})");

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            Console.ReadLine();
        }
    }

}
