using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Playwright;

namespace ThreatScanner
{
    public partial class AutoFillForm : Form
    {
        // ── Constants ─────────────────────────────────────────────────────────
        private const string CDP_ENDPOINT = "http://localhost:65535";
        private const string EDGE_PATH_X86 = @"C:\Program Files (x86)\Microsoft\Edge\Application\msedge.exe";
        private const string EDGE_PATH_X64 = @"C:\Program Files\Microsoft\Edge\Application\msedge.exe";
        private const string EDGE_SESSION = @"C:\EdgeSession";

        private const int DELAY_MIN = 60;
        private const int DELAY_MAX = 180;

        private static readonly Random Rng = new Random();

        // ── Value pools ───────────────────────────────────────────────────────
        private static readonly string[] FirstNames = { "James", "Maria", "Carlos", "Angela", "Ricardo", "Sofia", "Miguel", "Anna" };
        private static readonly string[] LastNames = { "Santos", "Reyes", "Cruz", "Garcia", "Lopez", "Torres", "Flores", "Ramos" };
        private static readonly string[] MiddleNames = { "Jose", "Marie", "Grace", "Paul", "Rose", "John", "Mae", "Luis" };
        private static readonly string[] Positions = { "Manager", "Analyst", "Developer", "Coordinator", "Supervisor", "Specialist" };
        private static readonly string[] Cities = { "Manila", "Cebu", "Davao", "Quezon City", "Makati" };
        private static readonly string[] Streets = { "Rizal Ave", "Mabini St", "Bonifacio Blvd", "Luna St", "Del Pilar" };
        private static readonly string[] Companies = { "Zion Corp", "Apex Solutions", "Nova Systems", "Sigma Group", "Orion Tech" };
        private static readonly string[] Departments = { "Engineering", "Finance", "HR", "Operations", "IT", "Marketing" };

        private static readonly HttpClient CdpHttpClient = new HttpClient { Timeout = TimeSpan.FromSeconds(5) };

        private CancellationTokenSource _cts;
        private bool _running = false;
        private bool _fieldsDetected = false;

        // Persistent browser session shared between Detect (Step 2) and Fill (Step 3)
        // so we never re-navigate and never lose WebForms-generated control IDs.
        private IPlaywright _playwright;
        private IBrowser _browser;
        private IPage _activePage;

        // ── Constructor ───────────────────────────────────────────────────────
        public AutoFillForm()
        {
            InitializeComponent();
            textBox_TargetUrl.Text = "http://localhost/ERP/Login";
            ApplyOutputTheme();
            // Step 3 is locked until Step 2 (Detect) has run
            button_FillForm.Enabled = false;
            this.FormClosing += AutoFillForm_FormClosing;
        }

        private void AutoFillForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Disconnect (but do not close) the CDP-attached Edge — the user's
            // session/window should keep living outside this tool.
            try { _browser?.CloseAsync(); } catch { }
            try { _playwright?.Dispose(); } catch { }
        }

        // =========================================================================
        //  UI EVENT HANDLERS
        // =========================================================================

