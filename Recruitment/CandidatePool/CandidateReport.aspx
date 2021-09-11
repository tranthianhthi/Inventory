<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="CandidateReport.aspx.cs" Inherits="Recruitment.CandidatePool.CandidateReport" %>
<%@ Register Assembly="System.Web.DataVisualization, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" Namespace="System.Web.UI.DataVisualization.Charting" TagPrefix="asp" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <script src="../Scripts/jquery-ui-1.12.1.js"></script>
    <script src="../Scripts/Moments/moment.js"></script>
    <script src="../Scripts/DataTables/media/js/jquery.dataTables.js"></script>
    <script src="../Scripts/DataTables/extensions/FixedHeader/js/dataTables.fixedHeader.js"></script>
    <script src="../Scripts/ACFC/CandidatePool/CandidateReport.js"></script>

    <link href="../Content/DataTables/media/css/jquery.dataTables.css" rel="stylesheet" />
    <link href="../Content/DataTables/extensions/FixedHeader/css/fixedHeader.dataTables.css" rel="stylesheet" />

    <div class="acfc-div">

        <ul class="nav nav-pills" id="ctrTab">
            <li class="nav-item"><a class="nav-link active" data-toggle="tab" href="#candidate">Báo cáo số lượng ứng viên</a></li>
            <li class="nav-item"><a class="nav-link" data-toggle="tab" href="#termination">Báo cáo nhân viên</a></li>
        </ul>

        <div class="tab-content">
            <div id="candidate" class="tab-pane fade in active acfc-border">
                <div class="container acfc-div">
                    <div class="container">
                        <div class="row">
                            <div class="col-sm-2">
                                <label>Từ ngày</label></div>
                            <div class="col-sm-4">
                                <input type="text" id="txtFrom" class="form-control input-sm"/></div>
                        </div>
                        <div class="row">
                            <div class="col-sm-2">
                                <label>Đến ngày</label></div>
                            <div class="col-sm-4">
                                <input type="text" id="txtTo" class="form-control input-sm"/></div>
                        </div>
                        <div class="row">
                            <div class="col-sm-2">
                                <label></label>
                            </div>
                            <div class="col-sm-4">
                                <button class="btn btn-secondary btn-sm" type="button" id="btnReport">Xem báo cáo</button></div>
                        </div>
                    </div>
                </div>
            </div>
            <div id="termination" class="tab-pane fade in acfc-border">
                <div class="container acfc-div">
                    <div class="container">
                        <div class="row">
                            <div class="col-sm-2">
                                <label>Từ ngày</label>
                            </div>
                            <div class="col-sm-4">
                                <input type="text" id="txtStartFrom" class="form-control input-sm"/>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-sm-2">
                                <label>Đến ngày</label>
                            </div>
                            <div class="col-sm-4">
                                <input type="text" id="txtStartTo" class="form-control input-sm"/>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-sm-2">
                                <label>Loại báo cáo</label>
                            </div>
                            <div class="col-sm-4 inline">
                                <div class="radio">
                                    <label>
                                        <input type="radio" name="rdbReportType" checked value="missingDocs">Danh sách nhân viên chưa nộp đủ hồ sơ</label>
                                </div>
                                <div class="radio">
                                    <label>
                                        <input type="radio" name="rdbReportType" value="newEmployee">Báo cáo số nhân viên mới/nghỉ theo brand</label>
                                </div>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-sm-2">
                                <label></label>
                            </div>
                            <div class="col-sm-4">
                                <button class="btn btn-secondary btn-sm" type="button" id="btnReportEmployee">Xem báo cáo</button>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>

        
        
        
    </div>

    <div class="container">
        <table id="example" class="table table-sm table-hover display stripe " style="width:100%">

        </table>
    </div>
    

    </asp:Content>