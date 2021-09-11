$(function () {
    $('#btnLogin').on('click', Login);
});

function Login() {

    var id = $('#username').val();
    var pwd = $('#psw').val();

    var obj = { username: id, password: pwd };

    $.ajax({
        type: "POST",
        url: rootUrl + "/Handlers/LoginHandler.ashx",
        data: JSON.stringify(obj),
        dataType: "json",
        //contentType: false,
        //processData: false, 
        success: function (data) {
            //(result, status, xhr) {
            alert(data);
            var res = JSON.parse(response.d);

            //if (res.result === "true") {
            //    //donothing
            //}
            //else {
            //    alert(res.message);
            //}
        }
    });

    e.preventDefault();
}