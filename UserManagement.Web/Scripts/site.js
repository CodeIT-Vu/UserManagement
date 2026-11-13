const $ = window.jQuery;
const defaultErrorMessage = "Có lỗi xảy ra, vui lòng thử lại.";

// Hiển thị lỗi chung, lỗi từng trường hoặc chuyển về trang đăng nhập khi hết phiên.
function showFormErrors(form, response) {
    const errorBox = form.find("[data-form-error]");
    let message = defaultErrorMessage;

    if (!response) {
        errorBox.text(message);
        errorBox.removeClass("d-none");
        return;
    }

    if (response.Code === "AUTH_REQUIRED") {
        const loginUrl = document.body.dataset.loginUrl;
        const returnUrl = window.location.pathname + window.location.search;
        window.location.assign(loginUrl + "?ReturnUrl=" + encodeURIComponent(returnUrl));
        return;
    }

    if (response.Message) {
        message = response.Message;
    }

    const validationErrors = {};
    let hasFieldErrors = false;

    if (response.Errors) {
        for (const fieldName in response.Errors) {
            const messages = response.Errors[fieldName];
            const fieldMessage = messages.join(" ");

            if (fieldName === "") {
                message += " " + fieldMessage;
            } else {
                validationErrors[fieldName] = fieldMessage;
                hasFieldErrors = true;
            }
        }
    }

    if (hasFieldErrors) {
        const validator = form.validate();
        validator.showErrors(validationErrors);
    }

    if (response.TraceId) {
        message += " Mã tra cứu: " + response.TraceId;
    }

    errorBox.text(message);
    errorBox.removeClass("d-none");
}

// Gửi form bằng AJAX, xử lý kết quả và khóa nút gửi trong lúc chờ.
function submitForm(form, data) {
    const submitButton = form.find(":submit");
    const errorBox = form.find("[data-form-error]");
    errorBox.empty();
    errorBox.addClass("d-none");
    submitButton.prop("disabled", true);

    $.ajax({
        url: form.attr("action"),
        type: "POST",
        dataType: "json",
        data: data,
        success: function (response) {
            if (response.Success && response.Data) {
                window.location.assign(response.Data);
                return;
            }
            showFormErrors(form, response);
        },
        error: function (xhr) {
            showFormErrors(form, xhr.responseJSON);
        },
        complete: function () {
            submitButton.prop("disabled", false);
        }
    });
}

// Dùng chung sự kiện submit; form đăng xuất không cần validation.
$("#loginForm, #logoutForm, #editUserForm, #registerForm, #changePasswordForm")
    .on("submit", function (event) {
        event.preventDefault();
        const form = $(this);

        if (!form.is("#logoutForm") && !form.valid()) {
            return;
        }

        submitForm(form, form.serialize());
    });
