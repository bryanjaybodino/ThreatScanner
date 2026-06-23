// FileUploadSecurityForm.cs
//
// Enhanced version: verdicts are now based on actually trying to retrieve the
// uploaded file (and checking whether your marker string comes back), instead
// of guessing from response wording. That's what makes the PHP target "just work"
// today — your keyword heuristic happens to line up with how that app phrases
// errors — and what makes ASP.NET (and most other frameworks/CMSs) look like
// false positives, because every framework phrases success/failure differently.
//
// IMPORTANT: only point this at applications you own or are explicitly
// authorized to test. The payloads here are inert markers, not real shells —
// keep it that way; this tool proves "the file persisted/executed", it
// doesn't need to do anything beyond that to be useful for a report.
//
using Microsoft.Playwright;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using ThreatScanner.Helpers;

namespace ThreatScanner
{
    public partial class FileUploadSecurityForm : Form
    {
        private const string Marker = "THREATSCANNER-UPLOAD-TEST-MARKER";

        private HttpClient _httpClient;
        private bool _running = false;
        private readonly CdpHelper _cdp = new CdpHelper();

        private string _detectedFieldName;
        private string _detectedAction;
        private string _detectedMethod;
        private Dictionary<string, string> _formStateFields = new Dictionary<string, string>();
        private string _submitName;
        private string _submitValue;

        // Populated once per run by ProbeFalsePositiveDirsAsync(). Holds any
        // "commonDirs" guess (e.g. "uploads/") that returns HTTP 200 with an
        // empty body for a file name THAT WE NEVER UPLOADED. If a directory
        // does that for a nonexistent file, a later 200+empty response from
        // that same directory is not evidence of anything — it's just how
        // that path always behaves (custom 404 handler, URL rewrite fallback,
        // empty placeholder page, etc.) and must NOT be treated as "confirmed
        // code execution."
        private HashSet<string> _poisonedDirPatterns = new HashSet<string>();
        private bool _baselineProbed = false;

        // True if this host returns HTTP 200 + empty body for ANY nonexistent
        // path — not just the commonDirs guesses. Some ASP.NET sites route
        // unmatched URLs through a catch-all handler / custom error page that
        // answers 200 with no content for literally anything. When that's
        // true, a blank-200 response is meaningless everywhere on this host,
        // including for links the upload response itself pointed us to —
        // those links can just as easily be unrelated bundled resources
        // (versioned script/image URLs) that happen to also resolve blank.
        private bool _hostBlankOkIsMeaningless = false;

        // ── Value pools ───────────────────────────────────────────────────────
        private static readonly Random Rng = new Random();
        private static readonly string[] FirstNames = { "James", "Maria", "Carlos", "Angela", "Ricardo", "Sofia", "Miguel", "Anna" };
        private static readonly string[] LastNames = { "Santos", "Reyes", "Cruz", "Garcia", "Lopez", "Torres", "Flores", "Ramos" };
        private static readonly string[] MiddleNames = { "Jose", "Marie", "Grace", "Paul", "Rose", "John", "Mae", "Luis" };
        private static readonly string[] Positions = { "Manager", "Analyst", "Developer", "Coordinator", "Supervisor", "Specialist" };
        private static readonly string[] Cities = { "Manila", "Cebu", "Davao", "Quezon City", "Makati" };
        private static readonly string[] Streets = { "Rizal Ave", "Mabini St", "Bonifacio Blvd", "Luna St", "Del Pilar" };
        private static readonly string[] Companies = { "Zion Corp", "Apex Solutions", "Nova Systems", "Sigma Group", "Orion Tech" };
        private static readonly string[] Departments = { "Engineering", "Finance", "HR", "Operations", "IT", "Marketing" };

        public FileUploadSecurityForm()
        {
            InitializeComponent();
            ScanHelpers.EnableRowDeletion(dataGridView_Results);
            dataGridView_Results.CellDoubleClick += (s, e) => ShowSnippetForRow(e.RowIndex);
            this.FormClosing += (s, e) =>
            {
                try { _cdp.Dispose(); } catch { }
                try { _httpClient?.Dispose(); } catch { }
            };
        }

        // =========================================================================
        //  RUN — auto-fill first, then fire all test cases
        // =========================================================================

