var loc = window.location.pathname;
var storePositions = '';
var dialog;
var table;
var tableDocs;
var submittedList = "";
var htmlTableDocs;

$(function () {
    $('#dtRecruitedDateFrom').datepicker().datepicker('setDate', new Date());
    $('#dtRecruitedDateTo').datepicker().datepicker('setDate', new Date());
    $('#dtStartDate').datepicker();
    $('#dtEndDate').datepicker();
    $('#dtEstStartDate').datepicker();

    $('#btnGetEmployee').on('click', GetNewEmployee);
    $('#btnUpdate').on('click', UpdateEmployeeInfo);
    $('#btnFindEmployee').on('click', GetEmployeeDetails);
    $('#btnShowDocumentList').on('click', ShowDocuments);



    //$('#btnDiag').on('click', GetNewEmployee);

    dialog = $("#diagNewEmployeeSelection").dialog({
        autoOpen: false,
        height: 600,
        width: 800,
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

    table = $('#tbNewEmployee').DataTable({
        paging: true
        , searching: false
        , scrollY: "300px"
        , ordering: false
        , scrollX: true
        , scrollCollapse: true
    });

    //$('#diagNewEmployeeSelection tbody').on('click', 'tr', function () {
    //    alert(table.row(this).data());
    //});

    $('#cboBrand').on('change', GetAllStores);

    $('#ctrTab:first').tab('show');

    GetAllBrands();
    GetPositions();

    
});

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

            $('#cboBrand').html(options);
            $('#cboBrand').change();

            options = '<option value="0">Tất cả các brand</option>' + options;
            $('#cboAssignedBrand').html(options);
            $('#cboSearchCurrentBrand').html(options);
        },
        fail: function () {

        }
    });
}

function GetAllStores() {
    var value = $(this).val();
    var obj = { brand: value };

    $.ajax({
        type: "POST",
        url: loc + ".aspx/GetStoresOfBrand",
        data: JSON.stringify(obj),
        dataType: "json",
        contentType: "application/json; charset=utf-8",
        success: function (response) {

            var data = JSON.parse(response.d);
            var options = "";

            $.each(data, function (k, v) {
                options += '<option value="' + this.StoreCode + '">[' + this.StoreCode + '] ' + this.StoreName + '</option>';
            });

            $('#cboStore').html(options);
            if (options !== "") {
                $('#cboPosition').html(storePositions);
            }
            else {
                $('#cboPosition').html("");
            }
        },
        fail: function (response) {

        }
    });
}

function GetPositions() {
    $.ajax({
        type: "POST",
        url: loc + ".aspx/GetPositions",
        data: "",
        dataType: "json",
        contentType: "application/json; charset=utf-8",
        success: function (response) {

            var data = JSON.parse(response.d);

            $.each(data, function (k, v) {
                storePositions += '<option value="' + this.id + '">' + this.position_name + '</option>';
            });
        },
        fail: function (response) {

        }
    });
}

function GetNewEmployee() {

    var fromDate = $('#dtRecruitedDateFrom').val();
    var toDate = $('#dtRecruitedDateTo').val();
    var brand = $('#cboAssignedBrand').val();

    var obj;

    if (brand === "0") {
        obj = { fromDate: fromDate, toDate: toDate };

        $.ajax({
            type: "POST",
            url: loc + ".aspx/GetNewEmployeeOfAllBrands",
            data: JSON.stringify(obj),
            dataType: "json",
            contentType: "application/json; charset=utf-8",
            success: function (response) {

                var data = JSON.parse(response.d);

                BindDataToEmployeeTable(data);
            },
            fail: function (response) {

            }
        });
    }
    else {
        obj = { fromDate: fromDate, toDate: toDate, brandCode: brand };

        $.ajax({
            type: "POST",
            url: loc + ".aspx/GetNewEmployeeOfBrand",
            data: JSON.stringify(obj),
            dataType: "json",
            contentType: "application/json; charset=utf-8",
            success: function (response) {

                var data = JSON.parse(response.d);

                BindDataToEmployeeTable(data);
            },
            fail: function (response) {

            }
        });
    }
}

function BindDataToEmployeeTable(data) {
    table = $('#tbNewEmployee2').DataTable({
        data: data
        , destroy: true
        , searching: false
        , columns: [
            { data: 'candidate_name' },
            {
                data: 'created_date'
                , render: function (data, type, row) {
                    if (type === "sort" || type === "type") {
                        return data;
                    }
                    return moment(data).format("MM-DD-YYYY");
                }
                , className: 'dt-body-center'
            },
            { data: 'brandname' },
            {
                data: 'id'
                , render: function (data, type, row) {
                    var str = '<button class="btn btn-secondary btn-sm" type="button" id="btnEdit_' + data + '" onclick="EditEmployee(' + data + ')" style="width:80px;" >Cập nhật</button>';
                    return str;
                }
            }
        ]
    });

    //dialog.dialog('open');
    $('#exampleModalLong').modal('show');
}

