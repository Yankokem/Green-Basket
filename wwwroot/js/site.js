// Green Basket UI Helpers: Table Search/Filter, Tabbed Details & Smooth Modals

document.addEventListener("DOMContentLoaded", () => {
    initTableToolbars();
    initTabGroups();
    initModals();
});

function initTableToolbars() {
    document.querySelectorAll("[data-toolbar-target]").forEach((toolbar) => {
        const targetId = toolbar.getAttribute("data-toolbar-target");
        const container = document.getElementById(targetId);
        if (!container) return;

        const searchInput = toolbar.querySelector("[data-table-search]");
        const filterSelect = toolbar.querySelector("[data-table-filter]");
        const countBadge = toolbar.querySelector("[data-table-count]");
        const emptyBanner = container.querySelector("[data-filter-empty]");
        const items = Array.from(container.querySelectorAll("[data-filter-item]"));

        const applyFilter = () => {
            const query = (searchInput ? searchInput.value : "").trim().toLowerCase();
            const category = (filterSelect ? filterSelect.value : "").trim().toLowerCase();
            let visible = 0;

            items.forEach((el) => {
                const text = (el.getAttribute("data-search-text") || el.textContent || "").toLowerCase();
                const cat = (el.getAttribute("data-filter-value") || "").toLowerCase();
                const matchesSearch = !query || text.includes(query);
                const matchesCategory = !category || cat === category;

                if (matchesSearch && matchesCategory) {
                    el.classList.remove("hidden");
                    visible++;
                } else {
                    el.classList.add("hidden");
                }
            });

            if (countBadge) {
                countBadge.textContent = String(visible);
            }

            if (emptyBanner) {
                if (items.length > 0 && visible === 0) {
                    emptyBanner.classList.remove("hidden");
                } else {
                    emptyBanner.classList.add("hidden");
                }
            }
        };

        if (searchInput) searchInput.addEventListener("input", applyFilter);
        if (filterSelect) filterSelect.addEventListener("change", applyFilter);
    });
}

function initTabGroups() {
    document.querySelectorAll("[data-tab-group]").forEach((group) => {
        const buttons = Array.from(group.querySelectorAll("[data-tab-btn]"));
        const panels = Array.from(group.querySelectorAll("[data-tab-panel]"));
        if (!buttons.length) return;

        const activate = (tabName) => {
            buttons.forEach((btn) => {
                const isActive = btn.getAttribute("data-tab-btn") === tabName;
                btn.setAttribute("aria-selected", isActive ? "true" : "false");
                if (isActive) {
                    btn.classList.add("border-[#426246]", "text-[#426246]", "bg-[#f4f6f2]/80");
                    btn.classList.remove("border-transparent", "text-slate-500", "hover:text-slate-800");
                } else {
                    btn.classList.remove("border-[#426246]", "text-[#426246]", "bg-[#f4f6f2]/80");
                    btn.classList.add("border-transparent", "text-slate-500", "hover:text-slate-800");
                }
            });

            panels.forEach((panel) => {
                const match = panel.getAttribute("data-tab-panel") === tabName;
                panel.classList.toggle("hidden", !match);
            });
        };

        buttons.forEach((btn) => {
            btn.addEventListener("click", () => {
                const name = btn.getAttribute("data-tab-btn");
                activate(name);
                if (window.history && window.history.replaceState) {
                    window.history.replaceState(null, "", "#" + name);
                }
            });
        });

        const hash = window.location.hash ? window.location.hash.substring(1) : "";
        const initial = buttons.some((b) => b.getAttribute("data-tab-btn") === hash)
            ? hash
            : buttons[0].getAttribute("data-tab-btn");
        activate(initial);
    });
}

function initModals() {
    const openModal = (modalId, trigger) => {
        const modal = document.getElementById(modalId);
        if (!modal) return;

        // Pre-fill AssetId if provided on trigger
        const assetVal = trigger ? trigger.getAttribute("data-modal-asset") : null;
        if (assetVal) {
            const assetSelect = modal.querySelector("select[name='AssetId']");
            if (assetSelect) {
                assetSelect.value = assetVal;
                assetSelect.dispatchEvent(new Event("change"));
            }
        }

        // Pre-fill Title if provided on trigger
        const titleVal = trigger ? trigger.getAttribute("data-modal-title") : null;
        if (titleVal) {
            const titleInput = modal.querySelector("input[name='Title']");
            if (titleInput) {
                titleInput.value = titleVal;
            }
        }

        const backdrop = modal.querySelector("[data-modal-backdrop]");
        const panel = modal.querySelector("[data-modal-panel]");

        modal.classList.remove("hidden");
        modal.classList.add("flex");
        document.body.classList.add("overflow-hidden");

        // Force reflow so transition runs smoothly from initial state
        void modal.offsetWidth;

        if (backdrop) {
            backdrop.classList.remove("opacity-0");
            backdrop.classList.add("opacity-100");
        }
        if (panel) {
            panel.classList.remove("opacity-0", "scale-95", "translate-y-3");
            panel.classList.add("opacity-100", "scale-100", "translate-y-0");
        }

        setTimeout(() => {
            const firstInput = modal.querySelector("input:not([type='hidden']):not([disabled]), select, textarea");
            if (firstInput) firstInput.focus();
        }, 120);
    };

    const closeModal = (modal) => {
        if (!modal || modal.classList.contains("hidden")) return;

        const backdrop = modal.querySelector("[data-modal-backdrop]");
        const panel = modal.querySelector("[data-modal-panel]");

        if (backdrop) {
            backdrop.classList.remove("opacity-100");
            backdrop.classList.add("opacity-0");
        }
        if (panel) {
            panel.classList.remove("opacity-100", "scale-100", "translate-y-0");
            panel.classList.add("opacity-0", "scale-95", "translate-y-3");
        }

        setTimeout(() => {
            modal.classList.add("hidden");
            modal.classList.remove("flex");
            if (!document.querySelector("[data-modal-root]:not(.hidden)")) {
                document.body.classList.remove("overflow-hidden");
            }
        }, 200);
    };

    document.addEventListener("click", (e) => {
        const openBtn = e.target.closest("[data-modal-open]");
        if (openBtn) {
            e.preventDefault();
            openModal(openBtn.getAttribute("data-modal-open"), openBtn);
            return;
        }

        const closeBtn = e.target.closest("[data-modal-close]");
        if (closeBtn) {
            e.preventDefault();
            closeModal(closeBtn.closest("[data-modal-root]"));
            return;
        }

        const backdrop = e.target.closest("[data-modal-backdrop]");
        if (backdrop && e.target === backdrop) {
            closeModal(backdrop.closest("[data-modal-root]"));
        }
    });

    document.addEventListener("keydown", (e) => {
        if (e.key === "Escape") {
            const openModals = document.querySelectorAll("[data-modal-root]:not(.hidden)");
            if (openModals.length) {
                closeModal(openModals[openModals.length - 1]);
            }
        }
    });
}