        private async void button_DetectFields_Click(object sender, EventArgs e)
        {
            if (_running) return;

            dataGridView_Detected.Rows.Clear();
            string url = textBox_TargetUrl.Text.Trim();
            if (string.IsNullOrEmpty(url)) { Log("ERROR: Target URL is empty.", Color.OrangeRed); return; }

            SetRunning(true);
            Log(string.Format("[Detect] Scanning {0} ...", url));

            try
            {
                bool headless = checkBox_HeadlessBrowser.Checked;
                List<FieldInfo> fields = await DetectFieldsAsync(url, headless);

                foreach (FieldInfo f in fields)
                {
                    int row = dataGridView_Detected.Rows.Add();
                    dataGridView_Detected.Rows[row].Cells["col_DetEnabled"].Value = true;
                    dataGridView_Detected.Rows[row].Cells["col_DetTag"].Value = f.Tag;
                    dataGridView_Detected.Rows[row].Cells["col_DetName"].Value = f.Name;
                    dataGridView_Detected.Rows[row].Cells["col_DetId"].Value = f.Id;
                    dataGridView_Detected.Rows[row].Cells["col_DetType"].Value = f.Type;
                    dataGridView_Detected.Rows[row].Cells["col_DetLabel"].Value = f.Label;
                    dataGridView_Detected.Rows[row].Cells["col_DetSelector"].Value = f.Selector;
                    dataGridView_Detected.Rows[row].Cells["col_DetValue"].Value = f.SuggestedValue;
                }

                Log(string.Format("[Detect] Found {0} field(s).", fields.Count), Color.LightGreen);
                _fieldsDetected = fields.Count > 0;
                button_FillForm.Enabled = _fieldsDetected;
            }
            catch (Exception ex)
            {
                Log(string.Format("[Detect] ERROR: {0}", ex.Message), Color.OrangeRed);
            }
            finally
            {
                SetRunning(false);
            }
        }

        private async void button_FillForm_Click(object sender, EventArgs e)
        {
            if (_running) return;

            string url = textBox_TargetUrl.Text.Trim();
            if (string.IsNullOrEmpty(url)) { Log("ERROR: Target URL is empty.", Color.OrangeRed); return; }

            bool dryRun = checkBox_DryRun.Checked;
            int delayMs = (int)numericUpDown_DelayMs.Value;

            SetRunning(true);
            Log(string.Format("[Fill] Starting {0} on {1} ...", dryRun ? "DRY RUN" : "LIVE RUN", url));

            _cts = new CancellationTokenSource();

            try
            {
                await FillAutoDetected(url, dryRun, delayMs, _cts.Token);

                Log("[Fill] Done.", Color.LightGreen);
            }
            catch (OperationCanceledException)
            {
                Log("[Fill] Cancelled.", Color.Yellow);
            }
            catch (Exception ex)
            {
                Log(string.Format("[Fill] ERROR: {0}", ex.Message), Color.OrangeRed);
            }
            finally
            {
                SetRunning(false);
            }
        }

