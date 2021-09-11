using COGInterfaceCommand.Common;
using COGInterfaceCommand.Common.COG;
using COGInterfaceCommand.Common.COG.ASN;
using Newtonsoft.Json.Linq;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;


namespace COGInterfaceCommand.Command
{
    public class ASNCommand : APICommand
    {
        string url = "ASN";
        string urlAcknowledgement = "ASN/acknowledge";
        string fileType = "ASN";      

        public ASNCommand(string URL, string id, string secret, string resource, string apiURL, string primarySub, string secondarySub, string apiParameter, string grantType = "client_credentials") : base(URL, id, secret, resource, apiURL, primarySub, secondarySub, apiParameter, grantType)
        {
            //fileName = Environment.CurrentDirectory + "\\" + fileName;
        }

        AcknowledgeMessage message;
        Guid guiderror;

        //public void GetASN()
        //{
        //    string fileName = fileType + ".txt";
        //    string newLocation = fileName ;
        //    try
        //    {
        //        // Lấy token
        //        base.GetCOGToken();

        //        // Gọi api & lưu chuỗi trả về vào file
        //        base.MakeRequest(url, fileName);

        //        // Đọc chuỗi json từ file
        //        string jsonString = base.GetJsonFromFile(fileName); //fileName
        //      //@"D:\Projects\ProjectVu\ACFC\COGInterfaceCommand\bin\Debug\BACKUP\ASN.txt_fe8645fa-7fd0-4388-899a-446a758b88ed.json"

        //        guiderror = GetGuidError(jsonString);

        //        // Parse chuỗi json thành object & lưu vào db. Xuất ra file acknowledgement để post về COG               
        //        message = ProcessJsonASN(jsonString);

        //        // Tên file mới trong thư mục backup ( ASN + GUID từ json )
        //        newLocation = new FileInfo(fileName).DirectoryName + "\\Backup\\" + fileName + "_" + message.AcknowledgementGuid.ToString() + ".json";

        //        // Gửi acknowledgement message qua cho COG
        //        base.SubmitAcknowledgementMessage(APIURL + urlAcknowledgement, message);             
        //    }
        //    catch(Exception ex)
        //    {
        //        base.SubmitAcknowledgementMessage(APIURL + urlAcknowledgement, new AcknowledgeMessage(){
        //            AcknowledgementGuid = guiderror,
        //            success_ind = 'Y',
        //            error_msg = "Acknowledgement message for ASN " + guiderror.ToString()+ ex.Message
        //        });
        //    }
        //    finally
        //    {
        //        // Move file asn vừa tạo vào thư mục backup để xử lý sau.
        //        FileInfo file = new FileInfo(fileName);
        //        file.MoveTo(newLocation);
        //    }
        //}
        public void GetASN()
        {
            string fileName = fileType + ".txt";
            string newLocation = fileName;
            try
            {
                // Lấy token
                base.GetCOGToken();

                // Xóa file nếu đã có trước đây
                FileInfo fInfo = new FileInfo(fileName);
                if (fInfo.Exists)
                    fInfo.Delete();

                // Gọi api & lưu chuỗi trả về vào file
                base.MakeRequest(url, fileName);

                fInfo = new FileInfo(fileName);
                // Nếu file được tạo sau khi request => gọi API thành công & có data
                if (fInfo.Exists)
                {
                    // Đọc chuỗi json từ file
                    string jsonString = base.GetJsonFromFile(fileName); //fileNam
                    //string jsonString = base.GetJsonFromFile(@"D:\Projects\ProjectVu\ACFC\COGInterfaceCommand\bin\Debug\ASN.txt");
                    
                    guiderror = GetGuidError(jsonString);

                    // kiem tra json ASN xem co thong tin khong
                    if (ValidateASNJson(jsonString))
                    {

                        // Parse chuỗi json thành object & lưu vào db. Xuất ra file acknowledgement để post về COG               
                        message = ProcessJsonASN(jsonString);

                        // Tên file mới trong thư mục backup ( ASN + GUID từ json )
                        newLocation = new FileInfo(fileName).DirectoryName + "\\Backup\\" + fileName + "_" + message.AcknowledgementGuid.ToString() + ".json";

                        // Gửi acknowledgement message qua cho COG
                        base.SubmitAcknowledgementMessage(APIURL + urlAcknowledgement, message);
                    }
                    else
                    {
                        // do nothing
                    }
                }
            }
            catch (NullReferenceException)
            {
                base.SubmitAcknowledgementMessage(APIURL + urlAcknowledgement, new AcknowledgeMessage()
                {
                    AcknowledgementGuid = guiderror,
                    success_ind = 'Y',
                    error_msg = "Acknowledgement message for ASN " + guiderror.ToString() + "File does not exits"
                }) ;
            }
            catch (Exception ex)
            {
                base.SubmitAcknowledgementMessage(APIURL + urlAcknowledgement, new AcknowledgeMessage()
                {
                    AcknowledgementGuid = guiderror,
                    success_ind = 'Y',
                    error_msg = "Acknowledgement message for ASN " + guiderror.ToString() + ex.Message
                });
            }
            finally
            {
                // Move file asn vừa tạo vào thư mục backup để xử lý sau.
                FileInfo file = new FileInfo(fileName);
                file.MoveTo(newLocation);
            }
        }
        public void GetASN(string fileName)
        {
            try
            {
                // Đọc chuỗi json từ file
                string jsonString = base.GetJsonFromFile(fileName);
                FileInfo file = new FileInfo(fileName);
                string document_name = file.Name.Replace(fileType  + "_", "").Replace(file.Extension, "");                              
                ProcessJsonASN(jsonString);               
            }
            catch(Exception ex) { throw; }
        }
        private void DeleteExistingDocument(string documentName)
        {

        }
        private void DeleteItemFromDocument(string tableName, string documentName, OracleTransaction trans = null)
        {
            Configurations configurations = new Configurations();
            string command = string.Format("DELETE {0} WHERE Document_Name = '{1}'", tableName, documentName);

            try
            {
                if (trans == null)
                    configurations.ExecuteRPCommand(configurations.RPConnection, command, null);
                else
                    configurations.ExecuteRPCommand(trans, command, null);
            }
            catch { throw; }
        }
        private Guid GetGuidError(string jsonString)
        {
            Guid merror = new Guid();
            JObject json = JObject.Parse(jsonString);
            string sHeader = json["header"].ToString();
            APIHeader apiHeader = (APIHeader)JObject.Parse(sHeader).ToObject(typeof(APIHeader));

            merror = apiHeader.AcknowledgementGuid;            
            return merror;
        }

