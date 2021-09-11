using OfficeOpenXml;
using Recruitment.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Services;

namespace Recruitment.CandidatePool
{
    public partial class ImportCandidates : System.Web.UI.Page
    {
        #region WebMethod

        /// <summary>
        /// Lấy danh sách resources
        /// </summary>
        [WebMethod]
        public static string LoadRecruitmentResource()
        {
            string jsonResources = string.Empty;
            try
            {
                DataTable tb = new DataTable();
                tb = QueryRecruitmentResources();
                jsonResources = Configuration.ConvertTableToJsonString(tb);
            }
            catch {  }
            return jsonResources;
        }

        /// <summary>
        /// Upload file Excel & đọc dữ liệu vào database.
        /// </summary>
        /// <param name="resourceId">The resource identifier.</param>
        /// <param name="isLocal">if set to <c>true</c> [is local].</param>
        /// <param name="fileName">Name of the file.</param>
        [WebMethod]
        public static void ImportCandidateData(int resourceId, string fileName, int startRow)
        {
            if (HttpContext.Current.Session["username"] == null)
            {
                return;
            }
            try
            {
                DataTable tbConfiguration = new DataTable();
                tbConfiguration = QueryCurrentResourceAssignedConfigurations(resourceId);
                ReadExcel(resourceId, fileName, startRow, tbConfiguration);
            }
            catch { }
        }

        /// <summary>
        /// Lấy cấu hình tất cả các field & cấu hình của resource hiện tại ( kể cả các field chưa set )
        /// </summary>
        /// <param name="resourceId">The resource identifier.</param>
        [WebMethod]
        public static string GetResourceAllConfigurations(int resourceId)
        {
            string jsonResult = string.Empty;

            try
            {
                DataTable tbConfiguration = new DataTable();
                tbConfiguration = QueryCurrentResourceAllConfigurations(resourceId);
                jsonResult = Configuration.ConvertTableToJsonString(tbConfiguration);
            }
            catch { }
            return jsonResult;
        }

        #endregion



        #region private helper

        /// <summary>
        /// Lấy danh sách các nguồn tuyển dụng.
        /// </summary>
        /// <returns>Table chứa danh sách nguồn</returns>
        public static DataTable QueryRecruitmentResources()
        {
            DataTable tb = new DataTable();
            try
            {
                SqlConnection conn = new SqlConnection(Configuration.ConnectionString);
                SqlDataAdapter da = new SqlDataAdapter();
                SqlCommand cmd = new SqlCommand(Configuration.GetResources, conn);
                da.SelectCommand = cmd;

                da.Fill(tb);
                return tb;
            }
            catch { throw; }
        }

        /// <summary>
        /// Lấy thông tin các nguồn tuyển dụng theo Id trên DB.
        /// </summary>
        /// <param name="resourceId">Id</param>
        /// <returns>Table chứa thông tin nguồn</returns>
        public static DataTable QueryRecruitmentResourceById(int resourceId)
        {
            DataTable tb = new DataTable();
            try
            {
                SqlConnection conn = new SqlConnection(Configuration.ConnectionString);
                SqlDataAdapter da = new SqlDataAdapter();
                SqlCommand cmd = new SqlCommand(Configuration.GetResourceByID, conn);
                cmd.Parameters.Add(new SqlParameter("resourceId", resourceId));
                da.SelectCommand = cmd;

                da.Fill(tb);
                return tb;
            }
            catch { throw; }
        }

        /// <summary>
        /// Lấy cấu hình các field mà resource đã set
        /// </summary>
        /// <param name="resourceId">The resource identifier.</param>
        /// <returns>Table chứa thông tin các field đã cấu hình</returns>
        public static DataTable QueryCurrentResourceAssignedConfigurations(int resourceId)
        {
            try
            {
                SqlConnection conn = new SqlConnection(Configuration.ConnectionString);
                SqlDataAdapter da = new SqlDataAdapter();
                SqlCommand cmd = new SqlCommand(Configuration.GetResourceReportConfiguration, conn);
                cmd.Parameters.Add(new SqlParameter("resourceId", resourceId));
                da.SelectCommand = cmd;

                DataTable tb = new DataTable();
                da.Fill(tb);
                return tb;
            }
            catch { throw; }
        }