        private async void button_RunTests_Click(object sender, EventArgs e)
        {
            if (_running) return;

            string url = textBox_PageUrl.Text.Trim();
            string field = string.IsNullOrWhiteSpace(textBox_FieldName.Text)
                ? "file" : textBox_FieldName.Text.Trim();

            if (string.IsNullOrEmpty(url) || !Uri.TryCreate(url, UriKind.Absolute, out _))
            {
                Log("[!] Enter a valid upload page URL first.", Color.OrangeRed);
                return;
            }

            var cases = BuildTestCases();
            if (cases.Count == 0)
            {
                Log("[!] Select at least one test case below.", Color.OrangeRed);
                return;
            }

            SetRunning(true);
            dataGridView_Results.Rows.Clear();

            // ── STEP 1: connect to browser, auto-fill form, detect file field ──
            try
            {
                Log("[*] Connecting to browser via CDP…", Color.DeepSkyBlue);
                IPage page = await _cdp.GetOrCreateActivePageAsync();

                Log("[*] Navigating to " + url, Color.DeepSkyBlue);
                await page.GotoAsync(url, new PageGotoOptions
                {
                    WaitUntil = WaitUntilState.DOMContentLoaded,
                    Timeout = 15000
                });

                try
                {
                    await page.WaitForLoadStateAsync(LoadState.NetworkIdle,
                        new PageWaitForLoadStateOptions { Timeout = 8000 });
                }
                catch { /* timeout is fine — continue */ }

                // Detect the file input and its parent form details
                var info = await DetectFileUploadFullAsync(page);
                if (info != null)
                {
                    _detectedFieldName = info.Value.fieldName;
                    _detectedAction = info.Value.action;
                    _detectedMethod = info.Value.method;
                    _formStateFields = info.Value.otherFields ?? new Dictionary<string, string>();
                    _submitName = info.Value.submitName;
                    _submitValue = info.Value.submitValue;

                    if (!string.IsNullOrEmpty(_detectedFieldName))
                        field = _detectedFieldName;

                    label_DetectStatus.Text =
                        $"Found: name=\"{_detectedFieldName}\", " +
                        $"method={_detectedMethod.ToUpperInvariant()}, " +
                        $"action={_detectedAction}";

                    Log($"[✓] Detected file input \"{_detectedFieldName}\" → " +
                        $"{_detectedMethod.ToUpperInvariant()} {_detectedAction}", Color.SeaGreen);

                    if (!string.IsNullOrEmpty(_submitName))
                        Log($"[✓] Detected submit control \"{_submitName}\"=\"{_submitValue}\" " +
                            $"and {_formStateFields.Count} hidden/state field(s) — will replay on every POST.", Color.SeaGreen);
                    else
                        Log("[!] No submit button found — Web Forms / postback pages may not process the upload without one.", Color.DarkOrange);

                    if (!info.Value.hasFile)
                        Log("[!] Form is not multipart/form-data — file may not transmit correctly.", Color.DarkOrange);
                }
                else
                {
                    label_DetectStatus.Text = "No <input type=\"file\"> found — using manual field name.";
                    Log("[!] No file input detected — using field name from textbox.", Color.DarkOrange);
                }

                await FillOtherFieldsAsync(page);

                // Re-harvest after autofill: text/password fields now have values,
                // and this also re-reads __VIEWSTATE/__EVENTVALIDATION in their
                // current state right before we start posting.
                var refreshed = await DetectFileUploadFullAsync(page);
                if (refreshed != null && refreshed.Value.otherFields != null)
                {
                    _formStateFields = refreshed.Value.otherFields;
                    if (!string.IsNullOrEmpty(refreshed.Value.submitName))
                    {
                        _submitName = refreshed.Value.submitName;
                        _submitValue = refreshed.Value.submitValue;
                    }
                }
            }
            catch (Exception ex)
            {
                Log("[!] Browser setup failed: " + ex.Message, Color.OrangeRed);
            }

            // ── STEP 2: fire all raw HTTP test cases ──────────────────────────
            Log($"[*] Running {cases.Count} test case(s) against {url} …", Color.DeepSkyBlue);

            _httpClient?.Dispose();
            _httpClient = BuildHttpClient(checkBox_IgnoreSslErrors.Checked);

            // Reset per-run false-positive cache and re-probe for this target.
            // Each target (PHP unsecured, PHP secured, ASPX unsecured, ASPX
            // secured, …) can have totally different routing/404 behavior, so
            // the poisoned-dir list must not leak across runs.
            _poisonedDirPatterns = new HashSet<string>();
            _baselineProbed = false;
            _hostBlankOkIsMeaningless = false;

            // Use the detected form action if we have one and it looks more
            // specific than the page URL itself (common when the upload page
            // posts to a different endpoint than the one you navigated to).
            string postUrl = !string.IsNullOrWhiteSpace(_detectedAction) ? _detectedAction : url;

            await ProbeFalsePositiveDirsAsync(postUrl);

            foreach (var test in cases)
            {
                if (!_running) break;
                await RunOneTest(postUrl, url, field, test);
            }

            Log("[*] Done.", Color.LimeGreen);
            SetRunning(false);
        }

        private static HttpClient BuildHttpClient(bool ignoreSslErrors)
        {
            var handler = new HttpClientHandler();
            if (ignoreSslErrors)
                handler.ServerCertificateCustomValidationCallback =
                    (request, cert, chain, errors) => true;
            return new HttpClient(handler) { Timeout = TimeSpan.FromSeconds(30) };
        }

        private void button_Stop_Click(object sender, EventArgs e) => _running = false;

        private void SetRunning(bool running)
        {
            _running = running;
            button_RunTests.Enabled = !running;
            button_Stop.Enabled = running;
            progressBar.Style = running
                ? ProgressBarStyle.Marquee
                : ProgressBarStyle.Blocks;
        }

        // =========================================================================
        //  AUTO-DETECT (manual button — still useful to preview without running)
        // =========================================================================

        private async void button_AutoDetect_Click(object sender, EventArgs e)
        {
            string pageUrl = textBox_PageUrl.Text.Trim();
            if (string.IsNullOrEmpty(pageUrl) || !Uri.TryCreate(pageUrl, UriKind.Absolute, out _))
            {
                Log("[!] Enter a valid page URL first.", Color.OrangeRed);
                return;
            }

            button_AutoDetect.Enabled = false;
            label_DetectStatus.Text = "Connecting to browser…";
            Log("[*] Auto-detect: connecting via CDP…", Color.DeepSkyBlue);

            try
            {
                IPage page = await _cdp.GetOrCreateActivePageAsync();

                Log("[*] Navigating to " + pageUrl, Color.DeepSkyBlue);
                await page.GotoAsync(pageUrl, new PageGotoOptions
                {
                    WaitUntil = WaitUntilState.DOMContentLoaded,
                    Timeout = 15000
                });

                try
                {
                    await page.WaitForLoadStateAsync(LoadState.NetworkIdle,
                        new PageWaitForLoadStateOptions { Timeout = 8000 });
                }
                catch { }

                var info = await DetectFileUploadAsync(page);
                if (info == null)
                {
                    label_DetectStatus.Text = "No <input type=\"file\"> found.";
                    Log("[!] No file input found on the active page.", Color.OrangeRed);
                    return;
                }

                _detectedFieldName = info.Value.fieldName;
                _detectedAction = info.Value.action;
                _detectedMethod = info.Value.method;
                textBox_FieldName.Text = _detectedFieldName;

                string multipartNote = info.Value.hasFile ? "" : "  ⚠ not multipart/form-data";
                label_DetectStatus.Text =
                    $"Found: name=\"{_detectedFieldName}\", " +
                    $"method={_detectedMethod.ToUpperInvariant()}, " +
                    $"action={_detectedAction}{multipartNote}";

                Log($"[✓] Detected \"{_detectedFieldName}\" → " +
                    $"{_detectedMethod.ToUpperInvariant()} {_detectedAction}", Color.SeaGreen);

                if (!info.Value.hasFile)
                    Log("[!] Form is not multipart/form-data.", Color.DarkOrange);
            }
            catch (Exception ex)
            {
                label_DetectStatus.Text = "Detection failed — see log.";
                Log("[!] Auto-detect failed: " + ex.Message, Color.OrangeRed);
            }
            finally
            {
                button_AutoDetect.Enabled = true;
            }
        }

        // =========================================================================
        //  DETECT FILE INPUT — searches all frames, waits per frame
        // =========================================================================

        private static async Task<(string fieldName, string action, string method, bool hasFile)?> DetectFileUploadAsync(IPage page)
        {
            var full = await DetectFileUploadFullAsync(page);
            if (full == null) return null;
            return (full.Value.fieldName, full.Value.action, full.Value.method, full.Value.hasFile);
        }

