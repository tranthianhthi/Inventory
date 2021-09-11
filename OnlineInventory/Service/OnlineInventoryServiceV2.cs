using OnlineInventoryLib;
using OnlineInventoryLib.Interfaces;
using OnlineInventoryLib.Lazada;
using OnlineInventoryLib.Lazada.Models;
using OnlineInventoryLib.Lazada.Responses;
using OnlineInventoryLib.Prism;
using OnlineInventoryLib.Prism.Models;
using OnlineInventoryLib.Shopee;
using OnlineInventoryLib.Shopee.Models;
using OnlineInventoryLib.Shopee.Requests;
using OnlineInventoryLib.Shopee.Responses;
using OnlineInventoryLib.Util;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineInventory
{
    public partial class OnlineInventoryServiceV2
    {
        readonly string cpoaConnectionString = "Data Source=" +
            "(" +
            "DESCRIPTION=" +
            "(" +
            "ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST={0})(PORT=1521))" +
            ")" +
            "(" +
            "CONNECT_DATA=(SERVICE_NAME=rproods)" +
            ")" +
            ");" +
            "User Id=reportuser;Password=report;Connection Timeout=999";

        private OnlineStoreAuthenticate storeAuthenticate;
        private Online_store_Lazada lazadaAuth;
        private Online_Store_Shopee shopeeAuth;
        private readonly string executionPath;

        /// <summary>
        /// Custom list sản phẩm & stock theo brand
        /// </summary>
        private Dictionary<string, int> ExcelUPCs = new Dictionary<string, int>();

        private const string datetimeFormat = "yyyy/MM/dd HH:mm:ss";


        /// <summary>
        /// DLL lấy data từ Prism
        /// </summary>
        public PrismLib PrismLib { get; private set; }

        #region Lazada

        /// <summary>
        /// DLL của Lazada
        /// </summary>
        private LazadaLib LazLib;

        /// <summary>LAZADA - Danh sách tất cả sản phẩm trên sàn</summary>
        private LazadaProduct[] LazProducts = new LazadaProduct[0];

        private readonly IConfigurations config;

        #endregion
                

        private IShopeeLib ShopeeLib;

        public List<ShopeeVariation> ShopeeVariations { get; set; }
        public List<LazadaSKU> LazadaSKU { get; set; }

        //private bool LazUseMultiWarehouse = false;

        //private string LazWarehouseCode = "";


        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="config"></param>
        /// <param name="executionPath"></param>
        public OnlineInventoryServiceV2(Configurations config, string executionPath)
        {
            this.config = config;
            this.cpoaConnectionString = string.Format(cpoaConnectionString, this.config.dbName);
            this.executionPath = executionPath;

            PrismLib = new PrismLib(this.cpoaConnectionString);

            //this.LazUseMultiWarehouse = config.UseMultiWarehouse;
            //this.LazWarehouseCode = config.WarehouseCode;
        }



        /// <summary>
        /// Init các task
        /// </summary>
        /* ===================================================================================================================================================================
        * 1. Kiểm tra store có active trên DB hay không
        *       không active 
        *           * return false
        *       có active
        *           * Khởi tạo các obj sync theo thông tin trên db
        *           * Kiểm tra & xử lý custom list theo config đã có
        *           * xử lý tùy vào platform
        * =================================================================================================================================================================== */
        public async Task<bool> Init()
        {
            try
            {
                string action = "";
                // 1. Kiểm tra active
                CheckActive();

                // 2. Không active => return
                if (storeAuthenticate == null || !storeAuthenticate.ActiveStore)
                {
                    return false;
                }

                action = "Read online UPC from Excel file";
                LoadCustomList();

                if (config.IsLazadaStore)
                {
                    InitLazada(); 
                }

                // khởi tạo thông tin SHOPEE
                if (config.IsShopeeStore)
                {
                    await InitShopee(storeAuthenticate.ShopId, storeAuthenticate.AppKey);   
                    //ShopeeVariations.RemoveAll(v => removed.Contains(v));
                }
                return true;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        /// <summary>
        /// Kiểm tra thông tin store có active không trên DB. Nếu store active thì lấy tiếp các thông tin cấu hình khác.
        /// </summary>
        private void CheckActive()
        {
            try
            {
                if (config.IsLazadaStore)
                {
                    storeAuthenticate = PrismLib.CheckActive(this.config.pickupStore, "lazada");
                    if (storeAuthenticate.ActiveStore)
                    {
                        lazadaAuth = PrismLib.GetLazadaAuthenticate(this.config.pickupStore);
                    }
                }
                if (config.IsShopeeStore)
                {
                    storeAuthenticate = PrismLib.CheckActiveShopee(this.config.pickupStore, "shopee");
                    if (storeAuthenticate.ActiveStore)
                    {

                    }
                }


            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        /// <summary>
        /// Xử lý custom list theo thông tin từ configuration - chung cho các platform sử dụng danh sách sp theo brand
        /// </summary>
        /* ===================================================================================================================================================================
         * Nếu UseCustomList = true
         *      * Nếu UseLocalFile = true
         *          Đọc file Excel vào dictionary
         *      * Nếu UseLocalFile = false
         *          Đọc db vào dictionary
         * Nếu UseCustomList = false
         *      List = Empty dictionary
         * ===================================================================================================================================================================*/
        private void LoadCustomList()
        {
            try
            {
                if (storeAuthenticate.UseCustomList)
                {
                    if (storeAuthenticate.UseLocalFile)
                    {
                        ReadLocalFile();
                    }
                    else
                    {
                        ReadOnlineFile();
                    }
                }
                else
                {
                    ExcelUPCs = new Dictionary<string, int>();
                }
            }
            catch { throw; }
        }


        /// <summary>
        /// Đọc custom list từ file Excel ở local
        /// </summary>
        private void ReadLocalFile()
        {
            try
            {
                //string filePath = executionPath + "\\" + config.OnlineFilePath; //local
                string filePath = @"X:\" + config.OnlineFilePath;  //server 10.20
                if (File.Exists(filePath))
                {
                    string connectionString =
                        "Provider=Microsoft.ACE.OLEDB.12.0;" +
                        @"Data Source='" + filePath +
                        "';Extended Properties=\"Excel 12.0;HDR=YES;\"";
                    OleDbConnection connection = new OleDbConnection(connectionString);
                    OleDbCommand cmd = new OleDbCommand("SELECT * FROM [Sheet1$]", connection);
                    DataTable tb = new DataTable();
                    OleDbDataAdapter da = new OleDbDataAdapter(cmd);
                    da.Fill(tb);

                    foreach (DataRow row in tb.Rows)
                    {
                        ExcelUPCs.Add(row[0].ToString(), int.Parse(row[1].ToString()));
                    }
                }
            }
            catch
            {
               
            }
        }


        /// <summary>
        /// Đọc custom list từ database online
        /// </summary>
        private void ReadOnlineFile()
        {
            try
            {
                ExcelUPCs = PrismLib.LoadCustomListFromDB();
            }
            catch(Exception ex)
            {

            }
        }

 
        /// <summary>
        /// Sync stock lên platform online
        /// </summary>
        /// <param name="lastSync"></param>
        /// <returns></returns>
        public async Task UpdateOnlineStock(DateTime? lastSync = null)
        {
            if (storeAuthenticate == null || !storeAuthenticate.ActiveStore)
                return;

            try
            {
                if (this.storeAuthenticate.IsLazada)
                {
                    UpdateLazadaSKUs(); // lazadaAuth.LazadaMultiWH, lazadaAuth.LazadaWHCode );
                    LazProducts = CallLazadaAPIProduct();
                    LazadaSKU = MergeLazWithExcel(LazProducts);
                }
                else if (this.config.EnableShopee)
                {
                    await UpdateShopeeVariationsStock(ShopeeVariations);
                }

            }
            catch { }
        }

        /// <summary>
        /// Lấy on hand từ Prism, xử lý cả trường hợp quét tồn 1 store lẫn trường hợp quét tồn nhiều stores.
        /// </summary>
        /// <returns></returns>
        private Dictionary<string, int> GetPrismOH(DateTime? modifiedDate)
        {
            Dictionary<string, int> result;
            if (storeAuthenticate.UseMultiStoresInventory)
            {
                result = PrismLib.GetMultiOnhands(storeAuthenticate.StoreList, modifiedDate);
            }
            else
            {
                result = PrismLib.GetOnhands(storeAuthenticate.StoreCode, modifiedDate);
            }
            
            return result;
        }


        /// <summary>
        /// Tính stock để sync lên platform.
        /// </summary>
        /// <param name="PrismOHQty">OnHand</param>
        /// <param name="ExcelQty">Qty trong Excel file</param>
        /// <param name="isReservedStock">Reserved Stock hay Sellable stock. True: Reserved - False: Sellable</param>
        /// <returns></returns>
        /*
         * Nếu là reserved stock
         *      OnHand < Reserved => Online = 0
         *      OnHand > Reserved => Online = OnHand - Reserved
         * Nếu là sellable stock
         *      OnHand < Sellable => OnHand
         *      OnHand > Sellable => Sellable
         */
        private int CalculateOHQty(int PrismOHQty, int ExcelQty, bool isReservedStock = true)
        {
            if (isReservedStock)
            {
                return PrismOHQty < ExcelQty ? 0 : PrismOHQty - ExcelQty;
            }
            else
            {
                return PrismOHQty < ExcelQty ? PrismOHQty : ExcelQty;
            }

        }

        private void WriteLog(string log)
        {
            string fileName = Environment.CurrentDirectory + "\\Log\\" + DateTime.Now.ToString("yyyyMMdd") + ".log";

            using (FileStream fstream = new FileStream(fileName, FileMode.Append, FileAccess.Write))
            {
                StreamWriter streamWriter = new StreamWriter(fstream);
                streamWriter.Write(log);
                streamWriter.Flush();
                streamWriter.Close();
                fstream.Close();
            }
        }


        #region SHOPEE

        private async Task InitShopee(string shopid, string appkey)
        {
            ShopeeLib = new ShopeeAPICommand(storeAuthenticate.BaseURL, storeAuthenticate.AppSecret);
            ShopeeVariations = await CallShopeeAPIProduct(shopid, appkey);
            List<ShopeeVariation> removed = await RemoveShopeeSKUs(ShopeeVariations);
        }

        /// <summary>
        /// Remove các UPC không được bán
        /// </summary>
        /// <param name="ShopeeVariations"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        private async Task<List<ShopeeVariation>> RemoveShopeeSKUs(List<ShopeeVariation> ShopeeVariations, int pageSize = 50)
        {
            if (ExcelUPCs.Count > 0)
            {
                var q = from v in ShopeeVariations
                        where v.stock > 0 && !ExcelUPCs.ContainsKey(v.PrismUPC)
                        select new ShopeeVariation()
                        {
                            item_id = v.item_id,
                            variation_id = v.variation_id,
                            variation_name = v.variation_name,
                            variation_sku = v.variation_sku,
                            stock = 0,
                            shopeeVariationSKUFormat = v.shopeeVariationSKUFormat,
                            shopeeVariationSKUSeparator = v.shopeeVariationSKUSeparator
                        };

                ShopeeUpdateVariationsLog log = await CallShopeeVariationStockBatch(q, pageSize);
                return q.ToList();
            }
            else
            {
                return ShopeeVariations;
            }     
        }

        /// <summary>
        /// Tính toán tồn & gọi hàm cập nhật
        /// </summary>
        /// <param name="ShopeeVariations"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        private async Task UpdateShopeeVariationsStock(List<ShopeeVariation> ShopeeVariations, int pageSize = 50, bool isReservedStock = true )
        {
            try
            {
                Dictionary<string, int> PrismOH = GetPrismOH(null);//this.PrismLib.GetOnhands(null);
                var q = from v in ShopeeVariations
                        where ExcelUPCs.ContainsKey(v.PrismUPC)
                        select new ShopeeVariation()
                        {
                            item_id = v.item_id,
                            variation_id = v.variation_id,
                            variation_name = v.variation_name,
                            variation_sku = v.variation_sku,
                            stock = PrismOH.ContainsKey(v.PrismUPC)? ( CalculateOHQty( PrismOH[v.PrismUPC] , ExcelUPCs[v.PrismUPC], isReservedStock) ) : 0,
                            shopeeVariationSKUFormat = v.shopeeVariationSKUFormat,
                            shopeeVariationSKUSeparator = v.shopeeVariationSKUSeparator
                        };
                ShopeeUpdateVariationsLog log = await CallShopeeVariationStockBatch(q, pageSize);
            }
            catch(Exception ex)
            {

            }
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
                    UpdateVariationStockBatchRequest request = new UpdateVariationStockBatchRequest(q.ToArray(), shopeeAuth.ShopID, int.Parse(shopeeAuth.AppKey));
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
        /// Lấy danh sách order chưa xử lý trên Shopee
        /// </summary>
        /// <returns></returns>
        private async Task GetShopeeOrders()
        {
            GetOrdersByStatusRequest getOrdersByStatusRequest;
            GetOrdersListResponse ordersListResponse;
            List<ShopeeOrder> orders = new List<ShopeeOrder>();
            try
            {
                getOrdersByStatusRequest = new GetOrdersByStatusRequest("UNPAID", shopeeAuth.ShopID, int.Parse(shopeeAuth.AppKey));
                ordersListResponse = await ShopeeLib.GetOrdersByStatus<GetOrdersListResponse, GetOrdersByStatusRequest>(getOrdersByStatusRequest);
                orders.AddRange(ordersListResponse.orders);
                getOrdersByStatusRequest = new GetOrdersByStatusRequest("READY_TO_SHIP", shopeeAuth.ShopID, int.Parse(shopeeAuth.AppKey));
                ordersListResponse = await ShopeeLib.GetOrdersByStatus<GetOrdersListResponse, GetOrdersByStatusRequest>(getOrdersByStatusRequest);
                orders.AddRange(ordersListResponse.orders);
            }
            catch
            {

            }
        }


        /// <summary>
        /// Gọi API của Shopee để lấy product detail ( danh sách variations )
        /// </summary>
        /// <param name="item_id">Mã product trên shopee ( 1 product Shopee có nhiều variations, 1 variation = 1 upc )</param>
        /// <returns></returns>
        public async Task<ShopeeVariation[]> CallShopeeAPIProductDetail(ulong item_id)
        {
            try
            {
                GetItemDetailRequest itemDetailRequest = new GetItemDetailRequest(item_id, shopeeAuth.ShopID, int.Parse(shopeeAuth.AppKey));
                GetItemDetailResponse itemDetailResponse = await ShopeeLib.GetItemDetail<GetItemDetailResponse, GetItemDetailRequest>(itemDetailRequest);
                return itemDetailResponse.item.variations;
            }
            catch { throw; }
        }

        /// <summary>
        /// SHOPEE - lấy danh sách tất cả sản phẩm đang có trên Shopee
        /// </summary>
        /// <param name="getDetail">Check if need to get more detail of products</param>
        public async Task<List<ShopeeVariation>> CallShopeeAPIProduct(string shopid, string appkey,bool getDetail = false)
        {
            List<ShopeeVariation> ShopeeVariations = new List<ShopeeVariation>();

            /*===========================================================
            /* Gọi API get item
            /*===========================================================*/

                // Tạo request
                //GetItemListRequest request = new GetItemListRequest(shopeeAuth.ShopID, int.Parse(shopeeAuth.AppKey), 0, 0, 0, 100); cũ của Vũ
                GetItemListRequest request = new GetItemListRequest(int.Parse(shopid), int.Parse(appkey), 0, 0, 0, 100);
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
                            variation.shopeeVariationSKUFormat = shopeeAuth.VariationSkuFormat;
                            variation.shopeeVariationSKUSeparator = shopeeAuth.VariationSkuSeparator;

                            ShopeeVariations.Add(variation);
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


        #region Lazada

        /// <summary>
        /// Khởi tạo lazada object
        /// </summary>
        /* ===================================================================================================================================================================
         * Refresh lazada token
         * Lấy danh sách sản phẩm active trên lazada
         * Nếu sử dụng danh sách sản phẩm riêng theo brand
         *      * Set stock các sku ngoài danh sách = 0
           =================================================================================================================================================================== */
        private void InitLazada()
        {
            LazLib = new LazadaLib(storeAuthenticate.BaseURL, storeAuthenticate.AppKey, storeAuthenticate.AppSecret);
            InitLazadaToken();
            LazProducts = CallLazadaAPIProduct();

            if (lazadaAuth.UseCustomList)
            {
                RemoveLazadaSKUs();
            }
            else
            {

            }
        }

        /// <summary>
        /// Lấy Access token & Refresh token mới cho Lazada store
        /// </summary>
        private void InitLazadaToken()
        {
            try
            {
                if (lazadaAuth.AuthenticateDate < DateTime.Today.Date)
                { 
                    AccessTokenResponse tokenResponse = LazLib.RefreshToken(lazadaAuth.LazadaRefreshToken);

                    lazadaAuth.AuthenticateDate = DateTime.Today.Date;
                    lazadaAuth.LazadaAccessToken = tokenResponse.access_token;
                    lazadaAuth.LazadaRefreshToken = tokenResponse.refresh_token;

                    PrismLib.UpdateAuthenticate(lazadaAuth);
                }
            }
            catch (Exception ex) { throw; }
        }

        /// <summary>
        /// Remove các sản phẩm có trên Lazada mà không có trên file excel ( sử dụng cho file reserved stock )
        /// </summary>
        /// <param name="isMultiWarehouse">Sử dụng multi warehouse trên Lazada. True: có - False: không</param>
        /// <param name="warehouseCode">Mã warehouse default trên Lazada - default là dropshipping. Nếu isMultiWarehouse = False thì không quan trọng.</param>
        private void RemoveLazadaSKUs()//bool isMultiWarehouse, string warehouseCode)
        {
            if (ExcelUPCs.Count > 0)
            {
                try
                {
                    IEnumerable<LazadaProduct> products = null ;
                    products = from p in LazProducts
                               select new LazadaProduct()
                               {
                                   skus =
                                       (
                                           from s in p.skus
                                           where !ExcelUPCs.ContainsKey(s.SellerSku)
                                           select new LazadaSKU()
                                           {
                                               SellerSku = s.SellerSku,
                                               NewQuantity = 0,
                                               UseMultiWarehouse = lazadaAuth.LazadaMultiWH,//isMultiWarehouse, // gán giá trị sử dụng multi warehouse
                                               WarehouseCode = lazadaAuth.LazadaWHCode//warehouseCode // gán warehouse code
                                           }
                                       ).ToArray()
                               };
                    products = products.Where(e => e.skus.Length > 0);
                    // Goi API update
                    UpdateLazadaProductStock(products, 1);
                }
                catch
                {

                }
            }
        }

        /// <summary>
        /// Lazada : Hàm cập nhật stock cho các sku. Chỉ chuẩn bị data & gọi UpdateLazadaProductStock để update stock
        /// </summary>
        /// <param name="isMultiWarehouse"></param>
        /// <param name="warehouseCode"></param>
        /// <param name="pageSize"></param>
        /// <param name="isReservedStock"></param>
        private void UpdateLazadaSKUs() //bool isMultiWarehouse, string warehouseCode, int pageSize = 50, bool isReservedStock = true )
        {
            try
            {
                Dictionary<string, int> PrismOH = GetPrismOH(null);// this.PrismLib.GetOnhands(null);
                IEnumerable<LazadaProduct> q;

                if (ExcelUPCs.Count > 0)
                {
                    q = from p in LazProducts
                        select new LazadaProduct()
                        {
                            skus =
                                (
                                    from s in p.skus
                                    where ExcelUPCs.ContainsKey(s.SellerSku)
                                    select new LazadaSKU()
                                    {
                                        SellerSku = s.SellerSku,
                                        NewQuantity = PrismOH.ContainsKey(s.SellerSku) ? (CalculateOHQty(PrismOH[s.SellerSku], ExcelUPCs[s.SellerSku], lazadaAuth.IsReservedStock)) : 0, //isReservedStock)) : 0,
                                        OfflineQty = ExcelUPCs[s.SellerSku],
                                        UseMultiWarehouse = lazadaAuth.LazadaMultiWH, // isMultiWarehouse, // gán giá trị sử dụng multi warehouse
                                        WarehouseCode = lazadaAuth.LazadaWHCode//warehouseCode // gán warehouse code
                                    }
                                ).ToArray()
                        };

                    q = q.Where(e => e.skus.Length > 0);

                    
                }
                else
                {
                    q = from p in LazProducts
                            select new LazadaProduct()
                            {
                                skus =
                                    (
                                        from s in p.skus
                                        select new LazadaSKU()
                                        {
                                            SellerSku = s.SellerSku,
                                            NewQuantity = PrismOH.ContainsKey(s.SellerSku) ? CalculateOHQty(PrismOH[s.SellerSku], 0, lazadaAuth.IsReservedStock) : 0, // isReservedStock) : 0,
                                            OfflineQty = 0,
                                            UseMultiWarehouse = lazadaAuth.LazadaMultiWH, // isMultiWarehouse, // gán giá trị sử dụng multi warehouse
                                            WarehouseCode = lazadaAuth.LazadaWHCode //warehouseCode // gán warehouse code
                                        }
                                    ).ToArray()
                            };
                }
                // Goi API update
                UpdateLazadaProductStock(q, 1);
            }
            catch
            {

            }

            return;
        }

        /// <summary>
        /// Lazada : Hàm gọi API cập nhật
        /// </summary>
        /// <param name="q"></param>
        /// <param name="pageSize"></param>
        private void UpdateLazadaProductStock(IEnumerable<LazadaProduct> q, int pageSize = 50)
        {
            if (q is null)
                return;

            int count = 0;
            int total = q.Count();

            while (count < total)
            {
                string payload = "<Request>{0}</Request>";
                string productArray = "";

                LazadaProduct[] prods = q.Skip(count).Take(pageSize).ToArray();

                foreach (LazadaProduct prod in prods)
                {
                    string productXML = prod.GenerateUpdateProductQuantity();
                    productArray += productXML;
                    //WriteLog(prod.GenerateProductLogString());
                }
                payload = string.Format(payload, productArray);

                try
                {
                    LazLib.UpdateStock(lazadaAuth.LazadaAccessToken, payload);
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
                LazadaGetProductData getProductResponse = LazLib.GetProducts(lazadaAuth.LazadaAccessToken, offset.ToString(), config.PageSize.ToString(), lazFilter, create_after);
                LazTotalProd = getProductResponse.total_products;

                while (totalProd < LazTotalProd)//(getProductResponse.products != null && getProductResponse.products.Length > 0)
                {
                    LazProducts = LazProducts.Concat(getProductResponse.products).ToArray();

                    totalProd += getProductResponse.CountProducts();
                    offset += getProductResponse.CountTotalSKUs();

                    if (totalProd < LazTotalProd)
                        getProductResponse = LazLib.GetProducts(lazadaAuth.LazadaAccessToken, totalProd.ToString(), config.PageSize.ToString(), lazFilter);
                }

                return LazProducts;
            }
            catch { throw; }
        }


        /// <summary>
        /// Join danh sách sản phẩm trên Lazada với file Excel để lấy ID sản phẩm trên Lazada
        /// </summary>
        /// <param name="LazProducts"></param>
        /// <returns></returns>
        private List<LazadaSKU> MergeLazWithExcel(LazadaProduct[] LazProducts)
        {
            List<LazadaSKU> q = new List<LazadaSKU>();
            
            foreach(LazadaProduct p in LazProducts)
            {
                if (ExcelUPCs.Count > 0)
                {
                    q.AddRange(p.skus.Where(s => ExcelUPCs.ContainsKey(s.SellerSku)));
                }
                else
                {
                    q.AddRange(p.skus);
                }
            }
            return q.ToList();
        }
        #endregion
    }
}
