<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="TestPage.aspx.cs"  MasterPageFile="~/Site.Master" Inherits="TestWeb.TestPage" %>

<%@ Register Assembly="Microsoft.ReportViewer.WebForms" Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %>



<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">

    <%--<script type="text/javascript" src="Scripts/jquery-ui.js"></script>--%>
    <script type="text/javascript" >

        $( function() {
            //$( '#datepicker' ).datepicker();
            $("#datepicker").datepicker();
        });

    </script>

    <div class="jumbotron">

        <div class="container">
            <div class="row">
                <div class="col-xs-2">Store code</div>
                <div class="col-xs-5">
                    <asp:TextBox ID="txtALU" runat="server" CssClass="form-control"></asp:TextBox>
                </div>
                <div class="col-xs-5"></div>
            </div>
        </div>

        <div class="container">
            <div class="row">
                <div class="col-xs-2">Store code</div>
                <div class="col-xs-5">
                    <input type="text" id="datepicker" class="form-control">
                </div>
                <div class="col-xs-5">
        
                </div>
            </div>
        </div>

        <div class="container">
            <div class="row">
                <div class="col-xs-2">Label</div>
                <div class="col-xs-5">
                    <asp:DropDownList ID="DropDownList1" runat="server" CssClass="form-control">
                        <asp:ListItem Value="Report1.rdlc">Report1</asp:ListItem>
                    </asp:DropDownList>
                </div>
                <div class="col-xs-5"></div>
            </div>
        </div>

        <div class="container">
            <div class="row">
                <div class="col-xs-2">
                    <input type="submit" class="btn btn-primary" value="View report"/> 
                </div>
                <div class="col-xs-10"></div>
            </div>
        </div>

    </div>

    <rsweb:ReportViewer ID="ReportViewer1" runat="server" width="100%" AsyncRendering="False" SizeToReportContent="True">
        <ServerReport ReportPath="" ReportServerUrl="" />
    </rsweb:ReportViewer>

</asp:Content>