        // Extended version: also harvests every other form field's current value
        // (including ASP.NET's __VIEWSTATE / __VIEWSTATEGENERATOR / __EVENTVALIDATION
        // hidden fields) plus the submit button's name/value. Without replaying
        // those exact values, a raw POST to a Web Forms page never triggers the
        // page's click handler — it just falls through to Page_Load and re-renders
        // the empty form, which is why "rejected" and "accepted" looked identical.
        private static async Task<(string fieldName, string action, string method, bool hasFile,
            Dictionary<string, string> otherFields, string submitName, string submitValue)?> DetectFileUploadFullAsync(IPage page)
        {
            try
            {
                await page.WaitForLoadStateAsync(LoadState.NetworkIdle,
                    new PageWaitForLoadStateOptions { Timeout = 10000 });
            }
            catch { }

            foreach (IFrame frame in page.Frames)
            {
                if (frame.IsDetached) continue;

                try
                {
                    try
                    {
                        await frame.WaitForSelectorAsync("input[type=file]",
                            new FrameWaitForSelectorOptions
                            {
                                Timeout = 3000,
                                State = WaitForSelectorState.Attached
                            });
                    }
                    catch { continue; }

                    const string js = @"() => {
                        const input = document.querySelector('input[type=file]');
                        if (!input) return null;
                        const form = input.closest('form') || document.querySelector('form');
                        const fields = {};
                        let submitName = null, submitValue = null;
                        if (form) {
                            const els = form.querySelectorAll('input, select, textarea, button');
                            els.forEach(el => {
                                const type = (el.type || '').toLowerCase();
                                if (el === input || type === 'file') return;
                                if (type === 'submit' || (el.tagName === 'BUTTON' && (type === 'submit' || type === ''))) {
                                    if (submitName === null && el.name) {
                                        submitName = el.name;
                                        submitValue = el.value || el.innerText || 'Submit';
                                    }
                                    return;
                                }
                                if (type === 'checkbox' || type === 'radio') {
                                    if (el.checked && el.name) fields[el.name] = el.value || 'on';
                                    return;
                                }
                                if (el.name) fields[el.name] = el.value != null ? el.value : '';
                            });
                        }
                        return {
                            fieldName : input.name || input.id || 'file',
                            action    : form
                                        ? new URL(form.getAttribute('action') || '', location.href).href
                                        : location.href,
                            method    : (form && form.method ? form.method : 'post').toLowerCase(),
                            hasFile   : !form || (form.enctype || '').toLowerCase().includes('multipart'),
                            fields    : fields,
                            submitName: submitName,
                            submitValue: submitValue
                        };
                    }";

                    var result = await frame.EvaluateAsync<System.Text.Json.JsonElement?>(js);
                    if (result == null || result.Value.ValueKind == System.Text.Json.JsonValueKind.Null)
                        continue;

                    var obj = result.Value;
                    var otherFields = new Dictionary<string, string>();
                    if (obj.TryGetProperty("fields", out var fieldsEl) && fieldsEl.ValueKind == System.Text.Json.JsonValueKind.Object)
                    {
                        foreach (var prop in fieldsEl.EnumerateObject())
                            otherFields[prop.Name] = prop.Value.ValueKind == System.Text.Json.JsonValueKind.String ? prop.Value.GetString() : prop.Value.ToString();
                    }

                    string submitName = obj.TryGetProperty("submitName", out var sn) && sn.ValueKind == System.Text.Json.JsonValueKind.String ? sn.GetString() : null;
                    string submitValue = obj.TryGetProperty("submitValue", out var sv) && sv.ValueKind == System.Text.Json.JsonValueKind.String ? sv.GetString() : null;

                    return (
                        fieldName: obj.GetProperty("fieldName").GetString(),
                        action: obj.GetProperty("action").GetString(),
                        method: obj.GetProperty("method").GetString(),
                        hasFile: obj.GetProperty("hasFile").GetBoolean(),
                        otherFields: otherFields,
                        submitName: submitName,
                        submitValue: submitValue
                    );
                }
                catch { continue; }
            }

            return null;
        }

        // =========================================================================
        //  SILENT AUTO-FILL — fills all non-file fields invisibly
        // =========================================================================

