// ============================================================
// BotAutoFill — Content Script
// Mirrors the C# Playwright BotAutoFill logic exactly.
// ============================================================

const DELAY_MIN = 60;
const DELAY_MAX = 180;

const FIRST_NAMES = ["James", "Maria", "Carlos", "Angela", "Ricardo", "Sofia", "Miguel", "Anna"];
const LAST_NAMES = ["Santos", "Reyes", "Cruz", "Garcia", "Lopez", "Torres", "Flores", "Ramos"];
const MIDDLE_NAMES = ["Jose", "Marie", "Grace", "Paul", "Rose", "John", "Mae", "Luis"];
const DOMAINS = ["gmail.com", "yahoo.com", "outlook.com", "testmail.com"];
const POSITIONS = ["Manager", "Analyst", "Developer", "Coordinator", "Supervisor", "Specialist"];
const CITIES = ["Manila", "Cebu", "Davao", "Quezon City", "Makati"];
const STREETS = ["Rizal Ave", "Mabini St", "Bonifacio Blvd", "Luna St", "Del Pilar"];
const COMPANIES = ["Zion Corp", "Apex Solutions", "Nova Systems", "Sigma Group", "Orion Tech"];
const DEPARTMENTS = ["Engineering", "Finance", "HR", "Operations", "IT", "Marketing"];

function pick(arr) { return arr[Math.floor(Math.random() * arr.length)]; }
function rng(min, max) { return Math.floor(Math.random() * (max - min + 1)) + min; }
function delay(min = 300, max = 700) {
    return new Promise(r => setTimeout(r, rng(min, max)));
}
function truncate(s, max) { return s.length <= max ? s : s.slice(0, max) + "…"; }

// ── Value Inference ──────────────────────────────────────────
function inferTextValue(hint, type) {
    if (type === "password" || hint.includes("pass"))
        return `Pass${rng(1000, 9999)}!A`;

    if (type === "email" || hint.includes("email") || hint.includes("mail"))
        return `${pick(FIRST_NAMES).toLowerCase()}${rng(10, 999)}@${pick(DOMAINS)}`;

    if (hint.includes("phone") || hint.includes("contact") || hint.includes("mobile")
        || hint.includes("tel") || hint.includes("cel") || hint.includes("number"))
        return `09${rng(100000000, 999999999)}`;

    if (hint.includes("user") || hint.includes("login") || hint.includes("account"))
        return `${pick(FIRST_NAMES).toLowerCase()}.${pick(LAST_NAMES).toLowerCase()}${rng(10, 99)}`;

    if (hint.includes("first") || hint.includes("fname") || hint.includes("given"))
        return pick(FIRST_NAMES);

    if (hint.includes("middle") || hint.includes("mname"))
        return pick(MIDDLE_NAMES);

    if (hint.includes("last") || hint.includes("lname") || hint.includes("surname")
        || hint.includes("family"))
        return pick(LAST_NAMES);

    if (hint.includes("suffix"))
        return pick(["Jr.", "Sr.", "III"]);

    if (hint.includes("fullname") || hint.includes("full_name") || hint.includes("full name"))
        return `${pick(FIRST_NAMES)} ${pick(LAST_NAMES)}`;

    if (hint.includes("position") || hint.includes("title") || hint.includes("jobtitle")
        || hint.includes("designation"))
        return pick(POSITIONS);

    if (hint.includes("department") || hint.includes("dept"))
        return pick(DEPARTMENTS);

    if (hint.includes("company") || hint.includes("organization") || hint.includes("org")
        || hint.includes("employer"))
        return pick(COMPANIES);

    if (hint.includes("address") || hint.includes("street") || hint.includes("line1")
        || hint.includes("addr"))
        return `${rng(1, 999)} ${pick(STREETS)}`;

    if (hint.includes("city") || hint.includes("municipality") || hint.includes("town"))
        return pick(CITIES);

    if (hint.includes("zip") || hint.includes("postal"))
        return String(rng(1000, 9999));

    if (hint.includes("age"))
        return String(rng(18, 65));

    if (type === "date" || hint.includes("date") || hint.includes("birth") || hint.includes("dob")) {
        const y = rng(1985, 2000), m = String(rng(1, 12)).padStart(2, "0"), d = String(rng(1, 28)).padStart(2, "0");
        return `${y}-${m}-${d}`;
    }

    if (type === "number" || hint.includes("amount") || hint.includes("qty")
        || hint.includes("quantity") || hint.includes("count"))
        return String(rng(1, 100));

    if (type === "url" || hint.includes("url") || hint.includes("website") || hint.includes("link"))
        return "https://example.com";

    if (hint.includes("note") || hint.includes("remark") || hint.includes("comment")
        || hint.includes("description") || hint.includes("message"))
        return "Auto-generated entry for testing purposes.";

    if (type === "text" || type === "search" || type === "tel" || !type)
        return `Value${rng(100, 999)}`;

    return null;
}

