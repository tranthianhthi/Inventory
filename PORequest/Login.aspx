<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="PORequest.Login" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <script type="text/javascript">
        function clickButton(e, buttonid) {
            var evt = e ? e : window.event;
            var bt = document.getElementById(buttonid);

            if (bt) {
                if (evt.keyCode == 13) {
                    bt.click();
                    return false;
                }
            }
        }
    </script>

    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>

            <div class="container acfc-div">

                <p class="text-danger"><i>Hãy nhập username và password!</i></p>

                <div class="row">
                    <div class="col-sm-2">
                        <label for="txtUserName">User name</label>
                    </div>
                    <div class="col-sm-2">
                        <asp:TextBox ID="txtUserName" runat="server" CssClass="form-control input-sm"></asp:TextBox>
                    </div>
                </div>
                <div class="row">
                    <div class="col-sm-2">
                        <label for="txtPassword">Password</label>
                    </div>
                    <div class="col-sm-2">
                        <asp:TextBox ID="txtPassword" runat="server" CssClass="form-control input-sm" TextMode="Password" ></asp:TextBox>
                    </div>
                </div>
                <div class="row">
                    <div class="col-sm-2"></div>
                    <div class="col-sm-2">
                        <asp:Button ID="btnLogin" runat="server" Text="Login" CssClass="btn btn-primary" Width="100%" OnClick="btnLogin_Click"/>
                    </div>
                </div>
            </div>
        </ContentTemplate>
        <Triggers>

        </Triggers>
    </asp:UpdatePanel>

</asp:Content>
