using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using PORequest.Common;

namespace PORequest
{
    public partial class Supplier : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["userid"] == null) Response.Redirect("~/Login.aspx");
            if (!this.IsPostBack)
            {
                Initialize();
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            SaveSupplier();
        }


        protected void dgSuppliers_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            dgSuppliers.DataSource = ViewState["Suppliers"] as DataTable;
            dgSuppliers.PageIndex = e.NewPageIndex;
            dgSuppliers.DataBind();
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtName.Text) && string.IsNullOrEmpty(txtTaxCode.Text) && string.IsNullOrEmpty(txtAddress.Text) && string.IsNullOrEmpty(txtAddressCode.Text))
            {
                dgSuppliers.DataSource = ViewState["Suppliers"] as DataTable;
                dgSuppliers.AllowPaging = true;
                ViewState["Searching"] = false;
            }
            else
            {
                DataTable tb = ViewState["Suppliers"] as DataTable;
                IEnumerable<DataRow> rows = tb.AsEnumerable();
                var q = rows;
                
                if (!string.IsNullOrEmpty(txtName.Text))
                {
                    q = rows.Where(r => r["name"].ToString().ToLower().Contains(txtName.Text.ToLower()));
                }

                if (!string.IsNullOrEmpty(txtTaxCode.Text))
                {
                    q = rows.Where(r => r["taxcode"].ToString().ToLower().Contains(txtTaxCode.Text.ToLower()));
                }

                if (!string.IsNullOrEmpty(txtAddress.Text))
                {
                    q = rows.Where(r => r["address"].ToString().ToLower().Contains(txtAddress.Text.ToLower()));
                }

                if (!string.IsNullOrEmpty(txtAddressCode.Text))
                {
                    q = rows.Where(r => r["addresscode"].ToString().ToLower().Contains(txtAddressCode.Text.ToLower()));
                }

                DataTable results = q.CopyToDataTable();
                dgSuppliers.DataSource = results;
                dgSuppliers.AllowPaging = false;
                ViewState["Searching"] = true;
            }

            dgSuppliers.DataBind();
        }

        protected void dgSuppliers_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (dgSuppliers.SelectedIndex != -1)
            {
                GridViewRow row = dgSuppliers.SelectedRow;
                DisplaySupplier(row);
            }
            else
            {
                ClearSupplierInfo();
            }
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            dgSuppliers.SelectedIndex = -1;
            ClearSupplierInfo();
        }

        #region private helpers

        private void Initialize()
        {
            ViewState["Searching"] = false;
            try
            {
                DataTable tb = new DataTable();
                tb = Configurations.ExecuteQueryData(Configurations.ConnectionString, Configurations.GetSuppliers, null);
                dgSuppliers.DataSource = tb;
                dgSuppliers.DataBind();

                ViewState["Suppliers"] = tb;
            }
            catch { }
        }

        private void DisplaySupplier(GridViewRow row)
        {
            hdfId.Value = row.Cells[1].Text.ToString();
            txtAddressCode.Text = HttpUtility.HtmlDecode(row.Cells[2].Text.ToString());
            txtName.Text = HttpUtility.HtmlDecode(row.Cells[3].Text.ToString());
            txtTaxCode.Text = HttpUtility.HtmlDecode(row.Cells[4].Text.ToString());
            txtAddress.Text = HttpUtility.HtmlDecode(row.Cells[5].Text.ToString());
        }

        private void ClearSupplierInfo()
        {
            hdfId.Value = "";
            txtAddressCode.Text = "";
            txtName.Text = "";
            txtTaxCode.Text = "";
            txtAddress.Text = "";
        }

        private void SaveSupplier()
        {
            List<SqlParameter> paras = new List<SqlParameter>()
            {
                new SqlParameter("@AddressCode", txtAddressCode.Text),
                new SqlParameter("@Address", txtAddress.Text),
                new SqlParameter("@Name", txtName.Text),
                new SqlParameter("@TaxCode", txtTaxCode.Text)
            };

            if (hdfId.Value == "")
            {
                Configurations.ExecuteNonQuery(Configurations.ConnectionString, Configurations.CreateSupplier, paras);
            }
            else
            {
                paras.Add(new SqlParameter("@Id", int.Parse(hdfId.Value.ToString())));

                Configurations.ExecuteNonQuery(Configurations.ConnectionString, Configurations.UpdateSupplier, paras);
            }
        }

        #endregion

    }
}