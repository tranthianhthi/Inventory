using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Globalization;
using System.Web.Services;

using Recruitment.Common;

namespace Recruitment.CandidatePool
{
    public partial class CandidateReport : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        [WebMethod]
        public static string ReportCandidateStatus(string fromDate, string toDate)
        {
            try
            {
                List<SqlParameter> paras = new List<SqlParameter>()
                {
                    new SqlParameter("@fromDate", DateTime.ParseExact(fromDate, "MM/dd/yyyy", CultureInfo.InvariantCulture)),
                    new SqlParameter("@toDate", DateTime.ParseExact(toDate, "MM/dd/yyyy", CultureInfo.InvariantCulture)),
                };
                return Configuration.ExecuteQuery(Configuration.ConnectionString, Configuration.ReportCandidateStatus, paras);
                
            }
            catch(Exception ex) { throw; }
        }

        [WebMethod]
        public static string ReportEmployeeNotSubmittedAllDocuments(string fromDate, string toDate)
        {
            try
            {
                List<SqlParameter> paras = new List<SqlParameter>()
                {
                    new SqlParameter("@fromDate", DateTime.ParseExact(fromDate, "MM/dd/yyyy", CultureInfo.InvariantCulture)),
                    new SqlParameter("@toDate", DateTime.ParseExact(toDate, "MM/dd/yyyy", CultureInfo.InvariantCulture)),
                };
                return Configuration.ExecuteQuery(Configuration.ConnectionString, Configuration.ReportEmployeeNotSubmittedAllDocuments, paras);

            }
            catch (Exception ex) { throw; }
        }

        [WebMethod]
        public static string ReportEmployeeEachBrand(string fromDate, string toDate)
        {
            try
            {
                List<SqlParameter> paras = new List<SqlParameter>()
                {
                    new SqlParameter("@fromDate", DateTime.ParseExact(fromDate, "MM/dd/yyyy", CultureInfo.InvariantCulture)),
                    new SqlParameter("@toDate", DateTime.ParseExact(toDate, "MM/dd/yyyy", CultureInfo.InvariantCulture)),
                };
                return Configuration.ExecuteQuery(Configuration.ConnectionString, Configuration.ReportEmployeeEachBrand, paras);

            }
            catch (Exception ex) { throw; }
        }
    }
}