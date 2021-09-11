using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Web.Script.Serialization;

namespace Recruitment.Common
{
    public class Configuration
    {
        /// <summary>
        /// Converts the table to json string.
        /// </summary>
        /// <param name="tb">The tb.</param>
        /// <returns></returns>
        public static string ConvertTableToJsonString(DataTable tb)
        {
            JavaScriptSerializer jsSerializer = new JavaScriptSerializer();
            List<Dictionary<string, object>> parentRow = new List<Dictionary<string, object>>();
            Dictionary<string, object> childRow;

            JsonSerializerSettings format = new JsonSerializerSettings
            {
                DateFormatHandling = DateFormatHandling.IsoDateFormat
            };

            //for (int i = 0; i < 40; i++)
            //{
            foreach (DataRow row in tb.Rows)
            {
                childRow = new Dictionary<string, object>();
                foreach (DataColumn col in tb.Columns)
                {
                    childRow.Add(col.ColumnName, row[col]);
                }
                parentRow.Add(childRow);
            }
            //}
            return JsonConvert.SerializeObject(parentRow, format);
            //return jsSerializer.Serialize(parentRow);
        }

        /// <summary>
        /// Converts the excel column name to number.
        /// </summary>
        /// <param name="columnName">Name of the column.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">columnName</exception>
        public static int ConvertExcelColumnNameToNumber(string columnName)
        {
            if (string.IsNullOrEmpty(columnName)) throw new ArgumentNullException("columnName");

            columnName = columnName.ToUpperInvariant();

            int sum = 0;

            for (int i = 0; i < columnName.Length; i++)
            {
                sum *= 26;
                sum += (columnName[i] - 'A' + 1);
            }

            return sum;
        }

        /// <summary>
        /// Executes the query and get jsonstring as result.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="commandText">The command text.</param>
        /// <param name="paras">The paras.</param>
        /// <returns>jsonString of datatables</returns>
        public static string ExecuteQuery(string connectionString, string commandText, List<SqlParameter> paras)
        {
            try
            {
                SqlConnection conn = new SqlConnection(connectionString);
                SqlCommand cmd = new SqlCommand(commandText, conn);

                if (paras == null)
                {

                }
                else
                {
                    foreach (SqlParameter param in paras)
                    {
                        cmd.Parameters.Add(param);
                    }
                }
                
                SqlDataAdapter da = new SqlDataAdapter(cmd);

                DataTable tb = new DataTable();
                conn.Open();
                da.Fill(tb);
                conn.Close();

                return ConvertTableToJsonString(tb);
            }
            catch (Exception ex) { throw; }
        }

        public static DataTable ExecuteQueryData(string connectionString, string commandText, List<SqlParameter> paras)
        {
            try
            {
                SqlConnection conn = new SqlConnection(connectionString);
                SqlCommand cmd = new SqlCommand(commandText, conn);

                if (paras == null)
                {

                }
                else
                {
                    foreach (SqlParameter param in paras)
                    {
                        cmd.Parameters.Add(param);
                    }
                }

                SqlDataAdapter da = new SqlDataAdapter(cmd);

                DataTable tb = new DataTable();
                conn.Open();
                da.Fill(tb);
                conn.Close();

                return tb;
            }
            catch (Exception ex) { throw; }
        }

        public static bool ExecuteNonQuery(string connectionString, string commandText, List<SqlParameter> paras)
        {
            bool result = false;
            try
            {
                SqlConnection conn = new SqlConnection(connectionString);
                SqlCommand cmd = new SqlCommand(commandText, conn);

                if (paras == null)
                {

                }
                else
                {
                    foreach (SqlParameter param in paras)
                    {
                        cmd.Parameters.Add(param);
                    }
                }

                conn.Open();
                cmd.ExecuteNonQuery();
                conn.Close();

                result = true;
            }
            catch (Exception ex) { throw; }
            return result;
        }



        /// <summary>
        /// The connection string
        /// </summary>
        public const string ConnectionString = 
            "Data Source=192.168.80.4;Initial Catalog=ACFCInventory;User ID=acfcmango;Password=acfcmango;Pooling=false;";

        /// <summary>
        /// Get all resources from DB
        /// </summary>
        public const string GetResources =
            "SELECT * FROM HR_recruitment_resource";

        /// <summary>
        /// The get resource by identifier
        /// </summary>
        public const string GetResourceByID =
            "SELECT * FROM HR_recruitment_resource WHERE id = @resourceId";

