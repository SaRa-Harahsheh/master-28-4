// وظيفة عرض معاينة الصورة عند التحميل
function previewImage(event) {
    const reader = new FileReader();
    reader.onload = function () {
        document.getElementById('profilePicPreview').src = reader.result;
    }
    reader.readAsDataURL(event.target.files[0]);
}