using OnlineInventoryLib.Shopee.Models;
using OnlineInventoryLib.Shopee.Requests;
using OnlineInventoryLib.Shopee.Responses;
using OnlineInventoryLib.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineInventory
{
    public partial class OnlineInventoryService
    {
        #region SHOPEE

        /// <summary>
        /// Gọi API của Shopee để lấy product detail ( danh sách variations )
        /// </summary>
        /// <param name="item_id">Mã product trên shopee ( 1 product Shopee có nhiều variations, 1 variation = 1 upc )</param>
        /// <returns></returns>
        public async Task<ShopeeVariation[]> CallShopeeAPIProductDetail(ulong item_id )
        {
            try
            {
                GetItemDetailRequest itemDetailRequest = new GetItemDetailRequest(item_id, config.ShopeeShopId, config.ShopeePartnerId);
                GetItemDetailResponse itemDetailResponse = await ShopeeLib.GetItemDetail<GetItemDetailResponse, GetItemDetailRequest>(itemDetailRequest);
                return itemDetailResponse.item.variations;
            }
            catch { throw; }
        }

        /// <summary>
        /// SHOPEE - lấy danh sách tất cả sản phẩm đang có trên Shopee
        /// </summary>
        /// <param name="getDetail">Check if need to get more detail of products</param>
        public async Task CallShopeeAPIProduct(bool getDetail = false)
        {
            ShopeeVariations = new List<ShopeeVariation>();

            /*===========================================================
            /* Gọi API get item
            /*===========================================================*/

            // Tạo request
            GetItemListRequest request = new GetItemListRequest(config.ShopeeShopId, config.ShopeePartnerId, 0, 0, 0, 100);
            bool more = true;
            // Nếu vẫn còn item

            try
            {
                while (more)
                {

                    GetItemListResponse response = await ShopeeLib.GetItemList<GetItemListResponse, GetItemListRequest>(request);
                    foreach (ShopeeItem item in response.items)
                    {
                        ShopeeVariation[] variations = item.variations;

                        if (getDetail)
                        {
                            variations = await CallShopeeAPIProductDetail(item.item_id);                           
                        }

                        // get stock on Shopee
                        foreach (ShopeeVariation variation in variations) //itemDetailResponse.item.variations)
                        {
                            variation.item_id = item.item_id;
                            variation.shopeeVariationSKUFormat = config.ShopeeVariationSKUFormat;
                            variation.shopeeVariationSKUSeparator = config.ShopeeVariationSKUSeparator;

                            ShopeeVariations.Add(variation);
                        }
                    }

                    more = response.more;
                    request.pagination_offset += 100;
                }
            }
            catch
            {
                throw;
            }
            finally
            {
                GC.Collect();
            }
        }



        /// <summary>
        /// SHOPEE - ShopeeOnSaleVariations = Lấy danh sách sản phẩm được bán trên shopee ( có trong file Excel của product )
        /// </summary>
        public void GetShopeeOnlineItemList()
        {
            if (OnlineUPCs != null)
            {
                /* ===============================================================================================================
                Join sản phẩm trên Shopee với danh sách hàng được bán trong Excel
                =============================================================================================================== */

                var q = from p in ShopeeVariations
                        join o in OnlineUPCs on p.PrismUPC equals o.UPC
                        select new ShopeeVariation()
                        {
                            variation_id = p.variation_id,
                            variation_sku = p.variation_sku,
                            variation_name = p.variation_name,

                            item_id = p.item_id,
                            keep_offline = o.Qty,
                            stock = p.stock,

                            shopeeVariationSKUFormat = config.ShopeeVariationSKUFormat,
                            shopeeVariationSKUSeparator = config.ShopeeVariationSKUSeparator
                        };

                ShopeeOnSaleVariations = q.ToList<ShopeeVariation>();
            }
            else
            {
                ShopeeOnSaleVariations = ShopeeVariations.Distinct().ToList();
            }
        }



        /// <summary>
        /// SHOPEE - Lấy danh sách sản phẩm có trên sàn nhưng không được bán. Điều kiện lọc: stock > 0 & không nằm trong file Excel
        /// </summary>
        private void GetShopeeRemovedList()
        {
            ShopeeRemovedVariations = new List<ShopeeVariation>();

            if (OnlineUPCs != null)
            {
                var q = from p in ShopeeVariations
                        where p.stock > 0 && !OnlineUPCs.Any(o => p.PrismUPC == o.UPC)
                        select p;//float.Parse(o.UPC) == float.Parse(p.variation_sku)));

                ShopeeRemovedVariations = q.ToList();
            }
            else
            {
                // do nothing
            }
        }



        /// <summary>
        /// SHOPEE - Bỏ các sản phẩm không được bán -> cập nhật qty = 0
        /// </summary>
        /// <param name="pageSize">Tối đa 50</param>
        /// <returns></returns>
        private async Task RemoveShopeeSKUs(int pageSize = 50)
        {

            var variations = from v in ShopeeRemovedVariations
                             select new ShopeeVariation
                             {
                                 item_id = v.item_id,
                                 variation_id = v.variation_id,
                                 variation_name = v.variation_name,
                                 variation_sku = v.variation_sku,
                                 stock = 0,
                                 shopeeVariationSKUFormat = v.shopeeVariationSKUFormat,
                                 shopeeVariationSKUSeparator = v.shopeeVariationSKUSeparator
                             };

            ShopeeUpdateVariationsLog log = await CallShopeeVariationStockBatch(variations, pageSize);
        }



        /// <summary>
        /// SHOPEE - Đồng bộ stock từ Prism lên Shopee.
        /// </summary>
        /// <returns>Danh sách upc - on-hand prism - keep offline - stock Shopee</returns>
        private async Task<List<ShopeeVariation>> UpdateShopeeVariationStock(int pageSize = 50)
        {
            /* ========================================================================================================================
             * Join list upc với tồn kho - left join list upc để nếu tồn kho ko có thì update stock = 0
             * ======================================================================================================================== */
            var result = from v in ShopeeOnSaleVariations
                         join oh in PrismQtyOnHands on v.PrismUPC equals oh.UPC into gj
                         from _oh in gj.DefaultIfEmpty()
                         select new ShopeeVariation
                         {
                             item_id = v.item_id,
                             variation_id = v.variation_id,
                             variation_name = v.variation_name,
                             variation_sku = v.variation_sku,

                             Prism_on_hand = _oh?.QtyOnHand.Value ?? 0,
                             keep_offline = v.keep_offline,
                             stock = _oh?.QtyOnline ?? 0,

                             shopeeVariationSKUFormat = v.shopeeVariationSKUFormat,
                             shopeeVariationSKUSeparator = v.shopeeVariationSKUSeparator
                         };
            // Cập nhật stock online và lấy log các lần cập nhật
            ShopeeUpdateVariationsLog log = await CallShopeeVariationStockBatch(result);

            List<ShopeeVariation> updateResult = result.ToList();

            if (log != null)
            {
                foreach(ShopeeUpdateVariationsInfo info in log.Info)
                {
                    UpdateVariationStockBatchResponse response = info.Response;
                    foreach(UpdateVariationStockBatchFailure fail in response.batch_result.failures )
                    {
                        //updateResult.RemoveAll(i => i.item_id == fail.item_id);
                        GetItemDetailRequest itemDetailRequest = new GetItemDetailRequest(fail.item_id, config.ShopeeShopId, config.ShopeePartnerId);
                        GetItemDetailResponse itemDetailResponse = await ShopeeLib.GetItemDetail<GetItemDetailResponse, GetItemDetailRequest>(itemDetailRequest);
                        foreach( ShopeeVariation variation in itemDetailResponse.item.variations)
                        {
                            ShopeeVariation var = updateResult.FirstOrDefault(i => i.variation_id == variation.variation_id);
                            if (var != null)
                                var.stock = variation.stock;
                        }
                    }
                }
            }

            return updateResult;
        }



        /// <summary>
        /// SHOPEE - gọi SHOPEE API để update batch variations
        /// </summary>
        /// <param name="variations">array of data</param>
        /// <param name="pageSize">api limit page size - default:50</param>
        /// <returns></returns>
        private async Task<ShopeeUpdateVariationsLog> CallShopeeVariationStockBatch(IEnumerable<ShopeeVariation> variations, int pageSize = 50)
        {
            try
            {
                string functionName = "CallShopeeVariationStockBatch";// this.GetCurrentMethod();


                // Tính tổng số "page" cần update
                double totalPage = Math.Round((variations.Count() / pageSize) + 0.5);
                List<ShopeeVariation> errorList = new List<ShopeeVariation>();


                ShopeeUpdateVariationsLog log = new ShopeeUpdateVariationsLog()
                {
                    TotalVariations = variations.Count(),
                    PageSize = pageSize,
                    TotalPage = totalPage
                };

                int index = 0;
                while (index <= totalPage)
                {
                    var q = from v in variations.Skip(index * pageSize).Take(pageSize)
                            select v;

                    // Khỏi tạo thông tin log
                    ShopeeUpdateVariationsInfo info = new ShopeeUpdateVariationsInfo();
                    log.Info.Add(info);


                    //select new ShopeeVariation { item_id = v.item_id, variation_id = v.variation_id, variation_name = v.variation_name, variation_sku = v.variation_sku, stock = 0 };
                    UpdateVariationStockBatchRequest request = new UpdateVariationStockBatchRequest(q.ToArray(), config.ShopeeShopId, config.ShopeePartnerId);
                    info.Request = request;
                    info.RequestTime = DateTime.Now;


                    try
                    {
                        UpdateVariationStockBatchResponse response = await ShopeeLib.UpdateVariationStockBatch<UpdateVariationStockBatchResponse, UpdateVariationStockBatchRequest>(request);
                        info.Response = response;
                        info.ResponseTime = DateTime.Now;
                    }
                    catch
                    {

                    }

                    index += 1;
                }

                WriteLog("," + APIUtil.ConvertToJson(log));
                return log;
            }
            catch { return null; }
            finally
            {
                GC.Collect();
            }
        }



        /// <summary>
        /// SHOPEE - Create authorization URL 
        /// </summary>
        /// <param name="url"></param>
        /// <param name="id"></param>
        /// <param name="key"></param>
        /// <param name="redirect"></param>
        /// <returns></returns>
        public string GetShopeeAuthorizationURL(string url, string id, string key, string redirect)
        {
            return ShopeeLib.GetAuthorizationURL(url, id, key, redirect);
        }



        #endregion
    }
}