function inferSelectValue(hint, options) {
    const preferredRoles = ["USER", "ADMIN", "MANAGER"];

    if (hint.includes("role") || hint.includes("access") || hint.includes("permission")) {
        for (const pref of preferredRoles)
            for (const opt of options)
                if (opt.value.toLowerCase() === pref.toLowerCase() || opt.text.toLowerCase() === pref.toLowerCase())
                    return opt.value;
    }

    if (hint.includes("gender") || hint.includes("sex")) {
        for (const opt of options)
            if (opt.text.toLowerCase().includes("male") || opt.value.toLowerCase() === "m")
                return opt.value;
    }

    if (hint.includes("status") || hint.includes("active")) {
        for (const opt of options)
            if (opt.text.toLowerCase().includes("active") || opt.value === "1")
                return opt.value;
    }

    return options[rng(0, options.length - 1)].value;
}

function getLabelText(id) {
    if (!id) return "";
    try {
        const label = document.querySelector(`label[for="${id}"]`);
        return label ? label.innerText.trim() : "";
    } catch { return ""; }
}

// ── Native input value setter (triggers React/Vue/Angular change detection) ──
function nativeSet(el, value) {
    const proto = Object.getPrototypeOf(el);
    const setter = Object.getOwnPropertyDescriptor(proto, "value")?.set
        || Object.getOwnPropertyDescriptor(HTMLInputElement.prototype, "value")?.set;
    if (setter) setter.call(el, value);
    el.dispatchEvent(new Event("input", { bubbles: true }));
    el.dispatchEvent(new Event("change", { bubbles: true }));
}

// ── Simulate human typing ─────────────────────────────────────
async function humanType(el, value) {
    el.focus();
    el.select?.();
    document.execCommand("selectAll", false, null);
    document.execCommand("delete", false, null);

    for (const ch of value) {
        el.focus();
        // Insert character
        document.execCommand("insertText", false, ch);
        el.dispatchEvent(new KeyboardEvent("keydown", { key: ch, bubbles: true }));
        el.dispatchEvent(new KeyboardEvent("keypress", { key: ch, bubbles: true }));
        el.dispatchEvent(new KeyboardEvent("keyup", { key: ch, bubbles: true }));
        await delay(DELAY_MIN, DELAY_MAX);
    }

    // Fallback: if value didn't stick, set natively
    if (el.value !== value) nativeSet(el, value);

    el.dispatchEvent(new Event("blur", { bubbles: true }));
}