        private async Task FillOtherFieldsAsync(IPage page)
        {
            Log("[*] Silently filling empty form fields…", Color.DeepSkyBlue);
            int filled = 0;

            foreach (IFrame frame in page.Frames)
            {
                if (frame.IsDetached) continue;

                IReadOnlyList<IElementHandle> inputs = await frame.QuerySelectorAllAsync(
                    "input:not([type='hidden']):not([type='submit']):not([type='button'])" +
                    ":not([type='reset']):not([type='image']):not([type='file'])" +
                    ":not([type='checkbox']):not([type='radio']), textarea");

                foreach (IElementHandle el in inputs)
                {
                    try
                    {
                        if (!await el.IsVisibleAsync() || !await el.IsEnabledAsync()) continue;

                        string existing = await el.InputValueAsync();
                        if (!string.IsNullOrWhiteSpace(existing)) continue;

                        string name = await el.GetAttributeAsync("name") ?? "";
                        string id = await el.GetAttributeAsync("id") ?? "";
                        string type = await el.GetAttributeAsync("type") ?? "text";
                        string ph = await el.GetAttributeAsync("placeholder") ?? "";
                        string hint = (name + " " + id + " " + ph).ToLowerInvariant();
                        string value = InferTextValue(hint, type);

                        if (string.IsNullOrEmpty(value)) continue;

                        if (type == "date" || type == "time" || type == "week" ||
                            type == "month" || type == "range" || type == "color")
                        {
                            await el.EvaluateAsync(
                                "(el, v) => { el.value = v; " +
                                "el.dispatchEvent(new Event('input',  {bubbles:true})); " +
                                "el.dispatchEvent(new Event('change', {bubbles:true})); }",
                                value);
                        }
                        else
                        {
                            await el.ClickAsync(new ElementHandleClickOptions { ClickCount = 3 });
                            await page.Keyboard.TypeAsync(value);
                            await page.Keyboard.PressAsync("Tab");
                        }

                        Log($"[~] Filled [{(string.IsNullOrEmpty(id) ? name : id)}] → \"{value}\"",
                            Color.SlateGray);
                        filled++;
                    }
                    catch { }
                }

                IReadOnlyList<IElementHandle> selects =
                    await frame.QuerySelectorAllAsync("select");

                foreach (IElementHandle el in selects)
                {
                    try
                    {
                        if (!await el.IsVisibleAsync() || !await el.IsEnabledAsync()) continue;

                        string existing = await el.InputValueAsync();
                        if (!string.IsNullOrWhiteSpace(existing) && existing != "0" && existing != "-1")
                            continue;

                        string name = await el.GetAttributeAsync("name") ?? "";
                        string id = await el.GetAttributeAsync("id") ?? "";
                        string hint = (name + " " + id).ToLowerInvariant();

                        IReadOnlyList<IElementHandle> opts =
                            await el.QuerySelectorAllAsync("option");
                        List<string[]> valid = new List<string[]>();
                        foreach (IElementHandle opt in opts)
                        {
                            string v = await opt.GetAttributeAsync("value") ?? "";
                            string t = (await opt.InnerTextAsync()).Trim();
                            if (!string.IsNullOrWhiteSpace(v) && v != "0" && v != "-1")
                                valid.Add(new string[] { v, t });
                        }
                        if (valid.Count == 0) continue;

                        string picked = InferSelectValue(hint, valid);
                        await el.SelectOptionAsync(picked);
                        Log($"[~] Selected [{(string.IsNullOrEmpty(id) ? name : id)}] → \"{picked}\"",
                            Color.SlateGray);
                        filled++;
                    }
                    catch { }
                }

                HashSet<string> checkedGroups = new HashSet<string>();
                IReadOnlyList<IElementHandle> checkboxes =
                    await frame.QuerySelectorAllAsync("input[type='checkbox']");

                foreach (IElementHandle el in checkboxes)
                {
                    try
                    {
                        if (!await el.IsVisibleAsync() || !await el.IsEnabledAsync()) continue;
                        string name = await el.GetAttributeAsync("name") ?? Guid.NewGuid().ToString();
                        if (checkedGroups.Contains(name)) continue;

                        if (await el.IsCheckedAsync()) { checkedGroups.Add(name); continue; }

                        await el.CheckAsync();
                        checkedGroups.Add(name);
                        filled++;
                    }
                    catch { }
                }

                HashSet<string> radioGroups = new HashSet<string>();
                IReadOnlyList<IElementHandle> radios =
                    await frame.QuerySelectorAllAsync("input[type='radio']");

                foreach (IElementHandle el in radios)
                {
                    try
                    {
                        if (!await el.IsVisibleAsync() || !await el.IsEnabledAsync()) continue;
                        string name = await el.GetAttributeAsync("name") ?? "";
                        if (string.IsNullOrEmpty(name) || radioGroups.Contains(name)) continue;

                        bool groupAlreadySelected = await page.EvaluateAsync<bool>(
                            @"(name) => {
                        const radios = document.querySelectorAll('input[type=radio][name=""' + name + '""]');
                        return Array.from(radios).some(r => r.checked);
                    }", name);
                        if (groupAlreadySelected) { radioGroups.Add(name); continue; }

                        await el.CheckAsync();
                        radioGroups.Add(name);
                        filled++;
                    }
                    catch { }
                }
            }

            Log($"[✓] Silent fill complete — {filled} empty field(s) filled.", Color.SeaGreen);
        }

        // ── Value inference ───────────────────────────────────────────────────

        private static string InferTextValue(string hint, string type)
        {
            if (type == "password" || hint.Contains("pass"))
                return string.Format("Pass{0}!A", Rng.Next(1000, 9999));
            if (type == "email" || hint.Contains("email") || hint.Contains("mail"))
                return string.Format("{0}{1}@gmail.com", Pick(FirstNames).ToLower(), Rng.Next(10, 999));
            if (type == "url" || hint.Contains("url") || hint.Contains("website"))
                return string.Format("https://{0}.com", Pick(FirstNames).ToLower());
            if (type == "search")
                return "test search";
            if (type == "date" || hint.Contains("date") || hint.Contains("birth") || hint.Contains("dob"))
                return string.Format("{0:D4}-{1:D2}-{2:D2}",
                    Rng.Next(1985, 2005), Rng.Next(1, 12), Rng.Next(1, 28));
            if (type == "time")
                return string.Format("{0:D2}:{1:D2}", Rng.Next(8, 17), Rng.Next(0, 59));
            if (type == "week")
                return string.Format("{0}-W{1:D2}", Rng.Next(2024, 2026), Rng.Next(1, 52));
            if (type == "month")
                return string.Format("{0}-{1:D2}", Rng.Next(2020, 2026), Rng.Next(1, 12));
            if (type == "range")
                return Rng.Next(3, 8).ToString();
            if (type == "color")
                return "#1E88E5";
            if (type == "number" || hint.Contains("amount") || hint.Contains("qty") || hint.Contains("age"))
                return Rng.Next(18, 65).ToString();
            if (hint.Contains("phone") || hint.Contains("mobile") || hint.Contains("tel") || hint.Contains("cel"))
                return string.Format("09{0}", Rng.Next(100000000, 999999999));
            if (hint.Contains("first") || hint.Contains("fname") || hint.Contains("given"))
                return Pick(FirstNames);
            if (hint.Contains("middle") || hint.Contains("mname"))
                return Pick(MiddleNames);
            if (hint.Contains("last") || hint.Contains("lname") || hint.Contains("surname"))
                return Pick(LastNames);
            if (hint.Contains("fullname") || hint.Contains("full_name") || hint.Contains("full name"))
                return string.Format("{0} {1}", Pick(FirstNames), Pick(LastNames));
            if (hint.Contains("user") || hint.Contains("login"))
                return string.Format("{0}.{1}{2}",
                    Pick(FirstNames).ToLower(), Pick(LastNames).ToLower(), Rng.Next(10, 99));
            if (hint.Contains("position") || hint.Contains("title") || hint.Contains("designation"))
                return Pick(Positions);
            if (hint.Contains("department") || hint.Contains("dept"))
                return Pick(Departments);
            if (hint.Contains("company") || hint.Contains("organization") || hint.Contains("org"))
                return Pick(Companies);
            if (hint.Contains("address") || hint.Contains("street"))
                return string.Format("{0} {1}", Rng.Next(1, 999), Pick(Streets));
            if (hint.Contains("city") || hint.Contains("municipality"))
                return Pick(Cities);
            if (hint.Contains("zip") || hint.Contains("postal"))
                return Rng.Next(1000, 9999).ToString();
            if (hint.Contains("note") || hint.Contains("remark") ||
                hint.Contains("comment") || hint.Contains("description"))
                return "Auto-generated entry for testing purposes.";

            return string.Format("Value{0}", Rng.Next(100, 999));
        }