function EditEmployee(id) {
    
    var btnEditName = 'btnEdit_' + id;
    var btnEdit = $('*[id=' + btnEditName + ']');

    var tr = $(btnEdit).closest('tr');
    obj = table.row(tr).data();

    $('#lblEmpName').text(obj.candidate_name);
    $('#lblDob').text(moment(obj.dob).format("MM/DD/YYYY"));
    $('#employeeId').val(id);

    $('#cboBrand option').filter(function () {
        return $(this).text() === obj.brand_code;
    }).prop('selected', true);  
    $('#cboBrand').change();

    //dialog.dialog('close');
    $('#exampleModalLong').modal('hide');
}

function UpdateEmployeeInfo() {

    var jsonDocs = "";

    var employeeId = $('#employeeId').val();
    var empCode = $('#txtEmpCode').val();
    var estStartDate = $('#dtEstStartDate').val();
    var startDate = $('#dtStartDate').val();
    var brandCode = $('#cboBrand').val();
    var storeCode = $('#cboStore').val();
    var positionId = $('#cboPosition').val();
    var probationSalary = $('#txtProbationSalary').val();
    var baseSalary = $('#txtBaseSalary').val();
    var additionalSalary = $('#txtAdditionalSalary').val();
    var bankAccount = $('#txtBankAccount').val();
    var isPrivateAccount = ( $('#rdbIsPrivateAccount :checked').val() === "private" );
    var submittedAllDocuments = $('#chkComplete').is(":checked");

    var endDate = $('#dtEndDate').val();
    var note = $('#txtReason').val();

    var docs = [];
    var totalRows = 0;

    if (tableDocs === null) {
        // do nothing
    }
    else {
        
        $('#tbDocuments tbody tr').each(function (i, row) {

            totalRows += 1;

            //var html = $(row).html();
            var children = $(row).children('td');
            var isSubmitted = $(children[1]).children('input').prop('checked');
            var submittedDate = $(children[2]).html();
            var docId = $(children[0]).children('input').val();

            if (isSubmitted) {
                var doc = { id: docId, isSubmitted: isSubmitted, submittedDate: submittedDate, employeeId: employeeId };
                docs.push(doc);
            }
        });       

        submittedAllDocuments = (docs.length === totalRows);
    }

    if (endDate === "") {
        obj =
        {
            employeeId: employeeId,
            empCode: empCode,
            estStartDate: estStartDate,
            startDate: startDate,
            brandCode: brandCode,
            storeCode: storeCode,
            positionId: positionId,
            probationSalary: probationSalary,
            baseSalary: baseSalary,
            additionalSalary: additionalSalary,
            bankAccount: bankAccount,
            isPrivateAccount: isPrivateAccount,
            submittedAllDocuments: submittedAllDocuments
        };

        $.ajax({
            type: "POST",
            url: loc + ".aspx/SaveNewEmployee",
            data: JSON.stringify(obj),
            dataType: "json",
            contentType: "application/json; charset=utf-8",
            success: function (response) {
                if (docs.length > 0) {
                    $.each(docs, function (i, v) {
                        SaveSubmittedDocuments(v);
                    });
                }
                else {
                    alert("Cập nhật thành công.");
                }
            },
            fail: function (response) {
                alert("Không cập nhật được dữ liệu. Vui lòng thử lại");
            }
        });
    }
    else {
        obj =
        {
            employeeId: employeeId,
            empCode: empCode,
            estStartDate: estStartDate,
            startDate: startDate,
            brandCode: brandCode,
            storeCode: storeCode,
            positionId: positionId,
            probationSalary: probationSalary,
            baseSalary: baseSalary,
            additionalSalary: additionalSalary,
            bankAccount: bankAccount,
            isPrivateAccount: isPrivateAccount,
            submittedAllDocuments: submittedAllDocuments,
            endDate: endDate,
            reason: note
        };

        $.ajax({
            type: "POST",
            url: loc + ".aspx/SaveTerminationEmployee",
            data: JSON.stringify(obj),
            dataType: "json",
            contentType: "application/json; charset=utf-8",
            success: function (response) {
                if (length(docs) > 0) {
                    SaveSubmittedDocuments(jsonDocs);
                }
                else {
                    alert("Cập nhật thành công.");
                }
            },
            fail: function (response) {
                alert("Không cập nhật được dữ liệu. Vui lòng thử lại");
            }
        });
    }
}

function GetEmployeeDetails() {
    var empCode = $('#txtSearchEmpNo').val();
    var obj = { empCode: empCode };
    var data = JSON.stringify(obj);

    $.ajax({
        type: "POST",
        url: loc + ".aspx/GetEmployeeDetails",
        data: data,
        dataType: "json",
        contentType: "application/json; charset=utf-8",
        success: function (response) {

            var emp = JSON.parse(response.d);
            DisplayEmployee(emp);
        },
        fail: function (response) {
            alert(response.d);
        }
    });
}

