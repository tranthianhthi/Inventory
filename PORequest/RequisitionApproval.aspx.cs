using PORequest.Common;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Mail;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace PORequest
{
    public partial class RequisitionApproval : System.Web.UI.Page
    {
        readonly Configurations configurations = new Configurations();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["userid"] == null)
                Response.Redirect("~/Login.aspx");
            else
            {
                txtUserName.Text = Session["userid"].ToString();                
            }
                if (!this.IsPostBack)
            {
                ViewState["IsApprover"] = false;
                ViewState["IsAdmin"] = false;
                //Session["brand"] = "Admin";

                if (Request.QueryString["prno"] == null)
                {
                }
                else
                {
                    InitializeData(Request.QueryString["prno"].ToString());
                }

            }
        }
        protected void btnApprove_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(hfId.Value))
            {
                int id = int.Parse(hfId.Value);
                int statusId = int.Parse(hfStatusId.Value);
                string user = Session["userid"].ToString();

                int nextStatusId = GetNextStatus(true);

                if (nextStatusId == -1)
                    return;

                
                try
                {
                    // Approver
                    if ((bool)ViewState["IsApprover"])
                    {
                        UpdateApproveStatus(nextStatusId, true, txtApprovalNote.Text, user);
                    }
                    // Admin
                    else
                    {
                        UpdateAdminAcceptStatus(nextStatusId, user);
                    }

                    InitializeData(Request.QueryString["prno"].ToString());
                    CreateAndSendMail(nextStatusId, txtPRNo.Text, hfApprove1.Value, hfApprove2.Value, hfAdminUser.Value);
                    configurations.ShowThongBao("Đã send mail thành công.", this);
                }
                catch(Exception ex)
                {

                }
            }
        }
        protected void btnReject_Click(object sender, EventArgs e)
        {
            int id = int.Parse(hfId.Value);
            int statusId = int.Parse(hfStatusId.Value);
            string user = Session["userid"].ToString();

            int nextStatusId = GetNextStatus(false);

            if (nextStatusId == -1)
                return;

            try
            {
                string emailMessage = string.Empty;
                if ((bool)ViewState["IsApprover"])
                {
                    UpdateApproveStatus(nextStatusId, false, txtApprovalNote.Text, user);
                }
                // Admin
                else
                {
                    UpdateAdminAcceptStatus(nextStatusId, user);
                }
                InitializeData(txtPRNo.Text);
                CreateAndSendMail(nextStatusId, txtPRNo.Text, hfApprove1.Value, hfApprove2.Value, hfAdminUser.Value);
            }
            catch {  }
        }
        protected void btnCancel_Click(object sender, EventArgs e)
        {
            try
            {
                UpdateUserCancel(hfRequester.Value, txtApprovalNote.Text);
                InitializeData(txtPRNo.Text);
                CreateAndSendMail(9, txtPRNo.Text, hfApprove1.Value, hfApprove2.Value, hfAdminUser.Value);
            }
            catch { }
        }
        #region private helpers
        private DataTable GetPRMaster(string prno)
        {
            DataTable tb = new DataTable();
            tb = Configurations.ExecuteQueryData(Configurations.ConnectionString, Configurations.GetPR, new List<SqlParameter>() { new SqlParameter("PRNo", prno) });
            return tb;
        }
        private DataTable GetPRDetails(string prno)
        {
            DataTable tb = new DataTable();
            tb = Configurations.ExecuteQueryData(Configurations.ConnectionString, Configurations.GetPRDetail, new List<SqlParameter>() { new SqlParameter("PRNo", prno) });
            return tb;
        }

        /// <summary>
        /// Tải thông tin requisition từ DB theo mã số request
        /// </summary>
        /// <param name="prno">Mã số request</param>
        private void InitializeData(string prno)
        {
            DataTable tb = new DataTable();
            string userId = Session["userid"].ToString();
            string brand = Session["brand"].ToString();
            bool isBM = (bool)Session["BM"];

            tb = GetPRMaster(prno);
            ViewState["PRMaster"] = tb;

            if (tb.Rows.Count == 1)
            {
                DisplayPRMasterToInterface(tb);

                // Approver : BM của BrandCode hoặc BM của DeptCost
                ViewState["IsApprover"] =
                    (userId == hfApprove1.Value || userId == hfApprove2.Value) ||
                    (
                        (
                            brand.ToLower().Contains(cboDept.Text.ToLower()) ||
                            brand.ToLower().Contains(cboDeptCost.Text.ToLower())
                        ) && isBM
                    );
                ViewState["IsAdmin"] = (Session["brand"].ToString().ToUpper() == "ADMIN") || userId == hfAdminUser.Value;

                FormatInfoLabel(int.Parse(hfStatusId.Value));
                FormatApprovalControls(userId,Session["brand"].ToString(), int.Parse(hfStatusId.Value));

                tb = GetPRDetails(prno);
                dgItems.DataSource = tb;
                dgItems.DataBind();
            }
        }

        /// <summary>
        /// Thể hiện thông tin PR Master lên giao diện
        /// </summary>
        /// <param name="tb">Thông tin PR</param>
        private void DisplayPRMasterToInterface(DataTable tb)
        {
            DataRow row = tb.Rows[0];
            

            //txtNote.Text = tb.Rows[0]["Remark"].ToString();
            txtDate.Text = ((DateTime)row["CreatedDate"]).ToString(Configurations.DatetimeFormat);
            chklstPR.Items[0].Selected = (bool)row["IsNew"];
            chklstPR.Items[1].Selected = (bool)row["IsRepair"];
            chklstPR.Items[2].Selected = (bool)row["IsReplace"];
            chkOnHand.Checked = (bool)row["IsStock"];
            txtPRNo.Text = row["PRNo"].ToString();
            cboDept.Text = row["BrandCode"].ToString();
            cboDeptCost.Text = row["DeptCost"].ToString();
            txtUserName.Text = row["UserId"].ToString();
            txtPRAmount.Text = double.Parse(row["TotalAmount"].ToString()).ToString(Configurations.NumberFormat);
            txtPurpose.Text = row["Remark"].ToString();

            lblStatusText.Text = row["StatusText"].ToString(); //"Đề xuất đang chờ duyệt.";

            hfId.Value = row["id"].ToString();
            hfStatusId.Value = row["PRStatusID"].ToString();
            hfRequester.Value = row["UserId"].ToString();
            hfApprove1.Value = row["Approve1"] == null ? "" : row["Approve1"].ToString();
            hfApprove2.Value = row["Approve2"] == null ? "" : row["Approve2"].ToString();
            hfAdminUser.Value = row["AdminAcceptUser"] == null ? "" : row["AdminAcceptUser"].ToString();
        }

        /// <summary>
        /// Chỉnh css class của label thể hiện trạng thái của requisition
        /// </summary>
        /// <param name="statusId">Trạng thái yêu cầu</param>
        private void FormatInfoLabel(int statusId)
        {
            if (statusId == 1 || statusId == 2 || statusId == 3)
            {
                lblStatusText.CssClass = "label label-success";
            }
            else if (statusId == 9 || statusId == 10 || statusId == 11 || statusId == 12 || statusId == 16 || statusId == 12)
            {
                lblStatusText.CssClass = "label label-warning";
            }
            else if (statusId == 4 || statusId == 5 || statusId == 15)
            {
                lblStatusText.CssClass = "label label-danger";
            }
            else if (statusId == 6 || statusId == 7 || statusId == 8)
            {
                lblStatusText.CssClass = "label label-primary";
            }
            else if (statusId == 13)
            {
                lblStatusText.CssClass = "label label-default";
            }
        }

        /// <summary>
        /// Enable / Disable các control phục vụ việc duyệt yêu cầu
        /// </summary>
        private void FormatApprovalControls(string userId, string brand, int statusId)
        {

            if (statusId == 8 || statusId == 12 || statusId == 13 || statusId == 15)
            {
                pnlApprovalArea.Visible = false;
            }

            //Nếu người đăng nhập là requester 
            if (userId == hfRequester.Value)
            {
                btnApprove.Visible = false;
                btnReject.Visible = false;
                btnCancel.Visible = !(statusId == 9 || statusId == 10 || statusId == 11 || statusId == 12 || statusId == 8 || statusId == 13 || statusId == 16);
                txtApprovalNote.Enabled = true;
                //return;
            }

            // Nếu người đăng nhập là approver
            if ((bool)ViewState["IsApprover"])
            {
                bool isBMOfCreater = brand.ToLower().Contains(cboDept.Text.ToLower()) || userId == hfApprove1.Value.ToLower();
                bool isBMOfDeptCost = brand.ToLower().Contains(cboDeptCost.Text.ToLower()) || userId == hfApprove2.Value.ToLower();

                btnApprove.Enabled = (
                    statusId == 1 || 
                    ( statusId == 2 && isBMOfDeptCost ) || 
                    ( statusId == 3 && isBMOfCreater) || 
                    statusId == 9 || 
                    ( statusId == 10 && isBMOfDeptCost) || 
                    ( statusId == 11 && isBMOfCreater ) );
                btnApprove.Text = (statusId == 1 || statusId == 2 || statusId == 3) ? "Duyệt đề xuất" : "Đồng ý hủy đề xuất";

                btnReject.Enabled = (
                    statusId == 1 || 
                    ( statusId == 2 && isBMOfDeptCost) || 
                    ( statusId == 3 && isBMOfCreater) );
                btnReject.Text = "Không duyệt đề xuất";


                btnApprove.Visible = btnApprove.Enabled;
                btnReject.Visible = btnReject.Enabled;
                txtApprovalNote.Enabled = btnApprove.Enabled;
                //return;
            }

            // Nếu người đăng nhập là admin
            if ((bool)ViewState["IsAdmin"])
            {
                if (statusId == 6)
                {
                    btnApprove.Enabled = true;
                    btnApprove.Text = "Chấp nhận yêu cầu";
                    btnReject.Visible = false;
                }

                if (statusId == 7)
                {
                    btnApprove.Text = "Hoàn thành & đóng phiếu";
                    btnApprove.Enabled = true;
                    btnReject.Text = "Không thực hiện được";
                    btnReject.Enabled = true;
                }

                if (statusId == 16)
                {
                    btnApprove.Text = "Chấp nhận hủy";
                    btnApprove.Enabled = true;
                    btnReject.Text = "Không chấp nhận hủy";
                    btnReject.Enabled = true;
                }

                txtApprovalNote.Enabled = btnApprove.Enabled;
            }
        }

        /// <summary>
        /// Lấy trạng thái kế tiếp của request
        /// </summary>
        /// <param name="isApproved">Duyệt/không duyệt</param>
        /// <returns></returns>
        private int GetNextStatus(bool isApproved)
        {
            int statusId = int.Parse(hfStatusId.Value);
            string brand = Session["brand"].ToString().ToLower();
            string userId = Session["userid"].ToString();

            #region Trình duyệt

            if (statusId == 1)
            {
                // Nếu được duyệt
                if (isApproved)
                {
                    // Nếu có 1 cấp approve => gửi tới admin
                    if (
                        (hfApprove1.Value == userId || brand.Contains(cboDept.Text.ToLower())) &&
                        (hfApprove2.Value == "" && string.IsNullOrWhiteSpace(cboDeptCost.Text)))
                    {
                        return 6;
                    }
                    // Nếu có 2 cấp approve => cập nhật theo status
                    else
                    {
                        return (hfApprove1.Value == userId || brand.Contains(cboDept.Text.ToLower())) ? 2 : 3;
                    }
                }
                // Nếu bị từ chối
                else
                {
                    return (hfApprove1.Value == userId || brand.Contains(cboDept.Text.ToLower())) ? 4 : 5;
                }
            }

            // Trường hợp trưởng phòng 1 đã duyệt & người đăng nhập là trưởng phòng 2
            if ( statusId == 2 && ( hfApprove2.Value == userId || brand.Contains(cboDeptCost.Text.ToLower()) ) )
            {
                //Nếu approve => gửi tới admin; Nếu cancel => hủy request
                return isApproved? 6 : 15;
            }

            // Trường hợp trưởng phòng 2 đã duyệt & người đăng nhập là trưởng phòng 1
            if (statusId == 3 && (  hfApprove1.Value == userId || brand.Contains(cboDept.Text.ToLower() ) ) )
            {
                //Nếu approve => gửi tới admin; Nếu cancel => hủy request
                return isApproved ? 6 : 15;
            }
            #endregion

            #region Gửi sang Admin

            if (statusId == 6 && (bool)ViewState["IsAdmin"] && isApproved)
            {
                return 7;
            }

            if (statusId == 7)
            {
                return isApproved ? 8 : 13;
            }

            #endregion 

            #region Yêu cầu hủy

            // Trường hợp yêu cầu hủy
            if (statusId == 9)
            {
                if (hfApprove1.Value == userId)
                    return string.IsNullOrWhiteSpace(cboDeptCost.Text)? 16 : 10;

                if (hfApprove2.Value == userId)
                    return 11;
            }

            if (statusId == 10)
            {
                if (hfApprove2.Value == userId || brand.Contains(cboDeptCost.Text.ToLower()))
                    return 16;
            }

            if (statusId == 11)
            {
                if (hfApprove1.Value == userId || brand.Contains(cboDept.Text.ToLower()))
                    return 16;
            }
            if (statusId == 16)
            {
                if ((bool)ViewState["IsAdmin"])
                    return 12;
            }
            #endregion

            return -1;
        }

        /// <summary>
        /// Cập nhật trạng thái mới khi trưởng phòng duyệt yêu cầu
        /// </summary>
        /// <param name="status">Trạng thái mới</param>
        /// <param name="isApproved1">KQ trưởng phòng 1 duyệt</param>
        /// <param name="approve1Note">Ghi chú của trưởng phòng 1</param>
        /// <param name="isApproved2">KQ trưởng phòng 2 duyệt</param>
        /// <param name="approve2Note">Ghi chú của trưởng phòng 2</param>
        private void UpdateApproveStatus(int status, bool isApproved, string approveNote, string user)
        {
            List<SqlParameter> paras = new List<SqlParameter>()
            {
                new SqlParameter("@PRStatusId", status),
                new SqlParameter("@id", int.Parse(hfId.Value)),
                new SqlParameter("@Approver", user),
                new SqlParameter("@ApproveStatus", isApproved),
                new SqlParameter("@ApprovedDate", DateTime.Now),
                new SqlParameter("@ApproveNote", approveNote)
            };

            try
            {
                if (hfApprove1.Value == user || Session["brand"].ToString().ToLower().Contains(cboDept.Text.ToLower() ) )
                    Configurations.ExecuteNonQuery(Configurations.ConnectionString, Configurations.UpdateRequisitionApproval1Status, paras);
                else
                    Configurations.ExecuteNonQuery(Configurations.ConnectionString, Configurations.UpdateRequisitionApproval2Status, paras);
            }
            catch { }
        }

        /// <summary>
        /// Cập nhật trạng thái mới khi Admin chấp nhận yêu cầu 
        /// </summary>
        /// <param name="status"></param>
        private void UpdateAdminAcceptStatus(int status, string user)
        {
            List<SqlParameter> paras = new List<SqlParameter>()
            {
                new SqlParameter("@PRStatusId", status),
                new SqlParameter("@id", int.Parse(hfId.Value)),
                new SqlParameter("@AdminAcceptUser", user),
            };

            try
            {
                Configurations.ExecuteNonQuery(Configurations.ConnectionString, Configurations.UpdateRequisitionAdminAccept, paras);
            }
            catch { }
        }

        /// <summary>
        /// Cập nhật trạng thái mới khi có yêu cầu hủy requisition
        /// </summary>
        private void UpdateUserCancel(string user, string note)
        {
            List<SqlParameter> paras = new List<SqlParameter>()
            {
                new SqlParameter("@PRStatusId", 9),
                new SqlParameter("@id", int.Parse(hfId.Value)),
                new SqlParameter("@UserUpdated", user),
                new SqlParameter("@CancelNote", note)
            };

            try
            {
                Configurations.ExecuteNonQuery(Configurations.ConnectionString, Configurations.UpdateRequisitionUserCancel, paras);
            }
            catch(Exception ex) { }
        }
        private void RemindPR()
        {
            try
            {
                DataTable tb = new DataTable();
                tb = Configurations.ExecuteQueryData(Configurations.ConnectionString, Configurations.GetExistPR, new List<SqlParameter>() { new SqlParameter("@PrNo", txtPRNo.Text.Trim()) });
                if (tb.Rows.Count == 0)
                {
                    configurations.ShowThongBao("Không tồn tại số PR No. này trong hệ thống.", this); return;
                }
                else
                {
                    foreach (DataRow row in tb.Rows)
                    {
                        if (int.Parse(row["PRStatusId"].ToString()) != 6 && int.Parse(row["PRStatusId"].ToString()) != 7 && int.Parse(row["PRStatusId"].ToString()) != 14)
                        { configurations.ShowThongBao("Requisition này không thể nhắc nhở. Hãy check status của Requisition.", this); return; }
                        else
                        {
                            string sub = "Requisition Remind - Requisition No: " + row["PRNo"];
                            string sms = InitSmsRemind(txtPRNo.Text);
                            //SendMailToOM(sub, sms, row["PRNo"].ToString(), row["DeptCost"].ToString(), "R");
                        }
                    }
                }
            }
            catch (Exception ex) { configurations.ShowThongBao("err: " + ex.Message, this); }
        }

        #endregion

        #region Gửi mail

        private void CreateAndSendMail(int nextStatusId, string PRNo, string approver1, string approver2, string adminUser)
        {
            string message = string.Empty;
            switch(nextStatusId)
            {
                case 1:
                    message = InitSms(PRNo);
                    SendMail("Requisition Submitted - Requisition No: " + PRNo, message, false);
                    break;
                case 2:
                    message = InitApproverApproveSms(PRNo, approver1);
                    SendMail("Requisition Approved - Requisition No: " + PRNo, message, false);
                    break;
                case 3:
                    message = InitApproverApproveSms(PRNo, approver2);
                    SendMail("Requisition Approved - Requisition No: " + PRNo, message, false);
                    break;
                case 4:
                    message = InitApproverRejectSms(PRNo, approver1);
                    SendMail("Requisition Rejected - Requisition No: " + PRNo, message, false);
                    break;
                case 5:
                    message = InitApproverRejectSms(PRNo, approver2);
                    SendMail("Requisition Rejected - Requisition No: " + PRNo, message, false);
                    break;
                case 6:
                    message = InitAdminSms(PRNo);
                    SendMail("Requisition Sent to Admin - Requisition No: " + PRNo, message, true);
                    break;
                case 7:
                    message = InitAdminAcceptSms(PRNo, adminUser);
                    SendMail("Requisition Accepted - Requisition No: " + PRNo, message, true, true);
                    break;
                case 8:
                    message = InitAdminCloseSms(PRNo, adminUser);
                    SendMail("Requisition Completed - Requisition No: " + PRNo, message, true, true);
                    break;
                case 9:
                    message = InitSmsCancel(PRNo);
                    SendMail("Cancelling Requisition Submitted - Requisition No: " + PRNo, message, false);
                    break;
                case 10:
                    message = InitSmsApproverCancel(PRNo, approver1);
                    SendMail("Requisition Accepted - Requisition No: " + PRNo, message, false);
                    break;
                case 11:
                    message = InitSmsApproverCancel(PRNo, approver2);
                    SendMail("Cancelling Requisition Approved - Requisition No: " + PRNo, message, false);
                    break;
                case 12:
                    message = InitAdminCancellingSms(PRNo);
                    SendMail("Cancelling Requisition Approved - Requisition No: " + PRNo, message, true);
                    break;
                case 13:
                    message = InitAdminFailSms(PRNo, adminUser);
                    SendMail("Requisition Is Unable To Finish - Requisition No: " + PRNo, message, true);
                    break;
                case 14:
                    message = InitSmsRemind(PRNo);
                    SendMail("Requisition Accepted - Requisition No: " + PRNo, message, true);
                    break;
                case 15:
                    message = InitApproverRejectSms(PRNo, Session["userid"].ToString());
                    SendMail("Requisition Rejected - Requisition No: " + PRNo, message, true);
                    break;
                case 16:
                    message = InitAdminCancellingSms(PRNo);
                    SendMail("Cancelling Requisition Accepted - Requisition No: " + PRNo, message, true);
                    break;
                default:
                    break;
            }

            return ;
        }

        private string GenerateRequisitionInfo()
        {
            string tsms = string.Empty;
            tsms = tsms + "User's booking:        " + hfRequester.Value + "\r\n";
            tsms = tsms + "Brand:     " + cboDept.Text +  ( hfApprove1.Value == ""? "" : ".    -----   Approver: " + hfApprove1.Value) + "\r\n";
            tsms = tsms + "Brand Chịu phí:    " + cboDeptCost.Text + (hfApprove2.Value == "" ? "" : ".    ------      Approver: " + hfApprove2.Value) + "\r\n\r\n";           
            tsms = tsms + "Request Note :  " + txtApprovalNote.Text + "\r\n\r\n";
            return tsms;
        }

        /// <summary>
        /// Status 1: Khởi tạo email thông báo tới Approver 
        /// </summary>
        /// <param name="PrNo"></param>
        /// <param name="brandcode"></param>
        /// <param name="remark"></param>
        /// <param name="deptcost"></param>
        /// <returns></returns>
        private string InitSms(string PrNo)
        {
            string tsms = "Dear Operation Manager,\r\n\r\n";
            tsms = tsms + "Please check and confirm for New Requisition No.:     " + PrNo + Environment.NewLine;
            tsms = tsms + GenerateRequisitionInfo();
            //tsms = tsms + "More detail to approve:  http://internal.acfc.com.vn/Login?" + PrNo + "\r\n\r\n";
            tsms = tsms + "More detail to approve:  http://localhost:23692/Login?" + PrNo + Environment.NewLine;
            tsms = tsms + "\r\n\r\n\r\n\r\n\r\n\r\n This is computer generated email. Please do not reply to this email!" + Environment.NewLine;
            tsms = tsms + "\r\n By ACFC support system. \r\n \r\n";
            tsms = tsms + "\r\n\r\n Thanks. \r\n ";
            return tsms;
        }

        /// <summary>
        /// Status 2 - 3 : Khởi tạo email thông báo tới Approver2 - MB 1 da approve send mail to MB 2 xin approve
        /// </summary>
        /// <param name="PrNo"></param>
        /// <param name="approver"></param>
        /// <returns></returns>
        private string InitApproverApproveSms(string PrNo, string approver)
        {
            string tsms = "Dear Operation Manager,\r\n\r\n";
            tsms = tsms + "Please check and approve for Requisition No.:     " + PrNo + "\r\n\r\n";
            tsms = tsms + "It's just been approved by: " + approver + ".\r\n\r\n";
            tsms = tsms + GenerateRequisitionInfo();
            //tsms = tsms + "More detail to approve:  http://internal.acfc.com.vn/Login?" + PrNo + "\r\n\r\n";
            tsms = tsms + "More detail to approve:  http://localhost:23692/Login?" + PrNo + "\r\n\r\n";
            tsms = tsms + "\r\n\r\n\r\n\r\n\r\n\r\n This is computer generated email. Please do not reply to this email! \r\n";
            tsms = tsms + "\r\n By ACFC support system. \r\n \r\n";
            tsms = tsms + "\r\n\r\n Thanks. \r\n ";
            return tsms;
        }

        /// <summary>
        /// Status 4 - 5 - 15: Khởi tạo email thông báo tới Approver2 - 4: BM1 reject/ 5: BM2 reject / 15: 1 torng 2 MB từ chối duyệt de xuat
        /// </summary>
        /// <param name="PrNo"></param>
        /// <param name="approver"></param>
        /// <returns></returns>
        private string InitApproverRejectSms(string PrNo, string approver)
        {
            string tsms = "Dear Operation Manager,\r\n\r\n";
            tsms = tsms + "Please review for Requisition No.:     " + PrNo + "\r\n\r\n";
            tsms = tsms + "It's just been rejected by:      " + approver + ".\r\n\r\n";
            tsms = tsms + GenerateRequisitionInfo();
            //tsms = tsms + "More detail to approve:  http://internal.acfc.com.vn/Login?" + PrNo + "\r\n\r\n";
            //tsms = tsms + "More detail to approve:  http://localhost:23692/Login?" + PrNo + "\r\n\r\n";
            tsms = tsms + "\r\n\r\n\r\n\r\n\r\n\r\n This is computer generated email. Please do not reply to this email! \r\n";
            tsms = tsms + "\r\n By ACFC support system. \r\n \r\n";
            tsms = tsms + "\r\n\r\n Thanks. \r\n ";
            return tsms;
        }

        /// <summary>
        /// Status 6: Khởi tạo email thông báo tới admin - RJ duoc Chuyen den admin
        /// </summary>
        /// <param name="PrNo"></param>
        /// <param name="approve1"></param>
        /// <returns></returns>
        private string InitAdminSms(string PrNo)
        {
            string tsms = "Dear Admin Dept.,\r\n\r\n";
            tsms = tsms + "Please check and confirm for Requisition No.:     " + PrNo + "\r\n\r\n";
            tsms = tsms + "It's been approved as below:\r\n\r\n";
            tsms = tsms + GenerateRequisitionInfo();
            //tsms = tsms + "More detail to approve:  http://internal.acfc.com.vn/Login?" + PrNo + "\r\n\r\n";
            tsms = tsms + "More detail to approve:  http://localhost:23692/Login?" + PrNo + "\r\n\r\n";
            tsms = tsms + "\r\n\r\n\r\n\r\n\r\n\r\n This is computer generated email. Please do not reply to this email! \r\n";
            tsms = tsms + "\r\n By ACFC support system. \r\n \r\n";
            tsms = tsms + "\r\n\r\n Thanks. \r\n ";
            return tsms;
        }

        /// <summary>
        /// Status 7: Admin nhận requisition - Khởi tạo email thông báo tới Approvers - Admin nhan va gui thong bao cho 2 Bm va User
        /// </summary>
        /// <param name="PrNo"></param>
        /// <param name="adminUser"></param>
        /// <returns></returns>
        private string InitAdminAcceptSms(string PrNo, string adminUser)
        {
            string tsms = "Dear Operation Manager,\r\n\r\n";
            tsms = tsms + "Please review for Requisition No.:     " + PrNo + "\r\n\r\n";
            tsms = tsms + "It's been accepted by:       " + adminUser + "     - Admin Dept.\r\n\r\n";
            tsms = tsms + GenerateRequisitionInfo();
            //tsms = tsms + "More detail to approve:  http://internal.acfc.com.vn/Login?" + PrNo + "\r\n\r\n";
            //tsms = tsms + "More detail to approve:  http://localhost:23692/Login?" + PrNo + "\r\n\r\n";
            tsms = tsms + "\r\n\r\n\r\n\r\n\r\n\r\n This is computer generated email. Please do not reply to this email! \r\n";
            tsms = tsms + "\r\n By ACFC support system. \r\n \r\n";
            tsms = tsms + "\r\n\r\n Thanks. \r\n ";
            return tsms;
        }

        /// <summary>
        /// Status 8: Admin hoàn thành & đóng yêu cầu
        /// </summary>
        /// <param name="PrNo"></param>
        /// <param name="adminUser"></param>
        /// <returns></returns>
        private string InitAdminCloseSms(string PrNo, string adminUser)
        {
            string tsms = "Dear Operation Manager,\r\n\r\n";
            tsms = tsms + "Please check and confirm for Requisition No.:     " + PrNo + "\r\n\r\n";
            tsms = tsms + "It's been completed and closed by:       " + adminUser + "     - Admin Dept.\r\n\r\n";
            tsms = tsms + GenerateRequisitionInfo();
            //tsms = tsms + "More detail to approve:  http://internal.acfc.com.vn/Login?" + PrNo + "\r\n\r\n";
            //tsms = tsms + "More detail to approve:  http://localhost:23692/Login?" + PrNo + "\r\n\r\n";
            tsms = tsms + "\r\n\r\n\r\n\r\n\r\n\r\n This is computer generated email. Please do not reply to this email! \r\n";
            tsms = tsms + "\r\n By ACFC support system. \r\n \r\n";
            tsms = tsms + "\r\n\r\n Thanks. \r\n ";
            return tsms;
        }

        /// <summary>
        /// Status 9: Khởi tạo email yêu cầu hủy  - USer cancel RJ - send cho 2 BM
        /// </summary>
        /// <param name="PrNo"></param>
        /// <returns></returns>
        private string InitSmsCancel(string PrNo)
        {
            string tsms = "Dear Operation Manager,\r\n\r\n";
            tsms = tsms + "Please check and confirm for Canceling Requisition Form No.:     " + PrNo + "\r\n\r\n";
            tsms = tsms + GenerateRequisitionInfo();
            //tsms = tsms + "More detail to approve:  http://internal.acfc.com.vn/Login?" + PrNo + "\r\n\r\n";
            tsms = tsms + "More detail to approve:  http://localhost:23692/Login?" + PrNo + "\r\n\r\n";
            tsms = tsms + "\r\n\r\n\r\n\r\n\r\n\r\n This is computer generated email. Please do not reply to this email! \r\n";
            tsms = tsms + "\r\n By ACFC support system. \r\n \r\n";
            tsms = tsms + "\r\n\r\n Thanks. \r\n ";
            return tsms;
        }

        /// <summary>
        /// Status 10 - 11: 1 trong 2 BM dong ý hủy va send mail cho BM kia -- chi send cho BM khong send cho user va admin
        /// </summary>
        /// <param name="PrNo"></param>
        /// <returns></returns>
        private string InitSmsApproverCancel(string PrNo, string approver)
        {
            string tsms = "Dear Operation Manager,\r\n\r\n";
            tsms = tsms + "Please check and cancel for Requisition No.:     " + PrNo + "\r\n\r\n";
            tsms = tsms + "It's been cancelled by: " + approver + ".\r\n\r\n";
            tsms = tsms + GenerateRequisitionInfo();
            //tsms = tsms + "More detail to approve:  http://internal.acfc.com.vn/Login?" + PrNo + "\r\n\r\n";
            tsms = tsms + "More detail to approve:  http://localhost:23692/Login?" + PrNo + "\r\n\r\n";
            tsms = tsms + "\r\n\r\n\r\n\r\n\r\n\r\n This is computer generated email. Please do not reply to this email! \r\n";
            tsms = tsms + "\r\n By ACFC support system. \r\n \r\n";
            tsms = tsms + "\r\n\r\n Thanks. \r\n ";
            return tsms;
        }

        /// <summary>
        /// Status 16: Yêu cầu hủy gửi tới Admin- 2 BM da dong y huy - send mail cho Admin va User
        /// </summary>
        /// <param name="PrNo"></param>
        /// <param name="approve1"></param>
        /// <returns></returns>
        private string InitAdminCancellingSms(string PrNo)
        {
            string tsms = "Dear Admin Dept.,\r\n\r\n";
            tsms = tsms + "Please check and cancel for Requisition No.:     " + PrNo + "\r\n\r\n";
            tsms = tsms + "Admin's been approved to cancel by " + hfAdminUser.Value + " as below:\r\n\r\n";
            tsms = tsms + GenerateRequisitionInfo();
            //tsms = tsms + "More detail to approve:  http://internal.acfc.com.vn/Login?" + PrNo + "\r\n\r\n";
            tsms = tsms + "More detail to approve:  http://localhost:23692/Login?" + PrNo + "\r\n\r\n";
            tsms = tsms + "\r\n\r\n\r\n\r\n\r\n\r\n This is computer generated email. Please do not reply to this email! \r\n";
            tsms = tsms + "\r\n By ACFC support system. \r\n \r\n";
            tsms = tsms + "\r\n\r\n Thanks. \r\n ";
            return tsms;
        }

        /// <summary>
        /// Status 12: Admin chấp nhận hủy và đóng yêu cầu - send mail cho 2 BM va user
        /// </summary>
        /// <param name="PrNo"></param>
        /// <param name="adminUser"></param>
        /// <returns></returns>
        private string InitAdminAcceptCancellingSms(string PrNo, string adminUser)
        {
            string tsms = "Dear Operation Manager,\r\n\r\n";
            tsms = tsms + "Please review for Requisition No.:     " + PrNo + "\r\n\r\n";
            tsms = tsms + "It's been accepted to cancel and close by " + adminUser + "- Admin Dept.\r\n\r\n";
            tsms = tsms + GenerateRequisitionInfo();
            //tsms = tsms + "More detail to approve:  http://internal.acfc.com.vn/Login?" + PrNo + "\r\n\r\n";
            //tsms = tsms + "More detail to approve:  http://localhost:23692/Login?" + PrNo + "\r\n\r\n";
            tsms = tsms + "\r\n\r\n\r\n\r\n\r\n\r\n This is computer generated email. Please do not reply to this email! \r\n";
            tsms = tsms + "\r\n By ACFC support system. \r\n \r\n";
            tsms = tsms + "\r\n\r\n Thanks. \r\n ";
            return tsms;
        }

        /// <summary>
        /// Status 13: Admin khong hoàn thành & đóng yêu cầu - send mail cho BM va user
        /// </summary>
        /// <param name="PrNo"></param>
        /// <param name="adminUser"></param>
        /// <returns></returns>
        private string InitAdminFailSms(string PrNo, string adminUser)
        {
            string tsms = "Dear Operation Manager,\r\n\r\n";
            tsms = tsms + "Please review for Requisition No.:     " + PrNo + "\r\n\r\n";
            tsms = tsms + "It is unable to complete and close by:       " + adminUser + "     - Admin Dept.\r\n\r\n";
            tsms = tsms + GenerateRequisitionInfo();
            //tsms = tsms + "More detail to approve:  http://internal.acfc.com.vn/Login?" + PrNo + "\r\n\r\n";
            //tsms = tsms + "More detail to approve:  http://localhost:23692/Login?" + PrNo + "\r\n\r\n";
            tsms = tsms + "\r\n\r\n\r\n\r\n\r\n\r\n This is computer generated email. Please do not reply to this email! \r\n";
            tsms = tsms + "\r\n By ACFC support system. \r\n \r\n";
            tsms = tsms + "\r\n\r\n Thanks. \r\n ";
            return tsms;
        }

        /// <summary>
        /// Status 14: Khởi tạo email nhắc nhở - send mail cho Admin, va 2 BM
        /// </summary>
        /// <param name="PrNo"></param>
        /// <returns></returns>
        private string InitSmsRemind(string PrNo)
        {
            string tsms = "Dear Admin Teams,\r\n\r\n";
            tsms = tsms + "Remind mail for Requisition No.:     " + PrNo + "\r\n\r\n";
            tsms = tsms + GenerateRequisitionInfo();
            //tsms = tsms + "More detail to approve:  http://internal.acfc.com.vn/Login?" + PrNo + "\r\n\r\n";
            tsms = tsms + "More detail to approve:  http://localhost:23692/Login?" + PrNo + "\r\n\r\n";
            tsms = tsms + "\r\n\r\n\r\n\r\n\r\n\r\n This is computer generated email. Please do not reply to this email! \r\n";
            tsms = tsms + "\r\n By ACFC support system. \r\n \r\n";
            tsms = tsms + "\r\n\r\n Thanks. \r\n ";
            return tsms;
        }

        /// <summary>
        /// Gửi mail tới BM 
        /// </summary>
        /// <param name="sub"></param>
        /// <param name="qsms"></param>
        /// <param name="prno"></param>
        /// <param name="deptcost"></param>
        /// <param name="status"></param>
        /// <param name="kindE"></param>
        private void SendMail(string sub, string qsms, bool isSendToAdmin, bool isCCAdmin = false)
        {
            try
            {
                MailMessage mail = new MailMessage();
                SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com");
                mail.From = new MailAddress(ConfigurationManager.AppSettings["EmailAddress"], ConfigurationManager.AppSettings["DisplayName"]);

                string to = "";
                string cc = "";

                if (isSendToAdmin)
                {
                    if (isCCAdmin)
                    {
                        to = GetEmailList();
                        cc = GetDefaultEmail("ADMIN");
                    }
                    else
                    {
                        to = GetDefaultEmail("ADMIN");
                        cc = GetEmailList();
                    }
                    
                    //to = "thi.tran@acfc.com.vn";
                    //cc = "thi.tran@acfc.com.vn";
                    mail.To.Add(to);
                    mail.CC.Add(cc);
                }
                else
                {
                    to = GetEmailList();
                    //to = "thi.tran@acfc.com.vn";
                    cc = "";
                    mail.To.Add(to);
                }              

                mail.Subject = sub;
                mail.Body = qsms;
                //mail.IsBodyHtml = true;
                SmtpServer.Port = 25; //587;
                SmtpServer.DeliveryMethod = SmtpDeliveryMethod.Network;
                SmtpServer.UseDefaultCredentials = false;
                SmtpServer.EnableSsl = true;
                SmtpServer.Credentials = new System.Net.NetworkCredential(ConfigurationManager.AppSettings["EmailAddress"], ConfigurationManager.AppSettings["Password"]);

                if (ConfigurationManager.AppSettings["IsTesting"] == "1")
                {
                    string itMails = ConfigurationManager.AppSettings["ITMailAddresses"];
                    mail.Body = "To: " + mail.To + "\r\n" + "Cc: " + mail.CC + "\r\n\r\n" + mail.Body;

                    mail.To.Clear();
                    mail.To.Add(itMails);

                    mail.CC.Clear();
                    mail.CC.Add(itMails);
                }
                else
                {

                }                
                SmtpServer.Send(mail);
                configurations.ShowThongBao("Send mail gửi yêu cầu thành công.", this);
            }

            catch (Exception ex) { configurations.ShowThongBao("err: " + ex.Message, this); }

        }

        /// <summary>
        /// Lấy danh sách email được nhận thông báo
        /// </summary>
        /// <param name="brandcost"></param>
        /// <returns></returns>
        private string GetEmailList()
        {
            string email = "";
            string cc = "";
            DataTable tb = new DataTable();

            // Nếu là Approver
            if ((bool)ViewState["IsApprover"] && !string.IsNullOrWhiteSpace(hfApprove1.Value))
            {
                tb = Configurations.ExecuteQueryData(Configurations.ConnectionString, Configurations.GetApproveEmail, new List<SqlParameter>() { new SqlParameter("@UserId", hfApprove1.Value) });
                email += tb.Rows.Count == 1? tb.Rows[0]["Email"].ToString() : "";
            }
            // Nếu là requester
            else 
            {
                tb = Configurations.ExecuteQueryData(Configurations.ConnectionString, Configurations.GetApproveEmail, new List<SqlParameter>() { new SqlParameter("@UserId", hfRequester.Value) });

                // Lấy BM của requester
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
            }
            
            // Lấy BM của brand chịu chi phí
            if (!string.IsNullOrWhiteSpace(cboDeptCost.Text))
            {
                // Nếu đã duyệt thì lấy email người duyệt
                if (!string.IsNullOrWhiteSpace(hfApprove2.Value))
                {
                    tb = Configurations.ExecuteQueryData(Configurations.ConnectionString, Configurations.GetApproveEmail, new List<SqlParameter>() { new SqlParameter("@UserId", hfApprove2.Value) });
                    email += tb.Rows.Count == 1 ? "," + tb.Rows[0]["Email"].ToString() : "";
                }
                // Nếu chưa duyệt thì lấy email default
                else
                {
                    tb = Configurations.ExecuteQueryData(Configurations.ConnectionString, Configurations.GetBrandCost, new List<SqlParameter>() { new SqlParameter("@DefaultEmail", cboDeptCost.Text) });
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
        #endregion 
        protected void btnRemind_Click(object sender, EventArgs e)
        {

        }
    }
}