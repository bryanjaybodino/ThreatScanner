// ── DOM refs ──────────────────────────────────────────────────
const btnFill = document.getElementById("btn-fill");
const btnClear = document.getElementById("btn-clear");
const dot = document.getElementById("dot");
const statusText = document.getElementById("status-text");
const logEl = document.getElementById("log");
const logCount = document.getElementById("log-count");
const progressW = document.getElementById("progress-wrap");
const progressB = document.getElementById("progress-bar");

// ── State ─────────────────────────────────────────────────────
let busy = false;

// ── Helpers ───────────────────────────────────────────────────
function setStatus(text, state = "ready") {
    statusText.textContent = text;
    dot.className = "dot " + state;         // ready | busy | error
}

function animateProgress() {
    progressW.classList.add("active");
    let pct = 0;
    const id = setInterval(() => {
        pct = Math.min(pct + rng(4, 9), 90);
        progressB.style.width = pct + "%";
        if (pct >= 90) clearInterval(id);
    }, 200);
    return id;
}

function finishProgress() {
    progressB.style.width = "100%";
    setTimeout(() => {
        progressW.classList.remove("active");
        progressB.style.width = "0%";
    }, 500);
}

function rng(a, b) { return Math.floor(Math.random() * (b - a + 1)) + a; }

function renderLog(entries) {
    if (!entries || entries.length === 0) {
        logEl.innerHTML = `<div class="empty-log">No fillable fields found on this page.</div>`;
        logCount.textContent = "0 fields";
        return;
    }

    const ok = entries.filter(e => e.status === "ok").length;
    const err = entries.filter(e => e.status === "error").length;
    logCount.textContent = `${ok} filled${err ? `, ${err} errors` : ""}`;

    logEl.innerHTML = entries.map(e => {
        if (e.status === "ok") {
            const icon = kindIcon(e.kind);
            const badge = e.kind.toUpperCase();
            const text = e.hint
                ? `<span class="log-text">${esc(e.hint)}</span> → <span class="log-val">${esc(e.value ?? "✓")}</span>`
                : `<span class="log-text">${esc(e.hint ?? "")}</span>`;
            return `<div class="log-row">
        <span class="log-icon">${icon}</span>
        <span class="log-kind">${badge}</span>
        <span>${text}</span>
      </div>`;
        } else {
            return `<div class="log-row">
        <span class="log-icon">✗</span>
        <span class="log-kind log-err">ERR</span>
        <span class="log-err">${esc(e.msg ?? "unknown error")}</span>
      </div>`;
        }
    }).join("");

    // Scroll to bottom
    logEl.scrollTop = logEl.scrollHeight;
}

function kindIcon(kind) {
    const map = {
        "input": "✎",
        "select": "▾",
        "select2": "▾",
        "select2-fallback": "▾",
        "checkbox": "☑",
        "radio": "◉",
    };
    return map[kind] ?? "·";
}

function esc(s) {
    return String(s ?? "")
        .replace(/&/g, "&amp;")
        .replace(/</g, "&lt;")
        .replace(/>/g, "&gt;");
}

// ── Fill ──────────────────────────────────────────────────────
btnFill.addEventListener("click", async () => {
    if (busy) return;
    busy = true;
    btnFill.disabled = true;
    btnClear.disabled = true;
    setStatus("Filling form fields…", "busy");
    logEl.innerHTML = `<div class="empty-log">Working…</div>`;
    logCount.textContent = "…";

    const progId = animateProgress();

    try {
        const [tab] = await chrome.tabs.query({ active: true, currentWindow: true });
        if (!tab?.id) throw new Error("No active tab found.");

        // Ensure content script is injected (handles pages where it wasn't auto-injected)
        await chrome.scripting.executeScript({
            target: { tabId: tab.id },
            files: ["content.js"]
        }).catch(() => { });   // ignore if already injected

        const response = await chrome.tabs.sendMessage(tab.id, { action: "fill" });

        clearInterval(progId);
        finishProgress();

        if (response?.ok) {
            const count = response.log?.filter(e => e.status === "ok").length ?? 0;
            setStatus(`Done — ${count} field${count !== 1 ? "s" : ""} filled`, "ready");
            renderLog(response.log);
        } else {
            setStatus("Fill returned no data", "error");
        }

    } catch (err) {
        clearInterval(progId);
        finishProgress();
        setStatus("Error: " + err.message, "error");
        logEl.innerHTML = `<div class="empty-log log-err">⚠ ${esc(err.message)}</div>`;
    } finally {
        busy = false;
        btnFill.disabled = false;
        btnClear.disabled = false;
    }
});

// ── Clear ─────────────────────────────────────────────────────
btnClear.addEventListener("click", async () => {
    if (busy) return;
    try {
        const [tab] = await chrome.tabs.query({ active: true, currentWindow: true });
        if (!tab?.id) return;

        await chrome.scripting.executeScript({
            target: { tabId: tab.id },
            files: ["content.js"]
        }).catch(() => { });

        await chrome.tabs.sendMessage(tab.id, { action: "clear" });
        setStatus("Fields cleared", "ready");
        logEl.innerHTML = `<div class="empty-log">Fields cleared.</div>`;
        logCount.textContent = "0 fields";
    } catch (err) {
        setStatus("Clear failed: " + err.message, "error");
    }
});