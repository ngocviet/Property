var amount_img = 0;
function chooseFile() {
    document.getElementById('avatar-input').click();
}
document.getElementById('avatar-input').addEventListener('change', function () {
    amount_img++;
    var fileInput = document.getElementById('avatar-input');
    var fileNameInput = document.getElementById('avatar-file-name');
    fileNameInput.value = fileInput.files[0].name;

    var selectedImageFile = fileInput.files[0];
    if (selectedImageFile) {
        fileNameInput.value = selectedImageFile.name;
        var imagePath = "/images/" + selectedImageFile.name;
        document.getElementById('url_imgs').value = document.getElementById('url_imgs').value + ".." + selectedImageFile.name;
        document.querySelector('.main_img').src = imagePath;
        document.querySelector('.main_img').style.display = "block";
        document.querySelector('.xem_Truoc_img').style.display = "block";
        document.querySelector('.amount_img_createlt').innerHTML = "( số lượng ảnh: " + amount_img + " )";
    }
});
var selectRealty = document.getElementById('selectRealty');
var selectAccount = document.getElementById('selectAccount');
var selectDirection = document.getElementById('selectDirection');
selectRealty.addEventListener('change', function () {
    if (selectRealty.value !== '') {
        selectRealty.classList.add('selected_lt');
    } else {
        selectRealty.classList.remove('selected_lt');
    }
});
selectAccount.addEventListener('change', function () {
    if (selectAccount.value !== '') {
        selectAccount.classList.add('selected_lt');
    } else {
        selectAccount.classList.remove('selected_lt');
    }
});
function handleBtnClick(value) {
    const btnNoiThatList = document.querySelectorAll(".btn_noithat");
    btnNoiThatList.forEach((btn) => {
        btn.classList.remove("btn_noithat_selected");
    });
    const clickedBtn = event.target;
    clickedBtn.classList.add("btn_noithat_selected");
    const noiThatInput = document.getElementById("noiThat");
    noiThatInput.value = value;
}