// ── Main fill logic ───────────────────────────────────────────
async function fillForm() {
    const log = [];

    // ── 1. Text / number / email / date inputs ─────────────────
    const textInputs = document.querySelectorAll(
        "input:not([type='hidden']):not([type='submit']):not([type='button'])" +
        ":not([type='reset']):not([type='image']):not([type='checkbox'])" +
        ":not([type='radio']):not([type='file']), textarea"
    );

    for (const el of textInputs) {
        try {
            const rect = el.getBoundingClientRect();
            const visible = rect.width > 0 && rect.height > 0 &&
                getComputedStyle(el).display !== "none" &&
                getComputedStyle(el).visibility !== "hidden";
            if (!visible || el.disabled || el.readOnly) continue;

            const name = (el.name || "").toLowerCase();
            const id = (el.id || "").toLowerCase();
            const type = (el.type || "text").toLowerCase();
            const ph = (el.placeholder || "").toLowerCase();
            const lbl = getLabelText(el.id).toLowerCase();
            const hint = `${name} ${id} ${ph} ${lbl}`;

            const value = inferTextValue(hint, type);
            if (!value) continue;

            el.scrollIntoView({ block: "center", behavior: "smooth" });
            await delay(150, 300);

            await humanType(el, value);

            log.push({ status: "ok", kind: "input", hint: truncate(hint, 40), value });
            await delay(100, 200);
        } catch (e) {
            log.push({ status: "error", kind: "input", msg: e.message });
        }
    }

    // ── 2. Native <select> dropdowns ───────────────────────────
    const selects = document.querySelectorAll("select:not(.select2-hidden-accessible)");
    for (const el of selects) {
        try {
            const rect = el.getBoundingClientRect();
            const visible = rect.width > 0 && rect.height > 0 &&
                getComputedStyle(el).display !== "none";
            if (!visible || el.disabled) continue;

            const hint = `${el.name} ${el.id} ${getLabelText(el.id)}`.toLowerCase();
            const options = [...el.options]
                .map(o => ({ value: o.value, text: o.text.trim() }))
                .filter(o => o.value && o.value !== "0" && o.value !== "-1");

            if (!options.length) continue;

            const picked = inferSelectValue(hint, options);
            el.scrollIntoView({ block: "center", behavior: "smooth" });
            await delay(150, 300);
            el.value = picked;
            el.dispatchEvent(new Event("change", { bubbles: true }));
            log.push({ status: "ok", kind: "select", hint: truncate(hint, 40), value: picked });
            await delay(100, 200);
        } catch (e) {
            log.push({ status: "error", kind: "select", msg: e.message });
        }
    }

    // ── 3. Select2 widgets ──────────────────────────────────────
    const hiddenSelects = document.querySelectorAll(
        "select.select2-hidden-accessible, select[style*='display: none'], select[style*='display:none']"
    );
    for (const hiddenSel of hiddenSelects) {
        try {
            const opts = [...hiddenSel.options]
                .map(o => ({ value: o.value, text: o.text.trim() }))
                .filter(o => o.value && o.value !== "0" && o.value !== "-1");
            if (!opts.length) continue;

            const picked = opts[rng(0, opts.length - 1)];
            const id = hiddenSel.id;
            const hint = `${hiddenSel.name} ${id}`.toLowerCase();

            // Try the Select2 search input
            const container = document.querySelector(`span[aria-owns*="${id}"], span.select2-container`);
            const trigger = container?.querySelector(".select2-selection");
            if (trigger) {
                trigger.dispatchEvent(new MouseEvent("mousedown", { bubbles: true }));
                await delay(400, 600);

                const search = document.querySelector(
                    ".select2-search__field, .select2-search input"
                );
                if (search) {
                    const term = picked.text.slice(0, 4);
                    search.value = term;
                    search.dispatchEvent(new Event("input", { bubbles: true }));
                    await delay(600, 900);

                    const resultItems = document.querySelectorAll(".select2-results__option");
                    for (const item of resultItems) {
                        if (item.textContent.includes(picked.text)) {
                            item.dispatchEvent(new MouseEvent("mouseup", { bubbles: true }));
                            log.push({ status: "ok", kind: "select2", hint, value: picked.text });
                            break;
                        }
                    }
                    // Close if nothing clicked
                    document.dispatchEvent(new KeyboardEvent("keydown", { key: "Escape", keyCode: 27, bubbles: true }));
                    continue;
                }
            }

            // Fallback: set the hidden select directly
            hiddenSel.value = picked.value;
            hiddenSel.dispatchEvent(new Event("change", { bubbles: true }));
            if (window.jQuery) window.jQuery(hiddenSel).trigger("change");
            log.push({ status: "ok", kind: "select2-fallback", hint, value: picked.text });
        } catch (e) {
            log.push({ status: "error", kind: "select2", msg: e.message });
        }
    }

    // ── 4. Checkboxes — check first per name-group ─────────────
    const checkedGroups = new Set();
    for (const el of document.querySelectorAll("input[type='checkbox']")) {
        try {
            const visible = el.offsetParent !== null;
            if (!visible || el.disabled) continue;
            const name = el.name || `__anon_${el.id}`;
            if (checkedGroups.has(name)) continue;
            if (!el.checked) {
                el.click();
                el.dispatchEvent(new Event("change", { bubbles: true }));
            }
            checkedGroups.add(name);
            log.push({ status: "ok", kind: "checkbox", hint: name });
            await delay(80, 150);
        } catch (e) {
            log.push({ status: "error", kind: "checkbox", msg: e.message });
        }
    }

    // ── 5. Radio buttons — first option per group ──────────────
    const radioGroups = new Set();
    for (const el of document.querySelectorAll("input[type='radio']")) {
        try {
            const visible = el.offsetParent !== null;
            if (!visible || el.disabled || !el.name) continue;
            if (radioGroups.has(el.name)) continue;
            el.click();
            el.dispatchEvent(new Event("change", { bubbles: true }));
            radioGroups.add(el.name);
            log.push({ status: "ok", kind: "radio", hint: el.name });
            await delay(80, 150);
        } catch (e) {
            log.push({ status: "error", kind: "radio", msg: e.message });
        }
    }

    return log;
}

// ── Message listener from popup ───────────────────────────────
chrome.runtime.onMessage.addListener((msg, _sender, sendResponse) => {
    if (msg.action === "fill") {
        fillForm().then(log => sendResponse({ ok: true, log }));
        return true; // async
    }
    if (msg.action === "clear") {
        clearForm().then(() => sendResponse({ ok: true }));
        return true;
    }
    if (msg.action === "ping") {
        sendResponse({ ok: true });
    }
});

// ── Clear all fields ──────────────────────────────────────────
async function clearForm() {
    const inputs = document.querySelectorAll(
        "input:not([type='hidden']):not([type='submit']):not([type='button'])" +
        ":not([type='reset']):not([type='image']):not([type='checkbox'])" +
        ":not([type='radio']):not([type='file']), textarea"
    );
    for (const el of inputs) {
        if (!el.disabled && !el.readOnly) {
            nativeSet(el, "");
        }
    }
    for (const el of document.querySelectorAll("input[type='checkbox'], input[type='radio']")) {
        if (!el.disabled) {
            el.checked = false;
            el.dispatchEvent(new Event("change", { bubbles: true }));
        }
    }
}