document.addEventListener('DOMContentLoaded', function () {
    var sortOrderSelect = document.getElementById('sortOrder');
    var sortForm = document.getElementById('sortForm');

    sortOrderSelect.addEventListener('change', function () {
        // Gửi yêu cầu sắp xếp khi giá trị của select thay đổi
        sortForm.submit();
    });
});