        private static string InferSelectValue(string hint, List<string[]> options)
        {
            if (hint.Contains("gender") || hint.Contains("sex"))
                foreach (string[] opt in options)
                    if (opt[1].IndexOf("Male", StringComparison.OrdinalIgnoreCase) >= 0 ||
                        string.Equals(opt[0], "M", StringComparison.OrdinalIgnoreCase))
                        return opt[0];

            if (hint.Contains("status") || hint.Contains("active"))
                foreach (string[] opt in options)
                    if (opt[1].IndexOf("Active", StringComparison.OrdinalIgnoreCase) >= 0 || opt[0] == "1")
                        return opt[0];

            if (hint.Contains("role") || hint.Contains("access"))
                foreach (string pref in new[] { "USER", "ADMIN", "MANAGER" })
                    foreach (string[] opt in options)
                        if (string.Equals(opt[0], pref, StringComparison.OrdinalIgnoreCase))
                            return opt[0];

            return options[Rng.Next(options.Count)][0];
        }

        private static T Pick<T>(T[] arr) => arr[Rng.Next(arr.Length)];

        // =========================================================================
        //  TEST CASE GENERATION
        // =========================================================================

        private class TestCase
        {
            public string Name;
            public string FileName;
            public string ContentType;
            public byte[] Content;
            public string Marker;   // unique per-test marker so verification can't get confused by old uploads
        }

        private List<TestCase> BuildTestCases()
        {
            var list = new List<TestCase>();
            byte[] jpegMagic = { 0xFF, 0xD8, 0xFF, 0xE0 };

            string NewMarker() => $"{Marker}-{Guid.NewGuid():N}";
            // Echo AND comment the marker. If the file is served as static text,
            // the response will contain the literal "<?php" tag alongside the
            // marker. If the file is actually executed as PHP, the echo prints
            // just the marker with no surrounding tags — that's how we tell
            // "stored but inert" apart from "stored and executed" even when the
            // payload itself produces no other visible output.
            byte[] Payload(string marker) => Encoding.UTF8.GetBytes(
                $"<?php echo \"{marker}\"; /* {marker} */ ?>");

            void Add(string name, string fileName, string contentType, Func<string, byte[]> contentFactory)
            {
                string m = NewMarker();
                list.Add(new TestCase { Name = name, FileName = fileName, ContentType = contentType, Content = contentFactory(m), Marker = m });
            }

            if (checkBox_DoubleExtension.Checked)
                Add("Double extension", "shell.php.jpg", "image/jpeg", Payload);
            if (checkBox_NullByte.Checked)
                Add("Null byte injection", "shell.php\0.jpg", "image/jpeg", Payload);
            if (checkBox_CaseVariation.Checked)
                Add("Case variation", "shell.PhP", "application/octet-stream", Payload);
            if (checkBox_MimeSpoof.Checked)
                Add("MIME type spoof", "shell.php", "image/png", Payload);
            if (checkBox_MagicByteMismatch.Checked)
                Add("Magic-byte/polyglot mismatch", "shell.php", "image/jpeg", m => jpegMagic.Concat(Payload(m)).ToArray());
            if (checkBox_PathTraversal.Checked)
                Add("Path traversal filename", "../../../tmp/shell.php", "application/octet-stream", Payload);
            if (checkBox_NoExtension.Checked)
                Add("No extension", "shell", "application/octet-stream", Payload);
            if (checkBox_TrailingDot.Checked)
                Add("Trailing dot/space", "shell.php.", "image/jpeg", Payload);
            if (checkBox_AlternateExtension.Checked)
                foreach (var ext in new[] { "phtml", "php5", "pht", "asp", "aspx", "ashx", "jsp", "jspx", "cer", "asa" })
                    Add($"Alternate ext (.{ext})", $"shell.{ext}", "application/octet-stream", Payload);
            if (checkBox_Oversized.Checked)
            {
                long sizeBytes = (long)numericUpDown_OversizeMb.Value * 1024 * 1024;
                string m = NewMarker();
                list.Add(new TestCase
                {
                    Name = $"Oversized ({numericUpDown_OversizeMb.Value} MB)",
                    FileName = "big.jpg",
                    ContentType = "image/jpeg",
                    Content = BuildFiller(sizeBytes, jpegMagic),
                    Marker = m
                });
            }
            if (checkBox_ControlBaseline.Checked)
            {
                string m = NewMarker();
                list.Add(new TestCase
                {
                    Name = "Control (legit .jpg)",
                    FileName = "control.jpg",
                    ContentType = "image/jpeg",
                    Content = jpegMagic.Concat(Encoding.UTF8.GetBytes(m)).ToArray(),
                    Marker = m
                });
            }

            return list;
        }

        private static byte[] BuildFiller(long size, byte[] header)
        {
            if (size < header.Length) size = header.Length;
            var buf = new byte[size];
            Array.Copy(header, buf, header.Length);
            return buf;
        }

        // =========================================================================
        //  EXECUTE ONE TEST
        // =========================================================================

        private async Task RunOneTest(string postUrl, string pageUrl, string field, TestCase test)
        {
            string status = "ERR";
            long respSize = 0;
            string verdict = "?";
            Color verdictColor = Color.Gray;
            string snippet = "";
            double ms = 0;

            var sw = System.Diagnostics.Stopwatch.StartNew();
            try
            {
                string body;
                System.Net.HttpStatusCode httpStatus;

                using (var content = new MultipartFormDataContent())
                using (var fileContent = new ByteArrayContent(test.Content))
                {
                    fileContent.Headers.ContentType =
                        MediaTypeHeaderValue.TryParse(test.ContentType, out var mt)
                            ? mt : new MediaTypeHeaderValue("application/octet-stream");

                    content.Add(fileContent);
                    fileContent.Headers.TryAddWithoutValidation("Content-Disposition",
                        $"form-data; name=\"{field}\"; filename=\"{test.FileName.Replace("\"", "")}\"");

                    // Replay every other field exactly as the live page had it —
                    // critically including ASP.NET's __VIEWSTATE / __EVENTVALIDATION
                    // and the submit button's name=value. Without these, Web Forms
                    // (and many other frameworks with CSRF tokens) never invoke the
                    // upload handler at all, regardless of how "secure" it is.
                    foreach (var kv in _formStateFields)
                    {
                        var part = new StringContent(kv.Value ?? "");
                        part.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data") { Name = kv.Key };
                        content.Add(part);
                    }
                    if (!string.IsNullOrEmpty(_submitName))
                    {
                        var btnPart = new StringContent(_submitValue ?? "Upload");
                        btnPart.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data") { Name = _submitName };
                        content.Add(btnPart);
                    }

                    using (var response = await _httpClient.PostAsync(postUrl, content))
                    {
                        sw.Stop();
                        ms = sw.Elapsed.TotalMilliseconds;
                        httpStatus = response.StatusCode;
                        status = ((int)httpStatus).ToString();
                        body = await response.Content.ReadAsStringAsync();
                        respSize = Encoding.UTF8.GetByteCount(body);
                        snippet = body.Length > 500 ? body.Substring(0, 500) + "…" : body;
                    }
                }

