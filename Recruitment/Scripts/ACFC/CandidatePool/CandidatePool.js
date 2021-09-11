var loc = window.location.pathname;
//var dir = loc.substring(0, loc.lastIndexOf('/'));
var statusOption = '';
var table = null;

var selectedStatusID= -1;
var selectedStatusText = '';

var dialog;

$(function () {

    AddFilter();

    dialog = $("#dialog-form").dialog({
        autoOpen: false,
        height: 350,
        width: 640,
        modal: true,
        buttons: {
            "Lưu thay đổi": SaveData,
            "Hủy": function () {
                dialog.dialog("close");
            }
        },
        close: function () {
            
        }
    });

    $('#btnLoadData').on('click', LoadCandidates);

    GetAllBrands();

    GetStatus();

    // Đăng ký xử lý sự kiện select change trong datatables.net
    //$(document).on('change', 'select[id*=cboStatus]', function () {
    //    StateChanged($(this));
    //});
});

///Lấy tất cả status
function GetStatus() {
    $.ajax({
        type: "POST",
        url: loc + ".aspx/LoadStatus",
        data: "",
        dataType: "json",
        contentType: "application/json; charset=utf-8",
        success: function (response) {
            var data = JSON.parse(response.d);

            $.each(data, function (k, v) {
                statusOption += '<option value="' + this.id + '" >' + this.status_txt + '</option>';
            });

            var filterOption = '<option value="0" >Tất cả trạng thái</option>' + statusOption;
            $('#cboFilterStatus').html(filterOption);
            $('#diagCandidateStatus').html(statusOption);
        }
    });
}

///Lấy tất cả các brand code ( có store )
function GetAllBrands() {
    $.ajax({
        type: "POST",
        url: loc + ".aspx/GetAllBrands",
        data: "",
        dataType: "json",
        contentType: "application/json; charset=utf-8",
        success: function (response) {

            var data = JSON.parse(response.d);
            var options = "";

            $.each(data, function (k, v) {
                options += '<option value="' + this.BrandCode + '" >' + this.BrandCode.toUpperCase() + '</option>';
            });

            $('#diagBrand').html(options);
        },
        fail: function () {

        }
    });
}

///Nếu trạng thái mới của nhân viên là "Được nhận" thì sẽ cho phép chọn brand ngay trên cửa sổ cập nhật
function DisplayBrand() {
    var value = $(this).val();

    if (value === "6") {
        $('#divDiagBrand').show();
    }
    else {
        $('#divDiagBrand').hide();
    }
}

