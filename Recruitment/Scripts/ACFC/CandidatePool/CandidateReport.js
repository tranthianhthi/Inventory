var loc = window.location.pathname;
var dataTable;
//var dir = loc.substring(0, loc.lastIndexOf('/'));

$(function () {
    $('#txtFrom').datepicker({
        dateFormat: "mm/dd/yy"
        , showButtonPanel: true
        , changeMonth: true
        , changeYear: true
    });

    $('#txtTo').datepicker({
        dateFormat: "mm/dd/yy"
        , showButtonPanel: true
        , changeMonth: true
        , changeYear: true
    });

    $('#txtStartFrom').datepicker({
        dateFormat: "mm/dd/yy"
        , showButtonPanel: true
        , changeMonth: true
        , changeYear: true
    });

    $('#txtStartTo').datepicker({
        dateFormat: "mm/dd/yy"
        , showButtonPanel: true
        , changeMonth: true
        , changeYear: true
    });

    $('#ctrTab:first').tab('show');

    $('#btnReport').on('click', Report);
    $('#btnReportEmployee').on('click', ReportEmployee);

});

function Report() {
    var from = $('#txtFrom').val();
    var to = $('#txtTo').val();

    var obj = { fromDate: from, toDate: to };

    $.ajax({
        type: "POST",
        url: loc + ".aspx/ReportCandidateStatus",
        data: JSON.stringify(obj),
        dataType: "json",
        contentType: "application/json; charset=utf-8", 
        success: function (response) {

            var data = JSON.parse(response.d);

            var columns = [
                { "title": "Nguồn tuyển dụng", "data": "resource_name"},
                { "title": "Đã xem", "data": "Đã xem", "className": "dt-body-center"  },
                { "title": "Đã liên hệ", "data": "Đã liên hệ", "className": "dt-body-center" },
                { "title": "Đã mời phỏng vấn", "data": "Đã mời phỏng vấn", "className": "dt-body-center"  },
                { "title": "Đã test", "data": "Đã test", "className": "dt-body-center"  },
                { "title": "Đã phỏng vấn", "data": "Đã phỏng vấn", "className": "dt-body-center"  },
                { "title": "Nhận", "data": "Nhận", "className": "dt-body-center"  },
                { "title": "Không nhận", "data": "Không nhận", "className": "dt-body-center"  },
                { "title": "Blacklist", "data": "Blacklist", "className": "dt-body-center"  },
                { "title": "Tổng số ứng viên", "data": "Tổng số ứng viên", "className": "dt-body-center"  }
            ];

            $('#example').DataTable({
                data: data
                , destroy: true
                , searching: false
                , ordering: false
                , paging: false
                , scrollY: "300px"
                , scrollX: true
                , scrollCollapse: true
                , columns: columns
            });

        },
        fail: function () {

        }
    });

}

function ReportEmployee() {
    var reportName = $("input[name='rdbReportType']:checked").val();
    var from = $('#txtStartFrom').val();
    var to = $('#txtStartTo').val();

    var js;
    var obj;
    var columns;
    var url = "";

    

    switch (reportName) {
        case "missingDocs":
            obj = { fromDate: from, toDate: to };
            js = JSON.stringify(obj);

            columns = [
                { "title": "Tên nhân viên", "data": "employee_name" },
                { "title": "Brand", "data": "brand_code" },
                { "title": "Mã cửa hàng", "data": "store_code" },
                { "title": "Tên cửa hàng", "data": "StoreName" } //, "className": "dt-body-center" 
            ];

            url = loc + ".aspx/ReportEmployeeNotSubmittedAllDocuments";
            break;
        case "newEmployee":
            obj = { fromDate: from, toDate: to };
            js = JSON.stringify(obj);

            columns = [
                { "title": "Brand", "data": "brand_code" },
                { "title": "Tổng số nhân viên mới", "data": "total_new_employees", "className": "dt-body-center" },
                { "title": "Tổng số nhân viên nghỉ", "data": "total_termination_employees", "className": "dt-body-center" }
            ];

            url = loc + ".aspx/ReportEmployeeEachBrand";
            break;
        default:
            break;
    }


    $.ajax({
        type: "POST",
        url: url,
        data: js,
        dataType: "json",
        contentType: "application/json; charset=utf-8",
        success: function (response) {

            var data = JSON.parse(response.d);

            dataTable = $('#example').DataTable({
                data: data
                , destroy: true
                , searching: false
                , ordering: false
                , paging: false
                , scrollY: "300px"
                , scrollX: true
                , scrollCollapse: true
                , columns: columns
            });

            dataTable.clear().draw();
        }
    });
}