                bool isControl = test.Name.StartsWith("Control");
                bool serverSaysRejected =
                    httpStatus == System.Net.HttpStatusCode.BadRequest ||
                    httpStatus == System.Net.HttpStatusCode.UnsupportedMediaType ||
                    httpStatus == System.Net.HttpStatusCode.Forbidden ||
                    ContainsRejectionKeyword(body);

                // ── Real verification: try to find and fetch the file we just
                //    sent, instead of trusting the upload response's wording. ──
                VerifyResult verify = await VerifyStoredAsync(pageUrl, postUrl, body, test);

                if (isControl)
                {
                    bool ok = verify.Found || (response_IsSuccess(httpStatus) && !serverSaysRejected);
                    verdict = ok ? "✔ Baseline OK" : "⚠ Baseline rejected?!";
                    verdictColor = ok ? Color.SeaGreen : Color.DarkOrange;
                }
                else if (verify.Found)
                {
                    verdict = verify.MarkerReflectedAsCode
                        ? "🔥 CONFIRMED — executed / execute-mapped path"
                        : "⚠ Confirmed — file stored & retrievable (static)";
                    verdictColor = Color.Firebrick;
                }
                else if (response_IsSuccess(httpStatus) && !serverSaysRejected)
                {
                    // Upload "succeeded" per HTTP, but we could not retrieve the
                    // file anywhere we looked. Could still be a real bypass
                    // (stored somewhere we didn't guess) — flag as unverified,
                    // not as a confirmed accept.
                    verdict = "? Accepted by server, unverified (check manually)";
                    verdictColor = Color.DarkOrange;
                }
                else
                {
                    verdict = "✔ Rejected";
                    verdictColor = Color.SeaGreen;
                }

