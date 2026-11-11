const $ = window.jQuery;
const defaultErrorMessage = "Có lỗi xảy ra, vui lòng thử lại.";

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

    errorBox.text(message);
    errorBox.removeClass("d-none");
}

$("#loginForm").on("submit", function (event) {
    event.preventDefault();
    const form = $(this);

    if (!form.valid()) {
        return;
    }

    const username = form.find("[name='Username']").val();
    const password = form.find("[name='Password']").val();
    const returnUrl = form.find("[name='ReturnUrl']").val();
    const token = form.find("[name='__RequestVerificationToken']").val();
    const submitButton = form.find(":submit");

    const errorBox = form.find("[data-form-error]");
    errorBox.empty();
    errorBox.addClass("d-none");
    submitButton.prop("disabled", true);

    $.ajax({
        url: form.attr("action"),
        type: "POST",
        dataType: "json",
        data: {
            Username: username,
            Password: password,
            ReturnUrl: returnUrl,
            __RequestVerificationToken: token
        },
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
});

$("#logoutForm").on("submit", function (event) {
    event.preventDefault();
    const form = $(this);
    const token = form.find("[name='__RequestVerificationToken']").val();
    const submitButton = form.find(":submit");

    const errorBox = form.find("[data-form-error]");
    errorBox.empty();
    errorBox.addClass("d-none");
    submitButton.prop("disabled", true);

    $.ajax({
        url: form.attr("action"),
        type: "POST",
        dataType: "json",
        data: {
            __RequestVerificationToken: token
        },
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
});

$("#editUserForm").on("submit", function (event) {
    event.preventDefault();
    const form = $(this);

    if (!form.valid()) {
        return;
    }

    const userId = form.find("[name='UserId']").val();
    const fullName = form.find("[name='FullName']").val();
    const email = form.find("[name='Email']").val();
    const isActive = form.find("[name='IsActive'][type='checkbox']").prop("checked");
    const token = form.find("[name='__RequestVerificationToken']").val();
    const submitButton = form.find(":submit");

    const errorBox = form.find("[data-form-error]");
    errorBox.empty();
    errorBox.addClass("d-none");
    submitButton.prop("disabled", true);

    $.ajax({
        url: form.attr("action"),
        type: "POST",
        dataType: "json",
        data: {
            UserId: userId,
            FullName: fullName,
            Email: email,
            IsActive: isActive,
            __RequestVerificationToken: token
        },
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
});

$("#registerForm").on("submit", function (event) {
    event.preventDefault();
    const form = $(this);

    if (!form.valid()) {
        return;
    }

    const username = form.find("[name='Username']").val();
    const fullName = form.find("[name='FullName']").val();
    const email = form.find("[name='Email']").val();
    const password = form.find("[name='Password']").val();
    const confirmPassword = form.find("[name='ConfirmPassword']").val();
    const token = form.find("[name='__RequestVerificationToken']").val();
    const submitButton = form.find(":submit");
    const errorBox = form.find("[data-form-error]");
    errorBox.empty();
    errorBox.addClass("d-none");
    submitButton.prop("disabled", true);

    $.ajax({
        url: form.attr("action"),
        type: "POST",
        dataType: "json",
        data: {
            Username: username,
            FullName: fullName,
            Email: email,
            Password: password,
            ConfirmPassword: confirmPassword,
            __RequestVerificationToken: token
        },
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
});

$("#changePasswordForm").on("submit", function (event) {
    event.preventDefault();
    const form = $(this);

    if (!form.valid()) {
        return;
    }

    const currentPassword = form.find("[name='CurrentPassword']").val();
    const newPassword = form.find("[name='NewPassword']").val();
    const confirmPassword = form.find("[name='ConfirmPassword']").val();
    const token = form.find("[name='__RequestVerificationToken']").val();
    const submitButton = form.find(":submit");
    const errorBox = form.find("[data-form-error]");
    errorBox.empty();
    errorBox.addClass("d-none");
    submitButton.prop("disabled", true);

    $.ajax({
        url: form.attr("action"),
        type: "POST",
        dataType: "json",
        data: {
            CurrentPassword: currentPassword,
            NewPassword: newPassword,
            ConfirmPassword: confirmPassword,
            __RequestVerificationToken: token
        },
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
});
