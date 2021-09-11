function SessionVerification() {
    var username = $('#hdusername').val();

    if (username === "") {
        alert("Vui lòng đăng nhập để sử dụng hệ thống");
        return false;
    }
    else {
        //do nothing
        return true;
    }
}