///Lấy danh sách ứng viên
function LoadCandidates() {
    var selectedStatus = $('#cboFilterStatus').val();
    var obj = { statusId: selectedStatus };
    var js = JSON.stringify(obj);

    $.ajax({
        type: "POST",
        url: loc + ".aspx/LoadCandidates",
        data: js,
        dataType: "json",
        contentType: "application/json; charset=utf-8",
        success: function (response) {

            var data = JSON.parse(response.d);

            table = $('#tbCandidates').DataTable({
                data: data
                , destroy: true
                , paging: true
                , scrollY: "500"
                , scrollX: true
                , scrollCollapse: true
                //, columnDefs: [
                //    { width: 100, targets: 0 },
                //    { width: 200, targets: 1 }
                //]
                , fixedColumns: { leftColumns: 1, rightColumns: 1 }
                , columns: [
                    { data: 'candidate_name', width: '100' },
                    { data: 'position', width: '200' },
                    {
                        data: 'dob'
                        , render: function (data, type, row) {
                            if (type === "sort" || type === "type") {
                                return data;
                            }
                            return moment(data).format("MM/DD/YYYY");
                        }
                        , className: 'dt-body-center'
                    },
                    { data: 'email' },
                    {
                        data: 'mobile_no'
                        , className: 'dt-body-center'
                    },
                    { data: 'address' },
                    {
                        data: 'submitted_date'
                        , render: function (data, type, row) {
                            if (type === "sort" || type === "type") {
                                return data;
                            }
                            return moment(data).format("MM-DD-YYYY");
                        }
                        , className: 'dt-body-center'
                    }, 
                    { data: 'status_txt' },
                    { data: 'note' },
                    {
                        data: 'id'
                        , render: function (data, type, row) {
                            var str = '<button class="btn btn-secondary btn-sm" type="button" id="btnEdit_' + data + '" onclick="EditRow(' + data + ')" style="width:80px;" >Cập nhật</button>'; 
                                        //    '<input type="text" id="txt_' + data + '" >' +
                                        //    '<select class="form-control form-control-sm" id="cboStatus_' + data + '" name="cboStatus_' + data + '" onchange="StatusChange(cboStatus_' + data + ')" style="width:100px;">' + statusOption + '</select>' +
                                        //    '<span class="btn btn-success btn-sm" id="btnSave_' + data + '" onclick="SaveRow(' + data + ')" style="width:40px;"><span>Lưu</span></span>' +
                                        //    '<span class="btn btn-danger btn-sm" id="btnCancel_' + data + '" onclick="CancelEditRow(' + data + ')" style="width:40px;"><span>Hủy</span></span>' + 
                                        //'</div>';
                            //style="display:none" 
                            return str;
                        }
                    }
                ]
            });

            $(table.table().container()).on('keyup change', 'thead input', function () {
                table
                    .columns($(this).data('index'))
                    .search(this.value)
                    .draw();
            });
        },
        fail: function () {
            alert("Không tải được danh sách ứng viên.");
        }
    });
}

///Nhấn button Edit -> hiện dialog cập nhật thông tin
function EditRow(id) {
    var btnEditName = 'btnEdit_' + id;
    //var divName = 'div_' + id;    
   
    var btnEdit = $('*[id=' + btnEditName + ']');
    //var div = $('*[id=' + divName + ']');

    //div.show();
    //btnEdit.hide();    

    //var editCell = btnEdit.parent("td");
    //var statusCell = editCell.prev();

    var tr = $(btnEdit).closest('tr');
    var txt = tr.children('td:first').text();

    var editCell = btnEdit.parent("td");
    var statusCell = editCell.prev();
    var currentStatus = statusCell.html();

    $('#diagCandidateName').html(txt);
    $('#diagId').val(id);

    $('#diagCandidateStatus option').filter(function () {
        return $(this).text() === currentStatus;
    }).prop('selected', true);

    //$("#cboEditCandidateStatus").val("val2");

    dialog.dialog("open");
    $('#diagCandidateStatus').on('change', DisplayBrand);

    //$(fixedCell).attr('rowspan', '2');
    //tr.after(format());   
}

///Lưu dữ liệu
function SaveData() {
    var id = $('#diagId').val();
    var selectedStatusID = $('#diagCandidateStatus').val();
    var selectedStatusText = $('#diagCandidateStatus').children('option:selected').text();
    var note = $('#diagNote').val();
    var brandCode = $('#diagBrand').val();

    var obj;

    if (selectedStatusID === "6") {
        obj = { id: id, newStatusId: selectedStatusID, brandCode: brandCode, note: note };

        $.ajax({
            type: "POST",
            url: loc + ".aspx/UpdateToNewEmployeeStatus",
            data: JSON.stringify(obj),
            dataType: "json",
            contentType: "application/json; charset=utf-8",
            success: function (response) {
                if (response.d === true) {

                    var filterId = $('#cboFilterStatus').val();

                    if (filterId === 0) {

                        var btnEditName = 'btnEdit_' + id;
                        var btnEdit = $('*[id=' + btnEditName + ']');

                        var editCell = btnEdit.parent("td");
                        var statusCell = editCell.prev();
                        statusCell.html(selectedStatusText);
                    }
                    else {
                        LoadCandidates();
                    }

                    dialog.dialog("close");
                }
            },
            fail: function (response) {
                alert("Có lỗi khi lưu dữ liệu.");
            }
        });
    }
    else {
        obj = { id: id, newStatusId: selectedStatusID, note: note };

        $.ajax({
            type: "POST",
            url: loc + ".aspx/UpdateCandidateStatus",
            data: JSON.stringify(obj),
            dataType: "json",
            contentType: "application/json; charset=utf-8",
            success: function (response) {
                if (response.d === true) {

                    var filterId = $('#cboFilterStatus').val();

                    if (filterId === 0) {

                        var btnEditName = 'btnEdit_' + id;
                        var btnEdit = $('*[id=' + btnEditName + ']');

                        var editCell = btnEdit.parent("td");
                        var statusCell = editCell.prev();
                        statusCell.html(selectedStatusText);
                    }
                    else {
                        LoadCandidates();
                    }

                    dialog.dialog("close");
                }
            },
            fail: function (response) {
                alert("Có lỗi khi lưu dữ liệu.");
            }
        });
    }
}

