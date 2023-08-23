function showUserList() {
    var userList = document.getElementById('userList');
    if (userList.style.display === 'none') {
        userList.style.display = 'block';
    } else {
        userList.style.display = 'none';
    }
}
function toggleActive(element) {
    // Xóa class "active" từ tất cả các li
    var navItems = document.querySelectorAll('.nav li');
    navItems.forEach(function (item) {
        item.classList.remove('active');
    });

    // Thêm class "active" vào li vừa được chọn
    element.classList.add('active');
}
$(document).ready(function () {
    $('.contact-now-btn').click(function (e) {
        e.preventDefault();
        $(this).hide();
        $(this).siblings('.contact-details').show();
        $(this).closest('.listing-item').siblings('.additional-details').show();
    });
});
function toggleListTypeRealty() {
    var listTypeRealty = document.getElementById("listTypeRealty");
    if (listTypeRealty.style.display === "none") {
        listTypeRealty.style.display = "block"; 
    } else {
        listTypeRealty.style.display = "none"; 
    }
}
function changeTitleContent(content, liElement, id) {
    var titleRealtyContent = document.getElementById("titleRealty").getElementsByClassName("title_realty_content")[0];
    titleRealtyContent.textContent = content;

    var liElements = document.getElementById("listTypeRealty").getElementsByTagName("li");
    for (var i = 0; i < liElements.length; i++) {
        liElements[i].classList.remove("hoverr");
    }

    liElement.classList.add("hoverr");
    var idcategorydetail = document.getElementById("idcategorydetail");
    idcategorydetail.value = id;
    toggleListTypeRealty();
}