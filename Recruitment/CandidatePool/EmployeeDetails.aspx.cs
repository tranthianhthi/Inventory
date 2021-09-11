using Recruitment.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.Services;

namespace Recruitment.CandidatePool
{
    public partial class EmployeeDetails : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// Gets all brands.
        /// </summary>
        /// <returns></returns>
        [WebMethod]
        public static string GetAllBrands()
        {
            var sBrands = "";
            try
            {
                sBrands = Configuration.ExecuteQuery(Configuration.ConnectionString, Configuration.GetAllBrands, null);
            }
            catch { }
            return sBrands;
        }

        /// <summary>
        /// Gets the stores of brand.
        /// </summary>
        /// <param name="brand">The brand.</param>
        /// <returns></returns>
        [WebMethod]
        public static string GetStoresOfBrand(string brand)
        {
            var sStores = "";
            try
            {
                List<SqlParameter> paras = new List<SqlParameter>()
                {
                    new SqlParameter("@Brand", brand)
                };

                sStores = Configuration.ExecuteQuery(Configuration.ConnectionString, Configuration.GetStoresOfBrand, paras);
            }
            catch { throw; }
            return sStores;
        }

        /// <summary>
        /// Gets the positions.
        /// </summary>
        /// <returns></returns>
        [WebMethod]
        public static string GetPositions()
        {
            var sPositions = "";
            try
            {
                sPositions = Configuration.ExecuteQuery(Configuration.ConnectionString, Configuration.GetPositions, null);
            }
            catch { throw; }
            return sPositions;
        }

        /// <summary>
        /// Gets the new employee of all brands.
        /// </summary>
        /// <param name="fromDate">From date.</param>
        /// <param name="toDate">To date.</param>
        /// <returns></returns>
        [WebMethod]
        public static string GetNewEmployeeOfAllBrands(string fromDate, string toDate)
        {
            var sEmployees = "";
            try
            {
                List<SqlParameter> paras = new List<SqlParameter>()
                {
                    new SqlParameter("@FromDate", DateTime.ParseExact(fromDate, "MM/dd/yyyy", CultureInfo.InvariantCulture)),
                    new SqlParameter("@ToDate", DateTime.ParseExact(toDate, "MM/dd/yyyy", CultureInfo.InvariantCulture).AddDays(1).AddMilliseconds(-2))
                };

                sEmployees = Configuration.ExecuteQuery(Configuration.ConnectionString, Configuration.GetNewEmployeeOfAllBrands, paras);
            }
            catch (Exception ex) { }
            return sEmployees;
        }

        /// <summary>
        /// Gets the new employee of brand.
        /// </summary>
        /// <param name="fromDate">From date.</param>
        /// <param name="toDate">To date.</param>
        /// <param name="brandCode">The brand code.</param>
        /// <returns></returns>
        [WebMethod]
        public static string GetNewEmployeeOfBrand(string fromDate, string toDate, string brandCode)
        {
            var sEmployees = "";
            try
            {
                List<SqlParameter> paras = new List<SqlParameter>()
                {
                    new SqlParameter("@FromDate", DateTime.ParseExact(fromDate, "MM/dd/yyyy", CultureInfo.InvariantCulture)),
                    new SqlParameter("@ToDate", DateTime.ParseExact(toDate, "MM/dd/yyyy", CultureInfo.InvariantCulture).AddDays(1).AddMilliseconds(-2)),
                    new SqlParameter("@BrandCode", brandCode)
                };

                sEmployees = Configuration.ExecuteQuery(Configuration.ConnectionString, Configuration.GetNewEmployeeOfBrand, paras);
            }
            catch (Exception ex) { }
            return sEmployees;
        }

