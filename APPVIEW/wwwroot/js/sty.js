$(document).ready(function () {
    var isDragging = false;
    var startPositionX;
    var moveDistance = 0;

    $(".voucher-list").on("mousedown", function (e) {
        isDragging = true;
        startPositionX = e.clientX;
    });

    $(document).on("mouseup", function () {
        isDragging = false;
    });

    $(document).on("mousemove", function (e) {
        if (isDragging) {
            var currentPositionX = e.clientX;
            moveDistance = startPositionX - currentPositionX;
            $(".voucher-list").scrollLeft($(".voucher-list").scrollLeft() + moveDistance);
            startPositionX = currentPositionX;
        }
    });

    $(".voucher-list").on("mouseleave", function () {
        isDragging = false;
    });

});
var applyButtons = document.querySelectorAll('.addVoucherButton');

applyButtons.forEach(function (button) {
    button.addEventListener('click', function (event) {
        event.preventDefault(); // Ngăn chặn sự kiện mặc định của nút submit

        var voucherId = $(this).data("voucher-id");

        saveVoucherToAccount(voucherId);
    });
});
// Hàm thực hiện lưu voucher vào tài khoản
function saveVoucherToAccount(voucherId) {
    $.ajax({
        url: `/Home/SaveVoucherForUser?voucherId=${voucherId}`,
        type: 'POST',
        contentType: 'application/json',
        success: function (rs) {
            console.log(rs);
            if (rs.success) {
                // Hiển thị thông báo thành công
                Swal.fire({
                    icon: 'success',
                    title: 'Thành công!',
                    text: 'Lưu thành công vào tài khoản',
                    showConfirmButton: false,
                    position: 'bottom-right',
                    timer: 2000,
                    toast: true
                });
                
            } else {
                if (rs.message && rs.message === "Người dùng chưa đăng nhập") {
                    Swal.fire({
                        icon: 'error',
                        title: 'Lỗi đăng nhập',
                        text: 'Bạn chưa đăng nhập!',
                        showConfirmButton: false,
                        position: 'bottom-right',
                        timer: 2000,
                        toast: true
                    });
                } else {
                    Swal.fire({
                        icon: 'error',
                        title: 'Lỗi',
                        text: 'Phiếu giảm giá đã có trong tài khoản của bạn',
                        showConfirmButton: false,
                        position: 'bottom-right',
                        timer: 2000,
                        toast: true
                    });
                }
            }
        }
    });
};