        private bool ValidateASNJson(string jsonString)
        {
            JObject json = JObject.Parse(jsonString);
            string mainASN = json["mainASN"].ToString();
            JObject mainJson = JObject.Parse(mainASN);


            JArray jHeaders = (JArray)mainJson["A_Header"];
            JArray jDetails = (JArray)mainJson["B_Detail"];
            JArray jCartons = (JArray)mainJson["C_Carton"];
            JArray jSKUs = (JArray)mainJson["D_SKU"];

            return jHeaders.Count + jDetails.Count + jCartons.Count + jSKUs.Count != 0;
        }
        private AcknowledgeMessage ProcessJsonASN(string jsonString)
        {
            /* =========================================================================================================================== *
             * Parse chuỗi json thành object
             * =========================================================================================================================== */

            List<string> iflasnno = null;
            List<string> ifldocname = null;
            List<string>  errorheader = null;
            List<string> errordetail = null;
            List<string> errorcarton = null;
            List<string> errorsku = null;
            DateTime processDate = DateTime.Now;
            JObject json = JObject.Parse(jsonString);
            string sHeader = json["header"].ToString();
            APIHeader apiHeader = (APIHeader)JObject.Parse(sHeader).ToObject(typeof(APIHeader));
            string mainASN = json["mainASN"].ToString();
            JObject mainJson = JObject.Parse(mainASN);

            JArray jHeaders = (JArray)mainJson["A_Header"];
            JArray jDetails = (JArray)mainJson["B_Detail"];
            JArray jCartons = (JArray)mainJson["C_Carton"];
            JArray jSKUs = (JArray)mainJson["D_SKU"];

            List<A_Header> headers = new List<A_Header>();
            List<B_Detail> details = new List<B_Detail>();
            List<C_Carton> cartons = new List<C_Carton>();
            List<D_SKU> skus = new List<D_SKU>();
            string exerror = "";
            string filename = Environment.CurrentDirectory + "\\FileUpload\\ASNAcknow\\asnAck"+DateTime.Now.Date.ToString("yyyyMMdd")+".txt";


            /* =========================================================================================================================== *
             * Lưu object xuống db
             * =========================================================================================================================== */

            Configurations config = new Configurations();
            OracleTransaction trans = config.CreateTransaction(config.RPConnection);
            iflasnno = new List<string>();
            ifldocname = new List<string>();
            errorheader = new List<string>();
            errordetail = new List<string>();
            errorcarton = new List<string>();
            errorsku = new List<string>();
            try
            {
                for (int i = 0; i < jHeaders.Count; i++)
                {                    
                    A_Header header = (A_Header)jHeaders[i].ToObject(typeof(A_Header));                    
                    header.COGFileName = apiHeader.AcknowledgementGuid.ToString();
                    header.CreatedDate = processDate;
                    iflasnno.Add(header.ASN_Number);
                    ifldocname.Add(header.COGFileName);
                    errorheader.Add(header.checkHeaderError(header));
                    header.DeleteExitAsn(header.ASN_Number);
                    header.AddToDatabase(trans);
                    headers.Add(header);
                }

                for (int i = 0; i < jDetails.Count; i++)
                {
                    B_Detail detail = (B_Detail)jDetails[i].ToObject(typeof(B_Detail));
                    detail.COGFileName = apiHeader.AcknowledgementGuid.ToString();
                    detail.CreatedDate = processDate;
                    errordetail.Add(detail.checkDetailError(detail)); //detail.ASN_Number+" - " + errordetail + detail.checkDetailError(detail);
                    detail.AddToDatabase(trans);
                    details.Add(detail);
                }

                for (int i = 0; i < jCartons.Count; i++)
                {
                    C_Carton carton = (C_Carton)jCartons[i].ToObject(typeof(C_Carton));
                    carton.COGFileName = apiHeader.AcknowledgementGuid.ToString();
                    carton.CreatedDate = processDate;
                    errorcarton.Add(carton.checkCartonError(carton)); //carton.ASN_Number+" - "+ errorcarton+carton.checkCartonError(carton);
                    carton.AddToDatabase(trans);
                    cartons.Add(carton);
                }

                for (int i = 0; i < jSKUs.Count; i++)
                {
                    D_SKU sku = (D_SKU)jSKUs[i].ToObject(typeof(D_SKU));
                    sku.COGFileName = apiHeader.AcknowledgementGuid.ToString();
                    sku.CreatedDate = processDate;
                    errorsku.Add(sku.checkSkuError(sku)); //sku.ASN_Number+" - "+ errorsku+sku.checkSkuError(sku);
                    sku.AddToDatabase(trans);
                    skus.Add(sku);
                }

                config.CommitTransaction(trans, true);

                // create file for IFL ASN

                //for (int i = 0; i < jHeaders.Count; i++)
                //{
                //    IFL_PO_Command iflCommand = new IFL_PO_Command();
                //    iflCommand.DeleteExistPO(iflasnno[i], ifldocname[i]);
                //    iflCommand.GetCogDataMaster('A', iflasnno[i], ifldocname[i]);
                //    iflCommand.GetCogPklMaster('A', iflasnno[i], ifldocname[i]);
                //    iflCommand.WriteCVSFile(iflasnno[i], ifldocname[i]);
                //}

               
                if (errorheader != null || errordetail != null || errorcarton != null || errorsku != null)
                {
                    if(jHeaders.Count!=0)
                    {
                        for (int i = 0; i < jHeaders.Count; i++)
                        {
                            exerror = exerror + iflasnno[i] + ":" + errorheader[i] + errordetail[i] + errorcarton[i] + errorsku[i] + "-";
                        }
                        if (exerror != "")
                        {
                            {
                                using (StreamWriter writer = new StreamWriter(filename, false, Encoding.UTF8))
                                    writer.WriteLine(exerror);
                            }
                        }
                        else
                        {
                            {
                                exerror = "File Empty";
                                using (StreamWriter writer = new StreamWriter(filename, false, Encoding.UTF8))
                                    writer.WriteLine(exerror);
                            }
                        }
                    }
                    
                    return new AcknowledgeMessage()
                    {
                        AcknowledgementGuid = apiHeader.AcknowledgementGuid,
                        success_ind = 'Y',
                        error_msg = "Acknowledgement message for ASN " + apiHeader.AcknowledgementGuid.ToString() + " " + exerror
                    };
                }
                else
                {
                    return new AcknowledgeMessage()
                    {
                        AcknowledgementGuid = apiHeader.AcknowledgementGuid,
                        success_ind = 'Y',
                        error_msg = "Acknowledgement message for ASN " + apiHeader.AcknowledgementGuid.ToString()
                    };
                }           
                
            }            
            catch(Exception ex)
            {               
                config.CommitTransaction(trans, false);
                exerror = ex.Message.ToString() + exerror;
                return new AcknowledgeMessage()
                {
                    AcknowledgementGuid = apiHeader.AcknowledgementGuid,
                    success_ind = 'N',
                    error_msg = exerror
                };
            }
        }
       
              
    }
}