        /// <summary>
        /// Saves the new employee.
        /// </summary>
        /// <param name="employeeId">The employee identifier.</param>
        /// <param name="empCode">The emp code.</param>
        /// <param name="estStartDate">The est start date.</param>
        /// <param name="startDate">The start date.</param>
        /// <param name="brandCode">The brand code.</param>
        /// <param name="storeCode">The store code.</param>
        /// <param name="positionId">The position identifier.</param>
        /// <param name="probationSalary">The probation salary.</param>
        /// <param name="baseSalary">The base salary.</param>
        /// <param name="additionalSalary">The additional salary.</param>
        /// <param name="bankAccount">The bank account.</param>
        /// <param name="isPrivateAccount">if set to <c>true</c> [is private account].</param>
        /// <param name="submittedAllDocuments">if set to <c>true</c> [submitted all documents].</param>
        /// <returns></returns>
        [WebMethod]
        public static bool SaveNewEmployee(int employeeId,
            string empCode,
            string estStartDate,
            string startDate,
            string brandCode,
            string storeCode,
            string positionId,
            string probationSalary,
            string baseSalary,
            string additionalSalary,
            string bankAccount,
            bool isPrivateAccount,
            bool submittedAllDocuments)
        {
            bool result = false;
            if (HttpContext.Current.Session["username"] == null)
            {
                return false;
            }
            try
            {
                List<SqlParameter> paras = new List<SqlParameter>()
                {
                    new SqlParameter("@employeeNo", empCode),
                    new SqlParameter("@estStartDate", DateTime.ParseExact(estStartDate, "MM/dd/yyyy", CultureInfo.InvariantCulture)),
                    new SqlParameter("@startDate", DateTime.ParseExact(startDate, "MM/dd/yyyy", CultureInfo.InvariantCulture)),
                    new SqlParameter("@brandCode", brandCode),
                    new SqlParameter("@storeCode", storeCode),
                    new SqlParameter("@positionId", positionId),
                    new SqlParameter("@probationSalary", double.Parse(probationSalary)),
                    new SqlParameter("@baseSalary", double.Parse(baseSalary)),
                    new SqlParameter("@additionalSalary", double.Parse(additionalSalary)),
                    new SqlParameter("@bankAccount", bankAccount),
                    new SqlParameter("@isPrivateAccount", isPrivateAccount),
                    new SqlParameter("@submittedAllDocuments", submittedAllDocuments),
                    new SqlParameter("@employeeId", employeeId)
                };

                result = Configuration.ExecuteNonQuery(Configuration.ConnectionString, Configuration.UpdateEmployee, paras);
            }
            catch (Exception ex) { }
            return result;
        }

        /// <summary>
        /// Gets the employee details.
        /// </summary>
        /// <param name="empCode">The emp code.</param>
        /// <returns></returns>
        [WebMethod]
        public static string GetEmployeeDetails(string empCode)
        {
            if (HttpContext.Current.Session["username"] == null)
            {
                return "";
            }
            try
            {
                List<SqlParameter> paras = new List<SqlParameter>()
                {
                    new SqlParameter("@employeeNo", SqlDbType.VarChar, 20),
                };

                paras[0].Value = empCode;

                return Configuration.ExecuteQuery(Configuration.ConnectionString, Configuration.GetEmployeeDetails, paras);
            }
            catch (Exception ex) { throw; }
        }

        /// <summary>
        /// Gets the document list.
        /// </summary>
        /// <param name="employeeId">The employee identifier.</param>
        /// <returns></returns>
        [WebMethod]
        public static string GetDocumentList(int employeeId)
        {
            if (HttpContext.Current.Session["username"] == null)
            {
                return "";
            }
            try
            {
                List<SqlParameter> paras = new List<SqlParameter>()
                {
                    new SqlParameter("@employeeId", employeeId)
                };

                return Configuration.ExecuteQuery(Configuration.ConnectionString, Configuration.GetAllDocuments, paras);
            }
            catch { throw; }
        }

