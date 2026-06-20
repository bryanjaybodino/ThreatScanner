// AutoFillForm.cs
using Microsoft.Playwright;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using ThreatScanner.Helpers;

namespace ThreatScanner
{
    public partial class AutoFillForm : Form
    {
        // ── Constants ─────────────────────────────────────────────────────────
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

        // ── Dummy files for upload fields ─────────────────────────────────────
        private static readonly string DummyFileDir = Path.Combine(Path.GetTempPath(), "AutoFillDummies");
        private static string _dummyPdf;
        private static string _dummyDocx;
        private static string _dummyImage;

        private static string GetDummyFilePath(string hint)
        {
            if (!Directory.Exists(DummyFileDir)) Directory.CreateDirectory(DummyFileDir);

            if (_dummyPdf == null)
            {
                _dummyPdf = Path.Combine(DummyFileDir, "dummy_resume.pdf");
                _dummyDocx = Path.Combine(DummyFileDir, "dummy_document.docx");
                _dummyImage = Path.Combine(DummyFileDir, "dummy_photo.png");

                if (!File.Exists(_dummyPdf))
                    File.WriteAllText(_dummyPdf,
                        "%PDF-1.4\n1 0 obj<</Type/Catalog/Pages 2 0 R>>endobj\n" +
                        "2 0 obj<</Type/Pages/Kids[3 0 R]/Count 1>>endobj\n" +
                        "3 0 obj<</Type/Page/MediaBox[0 0 612 792]/Parent 2 0 R>>endobj\n" +
                        "xref\n0 4\n0000000000 65535 f\n0000000009 00000 n\n" +
                        "0000000058 00000 n\n0000000115 00000 n\n" +
                        "trailer<</Size 4/Root 1 0 R>>\nstartxref\n190\n%%EOF");

                if (!File.Exists(_dummyDocx))
                    File.WriteAllBytes(_dummyDocx, CreateMinimalDocx());

                if (!File.Exists(_dummyImage))
                    File.WriteAllBytes(_dummyImage, new byte[] {
                        0x89,0x50,0x4E,0x47,0x0D,0x0A,0x1A,0x0A,0x00,0x00,0x00,0x0D,0x49,0x48,0x44,0x52,
                        0x00,0x00,0x00,0x01,0x00,0x00,0x00,0x01,0x08,0x02,0x00,0x00,0x00,0x90,0x77,0x53,
                        0xDE,0x00,0x00,0x00,0x0C,0x49,0x44,0x41,0x54,0x08,0xD7,0x63,0xF8,0xFF,0xFF,0x3F,
                        0x00,0x05,0xFE,0x02,0xFE,0xDC,0xCC,0x59,0xE7,0x00,0x00,0x00,0x00,0x49,0x45,0x4E,
                        0x44,0xAE,0x42,0x60,0x82 });
            }

            if (hint.Contains("photo") || hint.Contains("image") || hint.Contains("avatar") || hint.Contains("picture"))
                return _dummyImage;
            if (hint.Contains("doc") || hint.Contains("word"))
                return _dummyDocx;
            return _dummyPdf;
        }

        private static byte[] CreateMinimalDocx()
        {
            using (var ms = new MemoryStream())
            {
                using (var zip = new ZipArchive(ms, ZipArchiveMode.Create, leaveOpen: true))
                {
                    WriteZipEntry(zip, "[Content_Types].xml",
                        "<?xml version=\"1.0\" encoding=\"UTF-8\"?>" +
                        "<Types xmlns=\"http://schemas.openxmlformats.org/package/2006/content-types\">" +
                        "<Default Extension=\"rels\" ContentType=\"application/vnd.openxmlformats-package.relationships+xml\"/>" +
                        "<Override PartName=\"/word/document.xml\" " +
                        "ContentType=\"application/vnd.openxmlformats-officedocument.wordprocessingml.document.main+xml\"/>" +
                        "</Types>");
                    WriteZipEntry(zip, "_rels/.rels",
                        "<?xml version=\"1.0\" encoding=\"UTF-8\"?>" +
                        "<Relationships xmlns=\"http://schemas.openxmlformats.org/package/2006/relationships\">" +
                        "<Relationship Id=\"rId1\" " +
                        "Type=\"http://schemas.openxmlformats.org/officeDocument/2006/relationships/officeDocument\" " +
                        "Target=\"word/document.xml\"/>" +
                        "</Relationships>");
                    WriteZipEntry(zip, "word/document.xml",
                        "<?xml version=\"1.0\" encoding=\"UTF-8\"?>" +
                        "<w:document xmlns:w=\"http://schemas.openxmlformats.org/wordprocessingml/2006/main\">" +
                        "<w:body><w:p><w:r><w:t>AutoFill dummy document.</w:t></w:r></w:p></w:body>" +
                        "</w:document>");
                }
                return ms.ToArray();
            }
        }

