using PORequest.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Net.Mail;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace PORequest
{
    public partial class CreatePR : System.Web.UI.Page
    {
        readonly Configurations configurations = new Configurations();
        public string userid = "";

        #region form events
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["userid"] == null)
            {
                configurations.ShowThongBao("Vui lòng đăng nhập trước khi tạo yêu cầu.<br/>Click vào <a href='~/Login.aspx'>đây</a> để đăng nhập.", this);
                Response.Redirect("~/Login.aspx");
            }
            else
            {
                string brand = Session["brand"].ToString();
                cboDept.SelectedValue = brand.Contains(",") ? brand : brand.Split(',')[0];
                txtUserName.Text = Session["userid"].ToString();
                userid= Session["userid"].ToString();

                if (!this.IsPostBack)
                {
                    ViewState["RemovedId"] = "";
                    InitializeCommonData();

                    if (Request.QueryString["prno"] != null)
                    {
                        InitializeData(Request.QueryString["prno"].ToString());
                    }
                    else
                    {
                        InitializeData();
                    }

                }
            }
            
        }

        /// <summary>
        /// Xóa 1 dòng trên grid
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void dgItems_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            GridViewRow gridRow = dgItems.Rows[e.RowIndex];
            string lineNo = gridRow.Cells[1].Text;

            IEnumerable<DataRow> rows = (ViewState["PRDetails"] as DataTable).AsEnumerable().Where(r => r["Line"].ToString() != lineNo).ToList().AsEnumerable();

            /* Cập nhật số thứ tự các line sau line đang chọn xóa */
            List<DataRow> selectedRows = rows.Where(r => int.Parse(r["Line"].ToString()) > int.Parse(gridRow.Cells[1].Text)).ToList();
            foreach (DataRow row in selectedRows)
            {
                row["Line"] = int.Parse(row["Line"].ToString()) - 1;
            }
            /* Xóa line đang chọn */
            int currentKey = int.Parse(dgItems.DataKeys[e.RowIndex].Value.ToString());
            if (currentKey != 0)
            {
                ViewState["RemovedId"] = ViewState["RemovedId"].ToString() + currentKey + ";";
            }
            ViewState["PRDetails"] = rows.CopyToDataTable();
            dgItems.DataSource = ViewState["PRDetails"] as DataTable;
            dgItems.DataBind();
            txtSTT.Text = ((ViewState["PRDetails"] as DataTable).Rows.Count + 1).ToString();
            CalculateAmount();
        }

        /// <summary>
        /// Chọn 1 dòng trên grid để đưa lên giao diện chỉnh sửa
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void dgItems_SelectedIndexChanged(object sender, EventArgs e)
        {
            GridViewRow gridRow = dgItems.SelectedRow;
            DisplayPRItemToInterface(gridRow);
            //btnAdd.Text = "Cập nhật sản phẩm";
        }

        /// <summary>
        /// Thêm PRItem vào list
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnAdd_Click(object sender, EventArgs e)
        {
            DataTable tb = ViewState["PRDetails"] as DataTable;
            IEnumerable<DataRow> rows = tb.AsEnumerable();

            //ScriptManager sm = Master.FindControl("ScriptManager");
            //ClientScriptManager csm = sm.getscript

            ((sender as Control), this.GetType(), "dlgOutOfRange", "ShowRangeDialog();", true);

            DataRow existedRow = rows.FirstOrDefault(r => r["Line"].ToString() == txtSTT.Text);

            if (ValidatePRItem(sender))
            {

                if (existedRow != null)
                {
                    ReadPRItemFromInterface(existedRow);
                }
                else
                {
                    existedRow = tb.NewRow();
                    ReadPRItemFromInterface(existedRow);
                    tb.Rows.Add(existedRow);
                }

                ViewState["PRDetails"] = tb;
                dgItems.DataSource = tb;
                dgItems.DataBind();

                CalculateAmount();
                ClearPRItem();
            }
        }

        /// <summary>
        /// Xóa thông tin PRItem trên giao diện
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnCancel_Click(object sender, EventArgs e)
        {
            ClearPRItem();
        }

        /// <summary>
        /// Lưu PR xuống DB
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                #region check data
                if (txtDate.Text == "") { configurations.ShowThongBao("Hãy nhập lại ngày tạo Requisition.", this); txtDate.Focus(); return; }
                if (!chklstPR.Items[0].Selected && !chklstPR.Items[1].Selected && !chklstPR.Items[2].Selected) { configurations.ShowThongBao("Hãy chọn lại nội dung đề xuất cho Requisition.", this);  return; }
              
                #endregion

                List<SqlParameter> paras = new List<SqlParameter>()
                {
                    new SqlParameter() { ParameterName = "@BrandCode", Value = cboDept.SelectedValue.ToString()},
                    new SqlParameter() { ParameterName = "@DeptCost", Value = cboDeptCost.SelectedValue.ToString()},
                    new SqlParameter() { ParameterName = "@IsNew", Value = chklstPR.Items[0].Selected},
                    new SqlParameter() { ParameterName = "@IsReplace", Value = chklstPR.Items[2].Selected},
                    new SqlParameter() { ParameterName = "@IsRepair", Value = chklstPR.Items[1].Selected},
                    new SqlParameter() { ParameterName = "@IsStock", Value = chkOnHand.Checked},
                    new SqlParameter() { ParameterName = "@TotalAmount", Value = string.IsNullOrWhiteSpace(txtPRAmount.Text) ? 0 : double.Parse(txtPRAmount.Text)},
                    new SqlParameter() { ParameterName = "@UserId", Value = Session["userid"].ToString() }
                };

                int PRId = 0;
                string PRNo = "";

                /* ===============================================================================================================================================
                 * Nếu ViewState PRMaster == null => Requisition mới tạo => Create
                 * Nếu ViewState PRMaster != null => Requisition load từ DB => Update
                 * ===============================================================================================================================================*/
                if (ViewState["PRMaster"] == null)
                {
                    paras.AddRange(
                        new SqlParameter[] 
                        {
                            new SqlParameter() { ParameterName = "@Id", Direction = ParameterDirection.InputOutput, Value = PRId },
                            new SqlParameter() { ParameterName = "@PRNo", Direction = ParameterDirection.InputOutput, Value = PRNo, Size = 20 }
                        }
                    );
                    Configurations.ExecuteNonQuery(Configurations.ConnectionString, Configurations.CreateRequisitionMaster, paras);
                    PRId = (int)paras[paras.Count - 2].Value;
                    PRNo = paras[paras.Count - 1].Value.ToString();
                }
                else
                {
                    PRId = (int)(ViewState["PRMaster"] as DataTable).Rows[0]["id"];
                    PRNo = (ViewState["PRMaster"] as DataTable).Rows[0]["PRNo"].ToString();
                    paras.Add(new SqlParameter() { ParameterName = "@Id", Value = PRId });

                    Configurations.ExecuteNonQuery(Configurations.ConnectionString, Configurations.UpdateRequisitionMaster, paras);
                }
                /* ===============================================================================================================================================
                 * Loop qua từng dòng trong RequisitionDetails
                 *      Xóa các id có trong viewState removedId.
                 *      Nếu row[id] == null => detail mới tạo => Create
                 *      Nếu row[id] != null => detail load từ DB => Update
                 * =============================================================================================================================================== */

                string[] removedId = ViewState["RemovedId"].ToString().Split(';');
                foreach(string id in removedId)
                {
                    if (!string.IsNullOrWhiteSpace(id))
                        Configurations.ExecuteNonQuery(Configurations.ConnectionString, Configurations.DeletePRDetail, new List<SqlParameter>() { new SqlParameter("id", int.Parse(id)) });
                }
                if (ViewState["PRDetails"] == null || (ViewState["PRDetails"] as DataTable).Rows.Count==0)
                {
                    if (txtItemName.Text == "") { configurations.ShowThongBao("Hãy nhập lại tên chi tiết cho hàng hóa.", this); txtItemName.Focus(); return; }
                    if (txtUnit.Text == "") { configurations.ShowThongBao("Hãy nhập lại đơn vị tính cho hàng hóa.", this); txtUnit.Focus(); return; }
                    if (txtQty.Text == "") { configurations.ShowThongBao("Hãy nhập lại số lượng mua cho hàng hóa.", this); txtQty.Focus(); return; }
                    if (txtDeliveryDate.Text == "") { configurations.ShowThongBao("Hãy nhập lại ngày yêu cầu nhận hàng.", this); txtDeliveryDate.Focus(); return; }
                    if (txtAmount.Text == "") { configurations.ShowThongBao("Hãy nhập lại thành tiền cho Requisition.", this); txtAmount.Focus(); return; }
                    if (txtPRAmount.Text == "") { configurations.ShowThongBao("Hãy nhập lại tổng tiền cho Requisition.", this); txtPRAmount.Focus(); return; }
                }
                foreach (DataRow row in (ViewState["PRDetails"] as DataTable).Rows)
                {
                    List<SqlParameter> paraDetail = new List<SqlParameter>()
                    {
                        new SqlParameter() { ParameterName = "@Line", Value = row["Line"] },
                        new SqlParameter() { ParameterName = "@ItemName", Value = row["ItemName"].ToString() },
                        new SqlParameter() { ParameterName = "@Unit", Value = row["Unit"].ToString() },
                        new SqlParameter() { ParameterName = "@QtyOrder", Value = row["QtyOrder"] },
                        new SqlParameter() { ParameterName = "@Amount", Value = row["Amount"] },
                        new SqlParameter() { ParameterName = "@DeliveryDate", Value = row["DeliveryDate"] },
                        new SqlParameter() { ParameterName = "@Note", Value = row["Note"].ToString() }
                    };

                    if (int.Parse(row["id"].ToString()) == 0)
                    {
                        paraDetail.AddRange(new SqlParameter[] {
                            new SqlParameter() { ParameterName = "@PRId", Value = PRId },
                            new SqlParameter() { ParameterName = "@PRNo", Value = PRNo } });

                        Configurations.ExecuteNonQuery(Configurations.ConnectionString, Configurations.CreatePRDetail, paraDetail);
                    }
                    else
                    {
                        paraDetail.Add(new SqlParameter() { ParameterName = "@id", Value = row["id"] });
                        Configurations.ExecuteNonQuery(Configurations.ConnectionString, Configurations.UpdatePRDetail, paraDetail);
                    }
                }
                txtPRNo.Text = PRNo;
                Response.Write("<script>alert('Đã lưu PR vào hệ thống thành công.');</script>");
            }
            catch (Exception ex) { Response.Write("<script>alert('Lỗi - không lưu được PR vào hệ thống.<br/>" + ex.ToString() + "');</script>"); };
        }

        /// <summary>
        /// Trình duyệt PR
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnSubmitPR_Click(object sender, EventArgs e)
        {
            SubmitPR();
        }
        #endregion

        #region Private helpers

        /// <summary>
        /// Khởi tạo dữ liệu & ViewState khi tải form
        /// </summary>
        private void InitializeData()
        {
            DataTable tb = new DataTable();

            /* ======================================================================================================================================
             * Khởi tạo bảng PO Detail
             * ====================================================================================================================================== */
            tb = CreatePRDetailsTable();
            dgItems.DataSource = tb;
            dgItems.DataBind();
            ViewState["PRDetails"] = tb;

            /* ======================================================================================================================================
             * Khởi tạo các giá trị ViewState
             * ====================================================================================================================================== */

            ViewState["PRAmount"] = 0.0;
            txtSTT.Text = "1";

            try
            {
                /* ======================================================================================================================================
                 * Tải danh sách brand
                 * ====================================================================================================================================== */

                tb = Configurations.ExecuteQueryData(Configurations.ConnectionString, Configurations.GetBrands, null);
                ViewState["Brand"] = tb;

                cboDept.DataSource = tb;
                cboDept.DataValueField = "BrandCode";
                cboDept.DataTextField = "BrandCode";
                cboDept.DataBind();

                DataTable tbWithEmpty = tb.Copy();

                DataRow row = tbWithEmpty.NewRow();
                row["BrandCode"] = "";
                tbWithEmpty.Rows.InsertAt(row, 0);

                cboDeptCost.DataSource = tbWithEmpty;
                cboDeptCost.DataValueField = "BrandCode";
                cboDeptCost.DataTextField = "BrandCode";
                cboDeptCost.DataBind();

            }
            catch { }
        }
        /// <summary>
        /// 
        /// </summary>
        private void InitializeData(string prno)
        {
            DataTable tb = new DataTable();

            /* ======================================================================================================================================
             * Khởi tạo bảng PO Detail
             * ====================================================================================================================================== */
            tb = GetPRMaster(prno);
            ViewState["PRMaster"] = tb;

            txtNote.Text = tb.Rows[0]["Remark"].ToString();
            txtDate.Text = ((DateTime)tb.Rows[0]["CreatedDate"]).ToString(Configurations.DatetimeFormat);
            chklstPR.Items[0].Selected = (bool)tb.Rows[0]["IsNew"];
            chklstPR.Items[1].Selected = (bool)tb.Rows[0]["IsRepair"];
            chklstPR.Items[2].Selected = (bool)tb.Rows[0]["IsReplace"];
            chkOnHand.Checked = (bool)tb.Rows[0]["IsStock"];
            txtPRNo.Text = tb.Rows[0]["PRNo"].ToString();
            cboDept.SelectedValue = tb.Rows[0]["BrandCode"].ToString();
            cboDeptCost.SelectedValue = tb.Rows[0]["DeptCost"].ToString();

            int statusId = (int)tb.Rows[0]["PRStatusId"];

            pnlReason.Visible = !(statusId == 0 || statusId == 8 || statusId == 9 || statusId == 10 || statusId == 11 || statusId == 12 || statusId == 13 || statusId == 14);
            pnlRemindNote.Visible = (statusId == 7);

            tb = GetPRDetails(prno);
            dgItems.DataSource = tb;
            dgItems.DataBind();
            ViewState["PRDetails"] = tb;

            /* ======================================================================================================================================
             * Khởi tạo các giá trị ViewState
             * ====================================================================================================================================== */

            ViewState["PRAmount"] = 0.0;
            txtSTT.Text = (tb.Rows.Count + 1).ToString();
        }
        private DataTable GetPRDetails(string prno)
        {
            DataTable tb = new DataTable();
            tb = Configurations.ExecuteQueryData(Configurations.ConnectionString, Configurations.GetPRDetail, new List<SqlParameter>() { new SqlParameter("PRNo", prno) });
            return tb;
        }
        private DataTable GetPRMaster(string prno)
        {
            DataTable tb = new DataTable();
            tb = Configurations.ExecuteQueryData(Configurations.ConnectionString, Configurations.GetPR, new List<SqlParameter>() { new SqlParameter("PRNo", prno) });
            return tb;
        }
        private void InitializeCommonData()
        {
            try
            {
                /* ======================================================================================================================================
                 * Tải danh sách brand
                 * ====================================================================================================================================== */

                DataTable tb = Configurations.ExecuteQueryData(Configurations.ConnectionString, Configurations.GetBrands, null);
                ViewState["Brand"] = tb;

                cboDept.DataSource = tb;
                cboDept.DataValueField = "BrandCode";
                cboDept.DataTextField = "BrandCode";
                cboDept.DataBind();

                DataTable tbWithEmpty = tb.Copy();

                DataRow row = tbWithEmpty.NewRow();
                row["BrandCode"] = "";
                tbWithEmpty.Rows.InsertAt(row, 0);

                cboDeptCost.DataSource = tbWithEmpty;
                cboDeptCost.DataValueField = "BrandCode";
                cboDeptCost.DataTextField = "BrandCode";
                cboDeptCost.DataBind();

            }
            catch { }
        }

        /// <summary>
        /// Tạo bảng PRDetails
        /// </summary>
        /// <returns></returns>
        private DataTable CreatePRDetailsTable()
        {
            DataTable tb = new DataTable();
            tb.Columns.Add("id");
            tb.Columns.Add("Line");
            tb.Columns.Add("ItemName");
            tb.Columns.Add("Unit");
            tb.Columns.Add("Note");
            tb.Columns.Add("QtyOrder");
            tb.Columns.Add("Amount");
            tb.Columns.Add("DeliveryDate");
            return tb;
        }

        ///// <summary>
        ///// Tạo bảng PRMaster
        ///// </summary>
        ///// <returns></returns>
        //private DataTable CreatePRMasterTable()
        //{
        //    DataTable tb = new DataTable();
        //    tb.Columns.Add("id");
        //    tb.Columns.Add("PRNo");
        //    tb.Columns.Add("BrandCode");
        //    tb.Columns.Add("BrandName");
        //    tb.Columns.Add("DisplayText");
        //    return tb;
        //}

        /// <summary>
        /// Tính tổng giá trị PR dựa trên danh sách item
        /// </summary>
        private void CalculateAmount()
        {
            txtPRAmount.Text = (ViewState["PRDetails"] as DataTable).AsEnumerable().Sum(r => double.Parse(r["Amount"].ToString())).ToString(Configurations.NumberFormat);
        }

        /// <summary>
        /// Xóa thông tin PRItem trên giao diện chỉnh sửa
        /// </summary>
        private void ClearPRItem()
        {
            txtSTT.Text = ((ViewState["PRDetails"] as DataTable).Rows.Count + 1).ToString();
            txtItemName.Text = "";
            txtQty.Text = "";
            txtNote.Text = "";
            txtAmount.Text = "";
            txtDeliveryDate.Text = "";
            txtUnit.Text = "";
        }
        /// <summary>
        /// Lấy thông tin PRItem từ giao diện & lưu vào table tạm
        /// </summary>
        /// <param name="row"></param>
        private void ReadPRItemFromInterface(DataRow row)
        {
            row["id"] = 0;
            row["Line"] = int.Parse(txtSTT.Text);
            row["ItemName"] = txtItemName.Text;
            row["Unit"] = txtUnit.Text;
            row["Note"] = txtNote.Text;
            row["QtyOrder"] = int.Parse(txtQty.Text);
            row["Amount"] = double.Parse(txtAmount.Text);
            row["DeliveryDate"] = DateTime.ParseExact(txtDeliveryDate.Text, Configurations.DatetimeFormat, CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Đưa thông tin dòng PRItem trên lưới lên giao diện để chỉnh sửa.
        /// </summary>
        /// <param name="row"></param>
        private void DisplayPRItemToInterface(GridViewRow row)
        {
            txtSTT.Text = row.Cells[1].Text;
            txtItemName.Text = HttpUtility.HtmlDecode(row.Cells[2].Text);
            txtUnit.Text = HttpUtility.HtmlDecode(row.Cells[3].Text);
            txtQty.Text = row.Cells[4].Text;
            txtAmount.Text = row.Cells[5].Text;
            txtDeliveryDate.Text = row.Cells[6].Text;
            txtNote.Text = HttpUtility.HtmlDecode(row.Cells[7].Text);
        }

        /// <summary>
        /// Kiểm tra dữ liệu PRItem có hợp lệ hay không.
        /// </summary>
        /// <returns></returns>
        private bool ValidatePRItem(object sender)
        {
            bool isValid = true;
            string message = string.Empty;

            message += string.IsNullOrWhiteSpace(txtItemName.Text) ? "Vui lòng nhập tên sản phẩm" + Environment.NewLine : "";
            message += string.IsNullOrWhiteSpace(txtUnit.Text) ? "Vui lòng nhập đơn vị tính của sản phẩm" + Environment.NewLine : "";
            message += string.IsNullOrWhiteSpace(txtQty.Text) ? "Vui lòng nhập số lượng cần mua" + Environment.NewLine : "";
            //message += string.IsNullOrWhiteSpace(txtItemName.Text) ? "Vui lòng nhập tên sản phẩm" + Environment.NewLine : "";
            /* Kiểm tra rỗng */
            isValid = !(
                string.IsNullOrWhiteSpace(txtItemName.Text) ||
                string.IsNullOrWhiteSpace(txtUnit.Text) ||
                string.IsNullOrWhiteSpace(txtQty.Text) );

            /*||
                string.IsNullOrWhiteSpace(txtAmount.Text)*/

            if (!isValid)
            {
                lblDialogMsg.Text = message;
                
            }
            /* Kiểm tra kiểu dữ liệu có phải số hay không */
            return isValid;
        }
        #region Workflow

        //private void CancelPR()
        //{
        //    try
        //    {
        //        DataTable tb = new DataTable();
        //        tb = Configurations.ExecuteQueryData(Configurations.ConnectionString, Configurations.GetExistPR, new List<SqlParameter>() { new SqlParameter("@PrNo", txtPRNo.Text.Trim()) });
        //        if (tb.Rows.Count == 0)
        //        {
        //            configurations.ShowThongBao("Không tồn tại số PR No. này trong hệ thống.", this); return;
        //        }
        //        else
        //        {
        //            foreach (DataRow row in tb.Rows)
        //            {
        //                if ((int.Parse(row["PRStatusId"].ToString()) == 0) || int.Parse(row["PRStatusId"].ToString()) == 8 || int.Parse(row["PRStatusId"].ToString()) == 10
        //                    || int.Parse(row["PRStatusId"].ToString()) == 11 || int.Parse(row["PRStatusId"].ToString()) == 12 || int.Parse(row["PRStatusId"].ToString()) == 13)
        //                { configurations.ShowThongBao("Requisition này không thể Hủy. Hãy check status của Requisition.", this); return; }
        //                else
        //                {
        //                    string sub = "Cancel Requisition - Requisition No: " + row["PRNo"];
        //                    string sms = InitSmsCancel(row["PRNo"].ToString(), row["BrandCode"].ToString(), row["Remark"].ToString(), row["DeptCost"].ToString());
        //                    SendMailToOM(sub, sms, row["PRNo"].ToString(), row["DeptCost"].ToString(), 9, "C");
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex) { configurations.ShowThongBao("err: " + ex.Message, this); }
        //}
        private void SubmitPR()
        {
            try
            {
                DataTable tb = new DataTable();
                tb = Configurations.ExecuteQueryData(Configurations.ConnectionString, Configurations.GetExistPR, new List<SqlParameter>() { new SqlParameter("@Prno", txtPRNo.Text.Trim()) });
                if (tb.Rows.Count == 0)
                {
                    configurations.ShowThongBao("Không tồn tại số PR No. này trong hệ thống.", this); return;
                }
                else
                {
                    foreach (DataRow row in tb.Rows)
                    {
                        if (int.Parse(row["PRStatusId"].ToString()) != 0)
                        { configurations.ShowThongBao("Yêu cầu này đã send mail cho BM rồi. Hãy check status của Requisition", this); return; }
                        else
                        {
                            string sub = "Requisition Confirm - Requisition No: " + row["PRNo"];
                            string sms = InitSms(row["PRNo"].ToString(), row["BrandCode"].ToString(), row["Remark"].ToString(), row["DeptCost"].ToString());
                            SendMailToOM(sub, sms, row["PRNo"].ToString(), row["DeptCost"].ToString(), 1, "S");
                        }
                    }
                }
            }
            catch (Exception ex) { configurations.ShowThongBao("err: " + ex.Message, this); }
        }
        private string InitSms(string PrNo, string brandcode, string remark, string deptcost)
        {
            string tsms = "Dear Operation Manager,\r\n\r\n";
            tsms = tsms + "Please check and confirm for Requisition No.:     " + PrNo + Environment.NewLine;
            tsms = tsms + "Brand:  " + brandcode + "\r\n";
            tsms = tsms + "Brand Chịu phí:  " + deptcost + "\r\n";
            tsms = tsms + "User's booking:  " + GetUserName() + "\r\n";
            tsms = tsms + "Request Note :  " + remark + "\r\n\r\n";
            //tsms = tsms + "More detail to approve:  http://internal.acfc.com.vn/Login?" + PrNo + "\r\n\r\n";
            tsms = tsms + "More detail to approve:  http://localhost:23692/Login?" + PrNo + "\r\n\r\n";
            tsms = tsms + "\r\n\r\n\r\n\r\n\r\n\r\n This is computer generated email. Please do not reply to this email! \r\n";
            tsms = tsms + "\r\n By ACFC support system. \r\n \r\n";
            tsms = tsms + "\r\n\r\n Thanks. \r\n ";
            return tsms;
        }
        private void SendMailToOM(string sub, string qsms, string prno, string deptcost, int status, string kindE)
        {
            try
            {
                MailMessage mail = new MailMessage();
                SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com");
                mail.From = new MailAddress("acfclogistictransportation@gmail.com", "Requisition Form.");
                string to = "";
                string cc = "";
                if (kindE == "R")
                { to = GetDefaultEmail("ADMIN"); cc = GetEmail(userid, deptcost); to = "thi.tran@acfc.com.vn"; cc = "thi.tran@acfc.com.vn"; mail.To.Add(to); mail.CC.Add(cc); }
                else { to = GetEmail(txtUserName.Text, deptcost); to = "thi.tran@acfc.com.vn"; mail.To.Add(to); }

                mail.Subject = sub;
                mail.Body = qsms;
                SmtpServer.Port = 25; //587;
                SmtpServer.DeliveryMethod = SmtpDeliveryMethod.Network;
                SmtpServer.UseDefaultCredentials = false;
                SmtpServer.EnableSsl = true;
                SmtpServer.Credentials = new System.Net.NetworkCredential("acfclogistictransportation@gmail.com", "acfc2016");
                SmtpServer.Send(mail);
                if (kindE == "S") { _UpdatePRStatus(status, prno); }
                if (kindE == "C") { _UpdatePRStatusForCancel(status, prno, "txtcancel.text"); }
                if (kindE == "R") { _UpdatePRStatusForRemind(status, prno, "txtremind.text"); }
                configurations.ShowThongBao("Send mail gửi yêu cầu thành công.", this);
            }

            catch (Exception ex) { configurations.ShowThongBao("err: " + ex.Message, this); }

        }
        private string GetEmail(string checkicd, string brandcost)
        {
            string email = "";
            string cc = "";
            DataTable tb = new DataTable();
            tb = Configurations.ExecuteQueryData(Configurations.ConnectionString, Configurations.GetApproveEmail, new List<SqlParameter>() { new SqlParameter("@UserId", checkicd) });

            if (tb.Rows.Count != 0)
            {
                foreach (DataRow row in tb.Rows)
                {
                    if (row["Email"].ToString() != "")
                    {
                        if (!email.Contains(row["Email"].ToString()))
                        {
                            if (email == "") email = email + row["Email"];
                            else email = email + "," + row["Email"];
                        }
                    }
                    if (row["OMEmail"].ToString() != "")
                    {
                        if (!email.Contains(row["BMEmail"].ToString()))
                        {
                            if (email == "") email = email + row["BMEmail"];
                            else email = email + "," + row["BMEmail"];
                        }
                    }
                    if (cc != "")
                    {
                        if (!email.Contains(cc))
                            email = cc + "," + email;
                    }
                }
            }
            if (brandcost != null)
            {
                tb = Configurations.ExecuteQueryData(Configurations.ConnectionString, Configurations.GetBrandCost, new List<SqlParameter>() { new SqlParameter("@DefaultEmail", brandcost) });
                if (tb.Rows.Count != 0)
                {
                    foreach (DataRow row in tb.Rows)
                    {
                        if (row["BMEmail"].ToString() != "")
                        {
                            if (!email.Contains(row["BMEmail"].ToString()))
                            {
                                if (email == "") email = email + row["BMEmail"];
                                else email = email + "," + row["BMEmail"];
                            }
                        }
                    }
                }
            }
            return email;
        }
        private string GetDefaultEmail(string defaultvalue)
        {
            string email = "";
            DataTable tb = new DataTable();
            tb = Configurations.ExecuteQueryData(Configurations.ConnectionString, Configurations.GetDefaultEmail, new List<SqlParameter>() { new SqlParameter("@DefaultEmail", defaultvalue) });

            if (tb.Rows.Count != 0)
            {
                foreach (DataRow row1 in tb.Rows)
                {
                    if (row1["Email"].ToString() != "")
                    {
                        email = email + row1["Email"] + ",";
                    }
                }
            }
            return email.Substring(0, email.Length - 1);
        }
        private void _UpdatePRStatus(int status, string prno)
        {
            List<SqlParameter> paras = new List<SqlParameter>()
            {
                new SqlParameter("@NewStatus", status),
                new SqlParameter("@userupdated", userid),
                new SqlParameter("@updateddate", DateTime.Now.ToShortDateString()),
                new SqlParameter("@Prno", prno)
            };
            Configurations.ExecuteNonQuery(Configurations.ConnectionString, Configurations.UpdatePRStatus, paras);
        }
        private void _UpdatePRStatusForCancel(int status, string prno, string cancelnote)
        {
            List<SqlParameter> paras = new List<SqlParameter>()
            {
                new SqlParameter("@NewStatus", status),
                new SqlParameter("@userupdated", userid),
                new SqlParameter("@updateddate", DateTime.Now.ToShortDateString()),
                new SqlParameter("@cancelnote", cancelnote),
                new SqlParameter("@Prno", prno)
            };
            Configurations.ExecuteNonQuery(Configurations.ConnectionString, Configurations.UpdatePRStatusForCancel, paras);
        }
        private void _UpdatePRStatusForRemind(int status, string prno, string remindnote)
        {
            List<SqlParameter> paras = new List<SqlParameter>()
            {
                new SqlParameter("@NewStatus", status),
                new SqlParameter("@userremind", userid),
                new SqlParameter("@remindddate", DateTime.Now.ToShortDateString()),
                new SqlParameter("@remindnote", remindnote),
                new SqlParameter("@Prno", prno)
            };
            Configurations.ExecuteNonQuery(Configurations.ConnectionString, Configurations.UpdatePRStatusForRemind, paras);
        }
        private string GetUserName()
        {
            string name = "";
            DataTable tb = new DataTable();
            tb = Configurations.ExecuteQueryData(Configurations.ConnectionString, Configurations.GetUserName, new List<SqlParameter>() { new SqlParameter("@UserId", userid) });

            if (tb.Rows.Count != 0)
            {
                foreach (DataRow row in tb.Rows)
                {
                    name = row["Name"].ToString();
                }
            }
            return name;
        }
        #endregion
        #endregion
    }
}