        /// <summary>
        /// Saves the termination employee.
        /// </summary>
        /// <param name="employeeId">The employee identifier.</param>
        /// <param name="empCode">The emp code.</param>
        /// <param name="estStartDate">The est start date.</param>
        /// <param name="startDate">The start date.</param>
        /// <param name="brandCode">The brand code.</param>
        /// <param name="storeCode">The store code.</param>
        /// <param name="positionId">The position identifier.</param>
        /// <param name="probationSalary">The probation salary.</param>
        /// <param name="baseSalary">The base salary.</param>
        /// <param name="additionalSalary">The additional salary.</param>
        /// <param name="bankAccount">The bank account.</param>
        /// <param name="isPrivateAccount">if set to <c>true</c> [is private account].</param>
        /// <param name="submittedAllDocuments">if set to <c>true</c> [submitted all documents].</param>
        /// <param name="endDate">The end date.</param>
        /// <param name="reason">The reason.</param>
        /// <returns></returns>
        [WebMethod]
        public static bool SaveTerminationEmployee(
            int employeeId,
            string empCode,
            string estStartDate,
            string startDate,
            string brandCode,
            string storeCode,
            string positionId,
            string probationSalary,
            string baseSalary,
            string additionalSalary,
            string bankAccount,
            bool isPrivateAccount,
            bool submittedAllDocuments,
            string endDate,
            string reason)
        {
            bool result = false;
            try
            {
                List<SqlParameter> paras = new List<SqlParameter>()
                {
                    new SqlParameter("@employeeNo", empCode),
                    new SqlParameter("@estStartDate", DateTime.ParseExact(estStartDate, "MM/dd/yyyy", CultureInfo.InvariantCulture)),
                    new SqlParameter("@startDate", DateTime.ParseExact(startDate, "MM/dd/yyyy", CultureInfo.InvariantCulture)),
                    new SqlParameter("@brandCode", brandCode),
                    new SqlParameter("@storeCode", storeCode),
                    new SqlParameter("@positionId", positionId),
                    new SqlParameter("@probationSalary", double.Parse(probationSalary)),
                    new SqlParameter("@baseSalary", double.Parse(baseSalary)),
                    new SqlParameter("@additionalSalary", double.Parse(additionalSalary)),
                    new SqlParameter("@bankAccount", bankAccount),
                    new SqlParameter("@isPrivateAccount", isPrivateAccount),
                    new SqlParameter("@submittedAllDocuments", submittedAllDocuments),
                    new SqlParameter("@employeeId", employeeId),
                    new SqlParameter("@terminationDate", DateTime.ParseExact(endDate, "MM/dd/yyyy", CultureInfo.InvariantCulture)),
                    new SqlParameter("@terminationReason", reason)
                };

                result = Configuration.ExecuteNonQuery(Configuration.ConnectionString, Configuration.UpdateTerminationEmployee, paras);
            }
            catch (Exception ex) { }
            return result;
        }

        [WebMethod]
        public static bool SaveSubmittedDocuments(int id, bool isSubmitted, string submittedDate, int employeeId)
        {
            if (HttpContext.Current.Session["username"] == null)
            {
                return false;
            }
            try
            {
                List<SqlParameter> paras = new List<SqlParameter>
                {
                    new SqlParameter("@employeeId", employeeId),
                    new SqlParameter("@submitted", isSubmitted),
                    new SqlParameter("@documentId", id),
                    new SqlParameter("@updatedDate", DateTime.ParseExact(submittedDate, "MM/dd/yyyy", CultureInfo.InvariantCulture))
                };
                string result = Configuration.ExecuteNonQuery(Configuration.ConnectionString, Configuration.SubmitDocument, paras).ToString();
                return true;
            }
            catch(Exception ex) { throw; }
        }

        //private class SubmittedDocument
        //{
        //    public int id { get; set; }
        //    public bool isSubmitted { get; set; }
        //    public string submittedDate { get; set; }
        //    public int employeeId { get; set; }
        //}


        // Converts the specified JSON string to an object of type T
        //public static T Deserialize<T>(string context)
        //{
        //    string jsonData = context;

        //    //cast to specified objectType
        //    var obj = (T)new System.Web.Script.Serialization.JavaScriptSerializer().Deserialize<T>(jsonData);
        //    return obj;
        //}
    }
}