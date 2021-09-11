using OnlineInventoryLib.Interfaces;
using OnlineInventoryLib.Prism.Models;
using OnlineInventoryLib.Shopee.Models;
using OnlineInventoryLib.Shopee.Requests;
using OnlineInventoryLib.Shopee.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineInventoryLib.Shopee.Library
{
    public class ShopeeService : IShopeeService
    {
        readonly IShopeeLib ShopeeLib;
        int ShopID;
        int PartnerID;
        string ShopeeVariationSKUFormat;
        string ShopeeVariationSKUSeparator;



        public ShopeeService(string domain, string secret, int shopID, int partnerID, string skuFormat, string skuSeparator)
        {
            ShopeeLib = new ShopeeAPICommand(domain, secret);
            ShopID = shopID;
            PartnerID = partnerID;
            ShopeeVariationSKUSeparator = skuSeparator;
            ShopeeVariationSKUFormat = skuFormat;
        }



        /// <summary>
        /// SHOPEE - Cập nhật tồn kho
        /// </summary>
        /// <param name="onlineUPCs">Danh sách sản phẩm trong file Excel</param>
        /// <param name="pageSize">Số variation mỗi lần update</param>
        /// <param name="prismQtyOnHands">Data on hand trên prism</param>
        public async Task<ShopeeUpdateVariationsLog> UpdateShopeeVariationStock(List<PrismQtyOnHand> prismQtyOnHands, int pageSize = 50, List<OnlineUPC> onlineUPCs = null)
        {

            List<ShopeeVariation> variations = await this.CallShopeeAPIProduct(false);
            variations = this.JoinOnlineListWithLocalFile(variations, onlineUPCs, true);

            /* ========================================================================================================================
             * Join list upc với tồn kho
             * ======================================================================================================================== */
            var result = from v in variations
                         join oh in prismQtyOnHands on v.PrismUPC equals oh.UPC
                         select new ShopeeVariation
                         {
                             item_id = v.item_id,
                             variation_id = v.variation_id,
                             variation_name = v.variation_name,
                             variation_sku = v.variation_sku,

                             Prism_on_hand = oh.QtyOnHand.Value,
                             keep_offline = oh.keep_offline,
                             stock = oh.QtyOnline,

                             shopeeVariationSKUFormat = v.shopeeVariationSKUFormat,
                             shopeeVariationSKUSeparator = v.shopeeVariationSKUSeparator
                         };

            //return await CallShopeeVariationStockBatch(result);
            return await CallAsyncShopeeVariationStockBatches(result, pageSize);
        }



        /// <summary>
        /// SHOPEE - Set stock = 0 với các variation ( UPC ) không có trong file Excel
        /// </summary>
        /// <param name="onlineUPCs">Danh sách UPCs trong file Excel</param>
        /// <param name="pageSize">Số record mỗi lần update</param>
        /// <returns></returns>
        public async Task<ShopeeUpdateVariationsLog> RemoveVariations(List<OnlineUPC> onlineUPCs, int pageSize = 50)
        {
            List<ShopeeVariation> variations = await this.CallShopeeAPIProduct(false);
            variations = this.JoinOnlineListWithLocalFile(variations, onlineUPCs, false);

            var result = from v in variations
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

            //return await CallShopeeVariationStockBatch(result, pageSize);
            return await CallAsyncShopeeVariationStockBatches(result, pageSize);
        }



        /// <summary>
        /// So sánh danh sách hàng online với danh sách upc trên file excel
        /// </summary>
        /// <param name="shopeeVariations">Danh sách sp online</param>
        /// <param name="contains">Data có trong file excel hay không. True: có - False: không</param>
        /// <param name="onlineUPCs"></param>
        /// <returns></returns>
        private List<ShopeeVariation> JoinOnlineListWithLocalFile(List<ShopeeVariation> shopeeVariations, List<OnlineUPC> onlineUPCs = null, bool contains = true)
        {
            if (onlineUPCs != null)
            {
                if (contains)
                {
                    return
                        (
                            from v in shopeeVariations.ToList<ShopeeVariation>()
                            join o in onlineUPCs on v.PrismUPC equals o.UPC
                            select new ShopeeVariation()
                            {
                                variation_id = v.variation_id,
                                variation_sku = v.variation_sku,
                                variation_name = v.variation_name,

                                item_id = v.item_id,
                                keep_offline = o.Qty,
                                stock = v.stock,

                                shopeeVariationSKUFormat = ShopeeVariationSKUFormat,
                                shopeeVariationSKUSeparator = ShopeeVariationSKUSeparator
                            }
                    ).ToList(); //.ToArray();
                }
                else
                {
                    return shopeeVariations.ToList<ShopeeVariation>().Where(v => !onlineUPCs.Any(o => o.UPC == v.PrismUPC)).ToList();
                      
                }
            }
            return shopeeVariations;
        }



        /// <summary>
        /// SHOPEE - lấy danh sách tất cả sản phẩm đang có trên Shopee
        /// </summary>
        /// <param name="getDetail">Lấy chi tiết ( stock ). True: có - False: không</param>
        private async Task<List<ShopeeVariation>> CallShopeeAPIProduct(bool getDetail = false)
        {
            List<ShopeeVariation> ShopeeVariations = new List<ShopeeVariation>();

            /*===========================================================
            /* Gọi API get item
            /*===========================================================*/

            // Tạo request
            GetItemListRequest request = new GetItemListRequest(ShopID, PartnerID, 0, 0, 0, 100);
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
                            GetItemDetailRequest itemDetailRequest = new GetItemDetailRequest(item.item_id, ShopID, PartnerID);
                            GetItemDetailResponse itemDetailResponse = await ShopeeLib.GetItemDetail<GetItemDetailResponse, GetItemDetailRequest>(itemDetailRequest);
                            variations = itemDetailResponse.item.variations;
                        }

                        foreach (ShopeeVariation variation in variations) //itemDetailResponse.item.variations)
                        {
                            ShopeeVariations.Add(new ShopeeVariation()
                            {
                                item_id = item.item_id,
                                variation_id = variation.variation_id,
                                variation_sku = variation.variation_sku,
                                variation_name = variation.variation_name,
                                stock = variation.stock,
                                shopeeVariationSKUFormat = ShopeeVariationSKUFormat,
                                shopeeVariationSKUSeparator = ShopeeVariationSKUSeparator
                            });
                        }
                    }

                    more = response.more;
                    request.pagination_offset += 100;
                }
                return ShopeeVariations;
            }
            catch
            {
                throw;
            }

        }



        #region ASYNC

        /// <summary>
        /// Hàm gọi API của Shopee để cập nhật stock nhiều variation - trả về log info
        /// </summary>
        /// <param name="q">Array ShopeeVariation - có qty</param>
        /// <returns></returns>
        private async Task<ShopeeUpdateVariationsInfo> CallShopeeVariationStockBatch(ShopeeVariation[] q)
        {
            // Khỏi tạo thông tin log
            ShopeeUpdateVariationsInfo info = new ShopeeUpdateVariationsInfo();

            UpdateVariationStockBatchRequest request = new UpdateVariationStockBatchRequest(q, ShopID, PartnerID);
            info.Request = request;
            info.RequestTime = DateTime.Now;

            UpdateVariationStockBatchResponse response = await ShopeeLib.UpdateVariationStockBatch<UpdateVariationStockBatchResponse, UpdateVariationStockBatchRequest>(request);
            info.Response = response;
            info.ResponseTime = DateTime.Now;

            return info;
        }

        /// <summary>
        /// ASYNC - gọi SHOPEE API để update batch variations
        /// </summary>
        /// <param name="variations">array of data</param>
        /// <param name="pageSize">api limit page size - default:50</param>
        /// <returns></returns>
        private async Task<ShopeeUpdateVariationsLog> CallAsyncShopeeVariationStockBatches(IEnumerable<ShopeeVariation> variations, int pageSize = 50)
        {
            // Tính tổng số "page" cần update
            double totalPage = Math.Round((variations.Count() / pageSize) + 0.5);
            List<ShopeeVariation> errorList = new List<ShopeeVariation>();
            List<Task<ShopeeUpdateVariationsInfo>> updateTasks = new List<Task<ShopeeUpdateVariationsInfo>>();

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

                updateTasks.Add(CallShopeeVariationStockBatch(q.ToArray()));

                index += 1;
            }

            while (updateTasks.Any())
            {
                Task<ShopeeUpdateVariationsInfo> finishedTask = await Task.WhenAny(updateTasks);
                updateTasks.Remove(finishedTask);
                log.Info.Add(await finishedTask);
            }

            return log;
        }

        #endregion


    }
}
