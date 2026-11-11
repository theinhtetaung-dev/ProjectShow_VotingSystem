// NEXUS UI Interactions & Helpers

document.addEventListener("DOMContentLoaded", () => {
  // 1. Sidebar Rail Toggle
  const railToggle = document.getElementById("railToggle");
  const rail = document.getElementById("rail");
  const shell = document.getElementById("shell");

  if (railToggle && rail) {
    railToggle.addEventListener("click", () => {
      if (window.innerWidth <= 560) {
        rail.classList.toggle("mobile-open");
      } else {
        rail.classList.toggle("is-mini");
        if (shell) shell.classList.toggle("is-wide");
      }
    });

    // Close mobile rail when clicking outside
    document.addEventListener("click", (e) => {
      if (window.innerWidth <= 560 && rail.classList.contains("mobile-open")) {
        if (!rail.contains(e.target) && !railToggle.contains(e.target)) {
          rail.classList.remove("mobile-open");
        }
      }
    });
  }

  // 2. Global Tooltip System
  const tooltip = document.getElementById("tooltip");
  window.showTip = (e, title, text) => {
    if (!tooltip) return;
    tooltip.innerHTML = "<b>" + title + "</b><span>" + text + "</span>";
    tooltip.style.left = Math.min(e.clientX + 12, window.innerWidth - 230) + "px";
    tooltip.style.top = Math.max(10, e.clientY - 20) + "px";
    tooltip.classList.add("show");
  };

  window.hideTip = () => {
    if (tooltip) tooltip.classList.remove("show");
  };

  // 3. Segmented Views (Cards vs Table)
  const cardsBtn = document.getElementById("cardsBtn");
  const tableBtn = document.getElementById("tableBtn");
  const projectCards = document.getElementById("projectCards");
  const projectTable = document.getElementById("projectTable");
  const cardPager = document.getElementById("cardPager");

  if (cardsBtn && tableBtn && projectCards && projectTable) {
    cardsBtn.addEventListener("click", () => {
      projectCards.style.display = "grid";
      projectTable.style.display = "none";
      if (cardPager) cardPager.style.display = "flex";
      cardsBtn.classList.add("active");
      tableBtn.classList.remove("active");
      try { localStorage.setItem("nexus_projects_view", "cards"); } catch(e){}
    });

    tableBtn.addEventListener("click", () => {
      projectCards.style.display = "none";
      projectTable.style.display = "block";
      if (cardPager) cardPager.style.display = "none";
      tableBtn.classList.add("active");
      cardsBtn.classList.remove("active");
      try { localStorage.setItem("nexus_projects_view", "table"); } catch(e){}
    });

    // Check stored preference
    try {
      if (localStorage.getItem("nexus_projects_view") === "table") {
        tableBtn.click();
      }
    } catch(e){}
  }

  // 4. Segmented Views for Events (Cards vs Table)
  const eventsCardsBtn = document.getElementById("eventsCardsBtn");
  const eventsTableBtn = document.getElementById("eventsTableBtn");
  const eventsCards = document.getElementById("eventsCards");
  const eventsTable = document.getElementById("eventsTable");

  if (eventsCardsBtn && eventsTableBtn && eventsCards && eventsTable) {
    eventsCardsBtn.addEventListener("click", () => {
      eventsCards.style.display = "grid";
      eventsTable.style.display = "none";
      eventsCardsBtn.classList.add("active");
      eventsTableBtn.classList.remove("active");
      try { localStorage.setItem("nexus_events_view", "cards"); } catch(e){}
    });

    eventsTableBtn.addEventListener("click", () => {
      eventsCards.style.display = "none";
      eventsTable.style.display = "block";
      eventsTableBtn.classList.add("active");
      eventsCardsBtn.classList.remove("active");
      try { localStorage.setItem("nexus_events_view", "table"); } catch(e){}
    });

    // Check stored preference
    try {
      if (localStorage.getItem("nexus_events_view") === "cards") {
        eventsCardsBtn.click();
      }
    } catch(e){}
  }

  // 5. Segmented Views for Users (Cards vs Table)
  const usersCardsBtn = document.getElementById("usersCardsBtn");
  const usersTableBtn = document.getElementById("usersTableBtn");
  const usersCards = document.getElementById("usersCards");
  const usersTable = document.getElementById("usersTable");

  if (usersCardsBtn && usersTableBtn && usersCards && usersTable) {
    usersCardsBtn.addEventListener("click", () => {
      usersCards.style.display = "grid";
      usersTable.style.display = "none";
      usersCardsBtn.classList.add("active");
      usersTableBtn.classList.remove("active");
      try { localStorage.setItem("nexus_users_view", "cards"); } catch(e){}
    });

    usersTableBtn.addEventListener("click", () => {
      usersCards.style.display = "none";
      usersTable.style.display = "block";
      usersTableBtn.classList.add("active");
      usersCardsBtn.classList.remove("active");
      try { localStorage.setItem("nexus_users_view", "table"); } catch(e){}
    });

    try {
      if (localStorage.getItem("nexus_users_view") === "cards") {
        usersCardsBtn.click();
      }
    } catch(e){}
  }

  // 6. Segmented Views for Email Accounts (Cards vs Table)
  const emailAccountsCardsBtn = document.getElementById("emailAccountsCardsBtn");
  const emailAccountsTableBtn = document.getElementById("emailAccountsTableBtn");
  const emailAccountsCards = document.getElementById("emailAccountsCards");
  const emailAccountsTable = document.getElementById("emailAccountsTable");

  if (emailAccountsCardsBtn && emailAccountsTableBtn && emailAccountsCards && emailAccountsTable) {
    emailAccountsCardsBtn.addEventListener("click", () => {
      emailAccountsCards.style.display = "grid";
      emailAccountsTable.style.display = "none";
      emailAccountsCardsBtn.classList.add("active");
      emailAccountsTableBtn.classList.remove("active");
      try { localStorage.setItem("nexus_email_accounts_view", "cards"); } catch(e){}
    });

    emailAccountsTableBtn.addEventListener("click", () => {
      emailAccountsCards.style.display = "none";
      emailAccountsTable.style.display = "block";
      emailAccountsTableBtn.classList.add("active");
      emailAccountsCardsBtn.classList.remove("active");
      try { localStorage.setItem("nexus_email_accounts_view", "table"); } catch(e){}
    });

    try {
      if (localStorage.getItem("nexus_email_accounts_view") === "cards") {
        emailAccountsCardsBtn.click();
      }
    } catch(e){}
  }

  // 7. Password / Secret Visibility Toggle
  document.querySelectorAll(".toggle-secret-btn").forEach((btn) => {
    btn.addEventListener("click", () => {
      const targetId = btn.getAttribute("data-target");
      const input = targetId ? document.getElementById(targetId) : null;
      if (!input) return;
      const isPassword = input.type === "password";
      input.type = isPassword ? "text" : "password";
      const icon = btn.querySelector("i");
      if (icon) {
        icon.classList.toggle("fa-eye", !isPassword);
        icon.classList.toggle("fa-eye-slash", isPassword);
      }
    });
  });

  // 8. Copy Pill Helper
  document.querySelectorAll(".copy-pill").forEach((pill) => {
    pill.addEventListener("click", async () => {
      const text = pill.dataset.copy || pill.innerText.trim();
      try {
        await navigator.clipboard.writeText(text);
        const originalHtml = pill.innerHTML;
        pill.innerHTML = '<i class="fa-solid fa-check text-success"></i> Copied!';
        setTimeout(() => {
          pill.innerHTML = originalHtml;
        }, 1800);
      } catch(err) {
        console.error("Copy failed", err);
      }
    });
  });
});
