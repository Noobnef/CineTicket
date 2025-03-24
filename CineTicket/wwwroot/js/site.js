document.addEventListener('DOMContentLoaded', function () {
    const toggleBtn = document.querySelector('.mode-toggle');
    const darkModeSheet = document.getElementById("darkModeStylesheet");
    const icon = toggleBtn?.querySelector('i');

    function enableDarkMode() {
        darkModeSheet.disabled = false;
        toggleBtn.classList.remove('btn-dark');
        toggleBtn.classList.add('btn-light');
        icon?.classList.replace('fa-moon', 'fa-sun');
        localStorage.setItem("darkMode", "enabled");
    }

    function disableDarkMode() {
        darkModeSheet.disabled = true;
        toggleBtn.classList.remove('btn-light');
        toggleBtn.classList.add('btn-dark');
        icon?.classList.replace('fa-sun', 'fa-moon');
        localStorage.setItem("darkMode", "disabled");
    }

    toggleBtn?.addEventListener("click", () => {
        const isDark = !darkModeSheet.disabled;
        isDark ? disableDarkMode() : enableDarkMode();
    });

    // Load trạng thái đã lưu
    if (localStorage.getItem("darkMode") === "enabled") {
        enableDarkMode();
    } else {
        disableDarkMode();
    }
});
