var selectedOption;
var loc = window.location.pathname;
//var dir = loc.substring(0, loc.lastIndexOf('/'));

//Document.ready
$(function () {

    //if (!SessionVerification()) {
    //    return;
    //}

    // Bind event handler
    $('#recruitmentResource').on("change", OnRecruitmentResourceChanged);

    $('#btnImportFile').on("click", function (e) {

        e.preventDefault();

        var selectedFile = $('#fileUpload').get(0).files.length;
        if (selectedFile > 0) {
            UploadFile();
        } 
    });

    $('#fileUpload').change(function (e) {
        var fileName = $(this).val().split("\\").pop();//e.target.files[0].name;
        $('#lblSelectedFile').html(fileName);
    });

    // Format calendar
    $('#calendar').datepicker({
        dateFormat: "mm/dd/yy"
    });

    LoadRecruitmentResources();
});

// Gan data vao datatable
function OnConfigurationLoaded(response) {

    var data = JSON.parse(response.d);

    $('#tbConfiguration').DataTable({
        data: data
        , destroy: true
        , columns: [
            { data: 'field_name' },
            { data: 'column_name' },
            { data: 'field_format' }
        ]
    });
}

// 
function LoadRecruitmentResources() {

    $.ajax({
        type: "POST",
        url: loc + ".aspx/LoadRecruitmentResource",
        data: "",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (response) {
            var resources = JSON.parse(response.d);
            var items = '';

            $.each(resources, function (k, v) {
                items += '<option value=' + this.id + ' data-local=' + this.is_local_resource + ' data-url=' + this.resource_url + ' data-startrow=' + this.report_start_row + '>' + this.resource_name + '</option>';
            });

            $('#recruitmentResource').html(items);
            OnRecruitmentResourceChanged();
        },
        failure: function () {
            alert('Cannot get recruitment resources from database');
        }
    });
}

function OnRecruitmentResourceChanged() {

    selectedOption = $('#recruitmentResource').children("option:selected");

    var id = selectedOption.val();
    var startRow = selectedOption.data('startrow');

    $('#numStartRow').val(startRow);

    var obj = { resourceId: id };
    var jsString = JSON.stringify(obj);

    $.ajax({
        type: "POST",
        url: loc + ".aspx/GetResourceAllConfigurations",
        data: jsString,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: OnConfigurationLoaded,
        failure: function (response) {
            alert("Cannot load data configuration");
        }
    });
}

function UploadFile() {
    var files = $('#fileUpload').get(0).files;
    var fileData = new FormData();
    fileData.append(files[0].name, files[0]);

    var url = rootUrl + "/Handlers/UploadFile.ashx";

    $.ajax({
        type: "POST",
        url: url,
        enctype: "multipart/form-data",
        contentType: false, // Not to set any content header
        processData: false, // Not to process data
        data: fileData,
        success: function (result, status, xhr) {
            //alert(status);
            //alert(xhr.responseText);
            ImportExcelFile( xhr.responseText );
        },
        error: function (xhr, status, error) {
            alert("ERROR " + error);
        }
    });
}


function ImportExcelFile(fileName) {

    var id = selectedOption.val();
    var startRow = $('#numStartRow').val();

    var obj = { resourceId: id, fileName: fileName, startRow: startRow };
    var jsString = JSON.stringify(obj);

    $.ajax({
        type: "POST",
        url: loc + ".aspx/ImportCandidateData",
        data: jsString,
        dataType: "json",
        contentType: "application/json; charset=utf-8",
        success: function (response) {
            alert("Import completed");
        },
        failure: function (response) {
            alert("Import failed");
        }
    });
}