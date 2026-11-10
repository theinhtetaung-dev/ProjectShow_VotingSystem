// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

document.addEventListener("DOMContentLoaded", () => {
    const controls = document.querySelectorAll("[data-catalog-view]");
    if (controls.length) {
        const selectView = (view) => {
            controls.forEach((button) => {
                const active = button.dataset.catalogView === view;
                button.setAttribute("aria-pressed", String(active));
                document.getElementById(button.getAttribute("aria-controls")).hidden = !active;
            });
        };
        controls.forEach((button) => button.addEventListener("click", () => {
            selectView(button.dataset.catalogView);
            try { localStorage.setItem("pjvs-catalog-view", button.dataset.catalogView); } catch { /* Storage may be unavailable. */ }
        }));
        try {
            if (localStorage.getItem("pjvs-catalog-view") === "table") selectView("table");
        } catch { /* Default to cards. */ }
    }
    document.querySelectorAll("[data-request-data-modal]").forEach((element) => {
        const modal = bootstrap.Modal.getOrCreateInstance(element, {
            backdrop: "static",
            keyboard: false
        });

        modal.show();
    });
});
