window.NotificationService = (() => {
    const displayed = new Set();
    const titles = { success: "Success", error: "Something went wrong", warning: "Please check", info: "Information" };

    const show = (options) => {
        if (typeof Swal === "undefined") {
            console.error("SweetAlert2 is unavailable.", options);
            return Promise.resolve({ isConfirmed: false });
        }

        const notification = {
            icon: options.icon || "info",
            title: options.title || titles[options.icon] || titles.info,
            text: options.message || "",
            confirmButtonColor: "#0d6efd",
            ...options
        };
        delete notification.message;

        const key = `${notification.icon}|${notification.title}|${notification.text}`;
        if (options.dedupe !== false && displayed.has(key)) return Promise.resolve({ isDismissed: true });
        if (options.dedupe !== false) displayed.add(key);
        delete notification.dedupe;

        if (notification.toast) {
            Object.assign(notification, { position: "top-end", showConfirmButton: false, timer: notification.timer || 3500, timerProgressBar: true });
        }
        return Swal.fire(notification);
    };

    const requestConfirmation = (message, options = {}) => show({
        icon: "warning",
        title: options.title || "Are you sure?",
        message,
        showCancelButton: true,
        confirmButtonText: options.confirmButtonText || "Yes, continue",
        cancelButtonText: options.cancelButtonText || "Cancel",
        confirmButtonColor: options.confirmButtonColor || "#dc3545",
        focusCancel: true,
        allowOutsideClick: false,
        dedupe: false
    });

    const fromApiError = (apiError) => {
        if (!apiError) return show({ icon: "error", message: "The request could not be completed. Please try again." });
        if (apiError.name === "TypeError") return show({ icon: "error", title: "Network error", message: "The server could not be reached. Check your connection and try again." });
        const validation = apiError.errors ? Object.values(apiError.errors).flat().join("\n") : null;
        return show({ icon: "error", title: validation ? "Please check your input" : titles.error, message: validation || apiError.message || apiError.title || "The server could not complete the request." });
    };

    return {
        show,
        success: (message, title = titles.success) => show({ icon: "success", title, message, toast: true }),
        error: (message, title = titles.error) => show({ icon: "error", title, message }),
        warning: (message, title = titles.warning) => show({ icon: "warning", title, message }),
        info: (message, title = titles.info) => show({ icon: "info", title, message }),
        requestConfirmation,
        fromApiError
    };
})();

document.addEventListener("DOMContentLoaded", () => {
    const notifications = [];
    const notificationData = document.getElementById("page-notifications");
    if (notificationData) {
        try {
            notifications.push(...JSON.parse(notificationData.textContent));
        } catch (error) {
            console.error("Unable to read page notifications.", error);
        }
    }

    document.querySelectorAll("[data-swal-notification]").forEach((element) => {
        notifications.push({ icon: element.dataset.swalIcon || "info", title: element.dataset.swalTitle, message: element.dataset.swalMessage, toast: element.dataset.swalToast === "true" });
    });
    notifications.reduce(
        (previous, notification) => previous.then(() => NotificationService.show(notification)),
        Promise.resolve());

    document.addEventListener("submit", async (event) => {
        const form = event.target;
        if (!(form instanceof HTMLFormElement) || !form.matches("[data-confirm-message]") || form.dataset.confirmed === "true") return;

        event.preventDefault();
        const result = await NotificationService.requestConfirmation(form.dataset.confirmMessage, { title: form.dataset.confirmTitle, confirmButtonText: form.dataset.confirmButtonText });
        if (result.isConfirmed) {
            form.dataset.confirmed = "true";
            form.requestSubmit(event.submitter || undefined);
        }
    });

    document.addEventListener("invalid", (event) => {
        const form = event.target.form;
        if (!form || form.dataset.validationNotificationShown === "true") return;
        form.dataset.validationNotificationShown = "true";
        NotificationService.warning("Complete all required fields and correct any invalid values before continuing.", "Please check your input");
        window.setTimeout(() => delete form.dataset.validationNotificationShown, 500);
    }, true);
});
