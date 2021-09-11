$(function () {

    $("#tabNavigation a").click(function () {
        var tabName = $(this).attr("href").substring(1);
        $('#hfActiveTab').val(tabName);
    });

    var selectedTab = $('#hfActiveTab').val();
    
    var tabId = selectedTab !== "" ? selectedTab : "info";

    $('#tabNavigation a[href="#' + tabId + '"]').tab('show');

});

var prm = Sys.WebForms.PageRequestManager.getInstance();

prm.add_endRequest(function () {
    $("#tabNavigation a").click(function () {
        var tabName = $(this).attr("href").substring(1);
        $('#hfActiveTab').val(tabName);
    });

    var selectedTab = $('#hfActiveTab').val();

    var tabId = selectedTab !== "" ? selectedTab : "info";

    $('#tabNavigation a[href="#' + tabId + '"]').tab('show');
});