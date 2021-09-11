<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="CreatePO.aspx.cs" Inherits="PORequest.CreatePO" %>

<%@ Register Assembly="CrystalDecisions.Web, Version=13.0.2000.0, Culture=neutral, PublicKeyToken=692fbea5521e1304" Namespace="CrystalDecisions.Web" TagPrefix="CR" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <script src="Scripts/moment.js"></script>
    <script src="Scripts/jquery-ui-1.12.1.js"></script>

    <script type="text/javascript">

        $(document).ready(function () {

            //$('#btnPreview').on("click", ShowPreviewDialog);
            start_switch();

            <%--$("#<%=rdbSub1.ClientID%>").change(function () {
                alert('Radio Box has been changed!');
            });--%>

            dialog = $("#diagPreview").dialog({
                autoOpen: false,
                height: 750,
                width: 1000,
                modal: true,
                buttons: {
                    //"Lưu thay đổi": SaveData,
                    "Hủy": function () {
                        dialog.dialog("close");
                    }
                },
                close: function () {

                }
            });
        });


        function start_switch() {
            $(".custom_check input").bootstrapSwitch();
        };

        function pageLoad() {

            var selectedTab = $("#<%=hfTab.ClientID%>");
            var tabId = selectedTab.val() != "" ? selectedTab.val() : "info";

            $('#tabNavigation a[href="#' + tabId + '"]').tab('show');

            $("#tabNavigation a").click(function () {
                selectedTab.val($(this).attr("href").substring(1));
            });

            $("#<%=txtReturnDeposit.ClientID%>").datepicker({ dateFormat: "mm/dd/yy" });
            $("#<%=txtDate.ClientID%>").datepicker({ dateFormat: "mm/dd/yy" }).datepicker("setDate", new Date());
            $("#<%=txtDeliveryDate.ClientID%>").datepicker({ dateFormat: "mm/dd/yy" });
        };

        function ShowProgress() {
            setTimeout(function () {
                var modal = $('<div />');
                modal.addClass("acfc-modal");
                $('body').append(modal);
                var loading = $(".acfc-loading");
                loading.show();
                var top = Math.max($(window).height() / 2 - loading[0].offsetHeight / 2, 0);
                var left = Math.max($(window).width() / 2 - loading[0].offsetWidth / 2, 0);
                loading.css({ top: top, left: left });
            }, 200);
        };

        function ShowPreviewDialog() {
            //alert("TEST");
            var loc = window.location.pathname + ".aspx/PreviewPO";
            alert(loc);
            $.ajax({
                type: "POST",
                url: loc + ".aspx/PreviewPO",
                data: "",
                dataType: "json",
                contentType: "application/json; charset=utf-8",
                success: function (response) {

                },
                fail: function () {

                }
            });
        };

    </script>

    <h3>
        Phiếu yêu cầu mua hàng
        <small class="text-muted">Purchase order</small>
    </h3>

    <asp:HiddenField ID="hfTab" runat="server" />
    <asp:HiddenField id="id" runat="server" />

    <asp:UpdatePanel runat="server" ID="pnlHeader" ChildrenAsTriggers="true">
        <ContentTemplate>
            <div class="acfc-div">
                <asp:RadioButton runat="server" ID="rdbSub1" GroupName="radioB" AutoPostBack="True" CssClass="acfc-checkbox" Text="ACFC"/>
                <asp:RadioButton runat="server" ID="rdbSub2" GroupName="radioB" AutoPostBack="True" CssClass="acfc-checkbox" Text="CMFC"/>
                <%--<div class="btn-group btn-group-toggle" data-toggle="buttons">
                                <label class="btn btn-default active">
                                    <asp:RadioButton runat="server" ID="rdbSub1" GroupName="radioB" AutoPostBack="True" CssClass="acfc-checkbox" />
                                    ACFC
                                </label>
                                <label class="btn btn-default">
                                    <asp:RadioButton runat="server" ID="rdbSub2" GroupName="radioB" AutoPostBack="True" CssClass="acfc-checkbox"/>
                                    CMFC
                                </label>
                            </div>--%>
            </div>
        </ContentTemplate>

    </asp:UpdatePanel>
    

    <ul class="nav nav-tabs" id="tabNavigation">
        <li><a data-toggle="tab" href="#info">Thông tin phiếu mua hàng</a></li>
        <li><a data-toggle="tab" href="#details">Chi tiết phiếu mua hàng</a></li>
    </ul>

    <div class="tab-content" id="dvTab">

        <div id="info" class="tab-pane fade in active acfc-border">

            <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                <ContentTemplate>

                    

                    <div class="container acfc-div">
                        <div class="row">
                            <div class="col-sm-2">
                                <label for="txtUserName">Họ và tên:</label>
                            </div>
                            <div class="col-sm-2">
                                <asp:TextBox ID="txtUserName" runat="server" CssClass="form-control input-sm" ReadOnly="true"></asp:TextBox>
                            </div>
                            <div class="col-sm-2">
                                <label for="txtDate">Ngày:</label>
                            </div>
                            <div class="col-sm-2">
                                <asp:TextBox ID="txtDate" runat="server" CssClass="form-control input-sm"></asp:TextBox>
                            </div>
                        </div>  

                        <div class="row">
                            <div class="col-sm-2">
                                <label for="cboDept">Bộ phận:</label><!--Bộ phận yêu cầu / bộ phận chịu chi phí -->
                            </div>
                            <div class="col-sm-2">
                                <asp:DropDownList ID="cboDept" runat="server" CssClass="form-control form-control-sm" AutoPostBack="true" OnSelectedIndexChanged="cboDept_SelectedIndexChanged"></asp:DropDownList>
                            </div>
                            <div class="col-sm-2">
                                <label for="txtPONo">Số PO:</label>
                            </div>
                            <div class="col-sm-2">
                                <asp:TextBox ID="txtPONo" runat="server" CssClass="form-control input-sm font-weight-bold" ReadOnly="true"></asp:TextBox>
                            </div>
                        </div>

                        <div class="row">
                            <div class="col-sm-2">
                                <label for="cboPRDepartment">Số requisition form:</label>
                            </div>
                            <div class="col-sm-2">
                                <asp:CheckBoxList ID="chklstPR" runat="server" CssClass="acfc-border acfc-checkbox" AutoPostBack="true" Width="100%" TextAlign="Left" OnSelectedIndexChanged="chklstPR_SelectedIndexChanged"></asp:CheckBoxList>
                            </div>
                            
                        </div>
                        <div class="row">
                             <div class="col-sm-2">
                                <label for="txtListPR">Danh Sách Requisition:</label>
                            </div>
                            <div class="col-sm-10">
                                 <asp:TextBox ID="txtListPR" runat="server" CssClass="form-control input-sm" ></asp:TextBox>
                            </div>
                        </div>

                        <div class="card acfc-div">
                            <div class="card-header">
                                <label class="form-control">Thông tin nhà cung cấp</label>
                            </div>
                            <div class="card-body">

                                <div class="row">
                                    <div class="col-sm-2">
                                        <label for="cboVendor require">Nhà cung cấp</label>
                                    </div>
                                    <div class="col-sm-2">
                                        <asp:TextBox runat="server" ID="txtFilterVendor" CssClass="form-control input-sm" AutoPostBack="true" OnTextChanged ="txtFilterVendor_TextChanged"  placeholder="Nhập tên để tìm kiếm"></asp:TextBox> 
                                    </div>
                                    <div class="col-sm-6">
                                        <asp:DropDownList ID="cboVendor" runat="server" CssClass="form-control input-sm" AutoPostBack="true" Width="100%"></asp:DropDownList>
                                    </div>
                                </div>

                            </div>
                        </div>

                        <div class="card acfc-div">
                            <div class="card-header">
                                <label class="form-control">Thông tin thanh toán</label>
                            </div>
                            <div class="card-body">
                                <div class="row">
                                    <div class="col-sm-2">
                                        <label for="chkDeposit">Tạm ứng/đặt cọc</label>
                                    </div>
                                    <div class="col-sm-2">
                                        <asp:CheckBox ID="chkDeposit" runat="server" CssClass="acfc-checkbox" AutoPostBack="true" OnCheckedChanged="chkDeposit_CheckedChanged"/>
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-sm-2">
                                        <label for="txtDeposit">Số tiền cọc</label>
                                    </div>
                                    <div class="col-sm-2">
                                        <asp:TextBox ID="txtDeposit" placeholder="Số tiền đặt cọc" runat="server" TextMode="Number" CssClass="form-control input-sm input-number" Enabled="false"></asp:TextBox>
                                    </div>
                                    <div class="col-sm-2">
                                        <label for="txtReturnDeposit">Ngày hoàn cọc</label>
                                    </div>
                                    <div class="col-sm-2">
                                        <asp:TextBox ID="txtReturnDeposit" placeholder="Ngày hoàn cọc" runat="server" CssClass="form-control input-sm" Enabled="false"></asp:TextBox>
                                    </div>
                                </div>
                                
                                <%--<div class="row">
                                    <div class="col-sm-2">
                                        <label for="txtSettlement1">Thanh toán lần 1</label>
                                    </div>
                                    <div class="col-sm-2">
                                        <asp:TextBox ID="txtSettlement1" runat="server" CssClass="form-control input-sm"></asp:TextBox>
                                    </div>
                                    <div class="col-sm-2">
                                        <label for="txtSettlement2">Thanh toán lần 2</label>
                                    </div>
                                    <div class="col-sm-2">
                                        <asp:TextBox ID="txtSettlement2" runat="server" CssClass="form-control input-sm"></asp:TextBox>
                                    </div>
                                    <div class="col-sm-2">
                                        <label for="txtSettlement3">Thanh toán lần cuối</label>
                                    </div>
                                    <div class="col-sm-2">
                                        <asp:TextBox ID="txtSettlement3" runat="server" CssClass="form-control input-sm"></asp:TextBox>
                                    </div>
                                </div>--%>
                                <div class="row">
                                    <div class="col-sm-2">
                                        <label for="txtDeliveryDate">Ngày dự kiến hoàn thành</label>
                                    </div>
                                    <div class="col-sm-2">
                                        <asp:TextBox ID="txtDeliveryDate" placeholder="Ngày dự kiến hoàn thành" runat="server" CssClass="form-control input-sm"></asp:TextBox>
                                    </div>
                                    <div class="col-sm-2">
                                        <label for="chkHaveContract">Có hợp đồng</label>
                                    </div>
                                    <div class="col-sm-2">
                                        <asp:CheckBox ID="chkHaveContract" runat="server" CssClass="acfc-checkbox" />
                                    </div>
                                </div>
                            </div>
                        </div>


                        <div class="row">
                            <div class="col-sm-2">
                                <label for="txtNote">Diễn giải</label>
                            </div>
                            <div class="col-sm-10">
                                <asp:TextBox ID="txtNote" runat="server" CssClass="form-control input-sm"></asp:TextBox>
                            </div>
                        </div>
                    </div>

                </ContentTemplate>
            </asp:UpdatePanel>

        </div>

        <div id="details" class="tab-pane fade acfc-border">

            <asp:UpdatePanel ID="UpdatePanel2" runat="server">
                <ContentTemplate>

                    <div class="container acfc-div">
                        <div class="card">
                            <div class="card-header">
                                <label class="form-control"><b>Chi tiết yêu cầu mua hàng</b></label>
                            </div>
                            <div class="card-body">
                                <div class="row">
                                    <div class="col-sm-2">
                                        <label for="txtSTT">STT</label>
                                    </div>
                                    <div class="col-sm-2">
                                        <asp:TextBox ID="txtSTT" runat="server" CssClass="form-control input-sm input-number" ReadOnly="true"></asp:TextBox>
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-sm-2 require">
                                        <label for="txtItemName">Chi tiết</label>
                                    </div>
                                    <div class="col-sm-6">
                                        <asp:TextBox ID="txtItemName" runat="server" CssClass="form-control input-sm" placeholder="Tên mặt hàng"></asp:TextBox>
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-sm-2 require">
                                        <label for="txtPrice">Đơn giá</label>
                                    </div>
                                    <div class="col-sm-2">
                                        <asp:TextBox ID="txtPrice" runat="server" TextMode="Number" placeholder="Đơn giá" CssClass="form-control input-sm input-number" OnTextChanged="txtPrice_TextChanged" AutoPostBack="true"></asp:TextBox>
                                    </div>
                                    <div class="col-sm-2 require">
                                        <label for="txtQty">Số lượng</label>
                                    </div>
                                    <div class="col-sm-2">
                                        <asp:TextBox ID="txtQty" runat="server" TextMode="Number" placeholder="Số lượng" CssClass="form-control input-sm input-number" OnTextChanged="txtQty_TextChanged" AutoPostBack="true"></asp:TextBox>
                                    </div>
                                    <div class="col-sm-2 require">
                                        <label for="cboVAT">VAT (%)</label>
                                    </div>
                                    <div class="col-sm-2">
                                        <asp:DropDownList ID="cboVAT" runat="server" CssClass="form-control input-sm input-number" AutoPostBack="True" OnSelectedIndexChanged="cboVAT_SelectedIndexChanged" OnTextChanged="cboVAT_TextChanged">
                                            <asp:ListItem Value="0">0%</asp:ListItem>
                                            <asp:ListItem Value="5">5%</asp:ListItem>
                                            <asp:ListItem Value="10">10%</asp:ListItem>
                                        </asp:DropDownList>
                                    </div>
                                </div>

                                <div class="row">
                                    <div class="col-sm-2">
                                        <label for="txtAmount">Thành tiền</label>
                                    </div>
                                    <div class="col-sm-2">
                                        <asp:TextBox ID="txtAmount" runat="server" placeholder="Số tiền trước thuế" CssClass="form-control input-sm input-number" ReadOnly="true"></asp:TextBox>
                                    </div>
                                    <div class="col-sm-2">
                                        <label for="txtVATAmount">Thuế VAT</label>
                                    </div>
                                    <div class="col-sm-2">
                                        <asp:TextBox ID="txtVATAmount" runat="server" placeholder="Tiền thuế" CssClass="form-control input-sm input-number" ReadOnly="true"></asp:TextBox>
                                    </div>
                                    <div class="col-sm-2">
                                        <label for="txtTotalAmount">Tổng thành tiền</label>
                                    </div>
                                    <div class="col-sm-2">
                                        <asp:TextBox ID="txtTotalAmount" runat="server" placeholder="Số tiền sau thuế" CssClass="form-control input-sm input-number" ReadOnly="true"></asp:TextBox>
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-sm-2">
                                        <label for="txtItemNote">Ghi chú</label>
                                    </div>
                                    <div class="col-sm-10">
                                        <asp:TextBox ID="txtItemNote" placeholder="Ghi chú - bộ phận chịu chi phí" runat="server" CssClass="form-control input-sm"></asp:TextBox>
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-sm-2">
                                    </div>
                                    <div class="col-sm-2">
                                        <asp:Button ID="btnAdd" runat="server" Text="Thêm vào PO" CssClass="btn btn-primary" OnClick="btnAdd_Click" Width="100%"/>
                                    </div>
                                    <div class="col-sm-2">
                                        <asp:Button ID="btnCancel" runat="server" Text="Hủy" CssClass="btn btn-danger" OnClick="btnCancel_Click" Width="100%"/>
                                    </div>
                                </div>

                            </div>
                        </div>

                        <div class="card acfc-div">
                            <div class="card-header">
                                <label class="form-control">Danh sách hàng hóa</label>
                            </div>
                            <div class="card-body">

                                <asp:GridView ID="dgItems" runat="server" CssClass="table table-striped" AutoGenerateColumns="false" DataKeyNames="id"
                                    AutoGenerateDeleteButton="true" OnRowDeleting="dgItems_RowDeleting" 
                                    AutoGenerateSelectButton="true" OnSelectedIndexChanged="dgItems_SelectedIndexChanged"> 
                                    <Columns>
                                        <asp:BoundField DataField="Line" HeaderText="STT" />
                                        <asp:BoundField DataField="ItemName" HeaderText="Chi tiết" />
                                        <%--<asp:BoundField DataField="ChargeToDept" HeaderText="Phân bổ bộ phận" />--%>
                                        <asp:BoundField DataField="UnitPrice" HeaderText="Đơn giá" DataFormatString="{0:#,##0.##}" ItemStyle-CssClass="input-number" />
                                        <asp:BoundField DataField="VAT" HeaderText="Thuế suất(%)" DataFormatString="{0:#,##0.##}" ItemStyle-CssClass="input-number" />
                                        <asp:BoundField DataField="Qty" HeaderText="Số lượng" DataFormatString="{0:#,##0.##}" ItemStyle-CssClass="input-number" />
                                        <asp:BoundField DataField="Amount" HeaderText="Thành tiền" DataFormatString="{0:#,##0.##}" ItemStyle-CssClass="input-number" />
                                        <asp:BoundField DataField="TotalVAT" HeaderText="Tiền thuế" DataFormatString="{0:#,##0.##}" ItemStyle-CssClass="input-number" />
                                        <asp:BoundField DataField="TotalAmount" HeaderText="Tổng thành tiền" DataFormatString="{0:#,##0.##}" ItemStyle-CssClass="input-number" />
                                        <asp:BoundField DataField="ItemNote" HeaderText="Ghi chú" />
                                    </Columns>
                                </asp:GridView>

                                <div class="row">
                                    <div class="col-sm-2">
                                        <label for="txtPOAmount">Tổng tiền</label>
                                    </div>
                                    <div class="col-sm-2">
                                        <asp:TextBox ID="txtPOAmount" placeholder="Tổng giá trị trước thuế" runat="server" CssClass="form-control input-sm input-number" ReadOnly="true"></asp:TextBox>
                                    </div>
                                    <div class="col-sm-2">
                                        <label for="txtPOVATAmount">VAT/ Thuế GTGT</label>
                                    </div>
                                    <div class="col-sm-2">
                                        <asp:TextBox ID="txtPOVATAmount" placeholder="Tổng tiền thuế" runat="server" CssClass="form-control input-sm input-number" ReadOnly="true"></asp:TextBox>
                                    </div>
                                    <div class="col-sm-2">
                                        <label for="txtPOTotalAmount">Tổng thành tiền</label>
                                    </div>
                                    <div class="col-sm-2">
                                        <asp:TextBox ID="txtPOTotalAmount" placeholder="Tổng giá trị sau thuế" runat="server" CssClass="form-control input-sm input-number" ReadOnly="true"></asp:TextBox>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>

                </ContentTemplate>
                <Triggers>
                    <asp:AsyncPostBackTrigger ControlID="dgItems" />
                </Triggers>
            </asp:UpdatePanel>

        </div>

    </div>

    <asp:UpdatePanel runat="server" ID="pnlCommonButtons">
        <ContentTemplate>
            <div class="acfc-div container">
                <div class="row">
                    <div class="col-sm-2">
                    </div>
                    <div class="col-sm-2">
                        <asp:Button ID="btnSave" runat="server" Text="Lưu phiếu mua hàng" CssClass="btn btn-primary" OnClick="btnSave_Click" Width="100%" />
                    </div>
                    <div class="col-sm-2">
                       <%-- <button type="button" class="btn btn-success" data-toggle="modal" data-target="#diagPreview" id="btnPreview" runat="server" onserverclick="btnPreview_Click">
                            Xem trước bản in
                        </button>--%>
                        <asp:Button ID="btnPreview"  OnClick="btnPreview_Click" runat="server" Text="Xem trước bản in" CssClass="btn btn-success" Width="100%" />
                    </div>
                    <div class="col-sm-2">
                        <asp:Button ID="btnPrint" runat="server" Text="In phiếu mua hàng" CssClass="btn btn-warning" OnClick="btnPrint_Click" Width="100%" Visible="false"/>
                    </div>
                </div>
            </div>

        </ContentTemplate>
    </asp:UpdatePanel>

</asp:Content>

