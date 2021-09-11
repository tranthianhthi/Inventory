using OnlineInventory.Models;
using OnlineInventoryLib;
using OnlineInventoryLib.Lazada;
using OnlineInventoryLib.Lazada.Models;
using OnlineInventoryLib.Lazada.Responses;
using OnlineInventoryLib.Prism;
using OnlineInventoryLib.Prism.Models;
using OnlineInventoryLib.Shopee;
using OnlineInventoryLib.Shopee.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace OnlineInventory
{
    public partial class OnlineInventoryService
    {

        #region PROPERTIES


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

        readonly IShopeeLib ShopeeLib;
        private IConfigurations config;

        private string executionPath;

        readonly PrismLib PrismLib;
        readonly LazadaLib LazLib;


        private string datetimeFormat = "yyyy/MM/dd HH:mm:ss";


        private bool LazUseMultiWarehouse = false;

        private string LazWarehouseCode = "";

        /// <summary>
        /// LAZADA - Danh sách tất cả sản phẩm trên sàn
        /// </summary>
        private LazadaProduct[] LazProducts = new LazadaProduct[0];

        /// <summary>
        /// LAZADA - Danh sách sản phẩm trên sàn có trong file Excel và không trong danh sách ordered, có warehouse_code
        /// </summary>
        private LazadaProduct[] LazOnSaleProducts = new LazadaProduct[0];

        /// <summary>
        /// LAZADA - Danh sách sản phẩm trên sàn có trong file Excel và đang trong danh sách ordered
        /// </summary>
        private LazadaProduct[] LazOnOrderedProducts = new LazadaProduct[0];

        /// <summary>
        /// LAZADA - Danh sách sản phẩm trên sàn không có trong file Excel -> sẽ gán new qty = 0
        /// </summary>
        private LazadaProduct[] LazRemovedProducts = new LazadaProduct[0];

        /// <summary>
        /// Danh sách sản phẩn Lazada đang được đặt hàng - ở trạng thái chờ xử lý
        /// </summary>
        private List<LazadaOrderItem> LazOrderedItems = new List<LazadaOrderItem>();




        /// <summary>
        /// SHOPEE - Danh sách tất cả sản phẩm trên sàn
        /// </summary>
        private List<ShopeeVariation> ShopeeVariations = new List<ShopeeVariation>();

        /// <summary>
        /// SHOPEE - Danh sách sản phẩm trên sàn có trong file Excel
        /// </summary>
        public List<ShopeeVariation> ShopeeOnSaleVariations = new List<ShopeeVariation>();

        /// <summary>
        /// SHOPEE - Danh sách sản phẩm trên sàn không có trong file Excel -> gán new qty = 0
        /// </summary>
        private List<ShopeeVariation> ShopeeRemovedVariations = new List<ShopeeVariation>();


        /// <summary>
        /// Danh sách sản phẩm trong file Excel => được bán
        /// </summary>
        private List<PrismOnStore> OnlineUPCs = new List<PrismOnStore>();
        

        /// <summary>
        /// Số tồn trên Prism - đã join với danh sách hàng keep offline nếu có sử dụng file offline
        /// </summary>
        private List<PrismQtyOnHand> PrismQtyOnHands = new List<PrismQtyOnHand>();




        /// <summary>
        /// Danh sách sản phẩm Lazada - join với số on hand
        /// </summary>
        public List<LazadaSKU> LazadaSellerSKUs
        {
            get
            {
                List<LazadaSKU> lazSkus = new List<LazadaSKU>();
                foreach (LazadaProduct prod in LazOnSaleProducts)
                {
                    lazSkus.AddRange(prod.skus);
                }

                if (PrismQtyOnHands.Count == 0)
                {
                    // do nothing
                }
                else
                {

                    var q = lazSkus.Join(
                            PrismQtyOnHands,
                            l => l.SellerSku,
                            oh => oh.UPC,
                            (l, oh) => new LazadaSKU()
                            {
                                SellerSku = l.SellerSku,
                                Quantity = l.Quantity,
                                OfflineQty = l.OfflineQty,
                                PrismOHQty = oh.QtyOnHand.Value
                            }
                    );

                    lazSkus = q.ToList();
                }

                return lazSkus;

            }
        }


        #endregion



        public OnlineInventoryService(Configurations config, string executionPath)
        {
            this.config = config;
            this.cpoaConnectionString = string.Format(cpoaConnectionString, this.config.dbName);
            this.executionPath = executionPath;

            this.ShopeeLib = new ShopeeAPICommand(this.config.ShopeeDomain, this.config.ShopeeSecretKey);
            this.PrismLib = new PrismLib(this.config.pickupStore, this.cpoaConnectionString);
            this.LazLib = new LazadaLib(this.config.LazadaDomain, this.config.LazadaAppKey, this.config.LazadaAppSecret);

            this.LazUseMultiWarehouse = config.UseMultiWarehouse;
            this.LazWarehouseCode = config.WarehouseCode;
        }


        /// <summary>
        /// Khởi tạo dữ liệu
        /// Đọc file config upc
        /// Load dữ liệu từ sàn TMDT
        /// *** Lưu danh sách sản phẩm từ JSON vào file Text
        /// </summary>
        public async Task Init()
        {
            string action = "";
            try
            {
                if(config.UseKeepOfflineFile)
                {
                    action = "Read online UPC from Excel file";
                    ReadOnlineUPCs(executionPath + "\\" + config.OnlineFilePath);
                }
                else
                {
                    OnlineUPCs = null; //new List<PrismOnStore>();
                }
                // Đọc file Excel các sản phẩm được bán trên sàn
                

                if (config.IsLazadaStore)
                {
                    InitLazada();
                }

                if (config.IsShopeeStore)
                {
                    await InitShopee();
                }
            }
            catch(Exception ex)
            {
                
            }
        }



        /// <summary>
        /// Update online stock for Online Stores
        /// </summary>
        /// <param name="lastSync">Thời gian sync cuối cùng</param>
        public async Task UpdateOnlineStock(DateTime? lastSync = null)
        {
            /* =======================================================================================================================================
             * Lấy số tồn trên RetailPro ( lấy những thay đổi lần cuối cùng nếu lastSync != null )
             * Kiểm tra danh sách sản phẩm mới trên Laz
             * Kiểm tra danh sách sản phẩm đang có trong các đơn hàng chờ xử lý
             * 
             * =======================================================================================================================================*/

            // Lấy số tồn trên Prism
            PrismQtyOnHands = this.GetStoreOnHands(lastSync, OnlineUPCs);


            // Kiểm tra last sync. Nếu tham số last sync có dữ liệu => kiểm tra xem danh sách sản phẩm trên Laz có gì thay đổi không
            if (lastSync.HasValue)
            {
                LazadaProduct[] newProducts = CallLazadaAPIProduct("all", lastSync);
                LazProducts = LazProducts.Concat(newProducts.AsEnumerable()).ToArray();
            }

            if (PrismQtyOnHands.Count > 0)
            {
                try
                {
                    if (config.IsShopeeStore)
                    {
                        // Cập nhật số tồn
                        ShopeeOnSaleVariations =  await UpdateShopeeVariationStock();
                        //await CallShopeeAPIProduct();// true);
                        //GetShopeeOnlineItemList();
                    }

                    if (config.IsLazadaStore)
                    {
                        WriteLog("{" + Environment.NewLine + "\"start\":\"" + DateTime.Now.ToString(datetimeFormat) + "\"," + Environment.NewLine + "\"requests\":[" + Environment.NewLine);
                        UpdateLazadaStock(1);
                        WriteLog("]," + Environment.NewLine + "\"end\":\"" + DateTime.Now.ToString(datetimeFormat) + "\"" + Environment.NewLine + "}" + Environment.NewLine);

                        LazProducts = CallLazadaAPIProduct();
                        if (LazUseMultiWarehouse)
                        {

                        }
                        else
                        {
                            JoinLazProductsWithPrismList();
                        }
                    }

                }
                catch (Exception ex)
                {
                    WriteException(ex);
                }

            }
        }



        #region PRIVATE HELPERS


        [MethodImpl(MethodImplOptions.NoInlining)]
        public string GetCurrentMethod()
        {
            var st = new StackTrace();
            var sf = st.GetFrame(1);

            return sf.GetMethod().Name;
        }



        /// <summary>
        /// Đọc file Excel các upc được bán online.
        /// </summary>
        /// <param name="filePath">Đường dẫn file - gộp từ execute path & file name trong configuration</param>
        /// <returns></returns>
        private List<PrismOnStore> ReadOnlineUPCs(string filePath)
        {
            try
            {
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
                        OnlineUPCs.Add(new PrismOnStore(row[0].ToString(), int.Parse(row[1].ToString())));
                    }

                    return OnlineUPCs;
                }
                else
                {
                    return null;
                }
            }
            catch { throw; }
        }

        


        /// <summary>
        /// Khởi tạo dữ liệu bán hàng trên Shopee
        /// </summary>
        /// <returns></returns>
        private async Task InitShopee()
        {
            string action = "";
            try
            {
                // Lấy danh sách sản phẩm trên Shopee
                action = "SHOPEE: load all Products";
                await CallShopeeAPIProduct();

                // Lấy danh sách các sản phẩm sẽ remove 
                action = "SHOPEE: Get removed Products";
                GetShopeeRemovedList();

                // Remove sản phẩm nếu danh sách có item
                action = "SHOPEE: Remove Products";
                if (ShopeeRemovedVariations.Count > 0)
                {
                    await RemoveShopeeSKUs();
                }

                action = "SHOPEE: Get on sale products";
                GetShopeeOnlineItemList();
            }
            catch (Exception ex)
            {
                WriteLog("{\"start\":\"" + DateTime.Now.ToString(datetimeFormat) + "\",\"action\":\"Init - " + action + "\",\"Exception\":\"" + ex.ToString() + "\"}");
            }
        }



        /// <summary>
        /// Khởi tạo dữ liệu bán hàng trên Lazada. Hàm này chỉ chạy lần đầu tiên khi 
        /// </summary>
        private void InitLazada()
        {
            string action = "";


            /* ========================================================================================================================
             * Xử lý chung cho cả 2 trường hợp Multi warehouse - Single warehouse 
             * Refresh token
             * Lấy danh sách sản phẩm đang bán trên Laz
             * Update các sản phẩm không có trong file excel : qty = 0 
             * Nếu không sử dụng file KeepOffline
               ========================================================================================================================*/

            try
            {
                // Refresh token
                action = "LAZADA: refresh Token";
                AccessTokenResponse tokenResponse = LazLib.RefreshToken(config.LazadaRefreshToken);
                config.UpdateLazadaAccessToken(tokenResponse.access_token);


                // tải danh sách sản phẩn trên Lazada 
                action = "LAZADA: load all Products";
                LazProducts = CallLazadaAPIProduct();


                // so sánh với file Excel -> lấy ra danh sách sản phẩm sẽ ngừng bán
                action = "LAZADA: Get remove products";
                GetLazadaRemoveProducts();

                // cập nhật qty = 0 cho các sản phẩm sẽ ngừng bán
                action = "LAZADA: Remove Products";
                WriteLog("{" + Environment.NewLine + "\"start\":\"" + DateTime.Now.ToString(datetimeFormat) + "\",\"action\":\"" + action + "\"," + Environment.NewLine + "\"requests\":[" + Environment.NewLine);
                RemoveLazadaProducts(1);
                WriteLog("]," + Environment.NewLine + "\"end\":\"" + DateTime.Now.ToString(datetimeFormat) + "\"," + Environment.NewLine + "}" + Environment.NewLine);

                if (LazUseMultiWarehouse)
                {
                    InitLazadaMultiWarehouse();
                }
                else
                {
                    InitLazadaSingleWarehouse();
                }

            }
            catch (Exception ex) { }

        }


        /// <summary>
        /// LAZADA - khởi tạo dữ liệu khi sử dụng 1 kho pickup
        /// </summary>
        private void InitLazadaSingleWarehouse()
        {
            string action = "";

            try
            {
                // so sánh với file Excel -> lấy ra danh sách sản phẩm được bán & số lượng stock tối thiểu tại cửa hàng
                action = "LAZADA: Get on sale products";
                JoinLazProductsWithPrismList();
            }
            catch (Exception ex)
            {
                WriteLog("{\"start\":\"" + DateTime.Now.ToString(datetimeFormat) + "\",\"action\":\"Init - " + action + "\",\"Exception\":\"" + ex.ToString() + "\"}");
            }
        }



        /// <summary>
        /// LAZADA - khởi tạo dữ liệu khi sử dụng nhiều kho pickup
        /// </summary>
        private void InitLazadaMultiWarehouse()
        {
            string action = "";

            try
            {
                // so sánh với file Excel -> lấy ra danh sách sản phẩm được bán & số lượng stock tối thiểu tại cửa hàng
                action = "LAZADA: Get on sale products";
                JoinLazProductsWithPrismList();
            }
            catch (Exception ex)
            {
                WriteLog("{\"start\":\"" + DateTime.Now.ToString(datetimeFormat) + "\",\"action\":\"Init - " + action + "\",\"Exception\":\"" + ex.ToString() + "\"}");
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



        private void WriteException(Exception ex)
        {
            WriteLog("{" + Environment.NewLine + "\"start\":\"" + DateTime.Now.ToString(datetimeFormat) + "\"," + Environment.NewLine + "\"requests\":\"" + ex.ToString() + "\"}" + Environment.NewLine);
        }



        #endregion



        #region PRISM



        /// <summary>
        /// PRISM - Get qty on hand of all UPCs of current store
        /// </summary>
        /// <returns></returns>
        public List<PrismQtyOnHand> GetStoreOnHands(DateTime? lastSyncDateTime, List<PrismOnStore> onlineUPCs = null)
        {
            try
            {
                List<PrismQtyOnHand> onHands = PrismLib.GetQtyOnHands(lastSyncDateTime);

                if (onlineUPCs != null)
                {
                    var result = onHands.Join(
                            onlineUPCs,
                            oh => oh.UPC,
                            on => on.UPC,
                            (oh, on) => new PrismQtyOnHand()
                            {
                                UPC = oh.UPC,
                                QtyOnHand = oh.QtyOnHand,
                                keep_offline = on.Qty,
                                QtyOnline = (( oh.QtyOnHand ?? 0 ) < on.Qty) ? 0 : ((oh.QtyOnHand ?? 0) - on.Qty)
                            }
                        ) ;

                    onHands = result.ToList();
                }
                else
                {
                    var result = from oh in onHands
                                 select new PrismQtyOnHand()
                                 {
                                     UPC = oh.UPC,
                                     QtyOnHand = oh.QtyOnHand,
                                     QtyOnline = oh.QtyOnHand.HasValue? oh.QtyOnHand.Value : 0
                                 };

                    onHands = result.ToList();
                }

                return onHands;
            }
            catch { throw; }
        }



        #endregion



    }
}
