using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using ThreatScanner.Helpers;

namespace ThreatScanner
{
    /// <summary>
    /// WebSocket tester: connect, send messages, receive frames, replay payloads.
    /// Output is displayed as a DataGridView table at the bottom of the form.
    /// </summary>
    public partial class WebSocketForm : Form
    {
        private ClientWebSocket _ws;
        private CancellationTokenSource _cts;
        private bool _connected = false;
        private int _rowCounter = 0;

        // Row-background colours for sent / received / system messages
        private static readonly Color COLOR_SENT = Color.FromArgb(20, 40, 20);   // dark green tint
        private static readonly Color COLOR_RECEIVED = Color.FromArgb(15, 30, 55);   // dark blue tint
        private static readonly Color COLOR_SYSTEM = Color.FromArgb(35, 35, 20);   // dark amber tint
        private static readonly Color COLOR_ERROR = Color.FromArgb(50, 15, 15);   // dark red tint

        // ── UI batching ──────────────────────────────────────────────────────
        // Worker threads (frame receive loop, fuzz/replay loop) can produce log
        // rows much faster than WinForms can paint them. Instead of marshalling
        // to the UI thread once per row (one Invoke + one grid layout/paint
        // each), we drop pending rows into a thread-safe queue and flush them
        // all at once on a timer. This turns "N thread-hops + N repaints" into
        // "~10 thread-hops/sec + ~10 repaints/sec" regardless of how fast rows
        // arrive, which is what actually removes the visible lag/freeze.
        private readonly ConcurrentQueue<PendingRow> _pendingRows = new ConcurrentQueue<PendingRow>();
        private readonly System.Windows.Forms.Timer _flushTimer = new System.Windows.Forms.Timer { Interval = 75 };
        private const int MaxRows = 5000; // cap so a long fuzz/replay run can't bloat memory/redraw cost

        private readonly struct PendingRow
        {
            public readonly string Direction, Type, Status, Data;
            public PendingRow(string d, string t, string s, string data)
            { Direction = d; Type = t; Status = s; Data = data; }
        }

        public WebSocketForm()
        {
            InitializeComponent();
            ScanHelpers.EnableRowDeletion(dataGridView_Headers);
            UpdateButtonStates();

            // Suppress the grid's own per-cell auto-resize/redraw while we're
            // batch-inserting rows; this alone removes a lot of paint overhead
            // compared to the default per-row autosize behaviour.
            dataGridView_Output.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;

            _flushTimer.Tick += (s, e) => FlushPendingRows();
            _flushTimer.Start();
            this.FormClosed += (s, e) => _flushTimer.Stop();

            // Ctrl+Enter in the message box fires Send
            richTextBox_Message.KeyDown += (s, e) =>
            {
                if (e.Control && e.KeyCode == Keys.Enter)
                {
                    e.SuppressKeyPress = true;
                    if (button_Send.Enabled)
                        button_Send_Click(s, EventArgs.Empty);
                }
            };
        }

        // ─── GRID LOGGING ────────────────────────────────────────────────────────

        /// <summary>
        /// Queues one row for the message log grid. Safe to call from any
        /// thread — never blocks the caller, never touches the grid directly.
        /// The actual insert happens in batches via FlushPendingRows().
        /// </summary>
        /// <param name="direction">→ Sent  |  ← Received  |  ℹ System  |  ❌ Error</param>
        /// <param name="type">TXT / BIN / INFO / ERR</param>
        /// <param name="status">Connected / Disconnected / OK / Fuzz / etc.</param>
        /// <param name="data">The payload or status text (single line, truncated)</param>
        private void LogRow(string direction, string type, string status, string data)
        {
            _pendingRows.Enqueue(new PendingRow(direction, type, status, data));
        }

