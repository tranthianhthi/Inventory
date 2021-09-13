using OnlineInventoryLib.Prism.Models;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Data;
using System.Globalization;

namespace OnlineInventoryLib.Prism
{
    /*=========================================================================================================================================================================
         * Mục đích : Centralized các cấu hình store để không phải config mỗi khi đổi máy hoặc đổi store pickup sẽ không ảnh hưởng nhiều
         * Bảng Online_Store_App : Lưu thông tin app ( platform, id, secret key, api url )
         * Bảng Online_Store_setting : Lưu thông tin pickup store ( store, platform, có sử dụng custom list để sync stock không, có sử dụng multi store inven hay không ... )
         * Bảng Online_Store_Lazada : Lưu thông tin riêng cho Lazada ( sử dụng multi WH hay không, WH code là gì...
         * Bảng Online_store_Shopee : Lưu thông tin riêng cho Shopee.
     =========================================================================================================================================================================*/

    public partial class PrismLib
    {
        private string GetShopeeAuthCmd =
            "Select shp.Setting_Id as Id , " +
                    "shp.Variation_SKU_Format, " +
                    "shp.Variation_SKU_Separator, " + 
                    "shp.ShopID, " + 
                    "St.platform, " +
                    "st.STORE_CODE, " +
                    "St.Is_Active As Activestore, " +
                    "St.Use_Custom_List, " +
                    "Case When St.Use_Custom_List = 'false' Then 'false' Else Is_Reserved_Stock End As Is_Reserved_Stock, " +
                    "Case When St.Use_Custom_List = 'false' Then 'false' Else Use_Local_File End As Use_Local_File, " +
                    "St.Use_Multi_Stores_Inventory, " +
                    "st.store_list, " +
                    "App.App_Id As App_Key, " +
                    "app.app_key as App_Secret, " +
                    "App.App_URL  " +
            "From Online_Store_Shopee shp " +
            "Inner Join Online_Store_Setting St On shp.Setting_Id = St.Id " +
            "Inner Join Online_Store_App App On Upper(St.Platform) = Upper(App.Platform) " +
            "Where St.Store_Code = :storeCode  " +
            "AND Upper(st.platform) = Upper( :platform )";

        private string GetLazadaAuthCmd =
            "Select " +
                    "Lzd.Setting_Id as Id ,  " +
                    "LAZADAACCESSTOKEN, " +
                    "LAZADAREFRESHTOKEN, " +
                    "to_char(Authenticatedate, 'yyyymmdd') as AuthenticateDate, " +
                    "Lazada_Multi_Wh, " +
                    "lazada_wh_code, " +
                    "St.platform, " +
                    "st.STORE_CODE, " +
                    "St.Is_Active As Activestore, " +
                    "St.Use_Custom_List, " +
                    "Case When St.Use_Custom_List = 'false' Then 'false' Else Is_Reserved_Stock End As Is_Reserved_Stock, " +
                    "Case When St.Use_Custom_List = 'false' Then 'false' Else Use_Local_File End As Use_Local_File, " +
                    "St.Use_Multi_Stores_Inventory, " +
                    "st.store_list, " +
                    "App.App_Id As App_Key, " +
                    "app.app_key as App_Secret, " +
                    "App.App_URL  " +
            "From Online_Store_Lazada Lzd " +
            "Inner Join Online_Store_Setting St On Lzd.Setting_Id = St.Id " +
            "Inner Join Online_Store_App App On Upper(St.Platform) = Upper(App.Platform) " +
            "Where St.Store_Code = :storeCode  " +
            "AND Upper(st.platform) = Upper( :platform )";