        /// <summary>
        /// Lấy tất cả các field & cấu hình của resource hiện tại
        /// </summary>
        public const string GetResourceReportConfiguration =
                @"SELECT	df.* 
                        , rrc.column_name 
                        , rrc.row_num 
                        , rrc.field_format 
                        , rrc.resource_id 
                        , rrc.resource_name 
                FROM    HR_data_field AS df 
                INNER JOIN 
                        ( 
                            SELECT  rr.id as resource_id 
                                    , rr.resource_name 
                                    , rdc.field_id 
                                    , rdc.column_name 
                                    , rdc.row_num 
                                    , rdc.field_format 
                            FROM    HR_resource_data_configuration AS rdc 
                            INNER JOIN  HR_recruitment_resource AS rr ON rdc.resource_id = rr.id AND rdc.active_flag = 1 
                            WHERE   rr.id = @resourceId 
                        ) AS rrc ON rrc.field_id = df.id ";

        /// <summary>
        /// Lấy tất cả các field & cấu hình của resource hiện tại ( kể cả các field chưa cấu hình )
        /// </summary>
        public const string GetResourceAllConfiguration =
                @"SELECT	df.* 
                        , rrc.column_name 
                        , rrc.row_num 
                        , rrc.field_format 
                        , rrc.resource_id 
                        , rrc.resource_name 
                FROM    HR_data_field AS df 
                LEFT JOIN 
                        ( 
                            SELECT  rr.id as resource_id 
                                    , rr.resource_name 
                                    , rdc.field_id 
                                    , rdc.column_name 
                                    , rdc.row_num 
                                    , rdc.field_format 
                            FROM    HR_resource_data_configuration AS rdc 
                            RIGHT JOIN  HR_recruitment_resource AS rr ON rdc.resource_id = rr.id 
                            WHERE   rr.id = @resourceId 
                        ) AS rrc ON rrc.field_id = df.id ";

        /// <summary>
        /// The all candidates
        /// </summary>
        public const string GetCandidates =
                @"SELECT    c.id
                            , c.resource_id
                            , c.candidate_name
                            , c.gender
                            , c.dob
                            , c.address
                            , c.district
                            , c.city
                            , c.email
                            , c.mobile_no
                            , c.position
                            , c.submitted_date
                            , c.resume_id
                            , c.status_id
                            , c.created_date
                            , c.modified_date 
                            , REPLACE(note, char(10), '</BR>') as note  
                            , s.status_txt 
                            , rr.resource_name 
                FROM      HR_candidate c 
                INNER JOIN HR_recruitment_resource rr ON rr.id = c.resource_id 
                LEFT JOIN HR_candidate_status s ON c.status_id = s.id";



        /// <summary>
        /// The all candidates - filter by status
        /// </summary>
        public const string GetCandidatesByStatus =
                @"SELECT    c.id
                            , c.resource_id
                            , c.candidate_name
                            , c.gender
                            , c.dob
                            , c.address
                            , c.district
                            , c.city
                            , c.email
                            , c.mobile_no
                            , c.position
                            , c.submitted_date
                            , c.resume_id
                            , c.status_id
                            , c.created_date
                            , c.modified_date 
                            , REPLACE(note, char(10), '</BR>') as note  
                            , s.status_txt 
                            , rr.resource_name 
                FROM      HR_candidate c 
                INNER JOIN HR_recruitment_resource rr ON rr.id = c.resource_id 
                LEFT JOIN HR_candidate_status s ON c.status_id = s.id 
                WHERE   c.status_id = @status_id ";

        //c.id, c.resource_id, c.gender, c.dob, c.address, c.district, c.city, c.email, c.mobile_no, c.position, c.submitted_date, c.resume_id, c.status_id, c.created_date 
        // +
        //OFFSET    @PageSize * (@PageNo - 1) ROWS  
        //FETCH NEXT @PageSize ROWS ONLY";

        /// <summary>
        /// The get candidate table
        /// </summary>
        public const string GetCandidateTable = 
            "SELECT TOP 1 * FROM HR_candidate ";

        /// <summary>
        /// The get all status
        /// </summary>
        public const string GetAllStatus = 
            "SELECT s.* FROM HR_candidate_status s ";

        /// <summary>
        /// The get first - default status of new candidate
        /// </summary>
        public const string GetFirstStatus = 
            "SELECT TOP 1 * FROM HR_candidate_status ORDER BY id ";

        /// <summary>
        /// The update candidate status ( add new note )
        /// </summary>
        public const string UpdateCandidateStatusWithNote =
            "UPDATE HR_candidate SET status_id = @status_id, note = coalesce(note, '') + char(10) + @note, , modified_by = @modified_by, modified_date = GETDATE() WHERE id = @id ";

        /// <summary>
        /// The update candidate status ( add new note )
        /// </summary>
        public const string UpdateCandidateStatusWithoutNote =
            "UPDATE HR_candidate SET status_id = @status_id, modified_by = @modified_by, modified_date = GETDATE() WHERE id = @id ";