function DisplayEmployee(emp) {

    var employeeId = emp[0].id;
    var employeeName = emp[0].employee_name;
    var dob = emp[0].dob;
    var empCode = emp[0].employee_no;
    var estStartDate = emp[0].est_start_date;
    var startDate = emp[0].start_date;
    var brandCode = emp[0].brand_code;
    var storeCode = emp[0].store_code;
    var positionId = emp[0].position_id;
    var probationSalary = emp[0].probation_salary;
    var baseSalary = emp[0].base_salary;
    var additionalSalary = emp[0].additional_salary;
    var bankAccount = emp[0].bank_account;
    var isPrivateAccount = emp[0].is_private_account;
    var submittedAllDocuments = emp[0].submitted_all_documents;

    $('#employeeId').val(employeeId);
    $('#txtEmpCode').val(empCode);
    $('#lblEmpName').text(employeeName);

    $('#lblDob').text(moment(dob).format("MM/DD/YYYY"));
    $('#dtEstStartDate').val(moment(estStartDate).format("MM/DD/YYYY"));
    $('#dtStartDate').val(moment(startDate).format("MM/DD/YYYY"));

    $('#cboBrand option').filter(function () {
        return $(this).text() === brandCode;
    }).prop('selected', true);
    $('#cboBrand').change();

    $('#cboPosition option').filter(function () {
        return $(this).text() === positionId;
    }).prop('selected', true);

    $('#txtProbationSalary').val(probationSalary);
    $('#txtBaseSalary').val(baseSalary);
    $('#txtAdditionalSalary').val(additionalSalary);
    $('#txtBankAccount').val(bankAccount);

    $('input[name="rdbIsPrivateAccount"][value="private"]').prop('checked', isPrivateAccount);
    $('input[name="rdbIsPrivateAccount"][value="acfc"]').prop('checked', !isPrivateAccount);
    $('#chkComplete').prop("checked", submittedAllDocuments);

    $('#cboStore option').filter(function () {
        return $(this).text() === storeCode;
    }).prop('selected', true);
}

function ShowDocuments() {

    var employeeId = $('#employeeId').val();
    var obj = { employeeId: employeeId };

    $.ajax({
        type: "POST",
        url: loc + ".aspx/GetDocumentList",
        data: JSON.stringify(obj),
        dataType: "json",
        contentType: "application/json; charset=utf-8",
        success: function (response) {

            var data = JSON.parse(response.d);
            BindDocuments(data);
            
        },
        fail: function (response) {

        }
    });

}

function BindDocuments(data) {
    tableDocs = $('#tbDocuments').DataTable({
        data: data
        , destroy: true
        , searching: false
        , paging: false
        , sorting: false
        , columns: [
            {
                data: 'document_name',
                render: function (data, type, row) {
                    return "<input type='hidden' value='" + row['id'] + "'>" + data;
                }
            },
            {
                data: 'submitted',
                render: function (data, type, row) {
                    if (type === "sort" || type === "type") {
                        return data;
                    }
                    if (data === null || data === false) {
                        return "<input type='checkbox'>";
                    }
                    else {
                        return "<input type='checkbox' checked='true'>";
                    }
                }
                , className: 'dt-body-center'
            },
            {
                data: 'updated_date'
                , render: function (data, type, row) {
                    if (type === "sort" || type === "type") {
                        return data;
                    }
                    if (data === null) {
                        return "";
                    }
                    else {
                        return moment(data).format("MM/DD/YYYY");
                    }
                }
                , className: 'dt-body-center'
            }
            //{
            //    data: 'id'
            //    , render: function (data, type, row) {
            //        if (type === "sort" || type === "type") {
            //            return data;
            //        }
            //        else {
                        
            //        }
            //    }
            //    , className: 'd-none'
            //}
        ]
    });

    $('#tbDocuments input').on('change', DocumentSubmitted);
}

function DocumentSubmitted() {
    var checked = $(this).prop('checked');
    var nextTd = $(this).closest('td').next('td');
    var row = $(this).closest('tr');
    htmlTableDocs = $(this).closest('table');
    //alert(nextTd.html());

    if (checked === true) {
        var today = moment(new Date).format("MM/DD/YYYY");
        //alert(today);
        nextTd.html(today);
    }
    else {
        nextTd.html("");
    }
}

function SaveSubmittedDocuments(data) {
    $.ajax({
        type: "POST",
        url: loc + ".aspx/SaveSubmittedDocuments",
        data: JSON.stringify(data),
        dataType: "json",
        contentType: "application/json; charset=utf-8",
        success: function (response) {

        },
        fail: function (response) {

        }
    });
}