<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="EmployeeDetails.aspx.cs" Inherits="Recruitment.CandidatePool.EmployeeDetails" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <script src="../Scripts/jquery-ui-1.12.1.js"></script>
    <script src="../Scripts/ACFC/CandidatePool/EmployeeDetails.js"></script>
    <script src="../Scripts/DataTables/media/js/jquery.dataTables.js"></script>
    <script src="../Scripts/DataTables/extensions/FixedHeader/js/dataTables.fixedHeader.js"></script>
    <script src="../Scripts/DataTables/extensions/FixedColumns/js/dataTables.fixedColumns.js"></script>
    <script src="../Scripts/Moments/moment.js"></script>
    <script src="../Scripts/jquery.validate.js"></script>

    <link href="../Content/DataTables/media/css/jquery.dataTables.css" rel="stylesheet" />
    <link href="../Content/DataTables/extensions/FixedHeader/css/fixedHeader.dataTables.css" rel="stylesheet" />
    <link href="../Content/DataTables/extensions/FixedColumns/css/fixedColumns.dataTables.css" rel="stylesheet" />

    <div class="acfc-div">
        <%--<div class="card-header">Danh sách nhân viên</div>
        <div class="card-body">--%>

        <ul class="nav nav-pills" id="ctrTab">
            <li class="nav-item"><a class="nav-link active" data-toggle="tab" href="#newEmployee">Nhập thông tin nhân viên mới</a></li>
            <li class="nav-item"><a class="nav-link" data-toggle="tab" href="#termination">Thông tin nhân viên</a></li>
        </ul>

        <div class="tab-content">
            <div id="newEmployee" class="tab-pane fade in active acfc-border">
                <div class="container acfc-div">
                    <div class="row">
                        <div class="col-sm-2">Từ ngày</div>
                        <div class="col-sm-3">
                            <input type="text" class="form-control form-control-sm" id="dtRecruitedDateFrom" />
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-sm-2">Đến ngày</div>
                        <div class="col-sm-3">
                            <input type="text" class="form-control form-control-sm" id="dtRecruitedDateTo" />
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-sm-2">Brand</div>
                        <div class="col-sm-3">
                            <select class="form-control form-control-sm" id="cboAssignedBrand"></select>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-sm-2"></div>
                        <div class="col-sm-3">
                            <button class="btn btn-sm btn-secondary" type="button" id="btnGetEmployee">Tải dữ liệu</button>
                            <button class="btn btn-sm btn-success" type="button" id="btnExportToExcel">Xuất file Excel</button>
                        </div>
                    </div>
                    <%--</div>--%>
                </div>
            </div>
            <div id="termination" class="tab-pane fade acfc-border">
                <div class="container acfc-div">
                    <div class="row">
                        <div class="col-sm-2">Mã nhân viên</div>
                        <div class="col-sm-3">
                            <input type="text" class="form-control form-control-sm" id="txtSearchEmpNo" />
                        </div>
                    </div>
                    <%--<div class="row">
                        <div class="col-sm-2">Tên nhân viên</div>
                        <div class="col-sm-3">
                            <input type="text" class="form-control form-control-sm" id="txtSearchEmpName" />
                        </div>
                    </div>--%>
                    <%--<div class="row">
                        <div class="col-sm-2">Brand</div>
                        <div class="col-sm-3">
                            <select class="form-control form-control-sm" id="cboSearchCurrentBrand"></select>
                        </div>
                    </div>--%>
                    <div class="row">
                        <div class="col-sm-2"></div>
                        <div class="col-sm-3">
                            <button class="btn btn-sm btn-secondary" type="button" id="btnFindEmployee">Tải dữ liệu</button>
                        </div>
                    </div>
                    <%--</div>--%>
                </div>
            </div>
        </div>

        
    </div>

    <div class="card acfc-div acfc-border">
        <div class="card-header">
            Thông tin nhân viên
            <input type="hidden" id="employeeId" />
        </div>
        <div class="card-body">
            <div class="row">
                <div class="col-sm-2">
                    Họ tên nhân viên
                </div>
                <div class="col-sm-6">
                    <label class="form-control form-control-sm" id="lblEmpName"></label>
                </div>
            </div>
            <div class="row">
                <div class="col-sm-2">
                    Ngày sinh
                </div>
                <div class="col-sm-6">
                    <label class="form-control form-control-sm" id="lblDob"></label>
                </div>
            </div>
            <div class="row">
                <div class="col-sm-2">
                    Mã nhân viên
                </div>
                <div class="col-sm-6">
                    <input class="form-control form-control-sm" type="text" id="txtEmpCode" />
                </div>
            </div>
            <div class="row">
                <div class="col-sm-2">
                    Brand
                </div>
                <div class="col-sm-6">
                    <select class="form-control form-control-sm" id="cboBrand"></select>
                </div>
            </div>
            <div class="row">
                <div class="col-sm-2">
                    Cửa hàng
                </div>
                <div class="col-sm-6">
                    <select class="form-control form-control-sm" id="cboStore"></select>
                </div>
            </div>
            <div class="row">
                <div class="col-sm-2">
                    Vị trí
                </div>
                <div class="col-sm-6">
                    <select class="form-control form-control-sm" id="cboPosition"></select>
                </div>
            </div>
            <div class="row">
                <div class="col-sm-2">
                    Mức lương thử việc
                </div>
                <div class="col-sm-6">
                    <input class="form-control form-control-sm" type="number" id="txtProbationSalary" />
                </div>
            </div>
            <div class="row">
                <div class="col-sm-2">
                    Mức lương cơ bản
                </div>
                <div class="col-sm-6">
                    <input class="form-control form-control-sm" type="number" id="txtBaseSalary" />
                </div>
            </div>
            <div class="row">
                <div class="col-sm-2">
                    Mức phụ cấp
                </div>
                <div class="col-sm-6">
                    <input class="form-control form-control-sm" type="number" id="txtAdditionalSalary" />
                </div>
            </div>
            <div class="row">
                <div class="col-sm-2">
                    Ngày nhận việc dự kiến
                </div>
                <div class="col-sm-6">
                    <input class="form-control form-control-sm" type="text" id="dtEstStartDate" />
                </div>
            </div>
            <div class="row">
                <div class="col-sm-2">
                    Ngày nhận việc
                </div>
                <div class="col-sm-6">
                    <input class="form-control form-control-sm" type="text" id="dtStartDate" />
                </div>
            </div>
            <div class="row">
                <div class="col-sm-2">
                    Số tài khoản
                </div>
                <div class="col-sm-6">
                    <input class="form-control form-control-sm" type="text" id="txtBankAccount" />
                </div>
            </div>
            <div class="row">
                <div class="col-sm-2">
                </div>
                <div class="col-sm-6">
                    <div class="radio">
                        <label>
                            <input type="radio" name="rdbIsPrivateAccount" checked value="private">Tài khoản có sẵn</label>
                    </div>
                    <div class="radio">
                        <label>
                            <input type="radio" name="rdbIsPrivateAccount" value="acfc">Tài khoản đăng ký bởi ACFC</label>
                    </div>
                </div>
            </div>
            <div class="row">
                <div class="col-sm-2">
                    Danh sách hồ sơ
                </div>
                <div class="col-sm-6">
                    <input type="checkbox" class="form-check-input" id="chkComplete"  />
                    <label class="form-check-label" for="chkComplete">Đã nộp đủ hồ sơ</label>
                    <button class="btn btn-sm btn-secondary" data-toggle="collapse" data-target="#divDocuments" type="button" id="btnShowDocumentList">Xem danh sách hồ sơ</button>
                    <div class="container collapse" id="divDocuments">
                        <table class="table table-sm" id="tbDocuments">
                            <thead>
                                <tr>
                                    <th>Tên hồ sơ</th>
                                    <th>Đã nộp</th>
                                    <th>Ngày nộp</th>
                                    <%--<th></th>--%>
                                </tr>
                            </thead>
                            <tbody>
                            </tbody>
                        </table>
                    </div>
                </div>
            </div>

            <div class="row">
                <div class="col-sm-2">
                    Ngày kết thúc
                </div>
                <div class="col-sm-6">
                    <input class="form-control form-control-sm" type="text" id="dtEndDate" />
                </div>
            </div>
            <div class="row">
                <div class="col-sm-2">
                    Lý do nghỉ việc
                </div>
                <div class="col-sm-6">
                    <textarea id="txtReason" class="form-control form-control-sm"></textarea>
                </div>
            </div>
            <div class="row">
                <div class="col-sm-3">
                </div>
                <div class="col-sm-6">
                </div>
            </div>
            <div class="row">
                <div class="col-sm-3">
                </div>
                <div class="col-sm-6">
                    <div class="acfc-div">
                        <button class="btn btn-sm btn-primary" id="btnUpdate" type="button">Cập nhật</button>
                        <button class="btn btn-sm btn-danger" id="btnCancel" type="button">Hủy</button>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <%--<div title="Danh sách nhân viên mới" id="diagNewEmployeeSelection">
        <table class="table table-sm table-hover display stripe row-border order-column" style="width:100%" id="tbNewEmployee">
            <thead>
                <tr>
                    <th>
                        Họ tên nhân viên
                    </th>
                    <th>
                        Ngày nhận
                    </th>
                    <th>
                        Brand
                    </th>
                    <th></th>
                </tr>
            </thead>
            <tbody>

            </tbody>
        </table>
    </div>--%>

    <%--<button type="button" class="btn btn-primary" data-toggle="modal" data-target="#exampleModalLong" id="btnDiag">
        Launch demo modal
    </button>--%>


    <div class="modal fade" id="exampleModalLong" tabindex="-1" role="dialog" aria-labelledby="exampleModalLongTitle" aria-hidden="true">
        <div class="modal-dialog modal-lg " role="document">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title" id="exampleModalLongTitle">Danh sách nhân viên mới</h5>
                    <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                        <span aria-hidden="true">&times;</span>
                    </button>
                </div>
                <div class="modal-body">
                    <table class="table table-sm table-hover display stripe row-border order-column" style="width: 100%" id="tbNewEmployee2">
                        <thead>
                            <tr>
                                <th>Họ tên nhân viên
                                </th>
                                <th>Ngày nhận
                                </th>
                                <th>Brand
                                </th>
                                <th></th>
                            </tr>
                        </thead>
                        <tbody>
                        </tbody>
                    </table>
                </div>
                <%--<div class="modal-footer">
                    <button type="button" class="btn btn-secondary" data-dismiss="modal">Close</button>
                    <button type="button" class="btn btn-primary">Save changes</button>
                </div>--%>
            </div>
        </div>
    </div>
</asp:Content>