        //for LZD
        private string CheckActiveCmd =
            "SELECT st.id, " +
                    "st.store_code, " +
                    "st.platform, " +
                    "st.is_active as Activestore, " +
                    "st.Use_Custom_List, " +
                    "st.Is_Reserved_Stock, " +
                    "st.Use_Local_File, " +
                    "st.Use_Multi_Stores_Inventory, " +
                    "st.store_list, " +
                    "App.App_Id As App_Key, " +
                    "app.app_key as App_Secret, " +
                    "App.App_URL  " +
            "FROM   Online_Store_Setting st " +
            "INNER JOIN Online_Store_App App On Upper(st.Platform) = Upper(App.Platform) " +
           "WHERE  Store_Code = :storeCode " +
            "AND    Upper(st.Platform) = Upper(:platform) ";
        //for Shopee
        private string CheckActiveCmdShopee =
            "SELECT st.id, shp.shop_id," +
                    "st.store_code, " +
                    "st.platform, " +
                    "st.is_active as Activestore, " +
                    "st.Use_Custom_List, " +
                    "st.Is_Reserved_Stock, " +
                    "st.Use_Local_File, " +
                    "st.Use_Multi_Stores_Inventory, " +
                    "st.store_list, " +
                    "App.App_Id As App_Key, " +
                    "app.app_key as App_Secret, " +
                    "App.App_URL  " +
            "FROM   Online_Store_Setting st " +
            "INNER JOIN Online_Store_App App On Upper(st.Platform) = Upper(App.Platform) " +
            " inner join online_store_shopee shp on shp.setting_id=St.ID " +
            "WHERE  Store_Code = :storeCode " +
            "AND    Upper(st.Platform) = Upper(:platform) ";


        #region Tiki

        private string GetTikiAuthCmd =
           "SELECT tiki.Setting_Id as ID, tiki.ApplicationID " +
           ",tiki.Secret,tiki.expire_in,tiki.AccessToken " +
           ",St.platform, st.STORE_CODE, St.Is_Active As Activestore " +
           ",St.Use_Custom_List , to_char(Authenticatedate, 'yyyymmdd') as AuthenticateDate " +
           ",Case When St.Use_Custom_List = 'false' Then 'false' Else Is_Reserved_Stock End As Is_Reserved_Stock " +
           ",Case When St.Use_Custom_List = 'false' Then 'false' Else Use_Local_File End As Use_Local_File, St.Use_Multi_Stores_Inventory " +
           ",st.store_list, App.App_Id As App_Key " +
           ",app.app_key as App_Secret, App.App_URL  " +
           "FROM Online_Store_Tiki tiki Inner Join Online_Store_Setting St  " +
           "On tiki.Setting_Id = St.Id Inner Join Online_Store_App App  " +
           "On Upper(St.Platform) = Upper(App.Platform) " +
           "Where St.Store_Code = :storeCode  AND Upper(st.platform) = Upper(:platform)";

        private string UpdateTikiAuthenticateCmd =
          "UPDATE ONLINE_STORE_TIKI " +  // Online_Store_Authenticate
          "SET ACCESSTOKEN = :TikiAccesstoken, EXPIRE_IN = :TikiExpireIn, Authenticatedate = TO_DATE( :Authenticatedate, 'yyyymmdd') " +
          "WHERE setting_id = :id";

        private string CheckActiveCmdTiki =
            "SELECT st.id, sht.sellerid," +
    "st.store_code, " +
    "st.platform, " +
    "st.is_active as Activestore, " +
    "st.Use_Custom_List, " +
    "st.Is_Reserved_Stock, " +
    "st.Use_Local_File, " +
    "st.Use_Multi_Stores_Inventory, " +
    "st.store_list, " +
    "App.App_Id As App_Key, " +
    "app.app_key as App_Secret, " +
    "App.App_URL  " +
    "FROM   Online_Store_Setting st " +
    "INNER JOIN Online_Store_App App On Upper(st.Platform) = Upper(App.Platform) " +
    "inner join online_store_tiki sht on sht.setting_id=St.ID " +
    "WHERE  Store_Code = :storeCode " +
    "AND    Upper(st.Platform) = Upper(:platform)";


