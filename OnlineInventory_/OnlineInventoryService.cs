using OnlineInventory.Models;
using OnlineShopLib;
using OnlineShopLib.Shopee;
using OnlineShopLib.Shopee.Models;
using OnlineShopLib.Shopee.Requests;
using OnlineShopLib.Shopee.Responses;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace OnlineInventory
{
    public class OnlineInventoryService
    {
        string secretKey = "d86b56a0e47616caae2679cbfe88399e7b3c4fcbaf3ac2ade972404c61c70555";

        IOnlineShopLib onlineLib;
        List<RPShopeeMapping> mapping;

        public OnlineInventoryService()
        {
            onlineLib = new ShopeeAPICommand(secretKey);
            mapping = ReadMappingFile("");
        }

        public List<RPShopeeMapping> ReadMappingFile(string path)
        {

        }

        public async void UpdateShopeeInventory()
        {
            // Lấy danh sách qty-on-hand của các item bán Shopee trên prism
            GetPrismInvoice();
            // Lấy danh sách order - order details trên shopee để xác định các upc trừ tồn
            GetOrdersListResponse orderResponse = null;
            orderResponse = await GetOrdersList();
            // Lấy danh sách cancel order trên Shopee để xác định các upc cộng tồn

            // Tính lại qty-on-hand

            // Cập nhật tồn trên Shopee 

        }

        public async Task<UpdateStockResponse> UpdateStock()
        {
            UpdateStockRequest request = new UpdateStockRequest(100772074, 24, 220035281, 843680);
            UpdateStockResponse response = await onlineLib.UpdateStock<UpdateStockResponse, UpdateStockRequest>(request);
            return response;
        }

        public async Task<UpdateStockBatchResponse> UpdateStockBatch()
        {
            Item[] items = new Item[]
            {
                new Item() {item_id = 100772074, stock = 18 },
                new Item() {item_id = 100771965, stock = 19 },
                new Item() {item_id = 100771966, stock = 20 }
            };
            UpdateStockBatchRequest request = new UpdateStockBatchRequest(items, 220035281, 843680);
            UpdateStockBatchResponse response = await onlineLib.UpdateStockBatch<UpdateStockBatchResponse, UpdateStockBatchRequest>(request);
            WriteLog(response.ToString());
            return response;
        }

        public async Task<GetOrdersListResponse> GetOrdersList()
        {
            long createFrom = DateTimeOffset.UtcNow.AddMinutes(-30).ToUnixTimeSeconds();
            long createTo = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            GetOrdersListRequest request = new GetOrdersListRequest(220035281, 843680, true, createFrom, createTo, 0, 0, 0, 0);
            GetOrdersListResponse response = await onlineLib.GetOrdersList<GetOrdersListResponse, GetOrdersListRequest>(request);
            return response;
        }

        public void GetPrismInvoice()
        {
            
        }

        private void WriteLog(string log)
        {
            FileStream fstream = new FileStream(Environment.CurrentDirectory + "\\" + DateTime.Now.ToString("yyyyMMdd") + ".log", FileMode.OpenOrCreate, FileAccess.ReadWrite);
            StreamWriter streamWriter = new StreamWriter(fstream);
            streamWriter.Write(log);
            streamWriter.Flush();
            streamWriter.Close();
            fstream.Close();
        }
    }
}