        private void button_SaveReport_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog dlg = new SaveFileDialog())
            {
                dlg.Filter = "Text Log (*.txt)|*.txt|All files (*.*)|*.*";
                dlg.FileName = string.Format("autofill_log_{0:yyyyMMdd_HHmmss}.txt", DateTime.Now);
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    File.WriteAllText(dlg.FileName, richTextBox_Output.Text);
                    Log(string.Format("[Log] Saved to {0}", dlg.FileName), Color.LightGreen);
                }
            }
        }

        private void button_ClearOutput_Click(object sender, EventArgs e)
        {
            richTextBox_Output.Clear();
        }

        // =========================================================================
        //  CORE LOGIC — DETECT
        // =========================================================================

        private async Task<List<FieldInfo>> DetectFieldsAsync(string url, bool headless)
        {
            if (headless)
                Log("[Detect] Note: this tool now attaches to your real Edge session via CDP, so the page will be visible regardless of 'Run headless'. Log in there first if needed.", Color.Yellow);

            List<FieldInfo> results = new List<FieldInfo>();

            IPage page = await GetOrCreateActivePageAsync();

            Log("[Detect] Navigating ...");
            await page.GotoAsync(url, new PageGotoOptions
            {
                WaitUntil = WaitUntilState.DOMContentLoaded,
                Timeout = 60000
            });
            await page.WaitForLoadStateAsync(LoadState.Load, new PageWaitForLoadStateOptions { Timeout = 30000 });

            // If the URL we're scanning bounced to a login page, warn loudly —
            // detecting fields on a login redirect produces selectors that won't
            // exist once the real (post-login) page is shown.
            if (page.Url.IndexOf("Login", StringComparison.OrdinalIgnoreCase) >= 0 &&
                url.IndexOf("Login", StringComparison.OrdinalIgnoreCase) < 0)
            {
                Log(string.Format("[Detect] WARNING: requested {0} but landed on {1} — log in first, then re-run Detect.", url, page.Url), Color.OrangeRed);
            }

            foreach (IFrame frame in page.Frames)
            {
                IReadOnlyList<IElementHandle> inputs = await frame.QuerySelectorAllAsync(
                    "input:not([type='hidden']):not([type='submit']):not([type='button'])" +
                    ":not([type='reset']):not([type='image']):not([type='file']), textarea");
                IReadOnlyList<IElementHandle> selects = await frame.QuerySelectorAllAsync("select");
                IReadOnlyList<IElementHandle> checkboxes = await frame.QuerySelectorAllAsync("input[type='checkbox']");
                IReadOnlyList<IElementHandle> radios = await frame.QuerySelectorAllAsync("input[type='radio']");

                foreach (IElementHandle el in inputs)
                {
                    if (!await el.IsVisibleAsync() || !await el.IsEnabledAsync()) continue;
                    string name = await el.GetAttributeAsync("name") ?? "";
                    string id = await el.GetAttributeAsync("id") ?? "";
                    string type = await el.GetAttributeAsync("type") ?? "text";
                    string ph = await el.GetAttributeAsync("placeholder") ?? "";
                    string label = await GetLabelText(frame, id);
                    string hint = (name + " " + id + " " + ph + " " + label).ToLowerInvariant();
                    results.Add(new FieldInfo
                    {
                        Tag = "input",
                        Name = name,
                        Id = id,
                        Type = type,
                        Label = label,
                        Selector = BuildSelector(name, id),
                        SuggestedValue = InferTextValue(hint, type)
                    });
                }

                foreach (IElementHandle el in selects)
                {
                    if (!await el.IsVisibleAsync() || !await el.IsEnabledAsync()) continue;
                    string name = await el.GetAttributeAsync("name") ?? "";
                    string id = await el.GetAttributeAsync("id") ?? "";
                    string label = await GetLabelText(frame, id);
                    results.Add(new FieldInfo
                    {
                        Tag = "select",
                        Name = name,
                        Id = id,
                        Type = "select",
                        Label = label,
                        Selector = BuildSelector(name, id),
                        SuggestedValue = "(first valid option)"
                    });
                }

                foreach (IElementHandle el in checkboxes)
                {
                    if (!await el.IsVisibleAsync() || !await el.IsEnabledAsync()) continue;
                    string name = await el.GetAttributeAsync("name") ?? "";
                    string id = await el.GetAttributeAsync("id") ?? "";
                    results.Add(new FieldInfo
                    {
                        Tag = "input",
                        Name = name,
                        Id = id,
                        Type = "checkbox",
                        Label = "",
                        Selector = BuildSelector(name, id),
                        SuggestedValue = "true"
                    });
                }

                foreach (IElementHandle el in radios)
                {
                    if (!await el.IsVisibleAsync() || !await el.IsEnabledAsync()) continue;
                    string name = await el.GetAttributeAsync("name") ?? "";
                    string id = await el.GetAttributeAsync("id") ?? "";
                    results.Add(new FieldInfo
                    {
                        Tag = "input",
                        Name = name,
                        Id = id,
                        Type = "radio",
                        Label = "",
                        Selector = BuildSelector(name, id),
                        SuggestedValue = "(first)"
                    });
                }
            }

            // NOTE: we deliberately do NOT close the browser/page here.
            // Step 3 (Fill) reuses this exact same page so the selectors
            // captured above stay valid (WebForms regenerates control IDs
            // on every fresh render, so a second navigation would break them).
            return results;
        }

        /// <summary>
        /// Returns the live page Detect/Fill should both operate on. Connects to the
        /// already-running Edge (via CDP) on first use and reuses the same page for
        /// every subsequent call within this form's lifetime.
        /// </summary>
        private async Task<IPage> GetOrCreateActivePageAsync()
        {
            if (_activePage != null && !_activePage.IsClosed)
                return _activePage;

            await EnsureEdgeRunningAsync();

            _playwright = await Playwright.CreateAsync();
            _browser = await _playwright.Chromium.ConnectOverCDPAsync(CDP_ENDPOINT);
            IBrowserContext context = _browser.Contexts[0];

            IPage page = null;
            foreach (IPage p in context.Pages)
            {
                if (p.Url.IndexOf("localhost", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    page = p;
                    break;
                }
            }
            if (page == null)
                page = context.Pages.Count > 0 ? context.Pages[0] : await context.NewPageAsync();

            _activePage = page;
            return _activePage;
        }

        // =========================================================================
        //  CORE LOGIC — FILL (AUTO DETECT tab)
        // =========================================================================
        private async Task FillAutoDetected(string url, bool dryRun, int delayMs, CancellationToken ct)
        {
            // Build the list of fields the user left checked in the grid
            List<FieldInfo> selectedFields = new List<FieldInfo>();
            foreach (DataGridViewRow row in dataGridView_Detected.Rows)
            {
                bool enabled = row.Cells["col_DetEnabled"].Value is true;
                if (!enabled) continue;
                selectedFields.Add(new FieldInfo
                {
                    Tag = row.Cells["col_DetTag"].Value?.ToString() ?? "",
                    Name = row.Cells["col_DetName"].Value?.ToString() ?? "",
                    Id = row.Cells["col_DetId"].Value?.ToString() ?? "",
                    Type = row.Cells["col_DetType"].Value?.ToString() ?? "",
                    Label = row.Cells["col_DetLabel"].Value?.ToString() ?? "",
                    Selector = row.Cells["col_DetSelector"].Value?.ToString() ?? "",
                    SuggestedValue = row.Cells["col_DetValue"].Value?.ToString() ?? ""
                });
            }

            if (selectedFields.Count == 0)
            {
                Log("[Fill] No fields selected — tick at least one checkbox in the grid.", Color.OrangeRed);
                return;
            }

            // Dry run: just echo what would happen, no browser needed
            if (dryRun)
            {
                foreach (FieldInfo f in selectedFields)
                    Log(string.Format("  [DRY] Would fill [{0}] -> \"{1}\"", f.Selector, f.SuggestedValue), Color.Cyan);
                return;
            }

            // Live run: reuse the SAME page Detect already opened — do NOT navigate.
            // Re-navigating an ASP.NET WebForms page regenerates ctl_XXXXX IDs and/or
            // can bounce through a login redirect, which is exactly what broke fill.
            if (_activePage == null || _activePage.IsClosed)
            {
                Log("[Fill] No active page from Detect found. Run 'Detect Fields' first, then Start Fill.", Color.OrangeRed);
                return;
            }

            IPage page = _activePage;
            Log(string.Format("[Fill] Reusing detected page: {0}  ({1} frame(s), {2} field(s) selected)", page.Url, page.Frames.Count, selectedFields.Count));

            HashSet<FieldInfo> handled = new HashSet<FieldInfo>();
            foreach (IFrame frame in page.Frames)
                await FillFrameFromGrid(frame, page, selectedFields, handled, delayMs, ct);

            int missed = selectedFields.Count - handled.Count;
            if (missed > 0)
                Log(string.Format("[Fill] {0} selected field(s) were not found on the page (the DOM may have changed since Detect ran — re-run Detect).", missed), Color.OrangeRed);
        }


        // =========================================================================
        //  GRID-DRIVEN FRAME FILLER  (respects checkbox selection from Step 2)
        // =========================================================================
        private async Task FillFrameFromGrid(IFrame frame, IPage page, List<FieldInfo> selectedFields, HashSet<FieldInfo> handled, int delayMs, CancellationToken ct)
        {
            foreach (FieldInfo f in selectedFields)
            {
                ct.ThrowIfCancellationRequested();
                if (handled.Contains(f)) continue;
                if (string.IsNullOrEmpty(f.Selector)) continue;

                IElementHandle el;
                try
                {
                    el = await frame.QuerySelectorAsync(f.Selector);
                }
                catch (Exception ex)
                {
                    Log(string.Format("  SKIP   [{0}] invalid selector: {1}", f.Selector, ex.Message), Color.OrangeRed);
                    continue;
                }

                if (el == null) continue; // not in this frame — try the next one

                bool visible = await el.IsVisibleAsync();
                bool enabled = await el.IsEnabledAsync();
                if (!visible || !enabled)
                {
                    Log(string.Format("  SKIP   [{0}] visible={1} enabled={2}", f.Selector, visible, enabled), Color.OrangeRed);
                    handled.Add(f); // found it, just can't act on it — don't recount as "missing"
                    continue;
                }

                await el.ScrollIntoViewIfNeededAsync();
                await Task.Delay(Rng.Next(150, 300));

                string type = f.Type.ToLowerInvariant();

                if (type == "checkbox")
                {
                    if (!await el.IsCheckedAsync()) await el.CheckAsync();
                    Log(string.Format("  CHECK  [{0}]", f.Selector));
                    handled.Add(f);
                }
                else if (type == "radio")
                {
                    await el.CheckAsync();
                    Log(string.Format("  RADIO  [{0}]", f.Selector));
                    handled.Add(f);
                }
                else if (type == "select")
                {
                    // pick first valid option if value is the placeholder text
                    string val = f.SuggestedValue;
                    if (string.IsNullOrEmpty(val) || val == "(first valid option)")
                    {
                        IReadOnlyList<IElementHandle> opts = await el.QuerySelectorAllAsync("option");
                        foreach (IElementHandle opt in opts)
                        {
                            string v = await opt.GetAttributeAsync("value") ?? "";
                            if (!string.IsNullOrWhiteSpace(v) && v != "0" && v != "-1") { val = v; break; }
                        }
                    }
                    if (!string.IsNullOrEmpty(val))
                    {
                        await el.SelectOptionAsync(val);
                        Log(string.Format("  SELECT [{0}] -> \"{1}\"", f.Selector, val));
                    }
                    handled.Add(f);
                }
                else
                {
                    // text / email / password / date / number / textarea …
                    string value = f.SuggestedValue;
                    if (string.IsNullOrEmpty(value))
                    {
                        handled.Add(f);
                        continue;
                    }

                    await el.ClickAsync(new ElementHandleClickOptions { ClickCount = 3 });
                    await Task.Delay(60);
                    await page.Keyboard.PressAsync("Delete");
                    foreach (char c in value)
                    {
                        await page.Keyboard.TypeAsync(c.ToString());
                        await Task.Delay(Rng.Next(DELAY_MIN, DELAY_MAX));
                    }
                    await page.Keyboard.PressAsync("Tab");
                    Log(string.Format("  FILL   [{0}] -> \"{1}\"", f.Selector, value));
                    handled.Add(f);
                }

                await Task.Delay(delayMs);
            }
        }


        // =========================================================================
        //  DYNAMIC FRAME FILLER
        // =========================================================================
        private async Task FillFrameDynamic(IFrame frame, IPage page, int delayMs, CancellationToken ct)
        {
            IReadOnlyList<IElementHandle> inputs = await frame.QuerySelectorAllAsync(
                "input:not([type='hidden']):not([type='submit']):not([type='button'])" +
                ":not([type='reset']):not([type='image']):not([type='checkbox'])" +
                ":not([type='radio']):not([type='file']), textarea");
            IReadOnlyList<IElementHandle> selects = await frame.QuerySelectorAllAsync("select");
            IReadOnlyList<IElementHandle> checkboxes = await frame.QuerySelectorAllAsync("input[type='checkbox']");
            IReadOnlyList<IElementHandle> radios = await frame.QuerySelectorAllAsync("input[type='radio']");

            if (inputs.Count == 0 && selects.Count == 0) return;

            Log(string.Format("[Frame] {0}  |  {1} inputs, {2} selects", frame.Url, inputs.Count, selects.Count));

            // ── Text inputs ──────────────────────────────────────────────────
            foreach (IElementHandle el in inputs)
            {
                ct.ThrowIfCancellationRequested();
                if (!await el.IsVisibleAsync() || !await el.IsEnabledAsync()) continue;

                string name = await el.GetAttributeAsync("name") ?? "";
                string id = await el.GetAttributeAsync("id") ?? "";
                string type = await el.GetAttributeAsync("type") ?? "text";
                string ph = await el.GetAttributeAsync("placeholder") ?? "";
                string label = await GetLabelText(frame, id);
                string hint = (name + " " + id + " " + ph + " " + label).ToLowerInvariant();
                string value = InferTextValue(hint, type);
                if (string.IsNullOrEmpty(value)) continue;

                await el.ScrollIntoViewIfNeededAsync();
                await Task.Delay(Rng.Next(150, 300));
                await el.ClickAsync(new ElementHandleClickOptions { ClickCount = 3 });
                await Task.Delay(60);
                await page.Keyboard.PressAsync("Delete");

                foreach (char c in value)
                {
                    await page.Keyboard.TypeAsync(c.ToString());
                    await Task.Delay(Rng.Next(DELAY_MIN, DELAY_MAX));
                }
                await page.Keyboard.PressAsync("Tab");
                Log(string.Format("  FILL  [{0}] \"{1}\" -> \"{2}\"", type, Truncate(hint, 38), value));
                await Task.Delay(delayMs);
            }

            // ── Selects ──────────────────────────────────────────────────────
            foreach (IElementHandle el in selects)
            {
                ct.ThrowIfCancellationRequested();
                if (!await el.IsVisibleAsync() || !await el.IsEnabledAsync()) continue;

                string name = await el.GetAttributeAsync("name") ?? "";
                string id = await el.GetAttributeAsync("id") ?? "";
                string label = await GetLabelText(frame, id);
                string hint = (name + " " + id + " " + label).ToLowerInvariant();

                IReadOnlyList<IElementHandle> options = await el.QuerySelectorAllAsync("option");
                List<string[]> valid = new List<string[]>();
                foreach (IElementHandle opt in options)
                {
                    string v = await opt.GetAttributeAsync("value") ?? "";
                    string t = (await opt.InnerTextAsync()).Trim();
                    if (!string.IsNullOrWhiteSpace(v) && v != "0" && v != "-1")
                        valid.Add(new string[] { v, t });
                }
                if (valid.Count == 0) continue;

                string picked = InferSelectValue(hint, valid);
                await el.ScrollIntoViewIfNeededAsync();
                await Task.Delay(Rng.Next(150, 300));
                await el.SelectOptionAsync(picked);
                Log(string.Format("  SELECT \"{0}\" -> \"{1}\"", Truncate(hint, 38), picked));
                await Task.Delay(delayMs);
            }

            // ── Checkboxes ───────────────────────────────────────────────────
            HashSet<string> checkedGroups = new HashSet<string>();
            foreach (IElementHandle el in checkboxes)
            {
                ct.ThrowIfCancellationRequested();
                if (!await el.IsVisibleAsync() || !await el.IsEnabledAsync()) continue;
                string name = await el.GetAttributeAsync("name") ?? Guid.NewGuid().ToString();
                if (checkedGroups.Contains(name)) continue;
                if (!await el.IsCheckedAsync()) { await el.ScrollIntoViewIfNeededAsync(); await el.CheckAsync(); }
                Log(string.Format("  CHECK  checkbox name=\"{0}\"", name));
                checkedGroups.Add(name);
                await Task.Delay(delayMs);
            }

            // ── Radios ───────────────────────────────────────────────────────
            HashSet<string> radioGroups = new HashSet<string>();
            foreach (IElementHandle el in radios)
            {
                ct.ThrowIfCancellationRequested();
                if (!await el.IsVisibleAsync() || !await el.IsEnabledAsync()) continue;
                string name = await el.GetAttributeAsync("name") ?? "";
                if (string.IsNullOrEmpty(name) || radioGroups.Contains(name)) continue;
                await el.ScrollIntoViewIfNeededAsync();
                await el.CheckAsync();
                Log(string.Format("  RADIO  name=\"{0}\" (first option)", name));
                radioGroups.Add(name);
                await Task.Delay(delayMs);
            }
        }

        // =========================================================================
        //  HELPERS
        // =========================================================================
        private async Task EnsureEdgeRunningAsync()
        {
            if (await IsCdpReady()) { Log("[Edge] Already connected via CDP."); return; }

            string edgePath = GetEdgePath();
            if (edgePath == null)
            {
                Log("[Edge] msedge.exe not found. Launch Edge manually with --remote-debugging-port=65535", Color.OrangeRed);
                return;
            }

            Log("[Edge] Closing existing Edge instances ...");
            foreach (Process proc in Process.GetProcessesByName("msedge"))
            {
                try { proc.Kill(); proc.WaitForExit(2000); } catch { }
            }
            await Task.Delay(2500);

            Log("[Edge] Launching Edge with remote debugging on port 65535 ...");
            Process.Start(new ProcessStartInfo
            {
                FileName = edgePath,
                Arguments = "--remote-debugging-port=65535 --user-data-dir=\"" + EDGE_SESSION + "\" --no-first-run --no-default-browser-check",
                UseShellExecute = true
            });

            Log("[Edge] Waiting for Edge to be ready ...", Color.Yellow);
            for (int i = 0; i < 20; i++)
            {
                await Task.Delay(1000);
                if (await IsCdpReady()) { Log("[Edge] Ready!", Color.LightGreen); return; }
            }
            Log("[Edge] Edge did not respond in time - trying anyway.", Color.OrangeRed);
        }

        private static async Task<bool> IsCdpReady()
        {
            try
            {
                using (CancellationTokenSource cts = new CancellationTokenSource(TimeSpan.FromSeconds(5)))
                {
                    HttpResponseMessage res = await CdpHttpClient.GetAsync(CDP_ENDPOINT + "/json/version", cts.Token);
                    return res.IsSuccessStatusCode;
                }
            }
            catch { return false; }
        }

        private static async Task<string> GetLabelText(IFrame frame, string fieldId)
        {
            if (string.IsNullOrEmpty(fieldId)) return "";
            try
            {
                IElementHandle label = await frame.QuerySelectorAsync("label[for='" + fieldId + "']");
                if (label != null) return (await label.InnerTextAsync()).Trim();
            }
            catch { }
            return "";
        }

        private static string BuildSelector(string name, string id)
        {
            if (!string.IsNullOrEmpty(id)) return "#" + id;
            if (!string.IsNullOrEmpty(name)) return "[name='" + name + "']";
            return "";
        }

        private static string InferTextValue(string hint, string type)
        {
            if (type == "password" || hint.Contains("pass"))
                return string.Format("Pass{0}!A", Rng.Next(1000, 9999));
            if (type == "email" || hint.Contains("email") || hint.Contains("mail"))
                return string.Format("{0}{1}@{2}", Pick(FirstNames).ToLower(), Rng.Next(10, 999), Pick(new[] { "gmail.com", "yahoo.com", "outlook.com" }));
            if (hint.Contains("phone") || hint.Contains("mobile") || hint.Contains("tel") || hint.Contains("cel"))
                return string.Format("09{0}", Rng.Next(100000000, 999999999));
            if (hint.Contains("user") || hint.Contains("login"))
                return string.Format("{0}.{1}{2}", Pick(FirstNames).ToLower(), Pick(LastNames).ToLower(), Rng.Next(10, 99));
            if (hint.Contains("first") || hint.Contains("fname") || hint.Contains("given"))
                return Pick(FirstNames);
            if (hint.Contains("middle") || hint.Contains("mname"))
                return Pick(MiddleNames);
            if (hint.Contains("last") || hint.Contains("lname") || hint.Contains("surname"))
                return Pick(LastNames);
            if (hint.Contains("suffix"))
                return Pick(new[] { "Jr.", "Sr.", "III" });
            if (hint.Contains("fullname") || hint.Contains("full_name") || hint.Contains("full name"))
                return string.Format("{0} {1}", Pick(FirstNames), Pick(LastNames));
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
            if (hint.Contains("age"))
                return Rng.Next(18, 65).ToString();
            if (type == "date" || hint.Contains("date") || hint.Contains("birth") || hint.Contains("dob"))
                return string.Format("{0:D4}-{1:D2}-{2:D2}", Rng.Next(1985, 2000), Rng.Next(1, 12), Rng.Next(1, 28));
            if (type == "number" || hint.Contains("amount") || hint.Contains("qty"))
                return Rng.Next(1, 100).ToString();
            if (type == "url" || hint.Contains("url") || hint.Contains("website"))
                return "https://example.com";
            if (hint.Contains("note") || hint.Contains("remark") || hint.Contains("comment") || hint.Contains("description"))
                return "Auto-generated entry for testing purposes.";
            if (type == "text" || type == "search" || string.IsNullOrEmpty(type))
                return string.Format("Value{0}", Rng.Next(100, 999));
            return null;
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
                        if (string.Equals(opt[0], pref, StringComparison.OrdinalIgnoreCase)) return opt[0];

            return options[Rng.Next(options.Count)][0];
        }

        private static string GetEdgePath()
        {
            if (File.Exists(EDGE_PATH_X86)) return EDGE_PATH_X86;
            if (File.Exists(EDGE_PATH_X64)) return EDGE_PATH_X64;
            return null;
        }

        // ── UI helpers ────────────────────────────────────────────────────────
        private void SetRunning(bool running)
        {
            _running = running;
            progressBar_Scan.Style = running ? ProgressBarStyle.Marquee : ProgressBarStyle.Blocks;
            button_DetectFields.Enabled = !running;
            // Fill (Step 3) only enabled when not running AND fields have been detected
            button_FillForm.Enabled = !running && _fieldsDetected;
            button_FillForm.Text = running ? "Running..." : "▶  Start Fill";
        }

        private void Log(string message, Color? color = null)
        {
            if (richTextBox_Output.InvokeRequired)
            {
                richTextBox_Output.Invoke(new Action<string, Color?>(Log), message, color);
                return;
            }

            richTextBox_Output.SelectionStart = richTextBox_Output.TextLength;
            richTextBox_Output.SelectionLength = 0;
            richTextBox_Output.SelectionColor = color.HasValue ? color.Value : Color.FromArgb(212, 212, 212);
            richTextBox_Output.AppendText(string.Format("[{0:HH:mm:ss}]  {1}\r\n", DateTime.Now, message));
            richTextBox_Output.ScrollToCaret();
        }

        private void ApplyOutputTheme()
        {
            richTextBox_Output.BackColor = Color.FromArgb(15, 23, 42);
            richTextBox_Output.ForeColor = Color.FromArgb(212, 212, 212);
        }

        private static T Pick<T>(T[] arr) { return arr[Rng.Next(arr.Length)]; }

        private static string Truncate(string s, int max)
        {
            return s.Length <= max ? s : s.Substring(0, max) + "...";
        }

        // ─── FieldInfo DTO ────────────────────────────────────────────────────
        private class FieldInfo
        {
            public string Tag { get; set; }
            public string Name { get; set; }
            public string Id { get; set; }
            public string Type { get; set; }
            public string Label { get; set; }
            public string Selector { get; set; }
            public string SuggestedValue { get; set; }
        }
    }
}