        /// <summary>
        /// The get all brands ( với điều kiện là có store )
        /// </summary>
        public const string GetAllBrands =
                @"SELECT	DISTINCT b.BrandCode, b.BrandName 
                FROM        Brand AS b
                INNER JOIN  Store AS s ON b.BrandCode = s.Brand";

        /// <summary>
        /// The get positions
        /// </summary>
        public const string GetPositions =
                @"SELECT    * 
                FROM        HR_Position
                WHERE       is_store_position = 1";

        /// <summary>
        /// The get stores of brand
        /// </summary>
        public const string GetStoresOfBrand =
                @"SELECT * 
                FROM    Store 
                WHERE   Brand = @brand";

        /// <summary>
        /// The all candidates in datetime range
        /// </summary>
        public const string GetCandidatesInRange =
                @"SELECT    c.* 
                            , s.status_txt 
                            , rr.resource_name 
                FROM        HR_candidate c 
                INNER JOIN  HR_recruitment_resource rr ON rr.id = c.resource_id 
                LEFT JOIN   HR_candidate_status s ON c.status_id = s.id 
                WHERE       c.submitted_date BETWEEN @fromdate AND @todate ";

        /// <summary>
        /// The get user account
        /// </summary>
        public const string FindUserAccount =
                @"SELECT    @Count = COUNT(*)  
                FROM        LogsUser 
                WHERE       UserId = @User 
                AND         Password = @Password 
                AND         SiteGroup = 2 ";

        /// <summary>
        /// The insert employee
        /// </summary>
        public const string InsertEmployee =
                @"INSERT    INTO HR_employee ( candidate_id, employee_name, dob, brand_code, created_date )
                SELECT	    id, candidate_name, dob, @brandCode, getdate()  
                FROM	    HR_candidate 
                WHERE	    id = @id ";

        /// <summary>
        /// The get new employee of all brands
        /// </summary>
        public const string GetNewEmployeeOfAllBrands =
                @"SELECT    c.candidate_name
                            , c.dob
                            , e.*
                            , b.brandname
                FROM        HR_candidate AS c 
                INNER JOIN  HR_employee AS e ON c.id = e.candidate_id 
                INNER JOIN  brand AS b ON e.brand_code = b.brandcode 
                WHERE       e.created_date BETWEEN @FromDate AND @ToDate ";
                //AND         employee_no IS NULL ";

        /// <summary>
        /// The get new employee of current brand
        /// </summary>
        public const string GetNewEmployeeOfBrand =
                @"SELECT    c.candidate_name
                            , c.dob
                            , e.*
                            , b.brandname
                FROM        HR_candidate AS c 
                INNER JOIN  HR_employee AS e ON c.id = e.candidate_id 
                INNER JOIN  brand AS b ON e.brand_code = b.brandcode 
                WHERE       e.created_date BETWEEN @FromDate AND @ToDate 
                AND         e.brand_code = @Brand ";

        /// <summary>
        /// The get all documents of selected employee
        /// </summary>
        public const string GetAllDocumentsOfSelectedEmployee =
                @"SELECT	d.document_name
                            , ed.submitted
			                , ed.updated_date 
			                , ed.employee_id 
                FROM        HR_document AS d 
                LEFT JOIN	HR_employee_documents AS ed ON ed.document_id = d.id AND d.active_flag = 1 
                WHERE       ed.employee_id = @id ";

        /// <summary>
        /// The get all documents of selected employee
        /// </summary>
        public const string GetAllDocuments =
                @"IF ( 
                 SELECT	COUNT(*) 
                 FROM	HR_employee_documents AS ed 
                 WHERE	ed.employee_id = @employeeId  
                ) = 0
                    BEGIN
                        SELECT		d.id 
                                    , d.document_name
                                    , ed.submitted
                                    , ed.updated_date 
                                    , @employeeId as employee_id
                        FROM        HR_document AS d 
                        LEFT JOIN	HR_employee_documents AS ed ON ed.document_id = d.id AND d.active_flag = 1 AND ed.employee_id = @employeeId 

                    END
                ELSE
                    BEGIN
                         SELECT		d.id
                                    , d.document_name
                                    , ed.submitted
                                    , ed.updated_date 
                                    , ed.employee_id 
                        FROM        HR_document AS d 
                        LEFT JOIN	HR_employee_documents AS ed ON ed.document_id = d.id AND d.active_flag = 1 AND ed.employee_id = @employeeId 
                 END";

        /// <summary>
        /// The get employee details
        /// </summary>
        public const string GetEmployeeDetails =
                @"SELECT    *   FROM    HR_employee WHERE   employee_no = @employeeNo";

