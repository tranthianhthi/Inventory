<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="RequisitionList.aspx.cs" Inherits="PORequest.PRList" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <script src="Scripts/moment.js"></script>
    <script src="Scripts/jquery-ui-1.12.1.js"></script>

    <script type="text/javascript">
        function pageLoad() {
            $("#<%=txtCreatedFrom.ClientID%>").datepicker({ dateFormat: "mm/dd/yy" });
            $("#<%=txtCreatedTo.ClientID%>").datepicker({ dateFormat: "mm/dd/yy" });//.datepicker("setDate", new Date());
        }

    </script>

    <asp:UpdatePanel ID="UpdatePanel1" runat="server" ChildrenAsTriggers="true">
        <ContentTemplate>

            <h3>
                Danh sách phiếu đề xuất
                <small class="text-muted">Requisition forms</small>
            </h3>

            <div class="container acfc-border acfc-div">

                <div class="acfc-div">
                    <div class="row">
                        <div class="col-sm-2">
                            <label for="txtPR">Số phiếu đề xuất</label>
                        </div>
                        <div class="col-sm-4">
                            <asp:TextBox ID="txtPR" runat="server" CssClass="form-control input-sm"></asp:TextBox>
                        </div>
                        <div class="col-sm-2">
                            <label for="txtPR">Brand</label>
                        </div>
                        <div class="col-sm-2">
                            <asp:DropDownList runat="server" id="cboBrand" Width="100%" CssClass="form-control form-control-sm"></asp:DropDownList>
                        </div>
                    </div>
                    <div class="row">
                         <div class="col-sm-2">
                            <label for="txtPR">Ngày tạo đề xuất</label>
                        </div>
                        <div class="col-sm-2">
                            <asp:TextBox ID="txtCreatedFrom" runat="server" CssClass="form-control input-sm"></asp:TextBox>
                        </div>
                        <div class="col-sm-2">
                            <asp:TextBox ID="txtCreatedTo" runat="server" CssClass="form-control input-sm"></asp:TextBox>
                        </div>
                        <div class="col-sm-2">
                            <label for="txtPR">Trạng thái đề xuất</label>
                        </div>
                        <div class="col-sm-4">
                            <asp:DropDownList runat="server" id="cboReqStatus" Width="100%" CssClass="form-control form-control-sm"></asp:DropDownList>
                        </div>
                    </div>

                    <div class="row">
                        <div class="col-sm-2"></div>
                        <div class="col-sm-2">
                            <asp:Button ID="btnSearch" runat="server" Text="Tìm kiếm" CssClass="btn btn-primary" OnClick="btnSearch_Click" Width="100%"/>
                        </div>
                    </div>
                </div>

                <asp:GridView ID="dgPR" runat="server" CssClass="table table-striped" AutoGenerateColumns="false"
                    AllowPaging="true" PageSize="50" OnPageIndexChanging="dgPR_PageIndexChanging">
                    <Columns>
                        <asp:TemplateField HeaderText="PR No.">
                            <ItemTemplate>
                                <asp:HyperLink ID="lnkPO" runat="server" Text='<%# Bind("PRNo") %>' OnDataBinding="lnkPO_DataBinding"></asp:HyperLink>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="BrandCode" HeaderText="Bộ phận tạo" />
                        <asp:BoundField DataField="DeptCost" HeaderText="Bộ phận chịu chi phí" />
                        <asp:TemplateField HeaderText="Mua mới">
                            <ItemTemplate>
                                <asp:Checkbox ID="chkIsNew" runat="server" Checked='<%# Bind("IsNew") %>' Enabled="false" Width="60px" CssClass="acfc-checkbox-inline"></asp:Checkbox>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Sửa chữa">
                            <ItemTemplate>
                                <asp:Checkbox ID="chkIsRepair" runat="server" Checked='<%# Bind("IsRepair") %>' Enabled="false" Width="60px" CssClass="acfc-checkbox-inline"></asp:Checkbox>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Thay thế">
                            <ItemTemplate>
                                <asp:Checkbox ID="chkIsReplace" runat="server" Checked='<%# Bind("IsReplace") %>' Enabled="false" Width="60px" CssClass="acfc-checkbox-inline"></asp:Checkbox>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="Remark" HeaderText="Ghi chú" />
                        <asp:BoundField DataField="TotalAmount" HeaderText="Tổng số tiền" DataFormatString="{0:#,##0.##}" ItemStyle-CssClass="input-number" />
                        <asp:BoundField DataField="CreatedDate" HeaderText="Ngày tạo" DataFormatString="{0:MM/dd/yyyy}"/>
                        <asp:BoundField DataField="StatusText" HeaderText="Trạng thái"/>
                    </Columns>
                </asp:GridView>

            </div>

        </ContentTemplate>
        <Triggers>
            <asp:AsyncPostBackTrigger ControlID="dgPR" />
        </Triggers>
    </asp:UpdatePanel>

</asp:Content>