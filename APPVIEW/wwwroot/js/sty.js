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
    //$.ajax({
    //    url: `/Home/SaveVoucherForUser?voucherId=${voucherId}`,
    //    type: 'POST',
    //    contentType: 'application/json',
    //    success: function (data) {
    //        console.log(data);
    //        if (data.alreadyInAccount) {
    //            alert('Phiếu giảm giá đã có trong tài khoản của bạn!');
    //        } else {
    //            alert('Lưu thành công!');
    //        }
    //    },
    //    error: function (xhr, status, error) {
    //        if (xhr.status === 401) {
    //            console.error('Lỗi khi áp dụng: Người dùng chưa đăng nhập');
    //        } else {
    //            console.error('Lỗi khi áp dụng:', error);
    //        }
    //    }
    //});
    $.ajax({
        url: "/Home/SaveVoucherForUser",
        type: 'POST',
        data: { voucherId: voucherId },
        contentType: 'application/json',
        success: function (rs) {
            if (rs.success) {
                // Hiển thị thông báo thành công
                Swal.fire({
                    icon: 'success',
                    title: 'Lưu thành công!',
                    showConfirmButton: false,
                    position: 'bottom-right',
                    timer: 2000,
                    toast: true
                });

                // Cập nhật số lượng sản phẩm trong giỏ hàng
                $('.nav-shop__circle').html(rs.count);
            } else {
                // Hiển thị thông báo lỗi nếu cần
                Swal.fire({
                    icon: 'error',
                    title: 'Lỗi',
                    text: 'Có lỗi xảy ra khi lưu phiếu giảm giá hoặc bạn chưa đăng nhập.',
                    showConfirmButton: false,
                    position: 'bottom-right',
                    timer: 2000,
                    toast: true
                });
            }
        }
    });
    //$.ajax({
    //    url: `/Home/SaveVoucherForUser?voucherId=${voucherId}`,
    //    type: 'POST',
    //    contentType: 'application/json',
    //    success: function (rs) {
    //        if (rs.success) {
    //            // Hiển thị thông báo thành công
    //            Swal.fire({
    //                icon: 'success',
    //                title: 'Lưu thành công!',
    //                showConfirmButton: false,
    //                position: 'bottom-right',
    //                timer: 2000,
    //                toast: true
    //            });

    //            // Cập nhật số lượng sản phẩm trong giỏ hàng
    //            $('.nav-shop__circle').html(rs.count);
    //        } else {
    //            // Hiển thị thông báo lỗi nếu cần
    //            Swal.fire({
    //                icon: 'error',
    //                title: 'Lỗi',
    //                text: 'Có lỗi xảy ra khi lưu phiếu giảm giá hoặc bạn chưa đăng nhập.',
    //                showConfirmButton: false,
    //                position: 'bottom-right',
    //                timer: 2000,
    //                toast: true
    //            });
    //        }
    //    }
    //});
};