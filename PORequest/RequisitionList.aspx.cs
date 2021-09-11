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
    public partial class PRList : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["userid"] == null) Response.Redirect("~/Login.aspx");
           
                if (!this.IsPostBack)
            {
                InitializeData();
            }
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            IEnumerable<DataRow> rows = (ViewState["MyPRList"] as DataTable).AsEnumerable();
            //var q = rows;

            if (txtPR.Text.Length > 0 || cboBrand.Text != "--" || cboReqStatus.Text != "--" || txtCreatedFrom.Text != "" || txtCreatedTo.Text!= "" )
            {
                if (txtPR.Text.Length > 0)
                    rows = rows.Where(r => r["PRNo"].ToString().ToLower().Contains(txtPR.Text.ToLower()));

                if (cboBrand.Text != "--")
                    rows = rows.Where(r => r["BrandCode"].ToString() == cboBrand.Text || r["DeptCost"].ToString() == cboBrand.Text);

                if (cboReqStatus.Text != "--")
                    rows = rows.Where(r => r["PRStatusId"].ToString() == cboReqStatus.SelectedValue.ToString());


                if (txtCreatedFrom.Text != "")
                {
                    DateTime createdDate = DateTime.ParseExact(txtCreatedFrom.Text, Configurations.DatetimeFormat, CultureInfo.InvariantCulture);
                    rows = rows.Where(r => (DateTime)r["CreatedDate"] >= createdDate);
                }

                if (txtCreatedTo.Text != "")
                {
                    DateTime createdDate = DateTime.ParseExact(txtCreatedTo.Text, Configurations.DatetimeFormat, CultureInfo.InvariantCulture);
                    rows = rows.Where(r => (DateTime)r["CreatedDate"] <= createdDate);
                }

                if (rows.Count() > 0)
                {
                    ViewState["FilteredPR"] = rows.ToList().CopyToDataTable();
                    dgPR.DataSource = ViewState["FilteredPR"] as DataTable;
                }
                else
                {
                    ViewState["FilteredPR"] = null;
                    dgPR.DataSource = ViewState["FilteredPR"] as DataTable;
                }
            }
            else
            {
                ViewState["FilteredPR"] = null;
                dgPR.DataSource = ViewState["MyPRList"] as DataTable;
            }

            dgPR.DataBind();
        }

        #region Private helpers

        private void InitializeData()
        {
            try
            {
                DataTable tb = new DataTable();
                tb = Configurations.ExecuteQueryData(Configurations.ConnectionString, Configurations.GetRequisitionStatus, null);

                ViewState["PRStatus"] = tb;
                cboReqStatus.DataSource = tb;
                cboReqStatus.DataTextField = "StatusText";
                cboReqStatus.DataValueField = "id";
                cboReqStatus.DataBind();
                cboReqStatus.Items.Insert(0, "--");

                string[] brands = Session["brand"].ToString().Split('-');
                if (brands.Length == 1)
                {
                    cboBrand.Items.Add(brands[0]);
                }
                else
                {
                    cboBrand.Items.Add("--");
                    foreach (string brand in brands)
                    {
                        cboBrand.Items.Add(brand.Trim());
                    }
                }
                
                if (Session["brand"].ToString().ToUpper() == "ADMIN")
                {
                    tb = Configurations.ExecuteQueryData(Configurations.ConnectionString, Configurations.GetAdminPR, null);
                }
                else if ((bool)Session["BM"])
                {
                    string deptFilter = " BrandCode IN (" + Session["brandgroup"].ToString() + ") ";
                    string deptCostFilter = " DeptCost IN (" + Session["brandgroup"].ToString() + ") ";
                    tb = Configurations.ExecuteQueryData(Configurations.ConnectionString, string.Format(Configurations.GetBMPR, deptFilter, deptCostFilter), new List<SqlParameter>() { new SqlParameter("@UserId", Session["UserId"].ToString()) });
                }
                else
                {
                    tb = Configurations.ExecuteQueryData(Configurations.ConnectionString, Configurations.GetMyPR, new List<SqlParameter>() { new SqlParameter("@UserId", Session["UserId"].ToString()) });
                }

                ViewState["MyPRList"] = tb;
                dgPR.DataSource = tb;
                dgPR.DataBind();
            }
            catch (Exception ex) { }
        }


        #endregion


        protected void dgPR_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            if (ViewState["FilteredPR"] == null)
            {
                dgPR.DataSource = ViewState["MyPRList"] as DataTable;
            }
            else
            {
                dgPR.DataSource = ViewState["FilteredPR"] as DataTable;
            }
            
            dgPR.PageIndex = e.NewPageIndex;
            dgPR.DataBind();
        }

        protected void lnkPO_DataBinding(object sender, EventArgs e)
        {
            HyperLink lnk = (HyperLink)sender;
            GridViewRow row = (GridViewRow)lnk.NamingContainer;
            DataRow dr = (row.DataItem as DataRowView).Row;

            int statusId = (int)dr["PRStatusId"];

            // Requisition mới tạo
            if (statusId == 0)
            {
                if (string.Compare(dr["UserID"].ToString(), Session["UserId"].ToString(), true) == 0)
                {
                    lnk.NavigateUrl = "~/CreateRequisition.aspx?prno=" + lnk.Text;
                }
            }
            // Requisition đã submit => chỉ view
            else
            {
                lnk.NavigateUrl = "~/RequisitionApproval.aspx?prno=" + lnk.Text;
            }
        }
    }
}