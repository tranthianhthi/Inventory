<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="POList.aspx.cs" Inherits="PORequest.POList" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <asp:UpdatePanel ID="UpdatePanel1" runat="server" ChildrenAsTriggers="true">
        <ContentTemplate>

            <h3>
                Danh sách yêu cầu mua hàng
                <small class="text-muted">Purchase orders</small>
            </h3>

            <div class="container acfc-border acfc-div">

                <div class="acfc-div">
                    <div class="row">
                        <div class="col-sm-1">
                            <label for="txtPO">Số PO</label>
                        </div>
                        <div class="col-sm-2">
                            <asp:TextBox ID="txtPO" runat="server" CssClass="form-control input-sm"></asp:TextBox>
                        </div>
                         <div class="col-sm-1">
                            <label for="txtUser">UserID</label>
                        </div>
                        <div class="col-sm-2">
                            <asp:TextBox ID="txtUser" runat="server" CssClass="form-control input-sm"></asp:TextBox>
                        </div>
                        
                        <div class="col-sm-2">
                            <asp:Button ID="btnSearch" runat="server" Text="Tìm PO" CssClass="btn btn-primary" OnClick="btnSearch_Click" Width="100%"/>
                        </div>
                    </div>
                </div>

                <asp:GridView ID="dgPO" runat="server" CssClass="table table-striped" AutoGenerateColumns="false" 
                    AllowPaging="true" PageSize="50" OnPageIndexChanging="dgPO_PageIndexChanging">
                    <Columns>
                        <asp:TemplateField HeaderText="PO No.">
                            <ItemTemplate>
                                <asp:HyperLink ID="lnkPO" runat="server" Text='<%# Bind("PONo") %>' OnDataBinding="lnkPO_DataBinding"></asp:HyperLink>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="Brand" HeaderText="Bộ phận" />
                        <asp:BoundField DataField="PRNoRef" HeaderText="Mã phiếu yêu cầu" />
                        <asp:BoundField DataField="VendorName" HeaderText="Nhà cung cấp" />
                        <asp:BoundField DataField="DeliveryDate" HeaderText="Ngày hoàn thành" />
                        <asp:BoundField DataField="Note" HeaderText="Ghi chú" />
                        <asp:BoundField DataField="Deposit" HeaderText="Đặt cọc" DataFormatString="{0:#,##0.##}" ItemStyle-CssClass="input-number" />
                        <asp:BoundField DataField="ReturnDeposit" HeaderText="Ngày hoàn cọc" DataFormatString="{0:MM/dd/yyyy}" />
                        <asp:BoundField DataField="Amount" HeaderText="Số tiền" DataFormatString="{0:#,##0.##}" ItemStyle-CssClass="input-number" />
                        <asp:BoundField DataField="VAT" HeaderText="VAT" DataFormatString="{0:#,##0.##}" ItemStyle-CssClass="input-number" />
                        <asp:BoundField DataField="TotalAmount" HeaderText="Tổng số tiền" DataFormatString="{0:#,##0.##}" ItemStyle-CssClass="input-number" />
                        <asp:BoundField DataField="CreatedDate" HeaderText="Ngày tạo" DataFormatString="{0:MM/dd/yyyy}"/>
                    </Columns>
                </asp:GridView>

            </div>

        </ContentTemplate>
        <Triggers>
            <asp:AsyncPostBackTrigger ControlID="dgPO" />
        </Triggers>
    </asp:UpdatePanel>

</asp:Content>