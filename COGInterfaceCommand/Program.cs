using System;
using System.Configuration;
using COGInterfaceCommand.Command;



namespace COGInterfaceCommand
{
    class Program
    {
        static string URL = ConfigurationManager.AppSettings["AuthenticationURL"]; //"https://login.microsoftonline.com/ab872c56-a5b0-452c-85fb-a87fa1350088/oauth2/token";
        static string ClientId = ConfigurationManager.AppSettings["AuthenticationID"];
        static string ClientSecret = ConfigurationManager.AppSettings["AuthenticationSecret"];
        static string Resource = ConfigurationManager.AppSettings["AuthenticationResource"];

        static string APIURL = ConfigurationManager.AppSettings["APIURL"];
        static string PrimaryKey = ConfigurationManager.AppSettings["PrimarySubcriptionKey"];
        static string SecondaryKey = ConfigurationManager.AppSettings["SecondarySubcriptionKey"];
        static string Licensee = ConfigurationManager.AppSettings["Licensee"];

        static string SFTP = ConfigurationManager.AppSettings["SftpURL"];
        static string SFTPUser = ConfigurationManager.AppSettings["SftpID"];
        static string SFTPPassword = ConfigurationManager.AppSettings["SftpSecret"];
        static int SftpPort = int.Parse(ConfigurationManager.AppSettings["SftpPort"]);

        static string COGDownloadDirectory = ConfigurationManager.AppSettings["SftpDownload"];
        static string COGUploadDirectory = ConfigurationManager.AppSettings["SftpUpload"];

        static string LocalDownloadDirectory = ConfigurationManager.AppSettings["ClientDownloadFolder"];
        static string LocalUploadDirectory = ConfigurationManager.AppSettings["ClientUploadFolder"];
        static string LocalBackupDirectory = ConfigurationManager.AppSettings["ClientBackupFolder"];
        static string cogBrandList = ConfigurationManager.AppSettings["COGBrandList"];


        /* =======================================================================================================
         * A: ASN
         *      No parameter:       Get data from API	
         *      String (filePath):  Re-process offline file 
         * B: Barcode
         *      No parameter:       Get file from COG
         *      String (filePath):  Re-process offline file 
         * I: Item
         *      No parameter:       Get file from COG
         *      String (filePath):  Re-process offline file 
         * H: Hierarchy
         *      No parameter:       Get file from COG
         *      String (filePath):  Re-process offline file 
         * P: Pricing
         *      No parameter:       Get file from COG
         *      String (filePath):  Re-process offline file 
         * No Para: get ALL from COG
         * ======================================================================================================= */

