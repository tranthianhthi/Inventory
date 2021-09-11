<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="MyRequests.aspx.cs" Inherits="Recruitment.RecruitmentRequest.MyRequests" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <link rel="stylesheet" type="text/css" href="../Content/DataTables/media/css/jquery.datatables.css">
    <script src="../Scripts/DataTables/media/js/jquery.datatables.js"></script>
    <script type="text/javascript">

        
        function Employee(name, position, salary, office) {
            this.name = name;
            this.position = position;
            this.salary = salary;
            this._office = office;

            this.office = function () {
                return this._office;
            }
        };

        $(function() {
            
            $.ajax({
                type: "POST",
                url: "../RecruitmentRequest/MyRequest.aspx/HelloWorld",
                data: "{}",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (msg) {
                    $("#HelloWorldContainer").text(msg.d);
                }
            });
           
        });
        
    </script>

    <div class="form-group">
        <div>
            <div id="HelloWorldContainer">
            </div>
        </div>
    </div>

</asp:Content>
