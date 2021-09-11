using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;

namespace PORequest.Common
{
    public class Configurations
    {
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
            catch (Exception ex) { throw new Exception(ex.Message); }
            return result;
        }

        public static int ExecuteScalar(string connectionString, string commandText, List<SqlParameter> paras)
        {
            int result = 0;
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
                result = (int) cmd.ExecuteScalar();
                conn.Close();
            }
            catch (Exception ex) { throw; }
            return result;
        }

        public void ShowThongBao(string pChuoi, Control ctr)
        {
            ScriptManager.RegisterStartupScript(ctr, ctr.GetType(), "redirectMe", "alert('" + pChuoi + "');", true);
        }

        public const string NumberFormat = "#,##0.##";

        public const string DatetimeFormat = "MM/dd/yyyy";

        /// <summary>
        /// The connection string
        /// </summary>
        public const string ConnectionString = "Data Source=192.168.80.4;Initial Catalog=ACFCInventory;User ID=acfcmango;Password=acfcmango;Pooling=false;";

        /// <summary>
        /// Lấy danh sách suppliers
        /// </summary>
        public const string GetSuppliers = 
            "SELECT * FROM POSupplier";

        /// <summary>
        /// Tạo suppliers
        /// </summary>
        public const string CreateSupplier = 
            "INSERT POSupplier (addresscode, name, taxcode, address) VALUES ( @AddressCode, @Name, @TaxCode, @Address ) ";

        /// <summary>
        /// Cập nhật thông tin suppliers
        /// </summary>
        public const string UpdateSupplier = 
            "UPDATE POSupplier SET addresscode = @AddressCode, name = @Name, taxcode = @TaxCode, address = @Address Where id = @Id ";

        /// <summary>
        /// Lấy danh sách brands
        /// </summary>
        public const string GetBrands = 
            "SELECT * FROM Brand";

        /// <summary>
        /// Lấy danh sách PO của brand của người đăng nhập ( brand tạo hoặc brand chịu chi phí )
        /// </summary>
        public const string GetPROfBrand =
            "SELECT * FROM PORequisitionMaster WHERE IsActive = 1 AND ( ( BrandCode = @BrandCode AND DeptCost = '' ) OR ( DeptCost = @BrandCode ) )";

        public const string GetRequisitionStatus =
            "SELECT * FROM PORequisitionStatus ";

        /// <summary>
        /// Tạo thông tin PR Master
        /// </summary>
        public const string CreateRequisitionMaster =
            "INSERT PORequisitionMaster ( PRNo, BrandCode, DeptCost, IsNew, IsRepair, IsReplace, IsStock, TotalAmount, UserId, CreatedDate, IsActive, PRStatusId, RemindCount ) " +
            "VALUES ( '', @BrandCode, @DeptCost, @IsNew, @IsRepair, @IsReplace, @IsStock, @TotalAmount, @UserId, getdate(), 1, 0, 0 );" +
            "SELECT @id = SCOPE_IDENTITY(), @PRNo = 'RequisitionNo_' + cast(SCOPE_IDENTITY() as varchar(18));" +
            "UPDATE PORequisitionMaster SET PRNo = @PRNo WHERE id = @id;";

        /// <summary>
        /// Cập nhật thông tin PR Master
        /// </summary>
        public const string UpdateRequisitionMaster =
            "UPDATE PORequisitionMaster " +
            "SET " +
            "DeptCost = @DeptCost, " +
            "IsNew = @IsNew, " +
            "IsRepair = @IsRepair, " +
            "IsReplace = @IsReplace, " +
            "IsStock = @IsStock, " +
            "TotalAmount = @TotalAmount, " +
            "UserUpdated = @UserId, " +
            "UpdatedDate = GETDATE() " +
            "WHERE id = @id";

        /// <summary>
        /// Cập nhật trạng thái Approve / reject & status kế tiếp khi Approver1 duyệt 
        /// </summary>
        public const string UpdateRequisitionApproval1Status =
            "UPDATE PORequisitionMaster " +
            "SET " +
            "  PRStatusId = @PRStatusId " + 
            ", Approve1 = @Approver " +
            ", Approve1Status = @ApproveStatus " +
            ", ApprovedDate1 = GetDate() " +
            ", Approve1Note = @ApproveNote " +
            "WHERE id = @id";


        /// <summary>
        /// Cập nhật trạng thái Approve / reject & status kế tiếp khi Approver2 duyệt 
        /// </summary>
        public const string UpdateRequisitionApproval2Status =
            "UPDATE PORequisitionMaster " +
            "SET " +
            "  PRStatusId = @PRStatusId " +
            ", Approve2 = @Approver " +
            ", Approve2Status = @ApproveStatus " +
            ", ApprovedDate2 = GetDate() " +
            ", Approve2Note = @ApproveNote " +
            "WHERE id = @id";

        /// <summary>
        /// Cập nhật trạng thái Approve / reject & status kế tiếp khi Admin nhận yêu cầu
        /// </summary>
        public const string UpdateRequisitionAdminAccept =
           "UPDATE PORequisitionMaster " +
           "SET " +
           "  PRStatusId = @PRStatusId " +
           ", AdminAcceptUser = @AdminAcceptUser " +
           ", AdminAcceptDate = GetDate() " +
           "WHERE id = @id";

        /// <summary>
        /// Cập nhật trạng thái reject
        /// </summary>
        public const string UpdateRequisitionCancelRequestApproval =
            "UPDATE PORequisitionMaster " +
            "SET " +
            "  PRStatusId = @PRStatusId " +
            ", Approve1Status = @ApproveStatus " +
            ", ApprovedDate1 = GetDate() " +
            ", Approve1Note = @ApproveNote " +
            "WHERE id = @id";

        public const string UpdateRequisitionUserCancel =
            "UPDATE PORequisitionMaster " +
            "SET " +
            "  PRStatusId = @PRStatusId " +
            ", UserUpdated = @UserUpdated "+
            ", UpdatedDate = GetDate() " +
            ", CancelNote = @CancelNote " +
            "WHERE id = @id";

        /// <summary>
        /// Tạo PO Master
        /// </summary>
        public const string CreatePOMaster =
            "INSERT POMaster ( SubNo, PONo, Brand, PRNoRef, SupplierCode, IsContract, Note, IsDeposit, Deposit, ReturnDeposit, DeliveryDate, Amount, VAT, TotalAmount, UserId, CreatedDate, IsActive ) " +
            "VALUES ( @SubNo, '', @Brand, @PRNoRef, @SupplierCode, @IsContract, @Note, @IsDeposit, @Deposit, @ReturnDeposit, @DeliveryDate, @Amount, @VAT, @TotalAmount, @UserId, getdate(), 1 );" +
            "SELECT @id = SCOPE_IDENTITY(), @PONo = 'PurchaseNo_' + cast(SCOPE_IDENTITY() as varchar(18));" +
            "UPDATE POMaster SET PONo = @PONo WHERE id = @id;";

        /// <summary>
        /// Tạo PR Detail
        /// </summary>
        public const string CreatePRDetail =
            "INSERT PORequisitionDetail ( PRId, PRNo, Line, ItemName, Unit, Note, QtyOrder, Amount, DeliveryDate ) " +
            "VALUES ( @PRId, @PRNo, @Line, @ItemName, @Unit, @Note, @QtyOrder, @Amount, @DeliveryDate ) ";

        /// <summary>
        /// Cập nhật PR Detail
        /// </summary>
        public const string UpdatePRDetail =
            "UPDATE PORequisitionDetail " +
            "SET " +
            "  Line = @Line" +
            ", ItemName = @ItemName " +
            ", Unit = @Unit " +
            ", Note = @Note " +
            ", QtyOrder = @QtyOrder " +
            ", Amount = @Amount " +
            ", DeliveryDate = DeliveryDate " +
            "WHERE id = @Id ";


        /// <summary>
        /// Xóa PR Detail
        /// </summary>
        public const string DeletePRDetail =
            "DELETE PORequisitionDetail WHERE id = @id ";

        /// <summary>
        /// Tạo PO Detail
        /// </summary>
        public const string CreatePODetail =
            "INSERT PODetail ( POId, PONo, Line, ItemName, UnitPrice, Qty, Amount, TotalVAT, TotalAmount, ItemNote, CreatedDate, UserId ) " +
            "VALUES ( @POId, @PONo, @Line, @ItemName, @UnitPrice, @Qty, @Amount, @TotalVAT, @TotalAmount, @ItemNote, getdate(), @UserId ) ";

        /// <summary>
        /// Cập nhật PO Detail
        /// </summary>
        public const string UpdatePODetail =
            "UPDATE PODetail " + 
            "SET Line = @Line, " +
            "ItemName = @ItemName, " +
            "UnitPrice = @UnitPrice, " +
            "Qty = @Qty, " +
            "Amount = @Amount, " +
            "TotalVAT = @TotalVAT, " +
            "TotalAmount = @TotalAmount, " +
            "ItemNote = @ItemNote, " +
            "UpdatedDate = getdate() " +
            "WHERE Id = @Id ";

        /// <summary>
        /// Lấy các PR do người dùng tạo, hoặc được gán cho người dùng
        /// </summary>
        public const string GetMyPR = 
            "SELECT prm.*, ps.StatusText " +
            "FROM PORequisitionMaster prm " +
            "INNER JOIN PORequisitionStatus ps ON prm.PRStatusID = ps.id " +
            "WHERE UserId = @UserId OR Approve1 = @UserId OR Approve2 = @UserId OR AdminAcceptUser = @UserId";


        public const string GetAdminPR =
            "SELECT prm.*, ps.StatusText " +
            "FROM PORequisitionMaster prm " +
            "INNER JOIN PORequisitionStatus ps ON prm.PRStatusID = ps.id " +
            "WHERE ps.id > 5";

        public const string GetBMPR =
            "SELECT prm.*, ps.StatusText " +
            "FROM PORequisitionMaster prm " +
            "INNER JOIN PORequisitionStatus ps ON prm.PRStatusID = ps.id " +
            "WHERE {0} OR {1} ";

        public const string GetMyPO =
            "SELECT m.*, s.name as VendorName " +
            "FROM POMaster AS m " +
            "INNER JOIN POSupplier AS s ON m.SupplierCode = s.id " +
            "WHERE UserId = @UserId ";

        public const string GetListPO =
           "SELECT m.*, s.name as VendorName " +
           "FROM POMaster AS m " +
           "INNER JOIN POSupplier AS s ON m.SupplierCode = s.id ";
          


        public const string CreateRequestMaster = "";
        public const string GetPO =
            "SELECT m.*, s.name as VendorName " +
            "FROM POMaster AS m " +
            "INNER JOIN POSupplier AS s ON m.SupplierCode = s.id " +
            "WHERE PONo = @PONo ";

        public const string GetPODetail =
            "SELECT d.* FROM POMaster m INNER JOIN PODetail d ON m.id = POId WHERE m.PONo = @PONo ";

        public const string GetPR =
            "SELECT rm.*, rs.StatusText " +
            "FROM PORequisitionMaster rm " +
            "INNER JOIN PORequisitionStatus rs ON rm.PRStatusId = rs.id " +
            "WHERE PRNo = @PRNo ";

        public const string GetPRDetail =
            "SELECT d.* FROM PORequisitionMaster m INNER JOIN PORequisitionDetail d ON m.id = d.PRId WHERE m.PRNo = @PRNo ";

        public const string GetExistPR = "SELECT top 1 * FROM PORequisitionMaster WHERE PRNO= @Prno ";
        public const string GetStatusPR = "SELECT * FROM PORequisitionStatus WHERE ";
        public const string GetApproveEmail = "select Email, OMEmail,BMEmail from LogsUser where UserId = @UserId";
        public const string GetDefaultEmail = "select  * from LogsUser where BelongBrand = @DefaultEmail";
        public const string UpdatePRStatus = "Update PORequisitionMaster set PRStatusId=@NewStatus,UserUpdated=@userupdated,UpdatedDate=@updateddate where PRNo=@Prno";
        public const string UpdatePRStatusForCancel = "Update PORequisitionMaster set PRStatusId=@NewStatus,UserUpdated=@userupdated,UpdatedDate=@updateddate,CancelNote=@cancelnote where PRNo=@Prno";
        public const string UpdatePRStatusForRemind = "Update PORequisitionMaster set PRStatusId=@NewStatus,UserRemind=@userremind,RemindDate=@remindddate,RemindNote=RemindNote+'--'+@remindnote,RemindCount=RemindCount+1 where PRNo=@Prno";
        public const string GetUserName = "select Name + '   -  Group -    ' + UserGroup Name from LogsUser where UserId=@UserId";
        public const string GetBrandCost = "select  top 1 * from LogsUser where BelongBrand = @DefaultEmail and BMAuthorities=1";
        public const string GetUserID = "select top 1 * from Logsuser where userid=@userid and password=@password";
        // <summary>
        /// Cập nhật PO Master
        /// </summary>
        public const string UpdatePOMaster =
            "UPDATE POMaster " +
            "SET " +
            "SubNo = @SubNo" +
            ", Brand = @Brand" +
            ", PRNoRef = @PRNoRef" +
            ", SupplierCode = @SupplierCode" +
            ", IsContract = @IsContract" +
            ", Note = @Note" +
            ", IsDeposit = @IsDeposit" +
            ", Deposit = @Deposit" +
            ", ReturnDeposit = @ReturnDeposit" +
            ", DeliveryDate = @DeliveryDate" +
            ", Amount = @Amount" +
            ", VAT = @VAT" +
            ", TotalAmount = @TotalAmount" +
            " " +
            "WHERE id = @id ";
        // <summary>
        /// Xóa PO Detail
        /// </summary>
        public const string DeletePODetail =
            "DELETE PODetail WHERE id = @id ";
    }
}