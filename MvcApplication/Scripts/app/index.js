function showLoading() {
    $("#loading").show();
}
function getMobileBill() {
    var phone = $("#txtPhone").val();
    window.location.href = "Data/Load?Phone=" + phone;
}
function submitPhoneNum() {
    showLoading();
    $("#form1").submit();
}
function submitPassword() {
    showLoading();
    $("#form2").submit();
}
function submitAutnCode() {
    showLoading();
    $("#form3").submit();
}
function submitSMSAuthCode() {
    showLoading();
    $("#form4").submit();
}
jQuery(function ($) {
    $(document).ready(function () {
        $('.navbar-wrapper').stickUp({
            parts:
                {
                    0: 'personInfo',
                    1: 'billData',
                    2: 'messageData',
                    3: 'telData',
                    4: 'flowBill',
                    5: 'flowDetail'
                },
            itemClass: 'menuItem',
            itemHover: 'active',
            marginTop: 'auto'
        });
    });
});