        private static void WriteZipEntry(ZipArchive zip, string entryName, string content)
        {
            ZipArchiveEntry entry = zip.CreateEntry(entryName);
            using (StreamWriter sw = new StreamWriter(entry.Open()))
            {
                sw.Write(content);
            }
        }

        // ── CDP ───────────────────────────────────────────────────────────────
        private CdpHelper _cdp;

        private CancellationTokenSource _cts;
        private bool _running = false;
        private bool _fieldsDetected = false;

        // ── Constructor ───────────────────────────────────────────────────────
        public AutoFillForm()
        {
            InitializeComponent();
            textBox_TargetUrl.Text = "";
            ApplyOutputTheme();
            button_FillForm.Enabled = false;

            _cdp = new CdpHelper((msg, isError) =>
                Log(msg, isError ? Color.OrangeRed : (Color?)null));

            this.FormClosing += AutoFillForm_FormClosing;
        }

        private void AutoFillForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            _cdp?.Dispose();
        }

        private Task<IPage> GetOrCreateActivePageAsync() =>
            _cdp.GetOrCreateActivePageAsync();

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
                List<FieldInfo> fields = await DetectFieldsAsync(url);

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

                // Minimize browser, bring app to front if checkbox is ticked
                if (checkBox_HeadlessBrowser.Checked)
                {
                    IPage page = await GetOrCreateActivePageAsync();
                    await page.EvaluateAsync("() => window.blur()");
                    this.Activate();
                    this.BringToFront();
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
                await FillAutoDetected(url, delayMs, _cts.Token);

                if (!dryRun)
                    await SubmitFormAsync();
                else
                    Log("[Fill] Dry run complete — form filled but NOT submitted.", Color.Yellow);

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
        private async Task FillAutoDetected(string url, int delayMs, CancellationToken ct)
        {
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

            IPage page = await GetOrCreateActivePageAsync();
            if (page == null || page.IsClosed)
            {
                Log("[Fill] No active page. Run 'Detect Fields' first.", Color.OrangeRed);
                return;
            }

            Log(string.Format("[Fill] Reusing detected page: {0}  ({1} frame(s), {2} field(s) selected)",
                page.Url, page.Frames.Count, selectedFields.Count));

            HashSet<FieldInfo> handled = new HashSet<FieldInfo>();
            foreach (IFrame frame in page.Frames)
                await FillFrameFromGrid(frame, page, selectedFields, handled, delayMs, ct);

            int missed = selectedFields.Count - handled.Count;
            if (missed > 0)
                Log(string.Format(
                    "[Fill] {0} selected field(s) were not found on the page (re-run Detect if the DOM changed).",
                    missed), Color.OrangeRed);
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

        private void button_ClearOutput_Click(object sender, EventArgs e) =>
            richTextBox_Output.Clear();

        // =========================================================================
        //  SUBMIT
        // =========================================================================

        private async Task SubmitFormAsync()
        {
            IPage page = await GetOrCreateActivePageAsync();
            if (page == null || page.IsClosed)
            {
                Log("[Submit] No active page.", Color.OrangeRed);
                return;
            }

            string[] submitSelectors = new string[]
            {
                "input[type='submit']",
                "button[type='submit']",
                "button:not([type='button']):not([type='reset'])"
            };

            string[] submitKeywords = new string[]
            {
                "submit", "save", "send", "confirm", "register",
                "sign up", "sign in", "login", "next", "proceed", "apply"
            };

            foreach (IFrame frame in page.Frames)
            {
                foreach (string sel in submitSelectors)
                {
                    IReadOnlyList<IElementHandle> buttons =
                        await frame.QuerySelectorAllAsync(sel);

                    foreach (IElementHandle btn in buttons)
                    {
                        if (!await btn.IsVisibleAsync() || !await btn.IsEnabledAsync())
                            continue;

                        // For generic <button>, verify the label looks like a submit action
                        if (sel == "button:not([type='button']):not([type='reset'])")
                        {
                            string text = (await btn.InnerTextAsync()).Trim();
                            bool isSubmitLike = false;
                            foreach (string kw in submitKeywords)
                            {
                                if (text.IndexOf(kw, StringComparison.OrdinalIgnoreCase) >= 0)
                                {
                                    isSubmitLike = true;
                                    break;
                                }
                            }
                            if (!isSubmitLike) continue;
                        }

                        await btn.ScrollIntoViewIfNeededAsync();
                        await Task.Delay(300);
                        await btn.ClickAsync();
                        Log(string.Format("[Submit] Clicked '{0}' in frame: {1}",
                            sel, frame.Url), Color.LightGreen);
                        return;
                    }
                }
            }

            // Last resort
            Log("[Submit] No submit button found — pressing Enter.", Color.Yellow);
            await page.Keyboard.PressAsync("Enter");
        }

        // =========================================================================
        //  DETECT
        // =========================================================================

        private async Task<List<FieldInfo>> DetectFieldsAsync(string url)
        {
            List<FieldInfo> results = new List<FieldInfo>();

            IPage page = await GetOrCreateActivePageAsync();

            Log("[Detect] Navigating ...");
            await page.GotoAsync(url, new PageGotoOptions
            {
                WaitUntil = WaitUntilState.DOMContentLoaded,
                Timeout = 60000
            });
            await page.WaitForLoadStateAsync(LoadState.Load,
                new PageWaitForLoadStateOptions { Timeout = 30000 });

            if (page.Url.IndexOf("Login", StringComparison.OrdinalIgnoreCase) >= 0 &&
                url.IndexOf("Login", StringComparison.OrdinalIgnoreCase) < 0)
            {
                Log(string.Format(
                    "[Detect] WARNING: requested {0} but landed on {1} — log in first, then re-run Detect.",
                    url, page.Url), Color.OrangeRed);
            }

            Log(string.Format("[Detect][DIAG] page.Frames count = {0}", page.Frames.Count), Color.Cyan);

            int frameIndex = -1;
            foreach (IFrame frame in page.Frames)
            {
                frameIndex++;

                string formDescriptorsJson = "[]";
                try
                {
                    formDescriptorsJson = await frame.EvaluateAsync<string>(@"
                        () => JSON.stringify(Array.from(document.forms).map((f, i) =>
                            [`#${i}`, `id=${f.id||''}`, `name=${f.name||''}`, `action=${f.action||''}`, `elements=${f.elements.length}`].join(' ')
                        ))
                    ");
                }
                catch (Exception evalEx)
                {
                    Log(string.Format("[Detect][DIAG] frame[{0}] form-inventory eval failed: {1}",
                        frameIndex, evalEx.Message), Color.OrangeRed);
                }

                List<string> formDescriptors = new List<string>();
                try
                {
                    formDescriptors = JsonSerializer.Deserialize<List<string>>(formDescriptorsJson)
                        ?? new List<string>();
                }
                catch
                {
                    formDescriptors = new List<string> { "(raw) " + formDescriptorsJson };
                }

                Log(string.Format(
                    "[Detect][DIAG] frame[{0}] url='{1}' isDetached={2} <form> count={3}",
                    frameIndex, frame.Url, frame.IsDetached, formDescriptors.Count), Color.Cyan);

                foreach (string fd in formDescriptors)
                    Log("[Detect][DIAG]    form " + fd, Color.Cyan);

                IReadOnlyList<IElementHandle> inputs = await frame.QuerySelectorAllAsync(
                    "input:not([type='hidden']):not([type='submit']):not([type='button'])" +
                    ":not([type='reset']):not([type='image']):not([type='file']), textarea");
                IReadOnlyList<IElementHandle> fileInputs = await frame.QuerySelectorAllAsync("input[type='file']");
                IReadOnlyList<IElementHandle> selects = await frame.QuerySelectorAllAsync("select");
                IReadOnlyList<IElementHandle> checkboxes = await frame.QuerySelectorAllAsync("input[type='checkbox']");
                IReadOnlyList<IElementHandle> radios = await frame.QuerySelectorAllAsync("input[type='radio']");

                Log(string.Format(
                    "[Detect][DIAG] frame[{0}] raw query counts -> inputs/textarea={1} selects={2} checkboxes={3} radios={4} fileInputs={5}",
                    frameIndex, inputs.Count, selects.Count, checkboxes.Count, radios.Count, fileInputs.Count), Color.Cyan);

                int skippedInvisible = 0, skippedDisabled = 0, kept = 0;

                foreach (IElementHandle el in inputs)
                {
                    bool visible = await el.IsVisibleAsync();
                    bool enabled = await el.IsEnabledAsync();
                    if (!visible) { skippedInvisible++; continue; }
                    if (!enabled) { skippedDisabled++; continue; }

                    string name = await el.GetAttributeAsync("name") ?? "";
                    string id = await el.GetAttributeAsync("id") ?? "";
                    string type = await el.GetAttributeAsync("type") ?? "text";
                    string ph = await el.GetAttributeAsync("placeholder") ?? "";
                    string label = await GetLabelText(frame, id);
                    string hint = (name + " " + id + " " + ph + " " + label).ToLowerInvariant();

                    string ownerForm = "(eval failed)";
                    try
                    {
                        ownerForm = await el.EvaluateAsync<string>(@"
                            el => {
                                const f = el.closest('form');
                                if (!f) return '(none)';
                                const all = Array.from(document.forms);
                                return '#' + all.indexOf(f) + ' id=' + (f.id || '') + ' name=' + (f.name || '');
                            }
                        ");
                    }
                    catch (Exception ownerEx)
                    {
                        ownerForm = "(eval failed: " + ownerEx.Message + ")";
                    }

                    kept++;
                    Log(string.Format("[Detect][DIAG]    + input name='{0}' id='{1}' type='{2}' ownerForm={3}",
                        name, id, type, ownerForm), Color.Gray);

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

                    string ownerForm = "(eval failed)";
                    try
                    {
                        ownerForm = await el.EvaluateAsync<string>(@"
                            el => {
                                const f = el.closest('form');
                                if (!f) return '(none)';
                                const all = Array.from(document.forms);
                                return '#' + all.indexOf(f) + ' id=' + (f.id || '') + ' name=' + (f.name || '');
                            }
                        ");
                    }
                    catch (Exception ownerEx)
                    {
                        ownerForm = "(eval failed: " + ownerEx.Message + ")";
                    }

                    Log(string.Format("[Detect][DIAG]    + select name='{0}' id='{1}' ownerForm={2}",
                        name, id, ownerForm), Color.Gray);

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

                foreach (IElementHandle el in fileInputs)
                {
                    if (!await el.IsVisibleAsync() && !await el.IsEnabledAsync()) continue;
                    string name = await el.GetAttributeAsync("name") ?? "";
                    string id = await el.GetAttributeAsync("id") ?? "";
                    string label = await GetLabelText(frame, id);
                    string hint = (name + " " + id + " " + label).ToLowerInvariant();
                    string dummyPath = GetDummyFilePath(hint);
                    Log(string.Format("[Detect][DIAG]    + file-input name='{0}' id='{1}' -> will upload '{2}'",
                        name, id, Path.GetFileName(dummyPath)), Color.Gray);
                    results.Add(new FieldInfo
                    {
                        Tag = "input",
                        Name = name,
                        Id = id,
                        Type = "file",
                        Label = label,
                        Selector = BuildSelector(name, id),
                        SuggestedValue = dummyPath
                    });
                }

                Log(string.Format(
                    "[Detect][DIAG] frame[{0}] visible-input summary -> kept={1} skippedInvisible={2} skippedDisabled={3}",
                    frameIndex, kept, skippedInvisible, skippedDisabled), Color.Cyan);
            }

            Log(string.Format("[Detect][DIAG] TOTAL fields collected = {0}", results.Count), Color.Cyan);

            Dictionary<string, int> selectorCounts = new Dictionary<string, int>();
            foreach (FieldInfo fi in results)
            {
                if (!selectorCounts.ContainsKey(fi.Selector))
                    selectorCounts[fi.Selector] = 0;
                selectorCounts[fi.Selector]++;
            }
            foreach (KeyValuePair<string, int> kv in selectorCounts)
            {
                if (kv.Value > 1)
                    Log(string.Format("[Detect][DIAG] !! DUPLICATE selector '{0}' used by {1} fields.",
                        kv.Key, kv.Value), Color.OrangeRed);
            }

            return results;
        }

        // =========================================================================
        //  FILL (AUTO DETECT tab)
        // =========================================================================

        private async Task FillAutoDetected(string url, bool dryRun, int delayMs, CancellationToken ct)
        {
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

            if (dryRun)
            {
                foreach (FieldInfo f in selectedFields)
                    Log(string.Format("  [DRY] Would fill [{0}] -> \"{1}\"",
                        f.Selector, f.SuggestedValue), Color.Cyan);
                return;
            }

            IPage page = await GetOrCreateActivePageAsync();
            if (page == null || page.IsClosed)
            {
                Log("[Fill] No active page. Run 'Detect Fields' first.", Color.OrangeRed);
                return;
            }

            Log(string.Format("[Fill] Reusing detected page: {0}  ({1} frame(s), {2} field(s) selected)",
                page.Url, page.Frames.Count, selectedFields.Count));

            HashSet<FieldInfo> handled = new HashSet<FieldInfo>();
            foreach (IFrame frame in page.Frames)
                await FillFrameFromGrid(frame, page, selectedFields, handled, delayMs, ct);

            int missed = selectedFields.Count - handled.Count;
            if (missed > 0)
                Log(string.Format(
                    "[Fill] {0} selected field(s) were not found on the page (re-run Detect if the DOM changed).",
                    missed), Color.OrangeRed);
        }

        // =========================================================================
        //  GRID-DRIVEN FRAME FILLER
        // =========================================================================

        private async Task FillFrameFromGrid(IFrame frame, IPage page, List<FieldInfo> selectedFields,
            HashSet<FieldInfo> handled, int delayMs, CancellationToken ct)
        {
            foreach (FieldInfo f in selectedFields)
            {
                ct.ThrowIfCancellationRequested();
                if (handled.Contains(f) || string.IsNullOrEmpty(f.Selector)) continue;

                IElementHandle el;
                try { el = await frame.QuerySelectorAsync(f.Selector); }
                catch (Exception ex)
                {
                    Log(string.Format("  SKIP   [{0}] invalid selector: {1}", f.Selector, ex.Message),
                        Color.OrangeRed);
                    continue;
                }
                if (el == null) continue;

                bool visible = await el.IsVisibleAsync();
                bool enabled = await el.IsEnabledAsync();
                if (!visible || !enabled)
                {
                    Log(string.Format("  SKIP   [{0}] visible={1} enabled={2}", f.Selector, visible, enabled),
                        Color.OrangeRed);
                    handled.Add(f);
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
                    string val = f.SuggestedValue;
                    if (string.IsNullOrEmpty(val) || val == "(first valid option)")
                    {
                        IReadOnlyList<IElementHandle> opts = await el.QuerySelectorAllAsync("option");
                        foreach (IElementHandle opt in opts)
                        {
                            string v = await opt.GetAttributeAsync("value") ?? "";
                            if (!string.IsNullOrWhiteSpace(v) && v != "0" && v != "-1")
                            {
                                val = v;
                                break;
                            }
                        }
                    }
                    if (!string.IsNullOrEmpty(val))
                    {
                        await el.SelectOptionAsync(val);
                        Log(string.Format("  SELECT [{0}] -> \"{1}\"", f.Selector, val));
                    }
                    handled.Add(f);
                }
                else if (type == "file")
                {
                    string filePath = f.SuggestedValue;
                    if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
                    {
                        string hint2 = (f.Name + " " + f.Id + " " + f.Label).ToLowerInvariant();
                        filePath = GetDummyFilePath(hint2);
                    }
                    await el.SetInputFilesAsync(filePath);
                    Log(string.Format("  UPLOAD [{0}] -> \"{1}\"", f.Selector, Path.GetFileName(filePath)));
                    handled.Add(f);
                }
                else if (type == "date" || type == "time" || type == "week" ||
                         type == "month" || type == "range" || type == "color")
                {
                    string value = f.SuggestedValue;
                    if (string.IsNullOrEmpty(value)) { handled.Add(f); continue; }
                    await el.EvaluateAsync(
                        "(el, v) => { el.value = v; el.dispatchEvent(new Event('input', {bubbles:true})); el.dispatchEvent(new Event('change', {bubbles:true})); }",
                        value);
                    Log(string.Format("  FILL   [{0}] ({1}) -> \"{2}\" [via JS]", f.Selector, type, value));
                    handled.Add(f);
                }
                else
                {
                    string value = f.SuggestedValue;
                    if (string.IsNullOrEmpty(value)) { handled.Add(f); continue; }

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
            IReadOnlyList<IElementHandle> fileInputs = await frame.QuerySelectorAllAsync("input[type='file']");
            IReadOnlyList<IElementHandle> selects = await frame.QuerySelectorAllAsync("select");
            IReadOnlyList<IElementHandle> checkboxes = await frame.QuerySelectorAllAsync("input[type='checkbox']");
            IReadOnlyList<IElementHandle> radios = await frame.QuerySelectorAllAsync("input[type='radio']");

            if (inputs.Count == 0 && selects.Count == 0) return;

            Log(string.Format("[Frame] {0}  |  {1} inputs, {2} selects",
                frame.Url, inputs.Count, selects.Count));

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

                if (type == "date" || type == "time" || type == "week" ||
                    type == "month" || type == "range" || type == "color")
                {
                    await el.EvaluateAsync(
                        "(el, v) => { el.value = v; el.dispatchEvent(new Event('input', {bubbles:true})); el.dispatchEvent(new Event('change', {bubbles:true})); }",
                        value);
                    Log(string.Format("  FILL  [{0}] \"{1}\" -> \"{2}\" [via JS]",
                        type, Truncate(hint, 38), value));
                }
                else
                {
                    await el.ClickAsync(new ElementHandleClickOptions { ClickCount = 3 });
                    await Task.Delay(60);
                    await page.Keyboard.PressAsync("Delete");
                    foreach (char c in value)
                    {
                        await page.Keyboard.TypeAsync(c.ToString());
                        await Task.Delay(Rng.Next(DELAY_MIN, DELAY_MAX));
                    }
                    await page.Keyboard.PressAsync("Tab");
                    Log(string.Format("  FILL  [{0}] \"{1}\" -> \"{2}\"",
                        type, Truncate(hint, 38), value));
                }
                await Task.Delay(delayMs);
            }

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

            HashSet<string> checkedGroups = new HashSet<string>();
            foreach (IElementHandle el in checkboxes)
            {
                ct.ThrowIfCancellationRequested();
                if (!await el.IsVisibleAsync() || !await el.IsEnabledAsync()) continue;
                string name = await el.GetAttributeAsync("name") ?? Guid.NewGuid().ToString();
                if (checkedGroups.Contains(name)) continue;
                if (!await el.IsCheckedAsync())
                {
                    await el.ScrollIntoViewIfNeededAsync();
                    await el.CheckAsync();
                }
                Log(string.Format("  CHECK  checkbox name=\"{0}\"", name));
                checkedGroups.Add(name);
                await Task.Delay(delayMs);
            }

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

            foreach (IElementHandle el in fileInputs)
            {
                ct.ThrowIfCancellationRequested();
                if (!await el.IsEnabledAsync()) continue;
                string name = await el.GetAttributeAsync("name") ?? "";
                string id = await el.GetAttributeAsync("id") ?? "";
                string label = await GetLabelText(frame, id);
                string hint = (name + " " + id + " " + label).ToLowerInvariant();
                string filePath = GetDummyFilePath(hint);
                await el.ScrollIntoViewIfNeededAsync();
                await el.SetInputFilesAsync(filePath);
                Log(string.Format("  UPLOAD file-input name=\"{0}\" -> \"{1}\"",
                    name, Path.GetFileName(filePath)));
                await Task.Delay(delayMs);
            }
        }

        // =========================================================================
        //  STATIC HELPERS
        // =========================================================================

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
                return string.Format("{0}{1}@{2}",
                    Pick(FirstNames).ToLower(), Rng.Next(10, 999),
                    Pick(new[] { "gmail.com", "yahoo.com", "outlook.com" }));
            if (type == "url" || hint.Contains("url") || hint.Contains("website"))
                return string.Format("https://{0}.com", Pick(FirstNames).ToLower());
            if (type == "search")
                return Pick(new[] { "sample query", "test search", "keyword example", "auto fill test" });
            if (type == "date")
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
                return Pick(new[] { "#E53935", "#1E88E5", "#43A047", "#FB8C00", "#8E24AA" });
            if (type == "number" || hint.Contains("amount") || hint.Contains("qty"))
                return Rng.Next(18, 65).ToString();

            if (hint.Contains("phone") || hint.Contains("mobile") || hint.Contains("tel") || hint.Contains("cel"))
                return string.Format("09{0}", Rng.Next(100000000, 999999999));
            if (hint.Contains("first") || hint.Contains("fname") || hint.Contains("given"))
                return Pick(FirstNames);
            if (hint.Contains("middle") || hint.Contains("mname"))
                return Pick(MiddleNames);
            if (hint.Contains("last") || hint.Contains("lname") || hint.Contains("surname"))
                return Pick(LastNames);
            if (hint.Contains("suffix"))
                return Pick(new[] { "Jr.", "Sr.", "III" });
            if (hint.Contains("fullname") || hint.Contains("full_name") ||
                hint.Contains("full name") || hint.Contains("full-name"))
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
            if (hint.Contains("age"))
                return Rng.Next(18, 65).ToString();
            if (hint.Contains("date") || hint.Contains("birth") || hint.Contains("dob"))
                return string.Format("{0:D4}-{1:D2}-{2:D2}",
                    Rng.Next(1985, 2005), Rng.Next(1, 12), Rng.Next(1, 28));
            if (hint.Contains("browser"))
                return Pick(new[] { "Google Chrome", "Mozilla Firefox", "Microsoft Edge", "Safari", "Opera" });
            if (hint.Contains("note") || hint.Contains("remark") ||
                hint.Contains("comment") || hint.Contains("description"))
                return "Auto-generated entry for testing purposes.";

            if (type == "text" || string.IsNullOrEmpty(type))
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
                        if (string.Equals(opt[0], pref, StringComparison.OrdinalIgnoreCase))
                            return opt[0];

            return options[Rng.Next(options.Count)][0];
        }

        // ── UI helpers ────────────────────────────────────────────────────────

        private void SetRunning(bool running)
        {
            _running = running;
            progressBar_Scan.Style = running ? ProgressBarStyle.Marquee : ProgressBarStyle.Blocks;
            button_DetectFields.Enabled = !running;
            button_FillForm.Enabled = !running && _fieldsDetected;
            button_FillForm.Text = running ? "Running..." : "▶  Start Fill";
        }

        private void Log(string message, Color? color = null) =>
            ScanHelpers.LogRtbColor(richTextBox_Output, message, color);

        private void ApplyOutputTheme()
        {
            richTextBox_Output.BackColor = Color.FromArgb(15, 23, 42);
            richTextBox_Output.ForeColor = Color.FromArgb(212, 212, 212);
        }

        private static T Pick<T>(T[] arr) => arr[Rng.Next(arr.Length)];

        private static string Truncate(string s, int max) =>
            s.Length <= max ? s : s.Substring(0, max) + "...";

        // ── FieldInfo DTO ─────────────────────────────────────────────────────

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