        public Online_store_Tiki GetTikiAuthenticate(string pickupStore)
        {
            Online_store_Tiki tikAuth = null;
            DataTable tb = new DataTable();

            try
            {
                tb = QueryData(GetTikiAuthCmd, new string[] { ":storeCode", ":platform" }, new object[] { pickupStore, "tiki" });

                if (tb.Rows.Count > 0)
                {
                    DataRow row = tb.Rows[0];

                    tikAuth = new Online_store_Tiki()
                    {
                        // bảng app
                        ID = int.Parse(row["id"].ToString()),
                        AppKey = row["App_key"].ToString(),
                        AppSecret = row["App_secret"].ToString(),
                        AppURL = row["App_url"].ToString(),
                        Platform = row["platform"].ToString(),

                        // bảng setting
                        StoreCode = row["store_code"].ToString(),
                        ActiveStore = bool.Parse(row["ActiveStore"].ToString()),
                        UseCustomList = bool.Parse(row["Use_custom_list"].ToString()),
                        UseLocalFile = bool.Parse(row["use_local_file"].ToString()),
                        IsReservedStock = bool.Parse(row["is_reserved_stock"].ToString()),
                        UseMultiStoresInventory = bool.Parse(row["use_multi_stores_inventory"].ToString()),
                        StoreList = row["store_list"].ToString(),

                        // bảng lazada auth
                        //LazadaAccessToken = row["LazadaAccessToken"].ToString(),
                        //LazadaRefreshToken = row["LazadaRefreshToken"].ToString(),
                        ApplicationID = row["ApplicationID"] == DBNull.Value ? null : row["ApplicationID"].ToString(),
                        AccessToken = row["AccessToken"] == DBNull.Value ? null : row["AccessToken"].ToString(),
                        AccessTokenSecret = row["Secret"] == DBNull.Value ? null : row["Secret"].ToString(),
                        AuthenticateDate = row["AuthenticateDate"] == DBNull.Value ? DateTime.Now : DateTime.ParseExact(row["AuthenticateDate"].ToString(), "yyyyMMdd", CultureInfo.InvariantCulture),
                        ExpiresIn = row["expire_in"] == DBNull.Value ? 0 : int.Parse(row["expire_in"].ToString()),
                        //LazadaMultiWH = bool.Parse(row["Lazada_multi_WH"].ToString()),
                        //LazadaWHCode = row["Lazada_WH_Code"].ToString()

                    };
                }
                return tikAuth;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public bool UpdateAuthenticate(Online_store_Tiki storeAuthenticate)
        {
            OracleConnection connection = new OracleConnection(connectionString);
            OracleTransaction trans;
            OracleCommand command = new OracleCommand(UpdateTikiAuthenticateCmd, connection);

            string dateVal = storeAuthenticate.AuthenticateDate.ToString("yyyyMMdd");

            command.Parameters.Add(new OracleParameter(":TikiAccesstoken", storeAuthenticate.AccessToken));
            command.Parameters.Add(new OracleParameter(":TikiExpireIn", storeAuthenticate.ExpiresIn));
            command.Parameters.Add(new OracleParameter(":Authenticatedate", dateVal));
            command.Parameters.Add(new OracleParameter(":id", storeAuthenticate.ID));

            try
            {
                connection.Open();
                trans = connection.BeginTransaction(IsolationLevel.ReadCommitted);
                command.Transaction = trans;
                int result = command.ExecuteNonQuery();
                trans.Commit();
                connection.Close();

                return result == 1;
            }
            catch (Exception ex)
            {
                return false;
            }
            finally
            {
                GC.Collect();
            }
        }

        #endregion

        private string UpdateAuthenticateCmd =
            "UPDATE Online_Store_lazada " +  // Online_Store_Authenticate
            "SET Lazadaaccesstoken = :Lazadaaccesstoken, LazadarefreshToken = :LazadarefreshToken, Authenticatedate = TO_DATE( :Authenticatedate, 'yyyymmdd') " +
            "WHERE setting_id = :id";

        private string CreateAuthenticateCmd =
            "INSERT INTO Online_store_Authenticate ( store_code, platform, LazadaAccessToken, LazadaRefreshToken, AuthenticateDate, activeStore ) " +
            "VALUES ( :storeCode, :platform, ";

       

        public OnlineStoreAuthenticate CheckActive(string pickupStore, string platform)
        {
            OnlineStoreAuthenticate onlineStoreAuthenticate = null;
            DataTable tb = new DataTable();           

            try
            {
                tb = QueryData(CheckActiveCmd, new string[] { ":storeCode", ":platform" }, new object[] { pickupStore, platform });

                if (tb.Rows.Count > 0)
                {
                    //string temp;
                    DataRow row = tb.Rows[0];
                    //temp = row["shop_id"].ToString()!=null ? row["shop_id"].ToString() : null;

                    onlineStoreAuthenticate = new OnlineStoreAuthenticate()
                    {                      

                        ID = int.Parse(row["id"].ToString()), // setting id
                        StoreCode = row["store_code"].ToString(),
                        Platform = row["platform"].ToString(),
                        BaseURL = row["app_url"].ToString(),
                        AppKey = row["app_key"].ToString(),
                        AppSecret = row["app_secret"].ToString(),
                        //ShopId = temp,                        
                        ActiveStore = bool.Parse(row["ActiveStore"].ToString()),
                        UseCustomList = bool.Parse(row["Use_custom_list"].ToString()),
                        UseLocalFile = bool.Parse(row["use_local_file"].ToString()),
                        IsReservedStock = bool.Parse(row["is_reserved_stock"].ToString()),
                        UseMultiStoresInventory = bool.Parse(row["use_multi_stores_inventory"].ToString()),
                        StoreList = row["store_list"].ToString()
                    };
                }
            }
            catch (Exception ex)
            {

            }

            return onlineStoreAuthenticate;
        }

        public OnlineStoreAuthenticate CheckActiveShopee(string pickupStore, string platform)
        {
            OnlineStoreAuthenticate onlineStoreAuthenticate = null;
            DataTable tb = new DataTable();

            try
            {
                tb = QueryData(CheckActiveCmdShopee, new string[] { ":storeCode", ":platform" }, new object[] { pickupStore, platform });

                if (tb.Rows.Count > 0)
                {
                    string temp;
                    DataRow row = tb.Rows[0];
                    temp = row["shop_id"].ToString() != null ? row["shop_id"].ToString() : null;

                    onlineStoreAuthenticate = new OnlineStoreAuthenticate()
                    {

                        ID = int.Parse(row["id"].ToString()), // setting id
                        StoreCode = row["store_code"].ToString(),
                        Platform = row["platform"].ToString(),
                        BaseURL = row["app_url"].ToString(),
                        AppKey = row["app_key"].ToString(),
                        AppSecret = row["app_secret"].ToString(),
                        ShopId = temp,
                        ActiveStore = bool.Parse(row["ActiveStore"].ToString()),
                        UseCustomList = bool.Parse(row["Use_custom_list"].ToString()),
                        UseLocalFile = bool.Parse(row["use_local_file"].ToString()),
                        IsReservedStock = bool.Parse(row["is_reserved_stock"].ToString()),
                        UseMultiStoresInventory = bool.Parse(row["use_multi_stores_inventory"].ToString()),
                        StoreList = row["store_list"].ToString()
                    };
                }
            }
            catch (Exception ex)
            {

            }

            return onlineStoreAuthenticate;
        }

        public OnlineStoreAuthenticate CheckActiveTiki(string pickupStore, string platform)
        {
            OnlineStoreAuthenticate onlineStoreAuthenticate = null;
            DataTable tb = new DataTable();

            try
            {
                tb = QueryData(CheckActiveCmdTiki, new string[] { ":storeCode", ":platform" }, new object[] { pickupStore, platform });

                if (tb.Rows.Count > 0)
                {
                    string temp;
                    DataRow row = tb.Rows[0];
                    temp = row["sellerid"].ToString() != null ? row["sellerid"].ToString() : null;

                    onlineStoreAuthenticate = new OnlineStoreAuthenticate()
                    {

                        ID = int.Parse(row["id"].ToString()), // setting id
                        StoreCode = row["store_code"].ToString(),
                        Platform = row["platform"].ToString(),
                        BaseURL = row["app_url"].ToString(),
                        AppKey = row["app_key"].ToString(),
                        AppSecret = row["app_secret"].ToString(),
                        ShopId = temp,
                        ActiveStore = bool.Parse(row["ActiveStore"].ToString()),
                        UseCustomList = bool.Parse(row["Use_custom_list"].ToString()),
                        UseLocalFile = bool.Parse(row["use_local_file"].ToString()),
                        IsReservedStock = bool.Parse(row["is_reserved_stock"].ToString()),
                        UseMultiStoresInventory = bool.Parse(row["use_multi_stores_inventory"].ToString()),
                        StoreList = row["store_list"].ToString()
                    };
                }
            }
            catch (Exception ex)
            {

            }

            return onlineStoreAuthenticate;
        }

        public Online_Store_Shopee GetShopeeAuthenticate(string pickupStore)
        {
            Online_Store_Shopee result = null;
            DataTable tb = new DataTable();

            try
            {
                tb = QueryData(GetShopeeAuthCmd, new string[] { ":storeCode", ":platform" }, new object[] { pickupStore, "shopee" });
                if (tb.Rows.Count == 1)
                {
                    DataRow row = tb.Rows[0];
                    result = new Online_Store_Shopee()
                    {
                        // bảng app
                        ID = int.Parse(row["id"].ToString()),
                        PartnerID = row["App_key"].ToString(),
                        SecretKey = row["App_secret"].ToString(),
                        AppURL = row["App_url"].ToString(),
                        Platform = row["platform"].ToString(),

                        // bảng setting
                        StoreCode = row["store_code"].ToString(),
                        ActiveStore = bool.Parse(row["ActiveStore"].ToString()),
                        UseCustomList = bool.Parse(row["Use_custom_list"].ToString()),
                        UseLocalFile = bool.Parse(row["use_local_file"].ToString()),
                        IsReservedStock = bool.Parse(row["is_reserved_stock"].ToString()),
                        UseMultiStoresInventory = bool.Parse(row["use_multi_stores_inventory"].ToString()),
                        StoreList = row["store_list"].ToString(),

                        // bảng shopee
                        ShopID = int.Parse(row["shop_id"].ToString()),
                        VariationSkuFormat = row["variation_sku_format"].ToString(),
                        VariationSkuSeparator = row["variation_sku_separator"].ToString()
                    };
                }
                return result;
            }
            catch
            {
                throw;
            }
                
        }

        public Online_store_Lazada GetLazadaAuthenticate(string pickupStore)
        {
            Online_store_Lazada lzdAuth = null;
            DataTable tb = new DataTable();
            
            try
            {
                tb = QueryData(GetLazadaAuthCmd, new string[] { ":storeCode", ":platform" }, new object[] { pickupStore, "lazada" });
                
                if (tb.Rows.Count > 0)
                {
                    DataRow row = tb.Rows[0];

                    lzdAuth = new Online_store_Lazada()
                    {
                        // bảng app
                        ID = int.Parse(row["id"].ToString()),
                        AppKey = row["App_key"].ToString(),
                        AppSecret = row["App_secret"].ToString(),
                        AppURL = row["App_url"].ToString(),
                        Platform = row["platform"].ToString(),

                        // bảng setting
                        StoreCode = row["store_code"].ToString(),
                        ActiveStore = bool.Parse(row["ActiveStore"].ToString()),
                        UseCustomList = bool.Parse(row["Use_custom_list"].ToString()),
                        UseLocalFile = bool.Parse(row["use_local_file"].ToString()),
                        IsReservedStock = bool.Parse(row["is_reserved_stock"].ToString()),
                        UseMultiStoresInventory = bool.Parse(row["use_multi_stores_inventory"].ToString()),
                        StoreList = row["store_list"].ToString(),

                        // bảng lazada auth
                        LazadaAccessToken = row["LazadaAccessToken"].ToString(),
                        LazadaRefreshToken = row["LazadaRefreshToken"].ToString(),
                        AuthenticateDate = DateTime.ParseExact(row["AuthenticateDate"].ToString(), "yyyyMMdd", CultureInfo.InvariantCulture),
                        LazadaMultiWH = bool.Parse(row["Lazada_multi_WH"].ToString()),
                        LazadaWHCode = row["Lazada_WH_Code"].ToString()
                    };
                }
                return lzdAuth;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public bool UpdateAuthenticate(Online_store_Lazada storeAuthenticate)
        {
            OracleConnection connection = new OracleConnection(connectionString);
            OracleTransaction trans;
            OracleCommand command = new OracleCommand(UpdateAuthenticateCmd, connection);

            string dateVal = storeAuthenticate.AuthenticateDate.ToString("yyyyMMdd");

            command.Parameters.Add(new OracleParameter(":Lazadaaccesstoken", storeAuthenticate.LazadaAccessToken));
            command.Parameters.Add(new OracleParameter(":LazadarefreshToken", storeAuthenticate.LazadaRefreshToken));
            command.Parameters.Add(new OracleParameter(":Authenticatedate", dateVal));
            command.Parameters.Add(new OracleParameter(":id", storeAuthenticate.ID));


            try
            {
                connection.Open();
                trans = connection.BeginTransaction(IsolationLevel.ReadCommitted);
                command.Transaction = trans;
                int result = command.ExecuteNonQuery();
                trans.Commit();
                connection.Close();

                return result == 1;
            }
            catch (Exception ex)
            {
                return false;
            }
            finally
            {
                GC.Collect();
            }
        }

        public bool CreateAuthenticate(Online_store_Lazada storeAuthenticate)
        {
            string dateVal = storeAuthenticate.AuthenticateDate.ToString("yyyyMMdd");

            try
            {
                int result = ExecuteCommand(UpdateAuthenticateCmd, new string[] { ":Lazadaaccesstoken", ":LazadarefreshToken", ":Authenticatedate" }, new object[] { storeAuthenticate.LazadaAccessToken, storeAuthenticate.LazadaRefreshToken, dateVal });

                return result == 1;
            }
            catch
            {
                return false;
            }
        }

    }
}