                if (!string.IsNullOrEmpty(verify.Note))
                    snippet = $"[Verification] {verify.Note}\n\n{snippet}";
            }
            catch (Exception ex)
            {
                sw.Stop();
                ms = sw.Elapsed.TotalMilliseconds;
                status = "ERR";
                snippet = DescribeExceptionChain(ex);
                verdict = "✖ Request failed";
                verdictColor = Color.Gray;
            }

            int rowIndex = dataGridView_Results.Rows.Add(
                test.Name, test.FileName, test.ContentType, status,
                FormatSize(respSize), $"{ms:0} ms", verdict);
            var row = dataGridView_Results.Rows[rowIndex];
            row.Tag = snippet;
            row.DefaultCellStyle.ForeColor = verdictColor;

            Log($"[{status}] {test.Name} → {verdict}", verdictColor);
        }

        private static bool response_IsSuccess(System.Net.HttpStatusCode s) => (int)s >= 200 && (int)s < 300;

        // =========================================================================
        //  BASELINE PROBE — find candidate dirs that lie before we trust them
        // =========================================================================

        // Same list used by VerifyStoredAsync's directory-guessing step. Kept
        // as a single source of truth so the probe and the real check never
        // drift apart.
        private static readonly string[] CommonUploadDirs =
            { "uploads/", "upload/", "files/", "media/", "assets/uploads/", "wp-content/uploads/", "Uploads/" };

        /// <summary>
        /// Hits each commonDirs guess with a filename that was never uploaded
        /// (a fresh GUID). If a "directory" responds with HTTP 200 and an
        /// empty body for a file that does not exist, that response pattern
        /// is meaningless noise for THIS target (custom 404 page, rewrite
        /// fallback, etc.) — not evidence of code execution. We blacklist
        /// that exact directory so VerifyStoredAsync ignores blank-200 hits
        /// from it later, no matter which test case triggers it.
        /// </summary>
        private async Task ProbeFalsePositiveDirsAsync(string postUrl)
        {
            if (_baselineProbed) return;
            _baselineProbed = true;

            var baseUri = new Uri(postUrl);
            string probeName = "tsx-baseline-" + Guid.NewGuid().ToString("N") + ".txt";

            // (a) Per-directory probes — same as before, for guessed dirs.
            foreach (var dir in CommonUploadDirs)
            {
                if (!Uri.TryCreate(baseUri, dir + probeName, out var probeUri)) continue;

                try
                {
                    using (var resp = await _httpClient.GetAsync(probeUri))
                    {
                        string content = await resp.Content.ReadAsStringAsync();

                        if (resp.IsSuccessStatusCode && content.Length == 0)
                        {
                            _poisonedDirPatterns.Add(dir);
                            Log($"[~] Baseline: \"{dir}\" returns HTTP 200 + empty body for a " +
                                "nonexistent file — excluding it from execution-confirmation checks.",
                                Color.DarkOrange);
                        }
                    }
                }
                catch { /* nothing to blacklist if it didn't even respond */ }
            }

            // (b) Host-wide probes — hit a few arbitrary nonexistent paths
            //     (site root and a random nested path) to see if 200+empty is
            //     just this host's universal answer for "doesn't exist" (e.g.
            //     IIS/ASP.NET catch-all routing or a custom error page that
            //     returns 200). If so, blank-200 is worthless as evidence
            //     ANYWHERE on this host, including links extracted from the
            //     upload response itself.
            string[] arbitraryProbes =
            {
                probeName,                                      // site root
                "tsx-baseline-" + Guid.NewGuid().ToString("N") + "/" + probeName, // nested random dir
                probeName.Replace(".txt", ".aspx"),              // same name, app's own extension
            };

            foreach (var rel in arbitraryProbes)
            {
                if (!Uri.TryCreate(baseUri, rel, out var probeUri)) continue;

                try
                {
                    using (var resp = await _httpClient.GetAsync(probeUri))
                    {
                        string content = await resp.Content.ReadAsStringAsync();

                        if (resp.IsSuccessStatusCode && content.Length == 0)
                        {
                            if (!_hostBlankOkIsMeaningless)
                            {
                                _hostBlankOkIsMeaningless = true;
                                Log("[~] Baseline: this host returns HTTP 200 + empty body for arbitrary " +
                                    "nonexistent paths — blank-200 will be ignored as execution evidence " +
                                    "everywhere on this host (including links from the upload response).",
                                    Color.DarkOrange);
                            }
                        }
                    }
                }
                catch { /* unreachable path — not evidence either way */ }
            }
        }

        // =========================================================================
        //  VERIFICATION — try to actually retrieve what we just uploaded
        // =========================================================================

        private class VerifyResult
        {
            public bool Found;
            public bool MarkerReflectedAsCode; // body did NOT literally contain marker text => likely executed rather than served as static file
            public string Note;
        }

        private async Task<VerifyResult> VerifyStoredAsync(string pageUrl, string postUrl, string uploadResponseBody, TestCase test)
        {
            var baseUri = new Uri(postUrl);
            var candidates = new List<Uri>();

            // 1. Pull any link/src in the upload response that mentions our filename
            //    (sanitized name, with or without the original extension).
            string bareName = Path.GetFileName(test.FileName.Replace("\0", ""));
            string bareNameNoExt = Path.GetFileNameWithoutExtension(bareName);

            foreach (Match m in Regex.Matches(uploadResponseBody, "(?:href|src)\\s*=\\s*[\"']([^\"']+)[\"']", RegexOptions.IgnoreCase))
            {
                string link = m.Groups[1].Value;
                if (link.IndexOf(bareNameNoExt, StringComparison.OrdinalIgnoreCase) >= 0 ||
                    Regex.IsMatch(link, "[a-f0-9]{16,}\\.[a-z0-9]+$", RegexOptions.IgnoreCase)) // random-hex stored names
                {
                    if (Uri.TryCreate(baseUri, link, out var abs))
                        candidates.Add(abs);
                }
            }

            // 2. Common upload directory guesses, in case the response doesn't
            //    expose a direct link (e.g. it just shows a status message).
            //    We remember which literal dir produced each candidate so the
            //    blank-200 check below can ignore dirs we already proved are
            //    false-positive generators for this target.
            var candidateDirOf = new Dictionary<Uri, string>();
            foreach (var dir in CommonUploadDirs)
            {
                if (Uri.TryCreate(baseUri, dir + bareName, out var abs1))
                {
                    candidates.Add(abs1);
                    candidateDirOf[abs1] = dir;
                }
                if (bareName != test.FileName && Uri.TryCreate(baseUri, dir + Uri.EscapeDataString(test.FileName), out var abs2))
                {
                    candidates.Add(abs2);
                    candidateDirOf[abs2] = dir;
                }
            }

            candidates = candidates.Distinct().Take(12).ToList(); // keep the request count sane

            foreach (var uri in candidates)
            {
                try
                {
                    using (var resp = await _httpClient.GetAsync(uri))
                    {
                        string content = await resp.Content.ReadAsStringAsync();

                        if (resp.IsSuccessStatusCode && content.Contains(test.Marker))
                        {
                            // Marker present without any of the source-code
                            // wrapper syntax that would surround it in the
                            // original payload means the wrapper ran (got
                            // evaluated as code) and only the printed marker
                            // survived — i.e. the file was executed, not just
                            // served back as static text. Checked across the
                            // common server-side tag styles, not just PHP,
                            // since this verification runs against PHP, ASP,
                            // ASPX, JSP, etc. test cases alike.
                            string[] sourceWrapperMarkers = { "<?php", "<%", "<%@", "<%=", "<jsp:" };
                            bool anyWrapperPresent = sourceWrapperMarkers.Any(w =>
                                content.IndexOf(w, StringComparison.OrdinalIgnoreCase) >= 0);
                            bool executed = !anyWrapperPresent;

                            return new VerifyResult
                            {
                                Found = true,
                                MarkerReflectedAsCode = executed,
                                Note = executed
                                    ? $"Marker printed with no surrounding source tags at {uri} — the payload executed."
                                    : $"Marker found alongside literal source tags at {uri} — file is stored but served statically (not executed)."
                            };
                        }

                        if (resp.IsSuccessStatusCode && content.Length == 0)
                        {
                            // Blank 200 only counts as evidence if:
                            //  (1) this host doesn't return that pattern for
                            //      ARBITRARY nonexistent paths in general, and
                            //  (2) if this candidate came from a guessed dir,
                            //      that specific dir doesn't do it either.
                            // (See ProbeFalsePositiveDirsAsync.)
                            bool fromGuessedDir = candidateDirOf.TryGetValue(uri, out var dirUsed);
                            bool dirIsPoisoned = fromGuessedDir && _poisonedDirPatterns.Contains(dirUsed);

                            if (!_hostBlankOkIsMeaningless && !dirIsPoisoned)
                            {
                                return new VerifyResult
                                {
                                    Found = true,
                                    MarkerReflectedAsCode = true,
                                    Note = $"{uri} returned HTTP 200 with an EMPTY body, and neither this host " +
                                           "nor this directory show that pattern for nonexistent baseline " +
                                           "content. Consistent with the file having been executed as code " +
                                           "that produced no output — verify by using a payload extension " +
                                           "that's visibly different if you want to confirm beyond doubt."
                                };
                            }
                            // else: known false-positive pattern for this host/dir — ignore and keep looking.
                        }

                        if (!resp.IsSuccessStatusCode &&
                            resp.StatusCode != System.Net.HttpStatusCode.NotFound &&
                            IsCompilerOrRuntimeError(content))
                        {
                            // A 500 that looks like a compiler/parser error (ASP.NET
                            // "Compiler Error", JSP "org.apache.jasper", etc.) means
                            // the server tried to RUN the file as code and choked on
                            // syntax — not the same as "rejected". The path is
                            // execute-mapped; a real payload in the right language
                            // would likely have run.
                            return new VerifyResult
                            {
                                Found = true,
                                MarkerReflectedAsCode = true,
                                Note = $"{uri} returned a compiler/runtime error (HTTP {(int)resp.StatusCode}) — " +
                                       "the path is execute-mapped and attempted to run the file as code. " +
                                       "A same-language payload would likely achieve real execution here."
                            };
                        }
                    }
                }
                catch { /* candidate didn't resolve — try the next */ }
            }

            // 3. Last resort: re-check the upload page itself for a freshly
            //    listed file entry that wasn't present as a link (e.g. just a
            //    filename in a <table> row), then try that as a path.
            try
            {
                using (var resp = await _httpClient.GetAsync(pageUrl))
                {
                    string pageBody = await resp.Content.ReadAsStringAsync();
                    if (pageBody.IndexOf(bareNameNoExt, StringComparison.OrdinalIgnoreCase) >= 0 &&
                        !ContainsRejectionKeyword(pageBody))
                    {
                        return new VerifyResult
                        {
                            Found = false,
                            Note = $"Filename fragment \"{bareNameNoExt}\" appears on the upload page listing, " +
                                   "but no retrievable URL was confirmed automatically — verify manually."
                        };
                    }
                }
            }
            catch { }

            return new VerifyResult { Found = false, Note = "No stored/retrievable copy found at guessed locations." };
        }

        // ── Helpers ───────────────────────────────────────────────────────────

        private static bool IsCompilerOrRuntimeError(string body)
        {
            if (string.IsNullOrEmpty(body)) return false;

            // "Server Error in '/X' Application" / "Runtime Error" are the
            // generic ASP.NET YSOD WRAPPER shown for almost any unhandled
            // condition on localhost (customErrors defaults to RemoteOnly,
            // which shows full diagnostics to localhost callers) - including
            // a perfectly ordinary 404 "the resource cannot be found" for a
            // file that was simply never written. That wrapper text alone is
            // NOT evidence of compilation/execution; only the presence of an
            // actual compiler diagnostic (an error code, "Compiler Error",
            // a named exception type with a real stack trace) means the
            // server tried to RUN the file as code rather than just
            // reporting it missing. A plain "resource cannot be found"
            // message anywhere in the body means this was a 404, full stop -
            // never a compile/execution attempt - so it's checked first and
            // disqualifies a match outright, even if the YSOD wrapper text
            // also appears.
            string[] notFoundSignatures =
            {
                "cannot be found", "resource cannot be found", "HTTP 404",
                "could not be found", "does not exist"
            };
            if (notFoundSignatures.Any(s => body.IndexOf(s, StringComparison.OrdinalIgnoreCase) >= 0))
                return false;

            string[] signatures = {
                "Compiler Error Message", "CS0103", "CS1002", "CS0246",     // ASP.NET / Roslyn compile errors
                "An error occurred during the compilation of a resource",  // ASP.NET dynamic-compile failure
                "org.apache.jasper", "JasperException",                    // JSP/Tomcat
                "Parse error: syntax error", "PHP Parse error",            // PHP
                "Fatal error:", "Uncaught Error"
            };
            return signatures.Any(s => body.IndexOf(s, StringComparison.OrdinalIgnoreCase) >= 0);
        }

        private static string DescribeExceptionChain(Exception ex)
        {
            var parts = new List<string>();
            for (var cur = ex; cur != null; cur = cur.InnerException)
                parts.Add($"{cur.GetType().Name}: {cur.Message}");
            return string.Join("  →  ", parts);
        }

        private static bool ContainsRejectionKeyword(string body)
        {
            if (string.IsNullOrEmpty(body)) return false;
            string b = body.ToLowerInvariant();
            string[] keywords = {
                "invalid", "not allowed", "rejected", "unsupported", "forbidden",
                "denied", "error", "too large", "not permitted", "blocked",
                "disallowed", "failed", "bad request"
            };
            return keywords.Any(k => b.Contains(k));
        }

        private static string FormatSize(long bytes)
        {
            if (bytes < 1024) return bytes + " B";
            if (bytes < 1024 * 1024) return (bytes / 1024.0).ToString("0.#") + " KB";
            return (bytes / 1024.0 / 1024.0).ToString("0.#") + " MB";
        }

        private void ShowSnippetForRow(int rowIndex)
        {
            if (rowIndex < 0 || rowIndex >= dataGridView_Results.Rows.Count) return;
            string snippet = dataGridView_Results.Rows[rowIndex].Tag as string;
            if (string.IsNullOrEmpty(snippet)) snippet = "(empty response body)";
            MessageBox.Show(this, snippet, "Response snippet",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void button_SelectAll_Click(object sender, EventArgs e) => SetAllChecks(true);
        private void button_SelectNone_Click(object sender, EventArgs e) => SetAllChecks(false);

        private void button_ClearLog_Click(object sender, EventArgs e)
        {
            richTextBox_Output.Clear();
            dataGridView_Results.Rows.Clear();
        }

        private void button_SaveResults_Click(object sender, EventArgs e)
        {
            if (dataGridView_Results.Rows.Count == 0)
            {
                Log("[!] Nothing to save — run some tests first.", Color.OrangeRed);
                return;
            }

            using (var sfd = new SaveFileDialog
            {
                Filter = "CSV file (*.csv)|*.csv|All files (*.*)|*.*",
                FileName = "file_upload_security_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".csv"
            })
            {
                if (sfd.ShowDialog(this) != DialogResult.OK) return;

                var sb = new StringBuilder();
                sb.AppendLine("Test,FileName,ContentType,Status,RespSize,Time,Verdict,Snippet");
                foreach (DataGridViewRow row in dataGridView_Results.Rows)
                {
                    if (row.IsNewRow) continue;
                    sb.AppendLine(string.Join(",", new[]
                    {
                        CsvEscape(row.Cells["col_TestName"].Value),
                        CsvEscape(row.Cells["col_FileName"].Value),
                        CsvEscape(row.Cells["col_ContentType"].Value),
                        CsvEscape(row.Cells["col_HttpStatus"].Value),
                        CsvEscape(row.Cells["col_RespSize"].Value),
                        CsvEscape(row.Cells["col_Time"].Value),
                        CsvEscape(row.Cells["col_Verdict"].Value),
                        CsvEscape(row.Tag as string)
                    }));
                }

                File.WriteAllText(sfd.FileName, sb.ToString());
                Log("[*] Saved to " + sfd.FileName, Color.LimeGreen);
            }
        }

        private static string CsvEscape(object value)
        {
            string s = value?.ToString() ?? "";
            if (s.IndexOfAny(new[] { ',', '"', '\n', '\r' }) >= 0)
                s = "\"" + s.Replace("\"", "\"\"") + "\"";
            return s;
        }

        private void SetAllChecks(bool value)
        {
            foreach (Control c in panel_Tests.Controls)
                if (c is CheckBox cb) cb.Checked = value;
        }

        private void Log(string message, Color? color = null) =>
            ScanHelpers.LogRtb(richTextBox_Output,
                color == Color.OrangeRed || color == Color.Firebrick || color == Color.Gray ? "⛔" :
                color == Color.SeaGreen || color == Color.LimeGreen ? "✅" :
                color == Color.DarkOrange ? "⚠️" : "ℹ️",
                message);
    }
}