        /// <summary>
        /// Lấy cấu hình tất cả các field & cấu hình của resource hiện tại ( kể cả các field chưa set )
        /// </summary>
        /// <param name="resourceId">The resource identifier.</param>
        /// <returns>Table chứa tất cả các cột & cấu hình của nguồn ( nếu có )</returns>
        public static DataTable QueryCurrentResourceAllConfigurations(int resourceId)
        {
            try
            {
                SqlConnection conn = new SqlConnection(Configuration.ConnectionString);
                SqlDataAdapter da = new SqlDataAdapter();
                SqlCommand cmd = new SqlCommand(Configuration.GetResourceAllConfiguration, conn);
                cmd.Parameters.Add(new SqlParameter("resourceId", resourceId));
                da.SelectCommand = cmd;

                DataTable tb = new DataTable();
                da.Fill(tb);
                return tb;
            }
            catch { throw; }
        }

        /// <summary>
        /// Đọc file Excel & ghi vào database
        /// </summary>
        public static void ReadExcel(int resourceId, string file, int startRow, DataTable tbConfiguration)
        {
            SqlConnection conn = new SqlConnection(Configuration.ConnectionString);
            FileInfo existingFile = new FileInfo(file);

            string fieldName;
            string fieldFormat;
            string exlColumnName;

            /// Chuỗi field để insert
            string insertClause = "";

            /// Chuỗi value để insert
            string valuesClause = "";

            /// Chuỗi update giá trị
            string updateClause = "";     
            
            // Danh sách giá trị Parameter
            List<SqlParameter> excelParas = new List<SqlParameter>();

            try
            {
                // Lấy cấu trúc table candidate
                conn.Open();
                SqlCommand cmdGetCandidateTable = new SqlCommand()
                {
                    Connection = conn,
                    CommandText = Configuration.GetCandidateTable
                };
                DataTable tbStruct = new DataTable();
                SqlDataAdapter da = new SqlDataAdapter(cmdGetCandidateTable);
                da.Fill(tbStruct);

                // Tạo commandText cho việc insert data
                foreach (DataRow cfgRow in tbConfiguration.Rows)
                {

                    fieldName = cfgRow["field_name"].ToString();

                    // Nếu tên cột <> NULL thì xử lý
                    if (tbStruct.Columns[fieldName] != null)
                    {
                        /* Thêm vào chuỗi insert */
                        insertClause += fieldName + ",";
                        valuesClause += "@" + fieldName + ",";
                        /* Thêm vào chuỗi update */
                        updateClause += fieldName + " = @" + fieldName + ",";
                    }
                }
                insertClause = insertClause.EndsWith(",") ? insertClause.Substring(0, insertClause.Length - 1) : insertClause;
                valuesClause = valuesClause.EndsWith(",") ? valuesClause.Substring(0, valuesClause.Length - 1) : valuesClause;
                updateClause = updateClause.EndsWith(",") ? updateClause.Substring(0, updateClause.Length - 1) : updateClause;

                /* Câu SQL insert */
                SqlCommand cmd = new SqlCommand
                {
                    Connection = conn,
                    CommandText = "INSERT HR_candidate ( " + insertClause + ", resource_id, status_id, created_date ) VALUES ( " + valuesClause + ", @resource_id, @defaultstatus, getdate() )"
                };

                /* Câu SQL update */
                SqlCommand updateCmd = new SqlCommand
                {
                    Connection = conn,
                    CommandText = "UPDATE " + updateClause + " WHERE id = @id"
                };

                /*=========== Câu SQL tìm ID cho các constraint ===========*/
                /* Lấy danh sách cột cần kiểm tra trùng */
                IEnumerable<DataRow> cfgRows = tbConfiguration.AsEnumerable();
                List<DataRow> checkRows = cfgRows
                    .Where(r =>
                        (bool)r["check_existed"] == true
                        && !string.IsNullOrEmpty(r["field_name"].ToString()) 
                        && (bool)r["active_flag"] == true).ToList();

                string filterExisted = "";

                /* Tạo câu SQL & danh sách parameter */
                foreach (DataRow row in checkRows)
                {
                    filterExisted += row["field_name"] + " = @" + row["field_name"] + " AND ";
                }
                filterExisted = filterExisted.EndsWith("AND ")? filterExisted.Substring(0, filterExisted.Length - 4) : filterExisted;
                
                /* Tạo câu lệnh SQL */
                SqlCommand checkExistedCmd = new SqlCommand
                {
                    Connection = conn,
                    CommandText = "SELECT @id = id FROM HR_candidate WHERE " + filterExisted
                };

                checkExistedCmd.Parameters.Add(
                    new SqlParameter
                    {
                        ParameterName = "@id",
                        Direction = ParameterDirection.Output
                    }
                );

                // Lấy status default cho nhân viên
                DataTable tbStatus = new DataTable();
                da.SelectCommand = new SqlCommand(Configuration.GetFirstStatus, conn);
                da.Fill(tbStatus);

                // Loop through Excel data
                using (ExcelPackage package = new ExcelPackage(existingFile))
                {
                    //get the first worksheet in the workbook
                    ExcelWorksheet worksheet = package.Workbook.Worksheets[1];
                    int colCount = worksheet.Dimension.End.Column;  //get Column Count
                    int rowCount = worksheet.Dimension.End.Row;     //get row count


                    // Lặp qua từng dòng & gán giá trị vào SQLParameter
                    for (int row = startRow; row <= rowCount; row++)
                    {
                        cmd.Parameters.Clear();
                        cmd.Parameters.Add(new SqlParameter("@resource_id", resourceId));

                        /* ======================================================================================================
                         * Lặp qua từng config row => lấy tên cột trong SQL & tên cột trong Excel
                         *      + Nếu tên cột Excel rỗng => cho SQLParameter giá trị null
                         *      + Nếu tên cột Excel không rỗng => Lấy giá trị Excel tương ứng.
                         *          + Xử lý giá trị Excel: 
                         *              + Nếu cột DB là datetime:
                         *                  + Giá trị Excel là datetime => giữ nguyên
                         *                  + Giá trị Excel là text => Chuyển đổi theo format trong DB
                         *                  + Giá trị Excel là timestamp => OADateTime
                         *              + Nếu cột là text, int 
                         *                  + Nếu có format thì định dạng theo format
                         *                  + Nếu không có format thì giữ nguyên.
                         * Nếu cột cần kiểm tra trùng thì sẽ add thêm SQLParamter vào câu SQL kiểm tra trùng
                         * ====================================================================================================== */

                        foreach (DataRow cfgRow in tbConfiguration.Rows)
                        {
                            fieldName = cfgRow["field_name"].ToString();
                            exlColumnName = cfgRow["column_name"].ToString();
                            fieldFormat = cfgRow["field_format"] is null ? "" : cfgRow["field_format"].ToString();

                            insertClause += fieldName + ",";
                            valuesClause += "@" + fieldName + ",";

                            if (string.IsNullOrEmpty(exlColumnName))
                            {
                                excelParas.Add(new SqlParameter("@" + fieldName, DBNull.Value));
                                if ((bool)cfgRow["check_existed"])
                                {
                                    checkExistedCmd.Parameters.Add(new SqlParameter("@" + fieldName, DBNull.Value));
                                }
                            }
                            else
                            {
                                object val = worksheet.Cells[row, Configuration.ConvertExcelColumnNameToNumber(exlColumnName)].Value;
                                if (string.IsNullOrEmpty(fieldFormat))
                                {
                                    excelParas.Add(new SqlParameter("@" + fieldName, val));
                                    if ((bool)cfgRow["check_existed"])
                                    {
                                        checkExistedCmd.Parameters.Add(new SqlParameter("@" + fieldName, DBNull.Value));
                                    }
                                }
                                else
                                {
                                    if (tbStruct.Columns[fieldName].DataType.Equals(typeof(DateTime)))
                                    {
                                        DateTime value;
                                        if (val.GetType().Equals(typeof(DateTime)))
                                        {
                                            value = (DateTime)val;
                                        }
                                        else
                                        {
                                            if (fieldFormat == "timestamp")
                                            {
                                                value = DateTime.FromOADate(double.Parse(val.ToString()));
                                            }
                                            else
                                            {
                                                value = DateTime.ParseExact(val.ToString(), fieldFormat, CultureInfo.InvariantCulture);
                                            }
                                        }
                                        excelParas.Add(new SqlParameter("@" + fieldName, val));
                                        if ((bool)cfgRow["check_existed"])
                                        {
                                            checkExistedCmd.Parameters.Add(new SqlParameter("@" + fieldName, value));
                                        }
                                    }
                                    else
                                    {
                                        string[] regexFormats = fieldFormat.Split(';');
                                        excelParas.Add(new SqlParameter("@" + fieldName, Regex.Replace(val.ToString(), regexFormats[0], regexFormats[1])));
                                        if ((bool)cfgRow["check_existed"])
                                        {
                                            checkExistedCmd.Parameters.Add(new SqlParameter("@" + fieldName, Regex.Replace(val.ToString(), regexFormats[0], regexFormats[1])));
                                        }
                                    }
                                }
                            }
                        }

                        try
                        {
                            checkExistedCmd.ExecuteNonQuery();
                            object existedId = checkExistedCmd.Parameters["@id"].SqlValue;
                            if (checkExistedCmd.Parameters["@id"].SqlValue != DBNull.Value )
                            {
                                excelParas.Add(new SqlParameter("@id", tbStatus.Rows[0]["id"]));
                                updateCmd.Parameters.AddRange(excelParas.ToArray<SqlParameter>());
                                updateCmd.ExecuteNonQuery();
                            }
                            else
                            {
                                if (tbStatus.Rows.Count == 1)
                                {
                                    excelParas.Add(new SqlParameter("@defaultstatus", tbStatus.Rows[0]["id"]));
                                }
                                else
                                {
                                    excelParas.Add(new SqlParameter("@defaultstatus", DBNull.Value));
                                }

                                cmd.Parameters.AddRange(excelParas.ToArray<SqlParameter>());
                                cmd.ExecuteNonQuery();
                            }
                        }
                        catch { }
                    }

                    conn.Close();
                }
            }
            catch(Exception ex) { }
        }

