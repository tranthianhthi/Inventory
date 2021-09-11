using CrystalDecisions.CrystalReports.Engine;
using PORequest.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace PORequest
{
    public partial class CreatePO : System.Web.UI.Page
    {
        readonly Configurations configurations = new Configurations();
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["userid"] == null)
                Response.Redirect("~/Login.aspx");
            else
            {
                txtUserName.Text = Session["userid"].ToString();
                //cboDept.SelectedValue = Session["brand"].ToString();

                if (!this.IsPostBack)
                {
                    ViewState["RemovedId"] = "";
                    ViewState["SelectedPRs"] = new List<SelectedPR>();
                    InitializeCommonData();

                    if (Request.QueryString["pono"] == null)
                    {
                        InitializeData();
                    }
                    else
                    {
                        InitializeData(Request.QueryString["pono"].ToString());
                    }
                }
            }
        }       

        /// <summary>
        /// Thêm item vào PO detail.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void btnAdd_Click(object sender, EventArgs e)
        {
            DataTable tb = (DataTable)ViewState["PODetails"];
            IEnumerable<DataRow> rows = tb.AsEnumerable();
            DataRow row = rows.FirstOrDefault(r => r["Line"].ToString() == txtSTT.Text);

            if (row != null)
            {
                GetPODetailFromInterface(row);

                ViewState["PODetails"] = rows.CopyToDataTable();
            }
            else
            {
                row = tb.NewRow();

                GetPODetailFromInterface(row);
                row["id"] = 0;
                row["Line"] = int.Parse(txtSTT.Text);

                tb.Rows.Add(row);

                ViewState["PODetails"] = tb;
            }

            dgItems.DataSource = ViewState["PODetails"] as DataTable;
            dgItems.DataBind();

            CalculateTotalAmount();
            ClearItemDisplay();
        }

        #region EVENTS: txtprice - txtqty - cbotax changed

        /// <summary>
        /// Chọn mức thuế VAT - tính lại giá trị PO
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void cboVAT_SelectedIndexChanged(object sender, EventArgs e)
        {
            CalculateAmount();
        }

        /// <summary>
        /// Cập nhật giá item => tính toán lại số tiền.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void txtPrice_TextChanged(object sender, EventArgs e)
        {
            //txtPrice.Text = txtPrice.Text.Length == 0 ? "0" : double.Parse(txtPrice.Text).ToString(Configurations.NumberFormat);
            CalculateAmount();
        }

        /// <summary>
        /// Cập nhật số lượng item => tính toán lại số tiền
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void txtQty_TextChanged(object sender, EventArgs e)
        {
            //txtQty.Text = txtQty.Text.Length == 0 ? "0" : double.Parse(txtQty.Text).ToString(Configurations.NumberFormat);
            CalculateAmount();
        }

        #endregion

        /// <summary>
        /// Xóa item khỏi danh sách PO detail.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Web.UI.WebControls.GridViewDeleteEventArgs"/> instance containing the event data.</param>
        protected void dgItems_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            int index = Convert.ToInt32(e.RowIndex);
            DataTable tb = ViewState["PODetails"] as DataTable;

            DataRow row = tb.Rows[index];

            DeleteItem(row, int.Parse(dgItems.DataKeys[e.RowIndex].Value.ToString()));
        }

        /// <summary>
        /// Lưu thông tin xuống database
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                #region check data
                if (txtDeliveryDate.Text == "") { configurations.ShowThongBao("Hãy nhập lại ngày hoàn thành.", this); txtDeliveryDate.Focus(); return; }
              
                #endregion
                string PRRef = "";
                foreach(ListItem item in chklstPR.Items)
                {
                    PRRef += item.Selected ? item.Text.ToString() + "," : "";
                }
                PRRef = PRRef.EndsWith(",")? PRRef.Substring(0, PRRef.Length - 1) : PRRef;
                List<SqlParameter> paras = new List<SqlParameter>()
                {
                    new SqlParameter() { ParameterName = "@SubNo", Value = rdbSub1.Checked? 1 : 2 },
                   // new SqlParameter() { ParameterName = "@PRNoRef", Value = PRRef },
                    new SqlParameter() { ParameterName = "@PRNoRef", Value = txtListPR.Text },
                    new SqlParameter() { ParameterName = "@Brand", Value = cboDept.SelectedValue.ToString()},
                    new SqlParameter() { ParameterName = "@SupplierCode", Value = cboVendor.SelectedValue.ToString()},
                    new SqlParameter() { ParameterName = "@IsContract", Value = chkHaveContract.Checked },
                    new SqlParameter() { ParameterName = "@Note", Value = txtNote.Text },
                    new SqlParameter() { ParameterName = "@Amount", Value = string.IsNullOrWhiteSpace(txtPOAmount.Text) ? 0 : double.Parse(txtPOAmount.Text) },
                    new SqlParameter() { ParameterName = "@VAT", Value = string.IsNullOrWhiteSpace(txtPOVATAmount.Text) ? 0 : double.Parse(txtPOVATAmount.Text) },
                    new SqlParameter() { ParameterName = "@TotalAmount",Value = string.IsNullOrWhiteSpace(txtPOTotalAmount.Text) ? 0 : double.Parse(txtPOTotalAmount.Text) },
                    new SqlParameter() { ParameterName = "@UserId", Value = Session["UserId"].ToString() }
                };
                                DateTime dt = string.IsNullOrWhiteSpace(txtDeliveryDate.Text) ? DateTime.MinValue : DateTime.ParseExact(txtDeliveryDate.Text, Configurations.DatetimeFormat, CultureInfo.InvariantCulture);
                if (dt == DateTime.MinValue)
                {
                    paras.Add(new SqlParameter() { ParameterName = "@DeliveryDate", Value = DBNull.Value });
                }
                else
                {
                    paras.Add(new SqlParameter() { ParameterName = "@DeliveryDate", Value = dt });
                }
                /* Trường hợp có deposit */
                paras.Add(new SqlParameter() { ParameterName = "@IsDeposit", Value = chkDeposit.Checked });
                if (chkDeposit.Checked)
                {
                    paras.Add(new SqlParameter() { ParameterName = "@Deposit", Value = string.IsNullOrWhiteSpace(txtDeposit.Text) ? 0 : double.Parse(txtDeposit.Text) });
                    DateTime returnDeposit = string.IsNullOrWhiteSpace(txtReturnDeposit.Text) ? DateTime.MinValue : DateTime.ParseExact(txtReturnDeposit.Text, Configurations.DatetimeFormat, CultureInfo.InvariantCulture);
                    if (returnDeposit == DateTime.MinValue)
                    {
                        paras.Add(new SqlParameter() { ParameterName = "@ReturnDeposit", SqlDbType = SqlDbType.DateTime, Value = DBNull.Value });
                    }
                    else
                    {
                        paras.Add(new SqlParameter() { ParameterName = "@ReturnDeposit",  Value = returnDeposit });
                    }
                }
                else
                {
                    paras.Add(new SqlParameter() { ParameterName = "@Deposit", Value = DBNull.Value });
                    paras.Add(new SqlParameter() { ParameterName = "@ReturnDeposit", SqlDbType = SqlDbType.DateTime, Value = DBNull.Value });
                }

                int POId;
                string PONo;

                if (string.IsNullOrWhiteSpace(txtPONo.Text) )
                {
                    paras.InsertRange( 0, 
                        new SqlParameter[] 
                        { new SqlParameter() { ParameterName = "@Id", Direction = ParameterDirection.InputOutput, Value = 0 },
                        new SqlParameter() { ParameterName = "@PONo", Direction = ParameterDirection.InputOutput, Value = "", Size = 20 } });

                    Configurations.ExecuteNonQuery(Configurations.ConnectionString, Configurations.CreatePOMaster, paras);  
                    POId = (int)paras[0].Value;
                    PONo = paras[1].Value.ToString();
                }
                else
                {
                    POId = int.Parse(id.Value);
                    PONo = txtPONo.Text;

                    paras.InsertRange(0,
                        new SqlParameter[]
                        { new SqlParameter() { ParameterName = "@Id", Value = POId },
                        new SqlParameter() { ParameterName = "@PONo", Value = PONo } });

                    Configurations.ExecuteNonQuery(Configurations.ConnectionString, Configurations.UpdatePOMaster, paras);
                }

                string[] removedIds = ViewState["RemovedId"].ToString().Split(';');
                foreach (string id in removedIds)
                {
                    if (!string.IsNullOrWhiteSpace(id))
                    {
                        Configurations.ExecuteNonQuery(Configurations.ConnectionString, Configurations.DeletePODetail, new List<SqlParameter>() { new SqlParameter("id", int.Parse(id)) });
                    }
                }
                if(ViewState["PODetails"]==null || (ViewState["PODetails"] as DataTable).Rows.Count==0)
                {
                    if (txtItemName.Text == "") { configurations.ShowThongBao("Hãy nhập chi tiết hàng hóa.", this); txtItemName.Focus(); return; }
                    if (txtPrice.Text == "") { configurations.ShowThongBao("Hãy nhập giá chi tiết giá cho hàng hóa.", this); txtPrice.Focus(); return; }
                    if (txtAmount.Text == "") { configurations.ShowThongBao("Chưa có tổng giá trị PR. Hãy nhập lại chi tiết hàng hóa.", this); txtAmount.Focus(); return; }
                    if (txtVATAmount.Text == "") { configurations.ShowThongBao("Chưa có tổng giá trị VAT cho PR. Hãy nhập lại chi tiết hàng hóa.", this); txtVATAmount.Focus(); return; }
                    if (txtTotalAmount.Text == "") { configurations.ShowThongBao("Chưa có tổng giá trị cho PR. Hãy nhập lại chi tiết hàng hóa.", this); txtTotalAmount.Focus(); return; }
                    if (txtPOAmount.Text == "") { configurations.ShowThongBao("Chưa có tổng giá trị cho PR. Hãy nhập lại chi tiết hàng hóa.", this); txtPOAmount.Focus(); return; }
                    if (txtPOVATAmount.Text == "") { configurations.ShowThongBao("Chưa có tổng giá trị VAT cho PR. Hãy nhập lại chi tiết hàng hóa.", this); txtPOVATAmount.Focus(); return; }
                    if (txtPOTotalAmount.Text == "") { configurations.ShowThongBao("Chưa có tổng giá trị cho PR. Hãy nhập lại chi tiết hàng hóa.", this); txtPOTotalAmount.Focus(); return; }
                }                
                foreach (DataRow row in (ViewState["PODetails"] as DataTable).Rows)
                {
                    List<SqlParameter> paraDetail = new List<SqlParameter>()
                    {
                        new SqlParameter() { ParameterName = "@POId", Value = POId },
                        new SqlParameter() { ParameterName = "@PONo", Value = PONo },
                        new SqlParameter() { ParameterName = "@Line", Value = row["Line"] },
                        new SqlParameter() { ParameterName = "@VAT", Value = row["VAT"] },
                        new SqlParameter() { ParameterName = "@ItemName", Value = row["ItemName"].ToString() },
                        new SqlParameter() { ParameterName = "@UnitPrice", Value = row["UnitPrice"].ToString() },
                        new SqlParameter() { ParameterName = "@Qty", Value = row["Qty"] },
                        new SqlParameter() { ParameterName = "@Amount", Value = row["Amount"] },
                        new SqlParameter() { ParameterName = "@TotalVAT", Value = row["TotalVAT"] },
                        new SqlParameter() { ParameterName = "@TotalAmount", Value = row["TotalAmount"] },
                        new SqlParameter() { ParameterName = "@ItemNote", Value = row["ItemNote"] },
                        new SqlParameter() { ParameterName = "@UserId", Value = Session["UserId"].ToString() }
                    };
                    
                    if (row["id"] == null || int.Parse(row["id"].ToString()) == 0)
                    {
                        Configurations.ExecuteNonQuery(Configurations.ConnectionString, Configurations.CreatePODetail, paraDetail);
                    }
                    else
                    {
                        paraDetail.Insert(0, new SqlParameter("id", row["id"]));
                        Configurations.ExecuteNonQuery(Configurations.ConnectionString, Configurations.UpdatePODetail, paraDetail);
                    }
                }
                txtPONo.Text = PONo;
                configurations.ShowThongBao("Đã lưu PO vào hệ thống.", this);
            }
            catch (Exception ex) { configurations.ShowThongBao("Lỗi - không lưu được PO vào hệ thống.", this); };
        }

        /// <summary>
        /// Clear thông tin item trên giao diện.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void btnCancel_Click(object sender, EventArgs e)
        {
            ClearItemDisplay();
        }

        /// <summary>
        /// Chọn PO item và thể hiện lên giao diện để điều chỉnh.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void dgItems_SelectedIndexChanged(object sender, EventArgs e)
        {
            GridViewRow row = dgItems.SelectedRow;
            DisplayGridItem(row);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        
        protected void chklstPR_SelectedIndexChanged(object sender, EventArgs e)
        {           
            foreach (ListItem item in chklstPR.Items)
            {
                if (item.Selected)
                {
                    if (!txtListPR.Text.Contains(item.Text))
                    {
                        txtListPR.Text = txtListPR.Text + item.Text + "-";
                    }
                    //else { txtListPR.Text = txtListPR.Text.Replace(item.Text + "-", ""); }
                }
                else
                {
                    txtListPR.Text = txtListPR.Text.Replace(item.Text + "-", "");
                }
            }                           
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void cboDept_SelectedIndexChanged(object sender, EventArgs e)
        {
            string brandCode = cboDept.SelectedValue;
            LoadDepartmentPR(brandCode);
        }

        #region private helpers        

        /// <summary>
        /// Khởi tạo dữ liệu - tạo định dạng bảng details & lấy các dữ liệu khác từ DB
        /// </summary>
        private void InitializeData()
        {
            DataTable tb = new DataTable();

            /* ======================================================================================================================================
             * Khởi tạo bảng PO Detail
             * ====================================================================================================================================== */
            tb = CreatePODetailsTable();
            dgItems.DataSource = tb;
            dgItems.DataBind();
            ViewState["PODetails"] = tb;

            /* ======================================================================================================================================
             * Khởi tạo bảng POMaster
             * ====================================================================================================================================== */
            tb = CreatePOMasterTable();
            ViewState["PO"] = tb;

            /* ======================================================================================================================================
             * Khởi tạo các giá trị ViewState
             * ====================================================================================================================================== */

            ViewState["POAmount"] = 0.0;
            ViewState["POAmountVAT"] = 0.0;
            ViewState["POTotalAmount"] = 0.0;
            ViewState["IsEditing"] = false;

            txtPOAmount.Text = 0.ToString(Configurations.NumberFormat);
            txtPOVATAmount.Text = 0.ToString(Configurations.NumberFormat);
            txtPOTotalAmount.Text = 0.ToString(Configurations.NumberFormat);
            txtSTT.Text = "1";
            rdbSub1.Checked = true;

            try
            {
                /* ======================================================================================================================================
                 * Tải danh sách Vendor
                 * ====================================================================================================================================== */

                ViewState["Vendor"] = Configurations.ExecuteQueryData(Configurations.ConnectionString, Configurations.GetSuppliers, null);
                cboVendor.DataSource = ViewState["Vendor"] as DataTable;
                cboVendor.DataValueField = "id";
                cboVendor.DataTextField = "name";
                cboVendor.DataBind();

                /* ======================================================================================================================================
                 * Tải danh sách brand
                 * ====================================================================================================================================== */

                tb = Configurations.ExecuteQueryData(Configurations.ConnectionString, Configurations.GetBrands, null);
                ViewState["Brand"] = tb;

                cboDept.DataSource = tb;
                cboDept.DataValueField = "BrandCode";
                cboDept.DataTextField = "BrandCode";
                cboDept.DataBind();

            }
            catch { }
        }
        private void InitializeData(string pono)
        {
            DataTable tb = new DataTable();

            /* ======================================================================================================================================
             * Khởi tạo bảng PO Detail
             * ====================================================================================================================================== */
            tb = GetPODetails(pono);
            dgItems.DataSource = tb;
            dgItems.DataBind();
            ViewState["PODetails"] = tb;

            txtSTT.Text = (tb.Rows.Count + 1).ToString();

            /* ======================================================================================================================================
             * Khởi tạo bảng POMaster
             * ====================================================================================================================================== */
            tb = GetPOMaster(pono);
            ViewState["PO"] = tb;
            if (tb.Rows.Count == 1)
            {
                cboDept.SelectedValue = tb.Rows[0]["Brand"].ToString();
                cboVendor.SelectedValue = tb.Rows[0]["SupplierCode"].ToString();

                ViewState["POAmount"] = (double)tb.Rows[0]["Amount"];
                ViewState["POAmountVAT"] = (double)tb.Rows[0]["VAT"];
                ViewState["POTotalAmount"] = (double)tb.Rows[0]["TotalAmount"];

                txtPOAmount.Text = ((double)tb.Rows[0]["Amount"]).ToString(Configurations.NumberFormat);
                txtPOVATAmount.Text = ((double)tb.Rows[0]["VAT"]).ToString(Configurations.NumberFormat);
                txtPOTotalAmount.Text = ((double)tb.Rows[0]["TotalAmount"]).ToString(Configurations.NumberFormat);

                id.Value = tb.Rows[0]["id"].ToString();

                chkHaveContract.Checked = (bool)tb.Rows[0]["IsContract"];
                
                txtPONo.Text = tb.Rows[0]["PONo"].ToString();
                txtNote.Text = tb.Rows[0]["Note"].ToString();

                rdbSub1.Checked = tb.Rows[0]["SubNo"].ToString() == "1";
                rdbSub2.Checked = !rdbSub1.Checked;


                /* Deposit */
                chkDeposit.Checked = (bool)tb.Rows[0]["IsDeposit"];
                txtReturnDeposit.Enabled = chkDeposit.Checked;
                txtDeposit.Enabled = chkDeposit.Checked;

                txtDeposit.Text = tb.Rows[0]["Deposit"] == DBNull.Value ? "0" : tb.Rows[0]["Deposit"].ToString();
                txtReturnDeposit.Text = tb.Rows[0]["ReturnDeposit"] == DBNull.Value ? "" : ((DateTime)tb.Rows[0]["ReturnDeposit"]).ToString(Configurations.DatetimeFormat);


                txtDeliveryDate.Text = tb.Rows[0]["DeliveryDate"] == DBNull.Value ? "" : ((DateTime)tb.Rows[0]["DeliveryDate"]).ToString(Configurations.DatetimeFormat);

            }
            
        }
        private void InitializeCommonData()
        {
            try
            {
                /* ======================================================================================================================================
                 * Tải danh sách Vendor
                 * ====================================================================================================================================== */

                ViewState["Vendor"] = Configurations.ExecuteQueryData(Configurations.ConnectionString, Configurations.GetSuppliers, null);

                cboVendor.DataSource = ViewState["Vendor"] as DataTable;
                cboVendor.DataValueField = "id";
                cboVendor.DataTextField = "name";
                cboVendor.DataBind();

                /* ======================================================================================================================================
                 * Tải danh sách brand
                 * ====================================================================================================================================== */

                ViewState["Brand"] = Configurations.ExecuteQueryData(Configurations.ConnectionString, Configurations.GetBrands, null);

                cboDept.DataSource = ViewState["Brand"] as DataTable ;
                cboDept.DataValueField = "BrandCode";
                cboDept.DataTextField = "BrandCode";
                cboDept.DataBind();

            }
            catch { }
        }
        private DataTable GetPOMaster(string pono)
        {
            try
            {
                DataTable tb = new DataTable();
                tb = Configurations.ExecuteQueryData(Configurations.ConnectionString, Configurations.GetPO, new List<SqlParameter>() { new SqlParameter("PONo", pono) });
                return tb;
            }
            catch { throw; }
        }
        private DataTable GetPODetails(string pono)
        {
            DataTable tb = new DataTable();
            tb = Configurations.ExecuteQueryData(Configurations.ConnectionString, Configurations.GetPODetail, new List<SqlParameter>() { new SqlParameter("PONo", pono) });
            return tb;
        }
        private DataTable CreatePODetailsTable()
        {
            DataTable tb = new DataTable();
            tb.Columns.Add("id");
            tb.Columns.Add("Line");
            tb.Columns.Add("ItemName");
            tb.Columns.Add("ChargeToDept");
            tb.Columns.Add("UnitPrice");
            tb.Columns.Add("VAT");
            tb.Columns.Add("Qty");
            tb.Columns.Add("Amount");
            tb.Columns.Add("TotalVAT");
            tb.Columns.Add("TotalAmount");
            tb.Columns.Add("ItemNote");
            return tb;
        }
        private DataTable  CreatePOMasterTable()
        {
            DataTable tb = new DataTable();
            tb.Columns.Add("id");
            tb.Columns.Add("PRNo");
            tb.Columns.Add("BrandCode");
            tb.Columns.Add("BrandName");
            tb.Columns.Add("DisplayText");
            return tb;
        }

        /// <summary>
        /// Tính toán các giá trị : số tiền - vat - tổng thành tiền
        /// </summary>
        private void CalculateAmount()
        {
            double price = string.IsNullOrWhiteSpace(txtPrice.Text)? 0 : double.Parse(txtPrice.Text);
            double vat = (cboVAT.SelectedValue == "0" || cboVAT.SelectedValue == "") ? 0 : double.Parse(cboVAT.SelectedValue) / 100;
            int qty = string.IsNullOrWhiteSpace(txtQty.Text)? 0 : int.Parse(txtQty.Text);

            double amount = price * qty;
            double vatAmount = amount * vat;
            double totalAmout = amount + vatAmount;

            txtAmount.Text = amount.ToString(Configurations.NumberFormat);
            txtVATAmount.Text = vatAmount.ToString(Configurations.NumberFormat);
            txtTotalAmount.Text = totalAmout.ToString(Configurations.NumberFormat);

            //ViewState["POAmount"] = amount;
            //ViewState["POAmountVAT"] = vatAmount;
            //ViewState["POTotalAmount"] = totalAmout;
        }
        private void CalculateTotalAmount()
        {
            double amount = 0;
            double vat = 0;
            double totalAmt = 0;

            foreach (DataRow row in (ViewState["PODetails"] as DataTable).Rows)
            {
                amount += double.Parse(row["Amount"].ToString());
                vat += double.Parse(row["TotalVAT"].ToString());
                totalAmt += double.Parse(row["TotalAmount"].ToString());
            }

            ViewState["POAmount"] = amount;
            ViewState["POAmountVAT"] = vat;
            ViewState["POTotalAmount"] = totalAmt;

            txtPOAmount.Text = amount.ToString(Configurations.NumberFormat);
            txtPOVATAmount.Text = vat.ToString(Configurations.NumberFormat);
            txtPOTotalAmount.Text = totalAmt.ToString(Configurations.NumberFormat);
        }

        /// <summary>
        /// Xóa thông tin PO item trên giao diện - hủy thao tác chỉnh sửa.
        /// </summary>
        private void ClearItemDisplay()
        {
            DataTable tb = ViewState["PODetails"] as DataTable;

            txtSTT.Text = (tb.Rows.Count + 1).ToString();
            txtItemName.Text = "";
            txtItemNote.Text = "";
            txtPrice.Text = "";
            cboVAT.SelectedIndex = 0;
            txtQty.Text = "";
            txtAmount.Text = "";
            txtVATAmount.Text = "";
            txtTotalAmount.Text = "";
            txtItemNote.Text = "";
        }

        /// <summary>
        /// Xóa dòng đang chọn khỏi danh sách PO.
        /// </summary>
        /// <param name="row">The row.</param>
        private void DeleteItem(DataRow row, int key)
        {
            DataTable tb = ViewState["PODetails"] as DataTable;
            int line = int.Parse(row["Line"].ToString());

            /* ======================================================================================================================================
             * Tính lại số thứ tự các line
             * ====================================================================================================================================== */

            IEnumerable<DataRow> rows = tb.AsEnumerable().Where(r => int.Parse(r["Line"].ToString()) != line).ToList().AsEnumerable(); 
            IEnumerable<DataRow> modifyRows = rows.Where(r => int.Parse(r["Line"].ToString()) > line).ToList();

            foreach (DataRow r in modifyRows)
            {
                r["Line"] = int.Parse(r["Line"].ToString()) - 1;
            }

            if (key != 0)
            {
                ViewState["RemovedId"] = ViewState["RemovedId"].ToString() + key + ";";
            }

            /* ======================================================================================================================================
             * Xóa dòng đang chọn & bind dữ liệu lại vào grid.
             * ====================================================================================================================================== */

            tb = rows.CopyToDataTable();
            ViewState["PODetails"] = tb;
            dgItems.DataSource = tb;
            dgItems.DataBind();

            CalculateAmount();
            CalculateTotalAmount();

            txtSTT.Text = (tb.Rows.Count + 1).ToString();
        }

        /// <summary>
        /// Tạo po detail mới từ thông tin trên giao diện.
        /// </summary>
        /// <param name="row">The row.</param>
        private void GetPODetailFromInterface(DataRow row)
        {
            /* ======================================================================================================================================
             * Gán thông tin trên giao diện vào data row
             * ====================================================================================================================================== */

            row["ItemName"] = HttpUtility.HtmlDecode(txtItemName.Text);
            row["UnitPrice"] = double.Parse(txtPrice.Text);
            row["VAT"] = double.Parse(cboVAT.SelectedValue);
            row["Qty"] = int.Parse(txtQty.Text);
            row["Amount"] = double.Parse(txtAmount.Text);
            row["TotalVAT"] = double.Parse(txtVATAmount.Text);
            row["TotalAmount"] = double.Parse(txtTotalAmount.Text);
            row["ItemNote"] = HttpUtility.HtmlDecode(txtItemNote.Text);

            /* ======================================================================================================================================
             * Cập nhật giá trị tổng số tiền trước thuế - tổng thuế - tổng số tiền sau thuế
             * ====================================================================================================================================== */

            ViewState["POAmount"] = ((double)ViewState["POAmount"]) + double.Parse(txtAmount.Text);
            ViewState["POAmountVAT"] = ((double)ViewState["POAmountVAT"]) + double.Parse(txtVATAmount.Text);
            ViewState["POTotalAmount"] = ((double)ViewState["POTotalAmount"]) + double.Parse(txtTotalAmount.Text);

            txtPOAmount.Text = ((double)ViewState["POAmount"]).ToString(Configurations.NumberFormat);
            txtPOVATAmount.Text = ((double)ViewState["POAmountVAT"]).ToString(Configurations.NumberFormat);
            txtPOTotalAmount.Text = ((double)ViewState["POTotalAmount"]).ToString(Configurations.NumberFormat);
        }

        /// <summary>
        /// Display dữ liệu PO item lên giao diện để chỉnh sửa.
        /// </summary>
        /// <param name="row">The row.</param>
        private void DisplayItem(DataRow row)
        {
            txtSTT.Text = row["Line"].ToString();
            txtItemName.Text = HttpUtility.HtmlDecode(row["ItemName"].ToString());
            txtPrice.Text = row["UnitPrice"].ToString();
            cboVAT.SelectedValue = row["VAT"].ToString();
            txtQty.Text = row["Qty"].ToString();
            txtAmount.Text = row["Amount"].ToString();
            txtVATAmount.Text = row["TotalVAT"].ToString();
            txtTotalAmount.Text = row["TotalAmount"].ToString();
            txtItemNote.Text = HttpUtility.HtmlDecode(row["ItemNote"].ToString());
        }

        /// <summary>
        /// Thể hiện PO item lên giao diện
        /// </summary>
        /// <param name="row">The PO grid view row.</param>
        private void DisplayGridItem(GridViewRow row)
        {
            txtSTT.Text = row.Cells[1].Text.ToString();
            txtItemName.Text = HttpUtility.HtmlDecode(row.Cells[2].Text.ToString());
            txtPrice.Text = double.Parse(row.Cells[3].Text.ToString()).ToString();
            cboVAT.SelectedValue = row.Cells[4].Text.ToString();
            txtQty.Text = row.Cells[5].Text.ToString();
            txtAmount.Text = row.Cells[6].Text.ToString();
            txtVATAmount.Text = row.Cells[7].Text.ToString();
            txtTotalAmount.Text = row.Cells[8].Text.ToString();
            txtItemNote.Text = HttpUtility.HtmlDecode(row.Cells[9].Text.ToString().Trim());
        }

        /// <summary>
        /// Lấy danh sách PR của brand đang chọn
        /// </summary>
        /// <param name="departmentCode"></param>
        private void LoadDepartmentPR(string departmentCode)
        {
            List<SqlParameter> paras = new List<SqlParameter>()
            {
                new SqlParameter("@BrandCode", departmentCode)
            };
            DataTable tb = Configurations.ExecuteQueryData(Configurations.ConnectionString, Configurations.GetPROfBrand, paras);

            if (tb.Rows.Count > 0)
            {
                chklstPR.DataSource = tb;
                chklstPR.DataValueField = "ID";
                chklstPR.DataTextField = "PRNo";
            }
            else
            {
                chklstPR.DataSource = null;
            }

            chklstPR.DataBind();
        }
        private void PreviewPO()
        {
            if (string.IsNullOrWhiteSpace(id.Value))
            {

            }
            else
            {
                string errorMsg = ValidatePOBeforePrinting();
                if (errorMsg != string.Empty)
                {
                    configurations.ShowThongBao(errorMsg, this);
                    return;
                }
                try
                {
                    ReportDocument cryRpt = new ReportDocument();
                    if (rdbSub1.Checked)
                    {
                        cryRpt.Load(Server.MapPath(".\\Reports\\PO_ACFC.rpt"));
                    }
                    else
                    {
                        cryRpt.Load(Server.MapPath(".\\Reports\\PO_CMFC.rpt"));
                    }

                    DataTable tb = ViewState["PO"] as DataTable;
                    DataRow row = tb.Rows[0];

                    DataTable details = ViewState["PODetails"] as DataTable;
                    int i = details.Rows.Count;
                    while (i < 15)
                    {
                        details.Rows.Add(details.NewRow());
                        i++;
                    }
                    cryRpt.SetDataSource(details);

                    cryRpt.SetParameterValue("@CreatedDate", row["CreatedDate"]);
                    cryRpt.SetParameterValue("@PONumber", row["PONo"]);
                    cryRpt.SetParameterValue("@SupplierName", row["VendorName"]);
                    
                    cryRpt.SetParameterValue("@DeliveryDate", row["DeliveryDate"]);
                    cryRpt.SetParameterValue("@Brand", row["Brand"]);
                    cryRpt.SetParameterValue("@AmountExVAT", row["Amount"]);
                    cryRpt.SetParameterValue("@VAT", row["VAT"]);
                    cryRpt.SetParameterValue("@Amount", row["TotalAmount"]);
                    cryRpt.SetParameterValue("@RequestedBy", row["UserId"]);
                    cryRpt.SetParameterValue("@DepositAmount", row["Deposit"] == DBNull.Value? 0 : row["Deposit"]);
                    cryRpt.SetParameterValue("@DepositDate", row["ReturnDeposit"] == DBNull.Value? DateTime.Now : row["ReturnDeposit"]);
                    cryRpt.SetParameterValue("@ReqNumber", row["PRNoRef"]);
                    cryRpt.SetParameterValue("@HaveContract", bool.Parse(row["IsContract"].ToString()) == true ? " Có " : " Không ");
                    cryRpt.SetParameterValue("@Description", row["Note"]);
                    cryRpt.SetParameterValue("@ApprovedBy", "");

                    Session["POCRPreview"] = cryRpt;

                    Type cstype = this.GetType();
                    string script = "<script>setTimeout(() => window.open('PreviewPage.aspx','inforep','menubar=0,toolbar=0,status=0'), 1000);</script>";
                    ScriptManager.RegisterClientScriptBlock(UpdatePanel1, cstype, "myReportScript", script, false);
                    //if (!ScriptManager.IsStartupScriptRegistered("myReportScript"))
                    //{
                    //    ScriptManager.RegisterStartupScript(cstype, "myReportScript", script, false);
                    //}
                }
                catch(Exception ex) { configurations.ShowThongBao(ex.ToString(), this); }
                
            }
        }
        private string ValidatePOBeforePrinting()
        {
            string errorMessage = string.Empty;

            errorMessage += string.IsNullOrWhiteSpace(cboDept.Text) ? "Chưa chọn phòng ban mua hàng." + Environment.NewLine : "";
            errorMessage += string.IsNullOrWhiteSpace(txtDeliveryDate.Text) ? "Chưa chọn ngày dự kiến hoàn thành." + Environment.NewLine : "";
            if (chkDeposit.Checked)
            {
                errorMessage += string.IsNullOrWhiteSpace(txtDeposit.Text) ? "Chưa nhập số tiền đặt cọc." + Environment.NewLine : "";
                //errorMessage += string.IsNullOrWhiteSpace(txtReturnDeposit.Text) ? "Chưa có ngày hoàn cọc." + Environment.NewLine : "";
            }

            return errorMessage;
            
            //errorMessage += string.IsNullOrWhiteSpace(cboDept.Text) ? "Chưa chọn phòng ban mua hàng." + Environment.NewLine : "";
            //errorMessage += string.IsNullOrWhiteSpace(cboDept.Text) ? "Chưa chọn phòng ban mua hàng." + Environment.NewLine : "";
        }
        #endregion
        protected void chkDeposit_CheckedChanged(object sender, EventArgs e)
        {
            txtDeposit.Enabled = chkDeposit.Checked;
            txtReturnDeposit.Enabled = chkDeposit.Checked;
        }
        protected void btnPreview_Click(object sender, EventArgs e)
        {
            PreviewPO();
        }
        protected void btnPrint_Click(object sender, EventArgs e)
        {

        }
        protected void txtFilterVendor_TextChanged(object sender, EventArgs e)
        {
            if (txtFilterVendor.Text.Length > 0)
            {
                cboVendor.Items.Clear();

                DataTable tb = ViewState["Vendor"] as DataTable;
                IEnumerable<DataRow> rows = tb.AsEnumerable();

                var q = rows.Where(r => r["name"].ToString().ToLower().Contains(txtFilterVendor.Text.ToLower()));

                if (q.Count() > 0)
                {
                    DataTable result = q.CopyToDataTable();
                    cboVendor.DataSource = result;
                }
                else
                {
                    
                    cboVendor.DataSource = null;
                }
                cboVendor.DataBind();
            }
            else
            {
                cboVendor.DataSource = ViewState["Vendor"] as DataTable;
                cboVendor.DataBind();
            }
        }
        protected void cboVAT_TextChanged(object sender, EventArgs e)
        {
            CalculateAmount();
        }
    }

    [Serializable]
    public class SelectedPR
    {
        public string Dept { get; set; }
        public List<string> PR { get; set; }
        public SelectedPR()
        {
            Dept = "";
            PR = new List<string>();
        }
    }
    //[Serializable]
    //public class PRInfo
    //{
    //    public int ID;
    //    public string PRNo;
    //    public string BrandCode;

    //    public PRInfo(DataRow row)
    //    {
    //        ID = int.Parse(row["id"].ToString());
    //        PRNo = row["PRNo"].ToString();
    //        BrandCode = row["BrandCode"].ToString();
    //    }
    //}
}
 