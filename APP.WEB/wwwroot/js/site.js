(() => {
    const contentSelector = "#app-content";
    const ajaxHeader = { "X-Requested-With": "XMLHttpRequest" };

    async function fetchHtml(url) {
        const response = await fetch(url, {
            headers: ajaxHeader,
            credentials: "same-origin"
        });

        if (response.redirected) {
            window.location.href = response.url;
            return null;
        }

        if (!response.ok) {
            throw new Error(`Request failed with status ${response.status}.`);
        }

        return await response.text();
    }


    async function loadPage(url, pushState = true) {
        const html = await fetchHtml(url);
        if (html === null) {
            return;
        }

        const parser = new DOMParser();
        const documentResult = parser.parseFromString(html, "text/html");
        const nextContent = documentResult.querySelector(contentSelector);
        const currentContent = document.querySelector(contentSelector);

        currentContent.innerHTML = nextContent ? nextContent.innerHTML : html;

        const title = documentResult.querySelector("title")?.textContent;
        if (title) {
            document.title = title;
        }

        if (pushState) {
            history.pushState({}, "", url);
        }

        setActiveLink(new URL(url, window.location.origin).href);
    }

    async function openModal(url, title) {
        const modalElement = document.querySelector("#app-modal");
        const modalTitle = modalElement.querySelector(".modal-title");
        const modalBody = modalElement.querySelector(".modal-body");

        modalTitle.textContent = title || "Details";
        modalBody.innerHTML = "<div class=\"text-muted py-3\">Loading...</div>";

        const modal = bootstrap.Modal.getOrCreateInstance(modalElement);
        modal.show();

        try {
            modalBody.innerHTML = await fetchHtml(url);
        } catch (error) {
            modalBody.innerHTML = `<div class="alert alert-danger">${error.message}</div>`;
        }
    }

    document.addEventListener("click", event => {
        const modalTrigger = event.target.closest("[data-modal-url]");
        if (modalTrigger) {
            event.preventDefault();
            openModal(modalTrigger.dataset.modalUrl, modalTrigger.dataset.modalTitle);
            return;
        }

        const spaLink = event.target.closest("a[data-spa-link]");
        if (!spaLink || event.ctrlKey || event.metaKey || event.shiftKey || event.altKey) {
            return;
        }

        if (spaLink.origin !== window.location.origin) {
            return;
        }

        event.preventDefault();
        loadPage(spaLink.href).catch(error => {
            document.querySelector(contentSelector).innerHTML = `<div class="alert alert-danger">${error.message}</div>`;
        });
    });

    window.addEventListener("popstate", () => {
        loadPage(window.location.href, false);
    });

/*    setActiveLink(window.location.href);*/

    window.AppModal = { open: openModal };
})();
