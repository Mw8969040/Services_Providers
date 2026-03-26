/* =========================================
   site.js — Smart Platform JavaScript
   AJAX helpers, Toast, Modal functions
   ========================================= */

// ===== Toast Notification =====
// بتعرض رسالة مؤقتة (نجاح/خطأ) في أعلى الصفحة
function showToast(message, type) {
    // type: 'success', 'error', 'info'
    var container = document.getElementById('toastContainer');
    var toast = document.createElement('div');
    toast.className = 'sp-toast sp-toast-' + type;
    toast.innerHTML = '<i class="fas ' +
        (type === 'success' ? 'fa-check-circle' : type === 'error' ? 'fa-times-circle' : 'fa-info-circle') +
        '"></i> ' + message;

    container.appendChild(toast);

    // حذف التوست بعد 3 ثواني
    setTimeout(function () {
        toast.remove();
    }, 3000);
}


// ===== AJAX POST Request =====
// بتبعت request من نوع POST وبترجع النتيجة
function ajaxPost(url, data, onSuccess, onError) {
    $.ajax({
        url: url,
        type: 'POST',
        data: data,
        headers: {
            'RequestVerificationToken': $('input[name="__RequestVerificationToken"]').val()
        },
        success: function (response) {
            if (onSuccess) onSuccess(response);
        },
        error: function (xhr) {
            var errorMsg = 'Something went wrong!';
            if (xhr.responseJSON && xhr.responseJSON.message) {
                errorMsg = xhr.responseJSON.message;
            }
            showToast(errorMsg, 'error');
            if (onError) onError(xhr);
        }
    });
}


// ===== Load Partial View in Modal =====
// بتحمل Partial View من السيرفر وتعرضه في الـ Modal
function openModal(url, title) {
    $('#spModalTitle').text(title);
    $('#spModalBody').html('<div class="text-center p-4"><div class="sp-spinner" style="border-color: var(--primary); border-top-color: var(--primary-dark); width:40px; height:40px;"></div></div>');

    var modal = new bootstrap.Modal(document.getElementById('spModal'));
    modal.show();

    // جلب المحتوى بـ AJAX
    $.get(url, function (html) {
        $('#spModalBody').html(html);
    }).fail(function () {
        $('#spModalBody').html('<div class="text-center text-danger"><i class="fas fa-exclamation-triangle fa-3x mb-3"></i><p>Failed to load content</p></div>');
    });
}


// ===== Submit Form inside Modal via AJAX =====
// بتبعت الـ Form اللي جوه الـ Modal بـ AJAX
function submitModalForm(formSelector, onSuccess) {
    var form = $(formSelector);
    var formData = new FormData(form[0]);

    $.ajax({
        url: form.attr('action'),
        type: 'POST',
        data: formData,
        processData: false,  // عشان الـ FormData (ملفات)
        contentType: false,  // عشان الـ FormData (ملفات)
        success: function (response) {
            // اقفل الـ Modal
            var modal = bootstrap.Modal.getInstance(document.getElementById('spModal'));
            if (modal) modal.hide();

            showToast(response.message, 'success');

            if (onSuccess) onSuccess(response);
        },
        error: function (xhr) {
            // لو فيه validation errors -> اعرضهم في الـ Modal
            if (xhr.status === 400 && xhr.responseText) {
                $('#spModalBody').html(xhr.responseText);
            } else {
                showToast('Something went wrong!', 'error');
            }
        }
    });
}


// ===== Confirm Delete =====
// بتعرض Modal تأكيد الحذف
var deleteUrl = '';
var deleteData = {};
var deleteCallback = null;

function confirmDelete(url, data, callback) {
    deleteUrl = url;
    deleteData = data;
    deleteCallback = callback;

    var modal = new bootstrap.Modal(document.getElementById('deleteModal'));
    modal.show();
}

// لما يضغط "Delete" في الـ Modal
$(document).on('click', '#confirmDeleteBtn', function () {
    var btn = $(this);
    btn.prop('disabled', true).html('<span class="sp-spinner"></span>');

    ajaxPost(deleteUrl, deleteData, function (response) {
        var modal = bootstrap.Modal.getInstance(document.getElementById('deleteModal'));
        modal.hide();

        showToast('Deleted successfully!', 'success');

        if (deleteCallback) deleteCallback(response);
    }, function () {
        btn.prop('disabled', false).html('<i class="fas fa-trash"></i> Delete');
    });
});


// ===== Image Preview =====
// لما يختار صورة -> يعرض preview
function setupImagePreview(inputSelector, previewSelector) {
    $(document).on('change', inputSelector, function () {
        var file = this.files[0];
        if (file) {
            var reader = new FileReader();
            reader.onload = function (e) {
                $(previewSelector).attr('src', e.target.result).show();
            };
            reader.readAsDataURL(file);
        }
    });
}

// تفعيل preview للصور
setupImagePreview('#ImageFile', '#imagePreview');
