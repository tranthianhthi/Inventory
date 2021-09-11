<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="CandidatePool.aspx.cs" Inherits="Recruitment.CandidatePool.CandidatePool" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <script src="../Scripts/jquery-ui-1.12.1.js"></script>
    <script src="../Scripts/DataTables/media/js/jquery.dataTables.js"></script>
    <script src="../Scripts/DataTables/extensions/FixedHeader/js/dataTables.fixedHeader.js"></script>
    <script src="../Scripts/DataTables/extensions/FixedColumns/js/dataTables.fixedColumns.js"></script>
    <script src="../Scripts/Moments/moment.js"></script>
    <script src="../Scripts/ACFC/CandidatePool/CandidatePool.js"></script>

    <link href="../Content/DataTables/media/css/jquery.dataTables.css" rel="stylesheet" />
    <link href="../Content/DataTables/extensions/FixedHeader/css/fixedHeader.dataTables.css" rel="stylesheet" />
    <link href="../Content/DataTables/extensions/FixedColumns/css/fixedColumns.dataTables.css" rel="stylesheet" />
    <%--<link href="../Content/themes/base/jquery-ui.css" rel="stylesheet" />--%>

    <div class="card acfc-div">
        <div class="card-header">Lọc trạng thái ứng viên</div>
        <div class="card-body">
            <div class="row">
                <div class="col-sm-3">Trạng thái ứng viên</div>
                <div class="col-sm-3">
                    <select class="form-control form-control-sm" id="cboFilterStatus"></select></div>
            </div>
            <div class="row">
                <div class="col-sm-3"></div>
                <div class="col-sm-3">
                    <button class="btn btn-secondary btn-sm" id="btnLoadData" type="button">Tải dữ liệu</button></div>
            </div>
        </div>
    </div>

    <div id="div-table">
        <table id="tbCandidates" class="table table-sm table-hover display stripe row-border order-column" style="width:100%">
            <thead>
                <tr>
                    <th data-type='text'>Tên ứng viên</th>
                    <th data-type='text'>Vị trí ứng tuyển</th>
                    <th data-type='date'>Ngày sinh</th>
                    <th data-type='text'>Email</th>
                    <th data-type='text'>Số điện thoại</th>
                    <th data-type='text'>Địa chỉ</th>
                    <th data-type='date'>Ngày nộp</th>
                    <th data-type='text'>Trạng thái</th>
                    <th data-type='text'>Ghi chú</th>
                    <th></th>
                </tr>
            </thead>
        </table>
    </div>

    
    <div id="dialog-form" title="Cập nhật tình trạng ứng viên">
        <%--<p class="validateTips">All form fields are required.</p>--%>

        <div class="container">

            <div class="row">
                <div class="col-sm-4">
                    <label>Họ tên ứng viên</label>
                </div>
                <div class="col-sm-8">
                    <%--<input type="text" class="form-control form-control-sm" />--%>
                    <label id="diagCandidateName"></label>
                    <input type="hidden" id="diagId" />
                </div>
            </div>
            <div class="row">
                <div class="col-sm-4">
                    <label>Trạng thái</label>
                </div>
                <div class="col-sm-8">
                    <select class="form-control form-control-sm" id="diagCandidateStatus"></select>
                </div>
            </div>
            <div class="row">
                <div class="col-sm-4">
                    <label>Ghi chú</label>
                </div>
                <div class="col-sm-8">
                    <textarea class="form-control form-control-sm" rows="3" id="diagNote" style="resize: none;"></textarea>
                </div>
            </div>
            <div class="row" style="display:none;" id="divDiagBrand">
                <div class="col-sm-4">
                    <label>Brand</label>
                </div>
                <div class="col-sm-8">
                    <select class="form-control form-control-sm" id="diagBrand"></select>
                </div>
            </div>
        </div>
    </div>

<%--</div>--%>

</asp:Content>
