<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="RequisitionApproval.aspx.cs" Inherits="PORequest.RequisitionApproval" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <h3>Duyệt đề xuất mua hàng
        <small class="text-muted">Requisition form Approval</small>
    </h3>

    <asp:HiddenField runat="server" ID="hfId" />
    <asp:HiddenField runat="server" id="hfStatusId" />
    <asp:HiddenField runat="server" ID="hfRequester" />
    <asp:HiddenField runat="server" ID="hfApprove1" />
    <asp:HiddenField runat="server" ID="hfApprove2" />
    <asp:HiddenField runat="server" ID="hfAdminUser" />

    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>

            <h4>
                <asp:Label runat="server" ID="lblStatusText"></asp:Label>
            </h4>

            <div class="container acfc-div acfc-border">

                <div class="acfc-div">

                    <div class="row">
                        <div class="col-sm-2">
                            <label>Người lập:</label>
                        </div>
                        <div class="col-sm-2">
                            <asp:Label ID="txtUserName" runat="server" CssClass="form-control  form-control-sm"></asp:Label>
                        </div>
                        <div class="col-sm-2">
                            <label>Bộ phận:</label>
                        </div>
                        <div class="col-sm-2">
                            <asp:Label ID="cboDept" runat="server" CssClass="form-control form-control-sm"></asp:Label>
                        </div>
                        <div class="col-sm-2">
                            <label>Bộ phận chịu chi phí:</label>
                        </div>
                        <div class="col-sm-2">
                            <asp:Label ID="cboDeptCost" runat="server" CssClass="form-control form-control-sm"></asp:Label>
                        </div>
                    </div>

                    <div class="row">
                        <div class="col-sm-2">
                            <label>Số PR:</label>
                        </div>
                        <div class="col-sm-2">
                            <asp:Label ID="txtPRNo" runat="server" CssClass="form-control  form-control-sm font-weight-bold"></asp:Label>
                        </div>
                        <div class="col-sm-2">
                            <label for="txtDate">Ngày:</label>
                        </div>
                        <div class="col-sm-2">
                            <asp:Label ID="txtDate" runat="server" CssClass="form-control form-control-sm"></asp:Label>
                        </div>
                        <div class="col-sm-2">
                            <label>Đã kiểm tra tồn kho</label>
                        </div>
                        <div class="col-sm-2">
                            <asp:CheckBox ID="chkOnHand" runat="server" CssClass="acfc-checkbox" AutoPostBack="true" Enabled="false" />
                        </div>
                    </div>

                    <div class="row">
                        <div class="col-sm-2">
                            <label for="rdbPRType">Nội dung đề xuất</label>
                        </div>
                        <div class="col-sm-10">
                            <asp:CheckBoxList ID="chklstPR" runat="server" CssClass="acfc-checkbox" AutoPostBack="true" BorderStyle="Solid" BorderWidth="1px" Width="100%" TextAlign="Left" RepeatDirection="Horizontal" Enabled="false">
                                <asp:ListItem Value="1">Mua mới</asp:ListItem>
                                <asp:ListItem Value="2">Sửa chữa</asp:ListItem>
                                <asp:ListItem Value="3">Thay thế</asp:ListItem>
                            </asp:CheckBoxList>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-sm-2">
                            <label>Mục đích sử dụng</label>
                        </div>
                        <div class="col-sm-10">
                            <asp:Label ID="txtPurpose" runat="server" CssClass="form-control form-control-sm"></asp:Label>
                        </div>
                    </div>

                </div>

                <div class="acfc-div">

                    <div class="card">
                        <div class="card-header">
                            <label class="form-control">Danh sách hàng hóa</label>
                        </div>
                        <div class="card-body">

                            <asp:GridView ID="dgItems" runat="server" CssClass="table table-striped" AutoGenerateColumns="false">
                                <Columns>
                                    <asp:BoundField DataField="Line" HeaderText="STT" />
                                    <asp:BoundField DataField="ItemName" HeaderText="Chi tiết" />
                                    <asp:BoundField DataField="Unit" HeaderText="Đơn vị tính" DataFormatString="{0:#,##0.##}" ItemStyle-CssClass="input-number" />
                                    <asp:BoundField DataField="QtyOrder" HeaderText="Số lượng đặt hàng" DataFormatString="{0:#,##0.##}" ItemStyle-CssClass="input-number" />
                                    <asp:BoundField DataField="Amount" HeaderText="Chi phí ước tính" DataFormatString="{0:#,##0.##}" ItemStyle-CssClass="input-number" />
                                    <asp:BoundField DataField="DeliveryDate" HeaderText="Ngày giao hàng" DataFormatString="{0:MM/dd/yyyy}" />
                                    <asp:BoundField DataField="Note" HeaderText="Ghi chú" />
                                </Columns>
                            </asp:GridView>

                            <div class="row">
                                <div class="col-sm-2">
                                    <label for="txtPRAmount">Tổng chi phí ước tính</label>
                                </div>
                                <div class="col-sm-2">
                                    <asp:Label ID="txtPRAmount" runat="server" CssClass="form-control form-control-sm input-number" />
                                </div>
                            </div>
                        </div>
                    </div>

                </div>

            </div>

            <div class="acfc-div container acfc-border">
                <asp:Panel runat="server" id="pnlApprovalArea">
                    <div class="acfc-div">

                        <div class="row">
                            <div class="col-sm-1">
                                <label for="txtApprovalNote">Ghi chú</label>
                            </div>
                            <div class="col-sm-11">
                                <asp:TextBox runat="server" ID="txtApprovalNote" CssClass="form-control form-control-sm" Enabled="false"></asp:TextBox>
                            </div>
                        </div>
                        <br />
                        <div class="row">
                            <div class="col-sm-3">
                                <asp:Button ID="btnCancel" runat="server" Text="Hủy phiếu đề xuất" CssClass="btn btn-warning" OnClick="btnCancel_Click" Visible="false"/>
                            </div>
                            <div class="col-sm-3">
                                <asp:Button ID="btnApprove" runat="server" Text="Đồng ý" CssClass="btn btn-primary" OnClick="btnApprove_Click" Enabled="false" />
                            </div>
                            <div class="col-sm-3">
                                <asp:Button ID="btnReject" runat="server" Text="Từ chối" CssClass="btn btn-danger" OnClick="btnReject_Click" Enabled="false" />
                            </div>
                            <div class="col-sm-3">
                                <asp:Button ID="btnRemind" runat="server" Text="Nhắc bộ phận Admin" CssClass="btn btn-info" OnClick="btnRemind_Click" Enabled="false" Visible="false"/>
                            </div>
                        </div>

                    </div>
                </asp:Panel>
            </div>

        </ContentTemplate>
    </asp:UpdatePanel>




</asp:Content>