        /// <summary>
        /// Update employee details
        /// </summary>
        public const string UpdateEmployee =
                @"UPDATE    HR_employee 
                SET         employee_no = @employeeNo
                            , est_start_date = @estStartDate 
                            , start_date = @startDate 
                            , base_salary = @baseSalary 
                            , probation_salary = @probationSalary 
                            , additional_salary = @additionalSalary 
                            , bank_account = @BankAccount 
                            , is_private_account = @IsPrivateAccount
                            , brand_code = @brandCode 
                            , store_code = @storeCode 
                            , position_id = @positionId 
                            , submitted_all_documents = @submittedAllDocuments 
                WHERE       id = @employeeId ";


        /// <summary>
        /// Update employee details
        /// </summary>
        public const string UpdateTerminationEmployee =
                @"UPDATE    HR_employee 
                SET         employee_no = @employeeNo
                            , est_start_date = @estStartDate 
                            , start_date = @startDate 
                            , base_salary = @baseSalary 
                            , probation_salary = @probationSalary 
                            , additional_salary = @additionalSalary 
                            , bank_account = @BankAccount 
                            , is_private_account = @IsPrivateAccount
                            , brand_code = @brandCode 
                            , store_code = @storeCode 
                            , position_id = @positionId 
                            , submitted_all_documents = @submittedAllDocuments
                            , termination_date = @terminationDate
                            , termination_reason = @terminationReason
                WHERE       id = @employeeId ";

        /// <summary>
        /// The sign in
        /// </summary>
        public const string SignIn =
                @"SELECT    * 
                FROM        LogsUser 
                WHERE       userId = @userId AND Password = @Password AND SiteGroup = 2";


        public const string SubmitDocument =
                @"IF ( SELECT COUNT(*) FROM HR_employee_documents WHERE employee_id = @employeeId AND document_id = @documentId ) = 0 
                    BEGIN 
                        INSERT HR_employee_documents ( employee_id, document_id, submitted, updated_date )  
                        VALUES ( @employeeId, @documentId, @submitted, @updatedDate ) 
                    END 
                ELSE 
                    BEGIN 
                        UPDATE  HR_employee_documents 
                        SET     submitted = @submitted 
                                , updated_date = @updatedDate 
                        WHERE   employee_id = @employeeId 
                        AND     document_id = @documentId 
                    END";

        /// <summary>
        /// The report candidate status
        /// </summary>
        public const string ReportCandidateStatus =
                @"SELECT	rr.resource_name, pv.*
                FROM	
		                (
			                SELECT	resource_id
			                , [1] as 'Đã xem'
			                , [2] as 'Đã liên hệ'
			                , [3] as 'Đã mời phỏng vấn'
			                , [4] as 'Đã test'
			                , [5] as 'Đã phỏng vấn'
			                , [6] as 'Nhận'
			                , [7] as 'Không nhận'
			                , [8] as 'Blacklist'
			                , total as 'Tổng số ứng viên'
			                FROM	
			                ( 
				                SELECT		1 as val, status_id, resource_id, count(*) over (partition by resource_id) as total
				                FROM		HR_candidate 
				                WHERE		submitted_date BETWEEN @fromDate AND @toDate ) AS SourceTable 
				                PIVOT(COUNT(val) FOR status_id IN ([1], [2], [3], [4], [5], [6], [7], [8]) 
			                ) AS PIVOTTABLE 
		                ) AS pv 
                INNER JOIN HR_recruitment_resource AS rr ON pv.resource_id = rr.id ";

        /// <summary>
        /// The report employee not submitted all documents
        /// </summary>
        public const string ReportEmployeeNotSubmittedAllDocuments =
                @"SELECT	employee_name
                            , brand_code
                            , store_code
                            , s.StoreName 
                FROM	    HR_employee AS e  
                INNER JOIN  Store AS s ON e.store_code = s.StoreCode 
                WHERE	    start_date BETWEEN @fromDate AND @toDate 
                AND         ( submitted_all_documents IS NULL OR submitted_all_documents = 0 )";

        /// <summary>
        /// ReportEmployeeEachBrand
        /// </summary>
        public const string ReportEmployeeEachBrand =
                @"SELECT	COUNT(*) AS total_new_employees
                            , SUM(CASE WHEN termination_date IS NOT NULL THEN 1 ELSE 0 END) AS total_termination_employees
                            , brand_code
                FROM	    HR_employee 
                WHERE	    start_date BETWEEN @fromDate AND @toDate 
                AND         ( termination_date IS NULL OR termination_date BETWEEN @fromDate AND @toDate ) 
                GROUP BY    brand_code ";
    }
}