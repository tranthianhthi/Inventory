using Recruitment.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Web;
using System.Web.Services;

namespace Recruitment.CandidatePool
{
    public partial class CandidatePool : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }
        

        /// <summary>
        /// Lấy danh sách ứng viên.
        /// </summary>
        /// <returns>Chuỗi json parse từ table chứa thông tin ứng viên</returns>
        [WebMethod]
        public static string LoadCandidates(string statusId)
        {
            int status = int.Parse(statusId);
            try
            {
                if (status == 0)
                {
                    string res =  Configuration.ExecuteQuery(Configuration.ConnectionString, Configuration.GetCandidates, null);
                    return res;
                }
                else
                {
                    List<SqlParameter> paras = new List<SqlParameter>() { new SqlParameter("@status_id", status) };
                    return Configuration.ExecuteQuery(Configuration.ConnectionString, Configuration.GetCandidatesByStatus, paras);
                }
            }
            catch(Exception ex) { throw; }
        }

        /// <summary>
        /// Loads the status.
        /// </summary>
        /// <returns></returns>
        [WebMethod]
        public static string LoadStatus()
        {
            try
            {
                string jsonString = Configuration.ExecuteQuery(Configuration.ConnectionString, Configuration.GetAllStatus, null);//Configuration.ConvertTableToJsonString(tb);
                return jsonString;
            }
            catch(Exception ex) { throw; }
        }

        [WebMethod]
        public static bool UpdateCandidateStatus(int id, int newStatusId, string note)
        {
            bool result = false;

            if (HttpContext.Current.Session["username"] == null)
            {
                return false;
            }
            try
            {
                List<SqlParameter> sqlParams = new List<SqlParameter>()
                {
                    new SqlParameter("@id", id),
                    new SqlParameter("@status_id", newStatusId),
                    new SqlParameter("@modified_by", HttpContext.Current.Session["username"])
                };

                if (string.IsNullOrEmpty(note))
                {
                    result = Configuration.ExecuteNonQuery(Configuration.ConnectionString, Configuration.UpdateCandidateStatusWithoutNote, sqlParams);//Configuration.ConvertTableToJsonString(tb);
                }
                else
                {
                    sqlParams.Add(new SqlParameter("@note", string.Format("{0}: {1}", DateTime.Now.ToString("MM/dd/yyyy HH:mm"), note)));
                    result = Configuration.ExecuteNonQuery(Configuration.ConnectionString, Configuration.UpdateCandidateStatusWithNote, sqlParams);//Configuration.ConvertTableToJsonString(tb);
                }
            }
            catch (Exception ex) {  }
            return result;
        }

        /// <summary>
        /// Updates the status of candidate & create new employee information
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="newStatusId">The new status identifier.</param>
        /// <param name="brandCode">The brand code.</param>
        /// <param name="note">The note.</param>
        /// <returns></returns>
        [WebMethod]
        public static bool UpdateToNewEmployeeStatus(int id, int newStatusId, string brandCode, string note)
        {
            bool result = false;
            List<SqlParameter> sqlParams;

            if (HttpContext.Current.Session["username"] == null)
            {
                return false;
            }

            try
            {
                result = UpdateCandidateStatus(id, newStatusId, note);//Configuration.ConvertTableToJsonString(tb);

                if (result)
                {
                    sqlParams = new List<SqlParameter>()
                    {
                        new SqlParameter("@id", id),
                        new SqlParameter("@brandCode", brandCode)
                    };

                    result = Configuration.ExecuteNonQuery(Configuration.ConnectionString, Configuration.InsertEmployee, sqlParams);
                }
            }
            catch (Exception ex) { }
            return result;
        }

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

    }
}