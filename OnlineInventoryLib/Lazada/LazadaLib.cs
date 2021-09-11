using Lazop.Api;
using Newtonsoft.Json;
using OnlineInventoryLib.Lazada.Models;
using OnlineInventoryLib.Lazada.Responses;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OnlineInventoryLib.Lazada
{
    public class LazadaLib
    {
        public string url { get; set; }
        public string appkey { get; set; }
        public string appSecret { get; set; }

        public LazadaLib(string url, string appKey, string appSecret)
        {
            this.url = url;
            this.appkey = appKey;
            this.appSecret = appSecret;
        }

        public string GetAuthorizationURL()
        {
            string url = @"https://auth.lazada.com/oauth/authorize?response_type=code&redirect_uri=https://www.acfc.com.vn&client_id={0}";
            return string.Format(url, appkey);

            //LazopClient client = new LazopClient("https://auth.lazada.com/rest", "121575", "eUJUo6ZANTcgbhvBWptubAsod8jVHAvd");
            //LazopRequest request = new LazopRequest("/auth/token/create");
            //request.AddApiParameter("code", "0_121575_KPWUZB2XUMNJU4dTY2YkqlMN17878");
            //LazopResponse response = client.Execute(request);
            //Console.WriteLine(response.Body);
        }

        public AccessTokenResponse GetAccessToken(string code)
        {
            LazopClient client = new LazopClient("https://auth.lazada.com/rest", appkey, appSecret);
            LazopRequest request = new LazopRequest("/auth/token/create");
            request.AddApiParameter("code", code);
            LazopResponse response = client.Execute(request);
            string js = response.Body;

            AccessTokenResponse token = JsonConvert.DeserializeObject<AccessTokenResponse>(js);
            return token;
        }

        /// <summary>
        /// Refresh access token của app - nếu refresh token còn hạn sử dụng
        /// </summary>
        /// <param name="refreshToken"></param>
        /// <returns></returns>
        public AccessTokenResponse RefreshToken(string refreshToken)
        {
            //---------------------
            // get refresh token
            LazopClient client = new LazopClient("https://auth.lazada.com/rest", appkey, appSecret);
            LazopRequest request = new LazopRequest("/auth/token/refresh");
            request.AddApiParameter("refresh_token", refreshToken);
            LazopResponse response = client.Execute(request);
            string js = response.Body;

            AccessTokenResponse token = JsonConvert.DeserializeObject<AccessTokenResponse>(js);
            return token;
        }

        /// <summary>
        /// Lấy danh sách sản phẩm trên Lazada
        /// </summary>
        /// <param name="accessToken">access token</param>
        /// <param name="offset">offset - theo skus</param>
        /// <param name="pageSize">pagesize - theo products</param>
        /// <param name="filter">product filter - mặc định all</param>
        /// <returns></returns>
        public LazadaGetProductData GetProducts(string accessToken, string offset, string pageSize, string filter = "all", DateTime? create_after = null)
        {
            try
            {
                ILazopClient client = new LazopClient(url, appkey, appSecret);
                LazopRequest request = new LazopRequest();
                request.SetApiName("/products/get");
                request.SetHttpMethod("GET");
                request.AddApiParameter("filter", filter);
                request.AddApiParameter("offset", offset);
                request.AddApiParameter("limit", pageSize);

                if (create_after.HasValue)
                    request.AddApiParameter("create_after", create_after.Value.ToString("yyyy-MM-ddThh:mm:ss+0700"));

                LazopResponse response = client.Execute(request, accessToken);
                //Console.WriteLine(response.IsError());

                if (response.IsError())
                {
                    ErrorResponse error = JsonConvert.DeserializeObject<ErrorResponse>(response.Body);
                    throw new Exception(error.ToString());
                }

                GetProductResponse getProductResponse = JsonConvert.DeserializeObject<GetProductResponse>(response.Body);
                return getProductResponse.data;
            }
            catch { throw; }
        }

        /// <summary>
        /// Cập nhật stock default
        /// </summary>
        /// <param name="accessToken">access token</param>
        /// <param name="payload">data</param>
        public void UpdateStock(string accessToken, string payload)
        {
            ILazopClient client = new LazopClient(url, appkey, appSecret);
            LazopRequest request = new LazopRequest();
            request.SetApiName("/product/price_quantity/update");
            request.AddApiParameter("payload", payload.Trim());

            LazopResponse response = client.Execute(request, accessToken);

            if (response.IsError())
            {
                ErrorResponse error = JsonConvert.DeserializeObject<ErrorResponse>(response.Body);
                throw new Exception(response.Body);
            }
            return;
        }

        /// <summary>
        /// Lấy danh sách các đơn hàng trên Lazada đang chờ xử lý
        /// </summary>
        /// <param name="accessToken">access token</param>
        /// <param name="status">order status - default: get all pending data</param>
        /// <returns></returns>
        public LazadaGetOrdersData GetOrders(string accessToken, int offset = 0, int limit = 100, string status = "pending", string created_after = "")
        {
            try
            {
                ILazopClient client = new LazopClient(url, appkey, appSecret);
                LazopRequest request = new LazopRequest();
                request.SetApiName("/orders/get");
                request.SetHttpMethod("GET");

                if (!string.IsNullOrEmpty(status))
                    request.AddApiParameter("status", status);

                if (!string.IsNullOrEmpty(created_after))
                    request.AddApiParameter("created_after", DateTime.Now.Date.AddDays(-2).ToString("yyyy-MM-dd") + "T00:00:00+07:00");
                else
                    request.AddApiParameter("created_after", created_after);

                request.AddApiParameter("ofset", offset.ToString());
                request.AddApiParameter("limit", limit.ToString());

                LazopResponse response = client.Execute(request, accessToken);

                if (response.IsError())
                {
                    ErrorResponse error = JsonConvert.DeserializeObject<ErrorResponse>(response.Body);
                    throw new Exception(error.ToString());
                }

                GetOrdersResponse getOrdersResponse = JsonConvert.DeserializeObject<GetOrdersResponse>(response.Body);
                return getOrdersResponse.data;
            }
            catch { throw; }
        }


        /// <summary>
        /// Lấy danh sách sản phẩm trong 1 đơn hàng
        /// </summary>
        /// <param name="accessToken">access token</param>
        /// <param name="orderId">Số đơn hàng</param>
        /// <returns></returns>
        public LazadaGetOrderItemsData GetOrderItems(string accessToken, float orderId)
        {
            try
            {
                ILazopClient client = new LazopClient(url, appkey, appSecret);
                LazopRequest request = new LazopRequest();
                request.SetApiName("/order/items/get");
                request.SetHttpMethod("GET");

                request.AddApiParameter("order_id", orderId.ToString());

                LazopResponse response = client.Execute(request, accessToken);

                if (response.IsError())
                {
                    ErrorResponse error = JsonConvert.DeserializeObject<ErrorResponse>(response.Body);
                    throw new Exception(error.ToString());
                }

                GetOrderItemsResponse getOrderItemsResponse = JsonConvert.DeserializeObject<GetOrderItemsResponse>(response.Body);
                return getOrderItemsResponse.data; 
            }
            catch { throw; }
        }


        /// <summary>
        /// Lấy tất cả sản phẩm đang được đặt hàng trong tất cả các đơn pending được tạo từ ngày
        /// </summary>
        /// <param name="accessToken"></param>
        /// <param name="created_after">Ngày tạo đơn hàng ( theo định dạng yyyy-MM-ddTHH:mm:ss+07:00</param>
        /// <returns></returns>
        public List<LazadaOrderItem> GetAllOrderItems(string accessToken, string status = "pending", string created_after = "")
        {
            List<LazadaOrderItem> items = new List<LazadaOrderItem>();

            try
            {
                LazadaOrder[] orders = GetOrders(accessToken, status: status, created_after: created_after).orders;

                foreach(LazadaOrder order in orders)
                {
                    LazadaOrderItem[] orderItems = GetOrderItems(accessToken, order.order_id).orderItems;
                    items.AddRange(orderItems);

                    items = items
                        .GroupBy(i => i.sku)
                        .Select(ordered => new LazadaOrderItem
                            {
                                sku = ordered.First().sku,
                                qty = ordered.Sum(c => c.qty)
                            }
                        ).ToList();
                }

                return items;
            }
            catch { throw; }
        }
    }
}