        static void Main(string[] args)
        {
            try
            {
                if (args.Length == 0)
                {
                    ASNCommand aCommand = new ASNCommand(URL, ClientId, ClientSecret, Resource, APIURL, PrimaryKey, SecondaryKey, Licensee);
                    aCommand.GetASN();
                    BarcodeCommand bCommand = new BarcodeCommand(SFTP, SFTPUser, SFTPPassword, SftpPort, LocalBackupDirectory, COGUploadDirectory, COGDownloadDirectory, LocalDownloadDirectory, LocalUploadDirectory);
                    bCommand.GetBarcodes();
                    ItemCommand iCommand = new ItemCommand(SFTP, SFTPUser, SFTPPassword, SftpPort, LocalBackupDirectory, COGUploadDirectory, COGDownloadDirectory, LocalDownloadDirectory, LocalUploadDirectory);
                    iCommand.GetItems();
                    HierarchyCommand hCommand = new HierarchyCommand(SFTP, SFTPUser, SFTPPassword, SftpPort, LocalBackupDirectory, COGUploadDirectory, COGDownloadDirectory, LocalDownloadDirectory, LocalUploadDirectory);
                    hCommand.GetHierarchyItems();
                    PricingCommand pCommand = new PricingCommand(SFTP, SFTPUser, SFTPPassword, SftpPort, LocalBackupDirectory, COGUploadDirectory, COGDownloadDirectory, LocalDownloadDirectory, LocalUploadDirectory);
                    pCommand.GetPricingItems();                      
                    //TransferReceiptCommand transferReceiptCommand = new TransferReceiptCommand(URL, ClientId, ClientSecret, Resource, APIURL, PrimaryKey, SecondaryKey, Licensee);
                    //transferReceiptCommand.TransferReceiptMain();
                   
                    //SalesCommand salesCommand = new SalesCommand(SFTP, SFTPUser, SFTPPassword, SftpPort, "FileBackUp", "Inbound", "Outbound", "Download", "FileUpload");
                    //salesCommand.GetSales();
                   
                    //SalesReconciliationCommand reconciliationCommand = new SalesReconciliationCommand(SFTP, SFTPUser, SFTPPassword, SftpPort, "FileBackUp", "Inbound", "Outbound", "Download", "FileUpload");
                    //reconciliationCommand.GetSales();  
                    
                    //StockReconciliationCommand stockReconciliationCommand = new StockReconciliationCommand(SFTP, SFTPUser, SFTPPassword, SftpPort, "FileBackUp", "Inbound", "Outbound", "Download", "FileUpload");
                    //stockReconciliationCommand.GetSales();
                    
                }
                else if (args.Length == 1)
                {
                    switch (args[0])
                    {
                        case "A":
                            ASNCommand aCommand = new ASNCommand(URL, ClientId, ClientSecret, Resource, APIURL, PrimaryKey, SecondaryKey, Licensee);
                            aCommand.GetASN();                            
                            break;
                        case "B":
                            BarcodeCommand bCommand = new BarcodeCommand(SFTP, SFTPUser, SFTPPassword, SftpPort, LocalBackupDirectory, COGUploadDirectory, COGDownloadDirectory, LocalDownloadDirectory, LocalUploadDirectory);
                            bCommand.GetBarcodes();
                            break;
                        case "I":
                            ItemCommand iCommand = new ItemCommand(SFTP, SFTPUser, SFTPPassword, SftpPort, LocalBackupDirectory, COGUploadDirectory, COGDownloadDirectory, LocalDownloadDirectory, LocalUploadDirectory);
                            iCommand.GetItems();
                            break;
                        case "H":
                            HierarchyCommand hCommand = new HierarchyCommand(SFTP, SFTPUser, SFTPPassword, SftpPort, LocalBackupDirectory, COGUploadDirectory, COGDownloadDirectory, LocalDownloadDirectory, LocalUploadDirectory);
                            hCommand.GetHierarchyItems();
                            break;
                        case "P":
                            PricingCommand pCommand = new PricingCommand(SFTP, SFTPUser, SFTPPassword, SftpPort, LocalBackupDirectory, COGUploadDirectory, COGDownloadDirectory, LocalDownloadDirectory, LocalUploadDirectory);
                            pCommand.GetPricingItems();
                            break;
                        case "TR":
                            TransferReceiptCommand transferReceiptCommand= new TransferReceiptCommand(URL, ClientId, ClientSecret, Resource, APIURL, PrimaryKey, SecondaryKey, Licensee);
                            transferReceiptCommand.SubmitWH2StoreStransfer(DateTime.Now.ToString("MM/dd/yyyy"), "-", cogBrandList);
                            break;
                        case "SA":
                            SalesCommand salesCommand = new SalesCommand(SFTP, SFTPUser, SFTPPassword, SftpPort, "FileBackUp", "Inbound/Sales/", "Outbound", "Download", "FileUpload");
                            salesCommand.GetSales();
                            break;
                        case "SARE":
                            SalesCommand salesCommandRemian = new SalesCommand(SFTP, SFTPUser, SFTPPassword, SftpPort, "FileBackUp", "Inbound/Sales/", "Outbound", "Download", "FileUpload");
                            salesCommandRemian.GetSalesRemain();
                            break;
                        case "SRE":
                            SalesReconciliationCommand reconciliationCommand = new SalesReconciliationCommand(SFTP, SFTPUser, SFTPPassword, SftpPort, "FileBackUp", "Inbound/SalesRecon/", "Outbound", "Download", "FileUpload");
                            reconciliationCommand.GetSales();
                            break;                      
                        case "STRE":
                            StockReconciliationCommand stockReconciliationCommand = new StockReconciliationCommand(SFTP, SFTPUser, SFTPPassword, SftpPort, "FileBackUp", "Inbound/StoreSOHRecon/", "Outbound", "Download", "FileUpload");
                            stockReconciliationCommand.GetSales();
                            break;                       
                        case "STS":
                            StoreTransferCommand storeTransferCommand = new StoreTransferCommand(URL, ClientId, ClientSecret, Resource, APIURL, PrimaryKey, SecondaryKey, Licensee);
                            storeTransferCommand.SubmitStoreStransfer(DateTime.Now.ToString("MM/dd/yyyy"), "-");
                            break;
                        case "STSREC":
                            StoreTransferCommand storeTransferReceiptCommand = new StoreTransferCommand(URL, ClientId, ClientSecret, Resource, APIURL, PrimaryKey, SecondaryKey, Licensee);
                            storeTransferReceiptCommand.SubmitStoreStransferReceipt(DateTime.Now.ToString("MM/dd/yyyy"), "-");
                            break;
                        case "ADJ":
                            StockAdjustCommand stockAdjustCommand = new StockAdjustCommand(URL, ClientId, ClientSecret, Resource, APIURL, PrimaryKey, SecondaryKey, Licensee);
                            stockAdjustCommand.SubmitStoreStransferAdjust(DateTime.Now.ToString("MM/dd/yyyy"), "-");
                            break;
                        default:
                            Console.WriteLine("Invalid command.");
                            break;
                    }
                }
                else if (args.Length == 2)
                {
                    string fileName = args[1];
                    switch (args[0])
                    {
                        case "A":
                            ASNCommand aCommand = new ASNCommand(URL, ClientId, ClientSecret, Resource, APIURL, PrimaryKey, SecondaryKey, Licensee);
                            aCommand.GetASN(fileName);                           
                            break;
                        case "B":
                            BarcodeCommand bCommand = new BarcodeCommand(SFTP, SFTPUser, SFTPPassword, SftpPort, LocalBackupDirectory, COGUploadDirectory, COGDownloadDirectory, LocalDownloadDirectory, LocalUploadDirectory);
                            bCommand.GetBarcodes(fileName);
                            break;
                        case "I":
                            ItemCommand iCommand = new ItemCommand(SFTP, SFTPUser, SFTPPassword, SftpPort, LocalBackupDirectory, COGUploadDirectory, COGDownloadDirectory, LocalDownloadDirectory, LocalUploadDirectory);
                            iCommand.GetItems(fileName);
                            break;
                        case "H":
                            HierarchyCommand hCommand = new HierarchyCommand(SFTP, SFTPUser, SFTPPassword, SftpPort, LocalBackupDirectory, COGUploadDirectory, COGDownloadDirectory, LocalDownloadDirectory, LocalUploadDirectory);
                            hCommand.GetHierarchyItems(fileName);
                            break;
                        case "P":
                            PricingCommand pCommand = new PricingCommand(SFTP, SFTPUser, SFTPPassword, SftpPort, LocalBackupDirectory, COGUploadDirectory, COGDownloadDirectory, LocalDownloadDirectory, LocalUploadDirectory);
                            pCommand.GetPricingItems(fileName);
                            break;
                        case "TR":
                            TransferReceiptCommand transferReceiptCommand = new TransferReceiptCommand(URL, ClientId, ClientSecret, Resource, APIURL, PrimaryKey, SecondaryKey, Licensee);
                            transferReceiptCommand.SubmitWH2StoreStransfer(DateTime.Now.ToString("MM/dd/yyyy"), "-", cogBrandList);
                            break;
                        case "SA":
                            SalesCommand salesCommand = new SalesCommand(SFTP, SFTPUser, SFTPPassword, SftpPort, "FileBackUp", "Inbound/Sales/", "Outbound", "Download", "FileUpload");
                            salesCommand.GetSales();
                            break;
                        case "SARE":
                            SalesCommand salesCommandRemian = new SalesCommand(SFTP, SFTPUser, SFTPPassword, SftpPort, "FileBackUp", "Inbound/Sales/", "Outbound", "Download", "FileUpload");
                            salesCommandRemian.GetSalesRemain();
                            break;
                        case "SRE":
                            SalesReconciliationCommand reconciliationCommand = new SalesReconciliationCommand(SFTP, SFTPUser, SFTPPassword, SftpPort, "FileBackUp", "Inbound/SalesRecon/", "Outbound", "Download", "FileUpload");
                            reconciliationCommand.GetSales();
                            break;
                        case "STRE":
                            StockReconciliationCommand stockReconciliationCommand = new StockReconciliationCommand(SFTP, SFTPUser, SFTPPassword, SftpPort, "FileBackUp", "Inbound/StoreSOHRecon/", "Outbound", "Download", "FileUpload");
                            stockReconciliationCommand.GetSales();
                            break;
                        case "STS":
                            StoreTransferCommand storeTransferCommand = new StoreTransferCommand(URL, ClientId, ClientSecret, Resource, APIURL, PrimaryKey, SecondaryKey, Licensee);
                            storeTransferCommand.SubmitStoreStransfer(DateTime.Now.ToString("MM/dd/yyyy"), "-");
                            break;
                        case "STSREC":
                            StoreTransferCommand storeTransferReceiptCommand = new StoreTransferCommand(URL, ClientId, ClientSecret, Resource, APIURL, PrimaryKey, SecondaryKey, Licensee);
                            storeTransferReceiptCommand.SubmitStoreStransferReceipt(DateTime.Now.ToString("MM/dd/yyyy"), "-");
                            break;
                        case "ADJ":
                            StockAdjustCommand stockAdjustCommand = new StockAdjustCommand(URL, ClientId, ClientSecret, Resource, APIURL, PrimaryKey, SecondaryKey, Licensee);
                            stockAdjustCommand.SubmitStoreStransferAdjust(DateTime.Now.ToString("MM/dd/yyyy"), "-");
                            break;
                        default:
                            Console.WriteLine("Invalid command.");
                            break;
                    }
                }
                else
                    Console.WriteLine("Invalid arguments.");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                Console.ReadKey();
            }
        }
    }
}