        /// <summary>
        /// Drains the pending-row queue and applies all of them to the grid in
        /// a single SuspendLayout/ResumeLayout batch. Runs on the UI thread
        /// only (called from the Timer.Tick).
        /// </summary>
        private void FlushPendingRows()
        {
            if (_pendingRows.IsEmpty) return;

            bool wasAtBottom = dataGridView_Output.Rows.Count == 0
                || (dataGridView_Output.FirstDisplayedScrollingRowIndex >= 0
                    && dataGridView_Output.FirstDisplayedScrollingRowIndex
                       + dataGridView_Output.DisplayedRowCount(false)
                       >= dataGridView_Output.Rows.Count - 2);

            dataGridView_Output.SuspendLayout();
            int lastIdx = -1;
            try
            {
                while (_pendingRows.TryDequeue(out var pr))
                {
                    lastIdx = InsertRow(pr.Direction, pr.Type, pr.Status, pr.Data);
                }

                // Trim from the front if we've grown past the cap, so a long
                // session doesn't keep paying paint/memory cost forever.
                int overflow = dataGridView_Output.Rows.Count - MaxRows;
                if (overflow > 0)
                {
                    for (int i = 0; i < overflow; i++)
                        dataGridView_Output.Rows.RemoveAt(0);
                    lastIdx = dataGridView_Output.Rows.Count - 1;
                }
            }
            finally
            {
                dataGridView_Output.ResumeLayout(performLayout: true);
            }

            // Only auto-scroll if the user was already at the bottom — avoids
            // yanking them away from rows they're reading mid-scan.
            if (wasAtBottom && lastIdx >= 0)
                dataGridView_Output.FirstDisplayedScrollingRowIndex = lastIdx;
        }

        /// <summary>Actual single-row insert. UI-thread only, called from FlushPendingRows.</summary>
        private int InsertRow(string direction, string type, string status, string data)
        {
            int num = ++_rowCounter;
            string ts = DateTime.Now.ToString("HH:mm:ss.fff");

            // Truncate data to a single displayable line
            string display = data ?? "";
            int nl = display.IndexOfAny(new[] { '\n', '\r' });
            if (nl >= 0) display = display.Substring(0, nl) + " …";
            if (display.Length > 400) display = display.Substring(0, 400) + " …";

            int byteLen = Encoding.UTF8.GetByteCount(data ?? "");
            string size = byteLen > 0 ? $"{byteLen} B" : "—";

            int rowIdx = dataGridView_Output.Rows.Add(
                num.ToString(),
                ts,
                direction,
                type,
                status,
                size,
                display
            );

            // Colour-code row by direction
            var row = dataGridView_Output.Rows[rowIdx];
            Color bg = direction.Contains("→") ? COLOR_SENT
                     : direction.Contains("←") ? COLOR_RECEIVED
                     : direction.Contains("❌") ? COLOR_ERROR
                     : COLOR_SYSTEM;

            row.DefaultCellStyle.BackColor = bg;
            row.DefaultCellStyle.SelectionBackColor = bg.IsEmpty ? Color.FromArgb(30, 58, 138)
                : ControlPaint.Light(bg, 0.3f);

            // Colour-code direction cell text
            var dirCell = row.Cells["col_Direction"];
            if (direction.Contains("→")) dirCell.Style.ForeColor = Color.FromArgb(134, 239, 172);  // green
            else if (direction.Contains("←")) dirCell.Style.ForeColor = Color.FromArgb(125, 211, 252);  // sky blue
            else if (direction.Contains("❌")) dirCell.Style.ForeColor = Color.FromArgb(252, 165, 165);  // red
            else dirCell.Style.ForeColor = Color.FromArgb(253, 224, 71);   // yellow

            // Status cell colour
            var statusCell = row.Cells["col_Status"];
            if (status == "OK" || status == "Connected")
                statusCell.Style.ForeColor = Color.FromArgb(134, 239, 172);
            else if (status.StartsWith("HTTP 4") || status == "Error" || status == "Timeout")
                statusCell.Style.ForeColor = Color.FromArgb(252, 165, 165);

            return rowIdx;
        }

        private void LogSysRow(string status, string msg) => LogRow("ℹ  System", "INFO", status, msg);
        private void LogErrRow(string msg) => LogRow("❌  Error", "ERR", "Error", msg);