///Tắt dialog và không lưu dữ liệu
function CancelEditRow(id) {
    var btnEditName = 'btnEdit_' + id;
    var divName = 'div_' + id;

    var btnEdit = $('*[id=' + btnEditName + ']');
    var div = $('*[id=' + divName + ']');

    div.hide();
    btnEdit.show();    
}

///Thêm column filter cho cột dữ liệu
function AddFilter() {
    $('#tbCandidates thead tr').clone(true).appendTo('#tbCandidates thead');

    $('#tbCandidates thead tr:eq(1) th').each(function (i) {
        var type = $(this).data('type');
        var title = $(this).text();

        if (type === null) {
            // do nothing
        }
        else {
            switch (type) {
                case 'text':
                    $(this).html('<input type="text" placeholder="Lọc ' + title + '" data-index="' + i + '" data-index="' + i + '" />'); /*class="form-control"*/
                    break;
                    
                case 'date':
                    $(this).html('<input type="text" placeholder="Lọc ' + title + '" id="calendar' + i + '" data-index="' + i + '" />'); /* class="form-control" */
                    break;

                case 'select':
                    break;
            }
        }
    });

    var year = new Date().getFullYear() - 70;

    $('*[id*=calendar]').datepicker({
        dateFormat: "mm/dd/yy"
        , showButtonPanel: true
        , changeMonth: true
        , changeYear: true
        , yearRange: year + ":"
    });


    $('#tbCandidates').DataTable({
         paging: true
        , scrollY: "500"
        , ordering: false
        , scrollX: true
        , scrollCollapse: true
        //, fixedColumns: true
    });

    

}

const uri = 'api/TodoItems';
let todos = [];


//function StateChanged(ctr) {
//    var selectedOption = $(ctr).children(":selected");
//    selectedStatusText = selectedOption.text();
//    selectedStatusID = selectedOption.val();
//}

//function format() {
//    // `d` is the original data object for the row
//    return '<tr><td colspan="7"><div>ABCDXYZ</div></td></tr>';
//}

//function SaveRow(id) {
//    //var btnEditName = 'btnEdit_' + id;
//    //var divName = 'div_' + id;

//    //var btnEdit = $('*[id=' + btnEditName + ']');
//    //var div = $('*[id=' + divName + ']');

//    //div.hide();
//    //btnEdit.show();    

//    var obj = { id: id, newStatusId: selectedStatusID };

//    $.ajax({
//        type: "POST",
//        url: loc + ".aspx/UpdateStatus",
//        data: JSON.stringify(obj),
//        dataType: "json",
//        contentType: "application/json; charset=utf-8",
//        success: function (response) {
//            if (response.d === true) {
//                var editCell = btnEdit.parent("td");
//                var statusCell = editCell.prev();
//                statusCell.html(selectedStatusText);
//            }
//        },
//        fail: function (response) {
//            alert("Có lỗi khi lưu dữ liệu.");
//        }
//    });
//}
