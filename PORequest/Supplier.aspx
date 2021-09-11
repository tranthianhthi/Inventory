<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Supplier.aspx.cs" Inherits="PORequest.Supplier" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <h3>
        Danh sách nhà cung cấp
    </h3>

    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>

            <asp:HiddenField ID="hdfId" runat="server" />

            <div class="container acfc-div">
                <div class="row">
                    <div class="col-sm-2">
                        <label for="txtAddressCode">Address code</label>
                    </div>
                    <div class="col-sm-10">
                        <asp:TextBox ID="txtAddressCode" runat="server" CssClass="form-control input-sm"></asp:TextBox>
                    </div>
                </div>
                <div class="row">
                    <div class="col-sm-2">
                        <label for="txtName">Tên nhà cung cấp</label>
                    </div>
                    <div class="col-sm-10">
                        <asp:TextBox ID="txtName" runat="server" CssClass="form-control input-sm"></asp:TextBox>
                    </div>
                </div>
                <div class="row">
                    <div class="col-sm-2">
                        <label for="txtTaxCode">Mã số thuế</label>
                    </div>
                    <div class="col-sm-10">
                        <asp:TextBox ID="txtTaxCode" runat="server" CssClass="form-control input-sm"></asp:TextBox>
                    </div>
                </div>
                <div class="row">
                    <div class="col-sm-2">
                        <label for="txtAddress">Địa chỉ nhà cung cấp</label>
                    </div>
                    <div class="col-sm-10">
                        <asp:TextBox ID="txtAddress" runat="server" CssClass="form-control input-sm"></asp:TextBox>
                    </div>
                </div>
                <div class="row">
                    <div class="col-sm-2">
                    </div>
                    <div class="col-sm-2">
                        <asp:Button ID="btnSearch" runat="server" CssClass="btn btn-sm btn-secondary" Text="Tìm kiếm" OnClick="btnSearch_Click" Width="100%"/>
                    </div>
                    <div class="col-sm-2">
                        <asp:Button ID="btnSave" runat="server" Text="Lưu thông tin" class="btn btn-sm btn-primary" OnClick="btnSave_Click" Width="100%"/>
                    </div>
                    <div class="col-sm-2">
                        <asp:Button ID="btnCancel" runat="server" Text="Hủy" class="btn btn-sm btn-danger" OnClick="btnCancel_Click" Width="100%"/>
                    </div>
                </div>

            </div>

            <asp:GridView ID="dgSuppliers" runat="server" CssClass="table table-striped table-bordered table-hover" AutoGenerateColumns="false" 
                AllowPaging="true" PageSize="50" OnPageIndexChanging="dgSuppliers_PageIndexChanging" 
                AutoGenerateSelectButton="true" OnSelectedIndexChanged="dgSuppliers_SelectedIndexChanged">
                <Columns>
                    <asp:BoundField DataField="id" HeaderText="STT" />
                    <asp:BoundField DataField="addresscode" HeaderText="Mã địa chỉ" />
                    <asp:BoundField DataField="name" HeaderText="Tên nhà cung cấp" />
                    <asp:BoundField DataField="taxcode" HeaderText="Mã số thuế" />
                    <asp:BoundField DataField="address" HeaderText="Địa chỉ nhà cung cấp" />
                </Columns>
            </asp:GridView>

        </ContentTemplate>
        <Triggers>

        </Triggers>
    </asp:UpdatePanel>

</asp:Content>
