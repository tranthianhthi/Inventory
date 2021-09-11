using PORequest.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Web.UI.WebControls;

namespace PORequest
{
    public partial class POList : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //Session["UserId"] = "Tester";
            if (Session["userid"] == null)
                Response.Redirect("~/Login.aspx");
            else
            {
                if (!IsPostBack)
                {
                    InitializeData();
                }
            }
        }
        protected void btnSearch_Click(object sender, EventArgs e)
        {
           
            IEnumerable<DataRow> rows = (ViewState["MyPOList"] as DataTable).AsEnumerable();
            if (txtPO.Text.Length > 0)
                rows = rows.Where(r => r["PONo"].ToString().ToLower().Contains(txtPO.Text.ToLower()));
            if (txtUser.Text.Length > 0)
                rows = rows.Where(r => r["Userid"].ToString().ToLower().Contains(txtUser.Text.ToLower()));
            if (rows.Count() > 0)
            {
                dgPO.DataSource = rows.CopyToDataTable();
            }
            else
            {
                dgPO.DataSource = null;
            }
           
            dgPO.DataBind();
        }
        #region private helper

        private void InitializeData()
        {
            try
            {
                DataTable tb = new DataTable();

                tb = Configurations.ExecuteQueryData(Configurations.ConnectionString, Configurations.GetListPO, new List<SqlParameter>() { new SqlParameter("@UserId", Session["UserId"].ToString()) });
                //tb = Configurations.ExecuteQueryData(Configurations.ConnectionString, Configurations.GetMyPO, new List<SqlParameter>() { new SqlParameter("@UserId", Session["UserId"].ToString()) });
                ViewState["MyPOList"] = tb;
                dgPO.DataSource = tb;
                dgPO.DataBind();
            }
            catch { }
        }
        #endregion
        protected void dgPO_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            dgPO.DataSource = ViewState["MyPOList"] as DataTable;
            dgPO.PageIndex = e.NewPageIndex;
            dgPO.DataBind();
        }
        protected void lnkPO_DataBinding(object sender, EventArgs e)
        {
            HyperLink lnk = (HyperLink)sender;
            lnk.NavigateUrl = "~/CreatePO.aspx?pono=" + lnk.Text;
        }
    }
}