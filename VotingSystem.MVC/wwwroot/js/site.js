// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

document.addEventListener("DOMContentLoaded", () => {
    document.querySelectorAll("[data-request-data-modal]").forEach((element) => {
        const modal = bootstrap.Modal.getOrCreateInstance(element, {
            backdrop: "static",
            keyboard: false
        });

        modal.show();
    });
});