        #endregion
    }
}

        #region TEST

        //private static readonly IAuthorizationCodeFlow flow =
        //    new GoogleAuthorizationCodeFlow(new GoogleAuthorizationCodeFlow.Initializer
        //    {
        //        ClientSecrets = new ClientSecrets
        //        {
        //            ClientId = "611041570577-5ccmss7b1ngpgs9muqmjjqhvtb2k4vnv.apps.googleusercontent.com",
        //            ClientSecret = "qiDoQiJkSMPjXajq0_-TN2vp"
        //        },
        //        Scopes = new[] { SheetsService.Scope.SpreadsheetsReadonly }
        //    });

        //protected static string ReadGoogleSheet1(string url, string sheetName)
        //{
        //    string result = "";

        //    try
        //    {

        //        // If modifying these scopes, delete your previously saved credentials
        //        // at ~/.credentials/sheets.googleapis.com-dotnet-quickstart.json
        //        string[] Scopes = { SheetsService.Scope.SpreadsheetsReadonly };
        //        string ApplicationName = "Quickstart";

        //        UserCredential credential;

        //        using (var stream = new FileStream(HostingEnvironment.MapPath("~\\Credentials\\credentials.json"), FileMode.Open, FileAccess.Read))
        //        {
        //            // The file token.json stores the user's access and refresh tokens, and is created
        //            // automatically when the authorization flow completes for the first time.
        //            string credPath = HostingEnvironment.MapPath("~\\Credentials\\token.json");
        //            credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
        //                GoogleClientSecrets.Load(stream).Secrets,
        //                Scopes,
        //                "user",
        //                CancellationToken.None,
        //                new FileDataStore(credPath, true)).Result;
        //            //Console.WriteLine("Credential file saved to: " + credPath);
        //        }

        //        // Create Google Sheets API service.
        //        var service = new SheetsService(new BaseClientService.Initializer()
        //        {
        //            HttpClientInitializer = credential,
        //            ApplicationName = ApplicationName,
        //        });

        //        // Define request parameters.
        //        string spreadsheetId = @"https://docs.google.com/spreadsheets/d/" + url;
        //        string range = sheetName + "!A1:H";
        //        SpreadsheetsResource.ValuesResource.GetRequest request = service.Spreadsheets.Values.Get(spreadsheetId, range);

        //        string st = "";

        //        // Prints the names and majors of students in a sample spreadsheet:
        //        // https://docs.google.com/spreadsheets/d/1OrvQA8sVj64lskhGcGaBnkLU78o_7ZPzoJvrNXCTbYE
        //        ValueRange response = request.Execute();
        //        IList<IList<Object>> values = response.Values;
        //        if (values != null && values.Count > 0)
        //        {
        //            Console.WriteLine("Name, Major");
        //            //foreach (var row in values)
        //            //{
        //            //    // Print columns A and E, which correspond to indices 0 and 4.
        //            //    st += string.Format("{0}, {1}", row[0], string.IsNullOrEmpty(row[4].ToString())? "" : row[4].ToString());
        //            //}

        //            for (int i = 0; i < 10; i++)
        //            {
        //                var row = values[i];

        //                st += string.Format("{0}, {1}", row[1], row[2]);
        //            }
        //        }
        //        else
        //        {

        //        }

        //    }
        //    catch (Exception ex) { }
        //    return result;
        //}

        ///// <summary>
        ///// Đọc dữ liệu từ Google sheet & ghi vào database
        ///// </summary>
        ///// <param name="resourceId">The resource identifier.</param>
        ///// <param name="isLocal">if set to <c>true</c> [is local].</param>
        ///// <param name="fileURL">The file URL.</param>
        //[WebMethod]
        //public static void ReadGoogleForm(int resourceId, string sheetName, string fromDate)
        //{
        //    DataTable tbConfiguration = new DataTable();
        //    DataTable tbResource = new DataTable();

        //    DateTime from = DateTime.ParseExact(fromDate, "MM/dd/yyyy", CultureInfo.InvariantCulture);

        //    try
        //    {
        //        tbResource = QueryRecruitmentResourceById(resourceId);
        //        tbConfiguration = QueryResourceConfiguration(resourceId);

        //        if (tbResource.Rows.Count == 1)
        //        {
        //            string url = tbResource.Rows[0]["resource_url"].ToString();

        //            ReadGoogleSheet1(url, sheetName);



        //        }
        //    }
        //    catch (Exception ex)
        //    {

        //    }
        //}

        //protected static string ReadGoogleSheet(string url, string sheetName)
        //{
        //    string result = "";

        //    try
        //    {
        //        //SpreadsheetsService myService = new SpreadsheetsService("");
        //        //myService.setUserCredentials("jo@gmail.com", "mypassword");
        //    }
        //    catch { }

        //    return result;
        //}

        #endregion 