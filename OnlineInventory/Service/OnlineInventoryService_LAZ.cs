using OnlineInventoryLib.Lazada.Models;
using OnlineInventoryLib.Lazada.Responses;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OnlineInventory
{
    public partial class OnlineInventoryService
    {
        #region LAZADA



        /// <summary>
        /// LAZADA - Lấy danh sách product trên Lazada - /Products/Get
        /// </summary>
        /// <param name="lazFilter">filter status of products - mặc định "all"</param>
        /// <param name="create_after">filter ngày tạo - mặc định null</param>
        private LazadaProduct[] CallLazadaAPIProduct(string lazFilter = "all", DateTime? create_after = null)
        {
            int offset = 0;
            int totalProd = 0;
            int LazTotalProd = 0;

            try
            {
                LazadaProduct[] LazProducts = new LazadaProduct[0];
                LazadaGetProductData getProductResponse = LazLib.GetProducts(config.LazadaAccessToken, offset.ToString(), config.PageSize.ToString(), lazFilter, create_after);
                LazTotalProd = getProductResponse.total_products;

                while (totalProd < LazTotalProd)//(getProductResponse.products != null && getProductResponse.products.Length > 0)
                {
                    LazProducts = LazProducts.Concat(getProductResponse.products).ToArray();

                    totalProd += getProductResponse.CountProducts();
                    offset += getProductResponse.CountTotalSKUs();

                    if (totalProd < LazTotalProd)
                        getProductResponse = LazLib.GetProducts(config.LazadaAccessToken, totalProd.ToString(), config.PageSize.ToString(), lazFilter);
                }

                return LazProducts;
            }
            catch { throw; }
        }


        /// <summary>
        /// LAZADA - Single warehouse - Lấy danh sách item có trên Lazada và có trong file cho bán online
        /// </summary>  
        private void JoinLazProductsWithPrismList()
        {
            LazOnSaleProducts = new LazadaProduct[0];
            GetLazadaOrderedItems();

            if (OnlineUPCs != null)
            {
                // Lấy danh sách sản phẩm từ API
                var q = from p in LazProducts
                        select new LazadaProduct()
                        {
                            // join với danh sách sản phẩm trong file Excel 
                            skus = p.skus.Join(
                                OnlineUPCs,
                                s => s.SellerSku,
                                oh => oh.UPC,
                                (s, oh) => new LazadaSKU()
                                {
                                    SellerSku = s.SellerSku,
                                    Quantity = s.Quantity,
                                    OfflineQty = oh.Qty,
                                    UseMultiWarehouse = this.LazUseMultiWarehouse, // gán giá trị sử dụng multi warehouse
                                    WarehouseCode = this.LazWarehouseCode // gán warehouse code
                                }
                            )
                            // lọc điều kiện không nằm trong danh sách đang order
                            .Where(s => !LazOrderedItems.Any(oi => oi.sku == s.SellerSku)).ToArray<LazadaSKU>()
                        };
                q = q.Where(l => l.skus.Count() > 0);

                LazOnSaleProducts = LazOnSaleProducts.Concat(q.ToArray()).ToArray();


            }
            else
            {
                var q = from p in LazProducts
                        select new LazadaProduct()
                        {
                            skus = (
                                from s in p.skus
                                select new LazadaSKU()
                                {
                                    SellerSku = s.SellerSku,
                                    Quantity = s.Quantity,
                                    OfflineQty = 0, // không sử dụng file keep offline => keep offline = 0
                                    UseMultiWarehouse = this.LazUseMultiWarehouse, // gán giá trị sử dụng multi warehouse
                                    WarehouseCode = this.LazWarehouseCode // gán warehouse code
                                }
                            ).Where(s => !LazOrderedItems.Any(oi => oi.sku == s.SellerSku)).ToArray<LazadaSKU>()
                        };

                LazOnSaleProducts = LazOnSaleProducts.Concat(q.ToArray()).ToArray();
            }

        }



        /// <summary>
        /// LAZADA - Lấy các sản phẩm có trên Lazada nhưng không có trong file bán online, gán NewQty = 0 cho các sản phẩm đó
        /// </summary>
        private void GetLazadaRemoveProducts()
        {
            LazRemovedProducts = new LazadaProduct[0];

            if (OnlineUPCs != null)
            {
                var q = from p in LazProducts
                        select new LazadaProduct()
                        {
                            skus = (
                               from s in p.skus.Where(s => !OnlineUPCs.Any(i => i.UPC == s.SellerSku) && s.Quantity > 0)
                               select new LazadaSKU()
                               {
                                   SellerSku = s.SellerSku,
                                   Quantity = s.Quantity,
                                   NewQuantity = 0,
                                   UseMultiWarehouse = this.LazUseMultiWarehouse, // gán giá trị sử dụng multi warehouse
                                   WarehouseCode = this.LazWarehouseCode
                               }).ToArray<LazadaSKU>()
                        };
                q = q.Where(l => l.skus.Count() > 0);

                LazRemovedProducts = LazRemovedProducts.Concat(q.ToArray()).ToArray();
            }
            else
            {
                // do nothing
            }
        }



        /// <summary>
        /// LAZADA - Lấy danh sách các sản phẩm đang được đặt hàng 
        /// </summary>
        private void GetLazadaOrderedItems()
        {
            LazOrderedItems = new List<LazadaOrderItem>();

            try
            {
                LazOrderedItems = LazLib.GetAllOrderItems(config.LazadaAccessToken);
            }
            catch
            {

            }
        }



        /// <summary>
        /// LAZADA - Cập nhật các sản phẩm không có trong file : qty = 0
        /// </summary>
        private void RemoveLazadaProducts(int pageSize)
        {
            //LazadaProduct[] productsWithNewQty;

            //// Gán new quantity = 0 cho các product không được bán trên sàn
            //var result =
            //     from p in LazRemovedProducts
            //     select new LazadaProduct()
            //     {
            //         //skus = (from s in p.skus select new LazadaSKU() { SellerSku = s.SellerSku, Quantity = s.Quantity, NewQuantity = 1}).ToArray()

            //         // Điều kiện where để xác định các item cần gọi API update
            //         skus = (from s in p.skus select new LazadaSKU() { SellerSku = s.SellerSku, Quantity = s.Quantity, NewQuantity = 0 }).Where(s => s.Quantity > s.NewQuantity).ToArray()
            //     };

            //productsWithNewQty = result.Where(p => p.skus.Length > 0).ToArray();

            int count = 0;

            // Lặp qua tất cả sản phẩm cần cập nhật
            while (count < LazRemovedProducts.Length)
            {
                string logMessage = "";
                string payload = "<Request>{0}</Request>";
                string productArray = "";

                // Mỗi lần lấy 1 số lượng sản phẩm = pagesize ( 1 )
                LazadaProduct[] prods = LazRemovedProducts.Skip(count).Take(pageSize).ToArray();

                // Lặp qua từng sản phẩm, lấy xml cập nhật qty => cập nhật payload
                foreach (LazadaProduct prod in prods)
                {
                    string productXML = prod.GenerateUpdateProductQuantity();
                    productArray += productXML;
                }
                payload = string.Format(payload, productArray);


                try
                {
                    // Gọi API cập nhật stock
                    LazLib.UpdateStock(config.LazadaAccessToken, payload);
                }
                catch (Exception ex)
                {
                    logMessage = ",\"exception\":\"" + ex.ToString() + "\"";
                }


                // Cập nhật số lượng upc đã xử lý
                count += prods.Length;

                // Ghi log
                if (count < LazRemovedProducts.Length)
                {
                    WriteLog("\"" + payload + "\"" + logMessage + "," + Environment.NewLine);
                }
                else
                {
                    WriteLog("\"" + payload + "\"" + logMessage + Environment.NewLine);
                }
            }
        }



        /// <summary>
        /// LAZADA - Call API to update Qty of Product
        /// </summary>
        /// <param name="lazadaProducts">Array of products on Lazada</param>
        /// <returns></returns>
        private void UpdateLazadaStock(int pageSize)
        {
            //LazadaProduct laz = LazOnSaleProducts.FirstOrDefault(s => s.skus.Any(r => r.SellerSku == "11334571074"));
            //LazOnSaleProducts = new LazadaProduct[] { laz };
            var result = from p in LazOnSaleProducts
                         select new LazadaProduct()
                         {
                             skus = (
                                from s in p.skus
                                join oh in PrismQtyOnHands on s.SellerSku equals oh.UPC into lazOnHands
                                from _oh in lazOnHands.DefaultIfEmpty()
                                select new LazadaSKU()
                                {
                                    SellerSku = s.SellerSku,
                                    Quantity = s.Quantity,
                                    UseMultiWarehouse = s.UseMultiWarehouse,
                                    WarehouseCode = s.WarehouseCode,
                                    NewQuantity = _oh ? .QtyOnline ?? 0 // 0 : (lazOnHands.First().QtyOnline < 0 ? 0 : lazOnHands.First().QtyOnline)
                                }
                            ).Where(s =>
                                !(s.NewQuantity == s.Quantity && s.Quantity == 0)  // Chỉ loại trừ trường hợp onhand = 0 & available = 0 là không cần cập nhật.
                            ).ToArray<LazadaSKU>().ToArray<LazadaSKU>()
                         };

            result = result.Where(p => p.skus.Length > 0).ToList();

            //productsWithNewQty = result.ToArray();//result.Where(p => p.skus.Length > 0).ToArray();

            int count = 0;
            int total = result.Count();

            while (count < total)
            {
                string payload = "<Request>{0}</Request>";
                string productArray = "";

                LazadaProduct[] prods = result.Skip(count).Take(pageSize).ToArray();

                foreach (LazadaProduct prod in prods)
                {
                    string productXML = prod.GenerateUpdateProductQuantity();
                    productArray += productXML;
                    //WriteLog(prod.GenerateProductLogString());
                }
                payload = string.Format(payload, productArray);

                try
                {
                    LazLib.UpdateStock(config.LazadaAccessToken, payload);
                }
                catch (Exception ex)
                {

                }

                count += prods.Length;
                if (count < total)
                {
                    WriteLog("\"" + payload + "\"," + Environment.NewLine);
                }
                else
                {
                    WriteLog("\"" + payload + "\"" + Environment.NewLine);
                }
            }


            //return productsWithNewQty;
        }



        /// <summary>
        /// LAZADA - Call API to update Qty by warehouse
        /// </summary>
        /// <param name="pageSize"></param>
        private void UpdateLazadaStockByWarehouse(int pageSize)
        {
            // Get stock from warehouse 

            var result = from p in LazOnSaleProducts
                         select new LazadaProduct()
                         {
                             skus = (
                                from s in p.skus
                                join oh in PrismQtyOnHands on s.SellerSku equals oh.UPC into lazOnHands
                                from _oh in lazOnHands.DefaultIfEmpty()
                                select new LazadaSKU()
                                {
                                    SellerSku = s.SellerSku,
                                    Quantity = s.Quantity,
                                    UseMultiWarehouse = s.UseMultiWarehouse,
                                    WarehouseCode = s.WarehouseCode,
                                    NewQuantity = _oh?.QtyOnline ?? 0  //lcount == 0 ? 0 : (lazOnHands.First().QtyOnline < 0 ? 0 : lazOnHands.First().QtyOnline)
                                }
                            ).Where(s =>
                                !(s.NewQuantity == s.Quantity && s.Quantity == 0)  // Chỉ loại trừ trường hợp onhand = 0 & available = 0 là không cần cập nhật.
                            ).ToArray<LazadaSKU>().ToArray<LazadaSKU>()
                         };

            result = result.Where(p => p.skus.Length > 0);

            //productsWithNewQty = result.ToArray();//result.Where(p => p.skus.Length > 0).ToArray();

            int count = 0;
            int total = result.Count();

            while (count < total)
            {
                string payload = "<Request>{0}</Request>";
                string productArray = "";

                LazadaProduct[] prods = result.Skip(count).Take(pageSize).ToArray();

                foreach (LazadaProduct prod in prods)
                {
                    string productXML = prod.GenerateUpdateProductQuantity();
                    productArray += productXML;
                    //WriteLog(prod.GenerateProductLogString());
                }
                payload = string.Format(payload, productArray);

                try
                {
                    LazLib.UpdateStock(config.LazadaAccessToken, payload);
                }
                catch (Exception ex)
                {

                }

                count += prods.Length;
                if (count < total)
                {
                    WriteLog("\"" + payload + "\"," + Environment.NewLine);
                }
                else
                {
                    WriteLog("\"" + payload + "\"" + Environment.NewLine);
                }
            }


            //return productsWithNewQty;
        }



        /// <summary>
        /// LAZADA - Tạo authenticate URL
        /// </summary>
        /// <returns></returns>
        public string GetLazadaAuthenticateURL()
        {
            return LazLib.GetAuthorizationURL();
        }



        /// <summary>
        /// LAZADA - Lấy access token
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public AccessTokenResponse GetAccessToken(string code)
        {
            return LazLib.GetAccessToken(code);
        }



        #endregion
    }
}
