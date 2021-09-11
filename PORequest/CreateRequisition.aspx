<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="CreateRequisition.aspx.cs" Inherits="PORequest.CreatePR" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <script src="Scripts/moment.js"></script>
    <script src="Scripts/jquery-ui-1.12.1.js"></script>

    <script type="text/javascript">
        function pageLoad() {

            $(":input").attr("autocomplete", "off");

            var selectedTab = $("#<%=hfTab.ClientID%>");
            var tabId = selectedTab.val() != "" ? selectedTab.val() : "info";
            

            $('#tabNavigation a[href="#' + tabId + '"]').tab('show');

            $("#tabNavigation a").click(function () {
                selectedTab.val($(this).attr("href").substring(1));
            });

            $("#<%=txtDeliveryDate.ClientID%>").datepicker({ dateFormat: "mm/dd/yy" });
            $("#<%=txtDate.ClientID%>").val(new Date().format("MM/dd/yy"));
            //$("#<%=txtDate.ClientID%>").datepicker({ dateFormat: "mm/dd/yy" }).datepicker("setDate", new Date());

            //$("#rangeDialog").dialog();
        }

        function ShowRangeDialog() {
            $(function () {
                $("#rangeDialog").dialog({
                    modal: true,
                    width: "auto",
                    resizable: false,
                    draggable: false,
                    close: function (event, ui) { $("body").find("#rangeDialog").remove(); },
                    buttons: {
                        "OK": function () { $(this).dialog("close"); }
                    }
                })
            }).dialog("open");
        }

    </script>

    <h3>
        Phiếu đề xuất mua hàng
        <small class="text-muted">Requisition form</small>
    </h3>

    <asp:HiddenField ID="hfTab" runat="server" />

    <ul class="nav nav-tabs" id="tabNavigation">
        <li><a data-toggle="tab" href="#info">Thông tin phiếu đề xuất</a></li>
        <li><a data-toggle="tab" href="#details">Chi tiết phiếu đề xuất</a></li>
    </ul>

    <div class="tab-content" id="dvTab">

        <div id="info" class="tab-pane fade in active acfc-border">

            <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                <ContentTemplate>

                    <div class="container acfc-div">

                        <div class="row">
                            <div class="col-sm-2">
                                <label for="txtUserName">Người lập:</label>
                            </div>
                            <div class="col-sm-2">
                                <asp:TextBox ID="txtUserName" runat="server" CssClass="form-control input-sm" ReadOnly="true"></asp:TextBox>
                            </div>
                            <div class="col-sm-2">
                                <label for="cboDept">Bộ phận:</label>
                            </div>
                            <div class="col-sm-2">
                                <asp:DropDownList ID="cboDept" runat="server" CssClass="form-control form-control-sm"></asp:DropDownList>
                            </div>
                            <div class="col-sm-2">
                                <label for="cboDeptCost">Bộ phận chịu chi phí:</label>
                            </div>
                            <div class="col-sm-2">
                                <asp:DropDownList ID="cboDeptCost" runat="server" CssClass="form-control form-control-sm"></asp:DropDownList>
                            </div>
                        </div>

                        <div class="row">
                            <div class="col-sm-2">
                                <label for="txtPRNo">Số PR:</label>
                            </div>
                            <div class="col-sm-2">
                                <asp:TextBox ID="txtPRNo" runat="server" CssClass="form-control input-sm font-weight-bold" ReadOnly="true" ></asp:TextBox>
                            </div>
                            <div class="col-sm-2">
                                <label for="txtDate">Ngày:</label>
                            </div>
                            <div class="col-sm-2">
                                <asp:TextBox ID="txtDate" runat="server" CssClass="form-control input-sm" readonly="true"></asp:TextBox>
                            </div>
                            <div class="col-sm-2">
                                <label for="chkOnHand">Đã kiểm tra tồn kho</label>
                            </div>
                            <div class="col-sm-2">
                                <asp:CheckBox ID="chkOnHand" runat="server" CssClass="acfc-checkbox" AutoPostBack="true" />
                            </div>
                        </div>

                        <div class="row">
                            <div class="col-sm-2">
                                <label for="rdbPRType">Nội dung đề xuất</label>
                            </div>
                            <div class="col-sm-10">
                                <asp:CheckBoxList ID="chklstPR" runat="server" CssClass="acfc-checkbox" AutoPostBack="true" BorderStyle="Solid" BorderWidth="1px" Width="100%" TextAlign="Left" RepeatDirection="Horizontal" >
                                    <asp:ListItem Value="1">Mua mới</asp:ListItem>
                                    <asp:ListItem Value="2">Sửa chữa</asp:ListItem>
                                    <asp:ListItem Value="3">Thay thế</asp:ListItem>
                                </asp:CheckBoxList>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-sm-2">
                                <label for="txtPurpose">Mục đích sử dụng</label>
                            </div>
                            <div class="col-sm-10">
                                <asp:TextBox ID="txtPurpose" runat="server" CssClass="form-control input-sm"></asp:TextBox>
                            </div>
                        </div>
                        <div class="row">
                            <asp:Panel runat="server" ID="pnlReason" Visible="false">
                                <div class="row">
                                    <div class="col-sm-2">
                                        <asp:Label runat="server" ID="lblReason" Text="Lý do yêu cầu hủy"></asp:Label>
                                    </div>
                                    <div class="col-sm-10">
                                        <asp:TextBox runat="server" ID="txtReason"></asp:TextBox>
                                    </div>
                                </div>
                            </asp:Panel>
                        </div>
                        <div class="row">
                            <asp:Panel runat="server" ID="pnlRemindNote" Visible="false">
                                <div class="row">
                                    <div class="col-sm-2">
                                        <asp:Label runat="server" ID="Label1" Text="Lời nhắc"></asp:Label>
                                    </div>
                                    <div class="col-sm-10">
                                        <asp:TextBox runat="server" ID="txtRemindNote"></asp:TextBox>
                                    </div>
                                </div>
                            </asp:Panel>
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
                                        <asp:TextBox ID="txtSTT" runat="server" CssClass="form-control input-sm" ReadOnly="true"></asp:TextBox>
                                    </div>
                                </div>

                                <div class="row">
                                    <div class="col-sm-2 require">
                                        <label for="txtItemName">Chi tiết</label>
                                    </div>
                                    <div class="col-sm-6">
                                        <asp:TextBox ID="txtItemName" runat="server" CssClass="form-control input-sm" PlaceHolder="Details"></asp:TextBox>
                                    </div>
                                </div>

                                <div class="row">
                                    <div class="col-sm-2">
                                        <label for="txtUnit">Đơn vị tính</label>
                                    </div>
                                    <div class="col-sm-2">
                                        <asp:TextBox ID="txtUnit" runat="server" CssClass="form-control input-sm" PlaceHolder="Unit"></asp:TextBox>
                                    </div>
                                </div>

                                <div class="row">
                                    <div class="col-sm-2">
                                        <label for="txtNote">Ghi chú</label>
                                    </div>
                                    <div class="col-sm-2">
                                        <asp:TextBox ID="txtNote" runat="server" CssClass="form-control input-sm" AutoPostBack="true" PlaceHolder="Note"></asp:TextBox>
                                    </div>
                                    <div class="col-sm-2 require">
                                        <label for="txtQty" >Số lượng đặt hàng</label>
                                    </div>
                                    <div class="col-sm-2">
                                        <asp:TextBox ID="txtQty" runat="server" CssClass="form-control input-sm input-number" AutoPostBack="true" TextMode="Number" PlaceHolder="Qty" min="1"></asp:TextBox>
                                    </div>
                                </div>

                                <div class="row">
                                    <div class="col-sm-2 require">
                                        <label for="txtDeliveryDate">Ngày giao hàng</label>
                                    </div>
                                    <div class="col-sm-2">
                                        <asp:TextBox ID="txtDeliveryDate" runat="server" CssClass="form-control input-sm" AutoPostBack="true" PlaceHolder="Delivery date"></asp:TextBox>
                                    </div>
                                    <div class="col-sm-2">
                                        <label for="txtAmount">Thành tiền</label>
                                    </div>
                                    <div class="col-sm-2">
                                        <asp:TextBox ID="txtAmount" runat="server" CssClass="form-control input-sm input-number" TextMode="Number" PlaceHolder="Estimate value"></asp:TextBox>
                                    </div>
                                </div>

                            </div>

                            <div class="row">
                                <div class="col-sm-2">
                                </div>
                                <div class="col-sm-2">
                                    <asp:Button ID="btnAdd" runat="server" Text="Thêm mặt hàng" CssClass="btn btn-primary" Width="100%" OnClick="btnAdd_Click" />
                                </div>
                                <div class="col-sm-2">
                                    <asp:Button ID="btnCancel" runat="server" Text="Hủy" CssClass="btn btn-danger" Width="100%" OnClick="btnCancel_Click"/>
                                </div>
                            </div>

                        </div>
                    
                    </div>

                    <div class="container acfc-div">

                        <div class="card">
                            <div class="card-header">
                                <label class="form-control">Danh sách hàng hóa</label>
                            </div>
                            <div class="card-body">

                                <asp:GridView ID="dgItems" runat="server" CssClass="table table-striped" AutoGenerateColumns="false"
                                    AutoGenerateDeleteButton="true" OnRowDeleting="dgItems_RowDeleting"
                                    AutoGenerateSelectButton="true" OnSelectedIndexChanged="dgItems_SelectedIndexChanged" DataKeyNames="id">
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
                                        <asp:TextBox ID="txtPRAmount" runat="server" CssClass="form-control input-sm input-number" ReadOnly="true"></asp:TextBox>
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

    <div class="acfc-div container">
        <div class="row">
            <div class="col-sm-3"></div>
            <div class="col-sm-3">
                <asp:Button ID="btnSave" runat="server" Text="Lưu đề xuất" CssClass="btn btn-primary" OnClick="btnSave_Click"/>
            </div>
            <div class="col-sm-3">
                <asp:Button ID="btnSubmitPR" runat="server" Text="Gửi đề xuất" CssClass="btn btn-success" OnClick="btnSubmitPR_Click"/>
            </div>
            <div class="col-sm-3"></div>
            <%--<div class="col-sm-3">
                <asp:Button ID="btnCancelPR" runat="server" Text="Hủy đề xuất" CssClass="btn btn-danger" OnClick="btnCancelPR_Click" />
            </div>   
            <div class="col-sm-3">
                <asp:Button ID="btnRemindPR" runat="server" Text="Nhắc đề xuất" CssClass="btn btn-warning" OnClick="btnRemindPR_Click"/>
            </div>--%>
        </div>
        
    </div>
    

    <div id="rangeDialog" style="display: none;" title="Total out of range">
        <p>
            <asp:Label runat="server" ID="lblDialogMsg"></asp:Label>
        </p>
    </div>


</asp:Content>