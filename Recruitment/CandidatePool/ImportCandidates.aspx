<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ImportCandidates.aspx.cs" Inherits="Recruitment.CandidatePool.ImportCandidates" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <script src="../Scripts/ACFC/SessionVerification.js"></script>

    <script src="../Scripts/jquery-ui-1.12.1.js"></script>
    <script src="../Scripts/jQuery.FileUpload/jquery.iframe-transport.js"></script>
    <script src="../Scripts/jQuery.FileUpload/jquery.fileupload.js"></script>

    <script src="../Scripts/DataTables/media/js/jquery.dataTables.js"></script>
    <script src="../Scripts/ACFC/CandidatePool/ImportCandidates.js"></script>

    <script>
        var rootUrl = "<% =(Request.Url.GetLeftPart(UriPartial.Authority) + Request.ApplicationPath) %>";
    </script>

    <link href="../Content/DataTables/media/css/jquery.dataTables.css" rel="stylesheet" />
    <link href="../Content/jQuery.FileUpload/css/jquery.fileupload.css" rel="stylesheet" />

    <%--<link href="../Content/themes/base/jquery-ui.css" rel="stylesheet" />--%>
    <%--<link href="../Content/jquery.tagsinput-revisited.css" rel="stylesheet" />--%>
    <%--<script src="../Scripts/tagsInput/jquery.tagsinput-revisited.js"></script>--%>

    <div class="container body-content">

        <div class="jumbotron">

            <div class="container">

                <div class="row">
                    <div class="col-lg-3"><label class="control-label">Nguồn tuyển dụng</label></div>
                    <div class="col-lg-4 input-text form-inline">

                        <select class="form-control form-control-sm" id="recruitmentResource" name="recruitmentResource"></select>
                        <button data-toggle="collapse" data-target="#divConfig" class="btn btn-secondary btn-sm" type="button">Xem danh sách cột</button>

                        <div id="divConfig" class="collapse">

                            <table id="tbConfiguration" class="display">
                                <thead class="thead-light">
                                    <tr>
                                        <th scope="col">Tên dữ liệu</th>
                                        <th scope="col">Cột excel</th>
                                        <th scope="col">Định dạng (dữ liệu ngày tháng)</th>
                                    </tr>
                                </thead>
                            </table>

                        </div>

                    </div>
                </div>

                <div class="row">
                    <div class="col-lg-3"><label class="control-label">Danh sách ứng viên</label></div>
                    <div class="col-lg-4 input-text">

                        <span class="btn btn-secondary btn-sm fileinput-button" id="btnBrowse" typeof="button">
                            <%--<i class="glyphicon glyphicon-plus"></i>--%>
                            <span>Chọn file</span>
                            <input id="fileUpload" type="file" name="files[]">
                        </span>
                        <label id="lblSelectedFile" />

                    </div>
                </div>

                <div class="row">
                    <div class="col-lg-3"><label class="control-label">Tải dữ liệu từ dòng</label></div>
                    <div class="col-lg-4 input-text">

                        <input class="form-control input-sm" id="numStartRow" type="number" value="2" />

                    </div>
                </div>

                <div class="row">
                    <div class="col-lg-3"><label class="control-label"></label></div>
                    <div class="col-lg-4 ">
                        <button id="btnImportFile" class="btn btn-primary btn-sm start" type="submit">
                            <%--<i class="glyphicon glyphicon-upload"></i>--%>
                            <span>Import data</span>
                        </button>
                    </div>
                </div>

            </div>

        </div>

    </div>

    </label>

</asp:Content>