        private void ClearOut()
        {
            // Drop anything still queued so it doesn't reappear after a clear.
            while (_pendingRows.TryDequeue(out _)) { }

            if (dataGridView_Output.InvokeRequired)
            { dataGridView_Output.Invoke((Action)ClearOut); return; }
            dataGridView_Output.Rows.Clear();
            _rowCounter = 0;
        }

        private void SetProgress(bool running)
        {
            progressBar_Scan.Style = running ? ProgressBarStyle.Marquee : ProgressBarStyle.Blocks;
            if (!running) progressBar_Scan.Value = 0;
        }

        private void UpdateButtonStates()
        {
            button_Connect.Enabled = !_connected;
            button_Disconnect.Enabled = _connected;
            button_Send.Enabled = _connected;
            button_Fuzz.Enabled = _connected;
            textBox_WsUrl.Enabled = !_connected;
            dataGridView_Headers.Enabled = !_connected;
        }

        // ─── CONNECT ─────────────────────────────────────────────────────────────

        private async void button_Connect_Click(object sender, EventArgs e)
        {
            string rawUrl = textBox_WsUrl.Text.Trim();
            if (string.IsNullOrWhiteSpace(rawUrl))
            {
                MessageBox.Show("Enter a WebSocket URL (ws:// or wss://).", "ThreatScanner",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (rawUrl.StartsWith("http://", StringComparison.OrdinalIgnoreCase)) rawUrl = "ws://" + rawUrl.Substring(7);
            else if (rawUrl.StartsWith("https://", StringComparison.OrdinalIgnoreCase)) rawUrl = "wss://" + rawUrl.Substring(8);
            if (!rawUrl.StartsWith("ws://", StringComparison.OrdinalIgnoreCase) &&
                !rawUrl.StartsWith("wss://", StringComparison.OrdinalIgnoreCase))
                rawUrl = "ws://" + rawUrl;

            Uri uri;
            try { uri = new Uri(rawUrl); }
            catch
            {
                MessageBox.Show("Invalid WebSocket URL.", "ThreatScanner",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            ClearOut();
            SetProgress(true);
            LogSysRow("Connecting", $"→ {uri}");

            _cts = new CancellationTokenSource();
            _ws = new ClientWebSocket();

            var extraHeaders = ScanHelpers.GetEnabledGridRows(dataGridView_Headers, "col_WsHdrKey", "col_WsHdrValue");
            foreach (var kv in extraHeaders)
                if (!string.IsNullOrWhiteSpace(kv.Key))
                    _ws.Options.SetRequestHeader(kv.Key, kv.Value);

            string proto = textBox_SubProtocol.Text.Trim();
            if (!string.IsNullOrEmpty(proto))
                _ws.Options.AddSubProtocol(proto);

            try
            {
                await _ws.ConnectAsync(uri, _cts.Token);
                _connected = true;
                UpdateButtonStates();
                LogSysRow("Connected", $"State: {_ws.State}" +
                    (!string.IsNullOrEmpty(_ws.SubProtocol) ? $"  |  Subprotocol: {_ws.SubProtocol}" : ""));
                SetProgress(false);
                _ = ReceiveLoopAsync();
            }
            catch (Exception ex)
            {
                LogErrRow($"Connection failed: {ex.Message}");
                SetProgress(false);
                _ws?.Dispose();
                _ws = null;
                _connected = false;
                UpdateButtonStates();
            }
        }

        // ─── RECEIVE LOOP ─────────────────────────────────────────────────────────

        private async Task ReceiveLoopAsync()
        {
            var buffer = new byte[8192];
            var sb = new StringBuilder();

            try
            {
                while (_ws != null && _ws.State == WebSocketState.Open)
                {
                    sb.Clear();
                    WebSocketReceiveResult result;
                    do
                    {
                        result = await _ws.ReceiveAsync(new ArraySegment<byte>(buffer), _cts.Token);
                        if (result.MessageType == WebSocketMessageType.Close)
                        {
                            Invoke((Action)(() =>
                            {
                                LogSysRow("Disconnected",
                                    $"Server closed: {result.CloseStatus} — {result.CloseStatusDescription}");
                                HandleDisconnect();
                            }));
                            return;
                        }
                        sb.Append(Encoding.UTF8.GetString(buffer, 0, result.Count));
                    } while (!result.EndOfMessage);

                    string msg = sb.ToString();
                    string type = result.MessageType == WebSocketMessageType.Binary ? "BIN" : "TXT";

                    // Pretty-print JSON for display
                    string display = msg;
                    try
                    {
                        var doc = System.Text.Json.JsonDocument.Parse(msg);
                        display = System.Text.Json.JsonSerializer.Serialize(
                            doc.RootElement,
                            new System.Text.Json.JsonSerializerOptions { WriteIndented = true });
                    }
                    catch { }

                    Invoke((Action)(() => LogRow("←  Received", type, "OK", display)));
                }
            }
            catch (OperationCanceledException) { /* normal disconnect */ }
            catch (Exception ex)
            {
                if (!_cts.IsCancellationRequested)
                    Invoke((Action)(() =>
                    {
                        LogErrRow($"Receive error: {ex.Message}");
                        HandleDisconnect();
                    }));
            }
        }

        // ─── SEND ─────────────────────────────────────────────────────────────────

        private async void button_Send_Click(object sender, EventArgs e)
        {
            if (_ws == null || _ws.State != WebSocketState.Open)
            {
                LogErrRow("Not connected.");
                return;
            }

            string msg = richTextBox_Message.Text;
            if (string.IsNullOrEmpty(msg)) return;

            bool isBinary = checkBox_SendBinary.Checked;
            byte[] bytes = isBinary ? ParseHexOrUtf8(msg) : Encoding.UTF8.GetBytes(msg);
            string type = isBinary ? "BIN" : "TXT";

            try
            {
                var msgType = isBinary ? WebSocketMessageType.Binary : WebSocketMessageType.Text;
                await _ws.SendAsync(new ArraySegment<byte>(bytes), msgType, true, _cts.Token);
                LogRow("→  Sent", type, "OK", msg);
            }
            catch (Exception ex)
            {
                LogErrRow($"Send error: {ex.Message}");
            }
        }

        // ─── FUZZ / REPLAY ───────────────────────────────────────────────────────

        private async void button_Fuzz_Click(object sender, EventArgs e)
        {
            if (_ws == null || _ws.State != WebSocketState.Open)
            {
                LogErrRow("Not connected.");
                return;
            }

            string wordlistPath = textBox_FuzzWordlist.Text.Trim();
            string template = richTextBox_Message.Text;

            if (string.IsNullOrEmpty(template))
            {
                MessageBox.Show("Enter a message template. Use §FUZZ§ as the injection point.", "ThreatScanner",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            List<string> payloads;
            if (!string.IsNullOrEmpty(wordlistPath) && File.Exists(wordlistPath))
            {
                payloads = File.ReadAllLines(wordlistPath).Where(l => !string.IsNullOrWhiteSpace(l)).ToList();
                LogSysRow("Fuzz", $"Wordlist: {payloads.Count} payload(s)");
            }
            else
            {
                payloads = new List<string>
                {
                    "<script>alert(1)</script>",
                    "' OR '1'='1",
                    "\" OR \"1\"=\"1",
                    "../../../etc/passwd",
                    "{{7*7}}",
                    "${7*7}",
                    "admin",
                    "null",
                    "undefined",
                    "true",
                    "false",
                    "[]",
                    "{}",
                    "9999999999",
                    "-1",
                    "A".PadRight(1000, 'A'),
                };
                LogSysRow("Fuzz", $"Using {payloads.Count} built-in payloads");
            }

            button_Fuzz.Enabled = false;
            bool useFuzzMarker = template.Contains("§FUZZ§");
            int delay = (int)numericUpDown_FuzzDelay.Value;

            LogSysRow("Fuzz", "Starting fuzz…");

            try
            {
                foreach (string payload in payloads)
                {
                    if (_ws == null || _ws.State != WebSocketState.Open)
                    {
                        LogSysRow("Fuzz", "Connection lost — stopping.");
                        break;
                    }

                    string msg = useFuzzMarker ? template.Replace("§FUZZ§", payload) : payload;
                    byte[] bytes = Encoding.UTF8.GetBytes(msg);

                    try
                    {
                        await _ws.SendAsync(new ArraySegment<byte>(bytes), WebSocketMessageType.Text, true, _cts.Token);
                        LogRow("→  Sent", "TXT", "Fuzz", msg);
                        await Task.Delay(delay);
                    }
                    catch (Exception ex)
                    {
                        LogErrRow($"Send error: {ex.Message}");
                        break;
                    }
                }
            }
            finally
            {
                LogSysRow("Fuzz", "Complete.");
                if (_connected) button_Fuzz.Enabled = true;
            }
        }

        // ─── TEST CONNECTION (PING) ───────────────────────────────────────────────

        private async void button_TestConnection_Click(object sender, EventArgs e)
        {
            string rawUrl = textBox_WsUrl.Text.Trim();
            if (string.IsNullOrWhiteSpace(rawUrl))
            {
                MessageBox.Show("Enter a WebSocket URL to test.", "ThreatScanner",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (rawUrl.StartsWith("http://", StringComparison.OrdinalIgnoreCase)) rawUrl = "ws://" + rawUrl.Substring(7);
            else if (rawUrl.StartsWith("https://", StringComparison.OrdinalIgnoreCase)) rawUrl = "wss://" + rawUrl.Substring(8);
            if (!rawUrl.StartsWith("ws://", StringComparison.OrdinalIgnoreCase) &&
                !rawUrl.StartsWith("wss://", StringComparison.OrdinalIgnoreCase))
                rawUrl = "ws://" + rawUrl;

            Uri uri;
            try { uri = new Uri(rawUrl); }
            catch
            {
                MessageBox.Show("Invalid WebSocket URL.", "ThreatScanner",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            button_TestConnection.Enabled = false;
            SetProgress(true);
            LogSysRow("Testing", $"→ {uri}");

            using (var testWs = new ClientWebSocket())
            using (var cts = new CancellationTokenSource(TimeSpan.FromSeconds(8)))
            {
                try
                {
                    var sw = System.Diagnostics.Stopwatch.StartNew();
                    await testWs.ConnectAsync(uri, cts.Token);
                    sw.Stop();

                    string sub = !string.IsNullOrEmpty(testWs.SubProtocol) ? $"  |  Sub: {testWs.SubProtocol}" : "";
                    LogSysRow("Reachable", $"Handshake: {sw.ElapsedMilliseconds} ms  |  State: {testWs.State}{sub}");
                    await testWs.CloseAsync(WebSocketCloseStatus.NormalClosure, "ping-test", CancellationToken.None);
                }
                catch (OperationCanceledException)
                {
                    LogRow("❌  Error", "INFO", "Timeout", "Connection timed out (8 s) — host unreachable or not a WebSocket endpoint.");
                }
                catch (Exception ex)
                {
                    LogErrRow($"Unreachable: {ex.Message}");
                }
            }

            SetProgress(false);
            button_TestConnection.Enabled = true;
        }

        // ─── DISCONNECT ───────────────────────────────────────────────────────────

        private async void button_Disconnect_Click(object sender, EventArgs e)
        {
            if (_ws == null) return;

            try
            {
                _cts?.Cancel();
                if (_ws.State == WebSocketState.Open)
                    await _ws.CloseAsync(WebSocketCloseStatus.NormalClosure, "User disconnected", CancellationToken.None);
            }
            catch { }
            finally
            {
                HandleDisconnect();
                LogSysRow("Disconnected", "User requested disconnect.");
            }
        }

        private void HandleDisconnect()
        {
            _connected = false;
            _ws?.Dispose();
            _ws = null;
            SetProgress(false);
            UpdateButtonStates();
        }

        // ─── DOUBLE-CLICK ROW → show full data in popup ───────────────────────────

        private void dataGridView_Output_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            var row = dataGridView_Output.Rows[e.RowIndex];
            string dir = row.Cells["col_Direction"].Value?.ToString() ?? "";
            string ts = row.Cells["col_Time"].Value?.ToString() ?? "";
            string type = row.Cells["col_Type"].Value?.ToString() ?? "";
            string status = row.Cells["col_Status"].Value?.ToString() ?? "";
            string data = row.Cells["col_Data"].Value?.ToString() ?? "";

            using (var dlg = new Form())
            {
                dlg.Text = $"{dir}  [{ts}]  {type}  —  {status}";
                dlg.Size = new Size(680, 460);
                dlg.StartPosition = FormStartPosition.CenterParent;
                dlg.BackColor = Color.FromArgb(15, 23, 42);

                var rtb = new RichTextBox
                {
                    Dock = DockStyle.Fill,
                    ReadOnly = true,
                    BackColor = Color.FromArgb(15, 23, 42),
                    ForeColor = Color.FromArgb(226, 232, 240),
                    Font = new Font("Consolas", 10F),
                    BorderStyle = BorderStyle.None,
                    ScrollBars = RichTextBoxScrollBars.Both,
                    WordWrap = false,
                    Text = data
                };
                dlg.Controls.Add(rtb);
                dlg.ShowDialog(this);
            }
        }

        // ─── BROWSE WORDLIST ─────────────────────────────────────────────────────

        private void button_BrowseFuzzWordlist_Click(object sender, EventArgs e)
        {
            var dlg = new OpenFileDialog { Filter = "Text Files|*.txt|All Files|*.*", Title = "Select Fuzz Wordlist" };
            if (dlg.ShowDialog() == DialogResult.OK)
                textBox_FuzzWordlist.Text = dlg.FileName;
        }

        // ─── SAVE / CLEAR ─────────────────────────────────────────────────────────

        private void button_SaveReport_Click(object sender, EventArgs e)
        {
            if (dataGridView_Output.Rows.Count == 0) return;

            var dlg = new SaveFileDialog
            {
                Filter = "CSV File|*.csv|Text File|*.txt",
                FileName = $"WebSocket_Report_{DateTime.Now:yyyyMMdd_HHmmss}"
            };
            if (dlg.ShowDialog() != DialogResult.OK) return;

            bool isCsv = dlg.FileName.EndsWith(".csv", StringComparison.OrdinalIgnoreCase);
            var sb = new StringBuilder();

            // Header
            string[] cols = { "#", "Time", "Direction", "Type", "Status", "Size", "Data" };
            if (isCsv)
                sb.AppendLine(string.Join(",", cols.Select(c => $"\"{c}\"")));
            else
                sb.AppendLine(string.Join("\t", cols));

            // Rows
            foreach (DataGridViewRow row in dataGridView_Output.Rows)
            {
                var vals = cols.Select((_, i) =>
                {
                    string v = row.Cells[i].Value?.ToString() ?? "";
                    return isCsv ? $"\"{v.Replace("\"", "\"\"")}\"" : v;
                });
                sb.AppendLine(string.Join(isCsv ? "," : "\t", vals));
            }

            File.WriteAllText(dlg.FileName, sb.ToString(), Encoding.UTF8);
        }

        private void button_ClearOutput_Click(object sender, EventArgs e) => ClearOut();

        // ─── CLEANUP ─────────────────────────────────────────────────────────────

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            _cts?.Cancel();
            _ws?.Dispose();
            base.OnFormClosing(e);
        }

        // ─── UTILITY ─────────────────────────────────────────────────────────────

        private static byte[] ParseHexOrUtf8(string input)
        {
            string clean = input.Replace(" ", "").Replace("-", "");
            if (clean.Length % 2 == 0 && clean.All(c => "0123456789abcdefABCDEF".Contains(c)))
            {
                try
                {
                    var bytes = new byte[clean.Length / 2];
                    for (int i = 0; i < bytes.Length; i++)
                        bytes[i] = Convert.ToByte(clean.Substring(i * 2, 2), 16);
                    return bytes;
                }
                catch { }
            }
            return Encoding.UTF8.GetBytes(input);
        }
    }
}