namespace RadioSunshine {
    using System;
    using System.Diagnostics;
    using System.Linq;
    using System.Windows.Forms;
    using Timer = System.Threading.Timer;
    using System.Net.NetworkInformation;
    using System.Management;
    using System.Reflection;

    public partial class RadioSunshineForm : Form {
        private const bool SHOW_NOTIFICATIONS = false;
        private static readonly int CHECK_INTERVAL_SECONDS = 2;
        private static int RESET_SCREEN_TIME_SECONDS = 3;
        private static int AWAIT_SUNSHINE_TIME_SECONDS = 10;

        private static readonly int[] PORTS_TO_CHECK = { 48010, 47998, 47999, 47800 };

        private NotifyIcon trayIcon;
        private ScreenConfig? originalConfig;
        private ScreenConfig? newConfig;
        private Timer streamTimer;
        private Timer sunshineTimer;

        private bool inOriginalResolution = true;
        private int noStreamDurationSeconds = 0;
        private int noSunshineDurationSeconds = 0;

        private bool sunshineHasConnectedAtLeastOnce = false;

        private SunshineClientDto? clientDto = null;
        private bool disposed = false;
        private string? executablePath = null;
        private static readonly string SUNSHINE_PROCESS_NAME = "sunshine"; 
        private string? selectedFilePath = null;

        public static bool AdminMode { get; internal set; }
        public RadioSunshineForm() {
            InitializeComponent();

            originalConfig = GetDefaultConfig();
            executablePath = Process.GetCurrentProcess().MainModule.FileName;
            if (AdminMode)
                return;
            //Debug.WriteLine("Executable Path: " + executablePath);


            ContextMenuStrip contextMenuStrip = new ContextMenuStrip();

            ToolStripMenuItem titleItem = new ToolStripMenuItem("Radio Sunshine");
            titleItem.Enabled = false;
            contextMenuStrip.Items.Add(titleItem);

            ToolStripMenuItem exitItem = new ToolStripMenuItem("Exit");

            exitItem.Click += OnExit;

            contextMenuStrip.Items.Add(exitItem);

            trayIcon = new NotifyIcon {
                Icon = this.Icon,
                Visible = true,
                Text = "Radio Sunshine"
            };
            trayIcon.ContextMenuStrip = contextMenuStrip;
            sunshineTimer = new Timer(OnCheckSunshineStatus, null, 0, CHECK_INTERVAL_SECONDS * 1000);

            //ChangeScreenConfigUtil.Test();
        }


        internal void HandleMoonlightConnectStart(string[] args) {
            string running = "ARGS" + Environment.NewLine;
            foreach (var item in args) {
                running += item + Environment.NewLine;
            }
            newConfig = null;
            if (args.Length >= 10) {
                clientDto = new SunshineClientDto {
                    AppId = args[0],
                    AppName = args[1],
                    ClientWidth = int.TryParse(args[2], out int width) ? width : 0,
                    ClientHeight = int.TryParse(args[3], out int height) ? height : 0,
                    ClientFPS = int.TryParse(args[4], out int fps) ? fps : 0,
                    ClientHDR = bool.TryParse(args[5], out bool hdr) && hdr,
                    ClientGCMAP = int.TryParse(args[6], out int gcmap) ? gcmap : 0,
                    ClientHostAudio = bool.TryParse(args[7], out bool hostAudio) && hostAudio,
                    ClientEnableSOPS = bool.TryParse(args[8], out bool enableSOPS) && enableSOPS,
                    ClientAudioConfiguration = args[9]
                };
                newConfig = new ScreenConfig(clientDto.ClientWidth, clientDto.ClientHeight, clientDto.ClientFPS);

                ShowNotification("The service has started.");
                streamTimer = new Timer(OnCheckStreamingStatus, null, 0, CHECK_INTERVAL_SECONDS * 1000);

            }

        }

        private void KillTimers() {
            if (streamTimer != null) streamTimer.Dispose();
            if (sunshineTimer != null) sunshineTimer.Dispose();
            streamTimer = null;
        }
        private void OnExit(object? sender, EventArgs e) {
            if (!inOriginalResolution)
                if (ChangeScreenConfig(originalConfig))
                    inOriginalResolution = true;

            Application.Exit();
            Environment.Exit(0);
        }
        private void OnCheckSunshineStatus(Object state) {
            Process? process = GetSunshineProcess();

            if (process is null) {
                noSunshineDurationSeconds += CHECK_INTERVAL_SECONDS;
                if (sunshineHasConnectedAtLeastOnce || noSunshineDurationSeconds > AWAIT_SUNSHINE_TIME_SECONDS) {
                    KillTimers();
                    ShowExitNotification();
                    OnExit(null, EventArgs.Empty);
                }
            }
            else {
                noStreamDurationSeconds = 0;
                sunshineHasConnectedAtLeastOnce = true;
            }
        }
        private void OnCheckStreamingStatus(Object state) {

            var udpEndpoints = IPGlobalProperties.GetIPGlobalProperties().GetActiveUdpListeners();
            bool isStreaming = udpEndpoints.Any(ep => PORTS_TO_CHECK.Contains(ep.Port));

            if (isStreaming) {
                noStreamDurationSeconds = 0;
                if (inOriginalResolution) {
                    if (ChangeScreenConfig(newConfig))
                        inOriginalResolution = false;
                }
            }
            else {
                //noStreamDurationSeconds += CHECK_INTERVAL_SECONDS;
                //if (noStreamDurationSeconds >= RESET_SCREEN_TIME_SECONDS) {
                if (!inOriginalResolution) {
                    if (ChangeScreenConfig(originalConfig))
                        inOriginalResolution = true;
                }
                noStreamDurationSeconds = 0;
                //}
            }
        }

        private bool ChangeScreenConfig(ScreenConfig config) {
            string? error;
            bool ok = ChangeScreenConfigUtil.ChangeScreenConfig(config, out error);

            string result = ok ? "OK: " : "ERROR: " + error + ": ";


            result += config.Width + " X " + config.Height + " : " + config.RefreshRate;
            ShowNotification(result);

            return ok;
        }
        private Process? GetSunshineProcess() {
            return Process.GetProcessesByName(SUNSHINE_PROCESS_NAME).FirstOrDefault();
        }
        private bool GetSunshineProcessRunning() {
            return GetSunshineProcess() != null;
        }
        private void ShowExitNotification() {
            ShowNotification("The service has exited.");
        }
        private void ShowNotification(string message) {
            if (!SHOW_NOTIFICATIONS) return;
            trayIcon.BalloonTipTitle = "Radio Sunshine";
            trayIcon.BalloonTipText = message;
            trayIcon.BalloonTipIcon = ToolTipIcon.Info;
            trayIcon.ShowBalloonTip(3000);
        }

        public ScreenConfig GetDefaultConfig() {
            int screenWidth = Screen.PrimaryScreen.Bounds.Width;
            int screenHeight = Screen.PrimaryScreen.Bounds.Height;
            int refreshRate = ChangeScreenConfigUtil.GetRefreshRate();// Screen.PrimaryScreen.RefreshRate;

            return new ScreenConfig(screenWidth, screenHeight, refreshRate);
        }


        private void changeBtn_Click(object sender, EventArgs e) {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Configuration Files (*.conf)|*.conf"; // Set the filter for .conf files
            openFileDialog.Title = "Select the Sunshine Config File";

            if (openFileDialog.ShowDialog() == DialogResult.OK) {
                selectedFilePath = openFileDialog.FileName;
                pathText.Text = selectedFilePath;
                applyBtn.Enabled = true;
            }
        }

        private void applyBtn_Click(object sender, EventArgs e) {
            if (selectedFilePath == null || executablePath == null)
                return;

            List<string> lines = File.ReadAllLines(selectedFilePath).ToList();
            int line = -1;
            for (int i = 0; i < lines.Count; i++) {
                //Debug.WriteLine(lines[i]);

                if (lines[i].Trim().ToLower().StartsWith("global_prep_cmd")) {
                    line = i; break;
                }
            }
            if (line < 0) {
                lines.Add(
                    SunshineCfgHelper.CreatePrepCmd(executablePath)
                );
            }
            else lines[line] = SunshineCfgHelper.CreatePrepCmd(executablePath);

            try {
                File.WriteAllLines(selectedFilePath, lines);
                //Debug.WriteLine("File saved successfully.");
                MessageBox.Show("The update wass successful. Please close this application and start / restart the Sunshine service.", "OK", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (IOException ex) {
                ShowErrorDialog("An IO exception occurred: " + ex.Message);
            }
            catch (UnauthorizedAccessException ex) {
                ShowErrorDialog("Unauthorized to write to the file: " + ex.Message);
            }

        }
        private void ShowErrorDialog(string message) {
            MessageBox.Show(message, "There was a problem", MessageBoxButtons.OK, MessageBoxIcon.Warning);

        }
        //protected override void Dispose(bool disposing) {
        //    if (!disposed) {
        //        if (disposing) {
        //            if (streamTimer != null)
        //                streamTimer.Dispose();
        //            if (sunshineTimer != null)
        //                sunshineTimer.Dispose();
        //            if (trayIcon != null)
        //                trayIcon.Dispose();
        //        }

        //        disposed = true;
        //    }

        //    base.Dispose(disposing);
        //}
    }
    public class SunshineClientDto {
        public string AppId { get; set; } = string.Empty;
        public string AppName { get; set; } = string.Empty;
        public int ClientWidth { get; set; }
        public int ClientHeight { get; set; }
        public int ClientFPS { get; set; }
        public bool ClientHDR { get; set; }
        public int ClientGCMAP { get; set; }
        public bool ClientHostAudio { get; set; }
        public bool ClientEnableSOPS { get; set; }
        public string ClientAudioConfiguration { get; set; } = string.Empty;
    }
    public class ScreenConfig {
        public int Width { get; set; }
        public int Height { get; set; }
        public int RefreshRate { get; set; }
        public ScreenConfig(int width, int height, int refreshRate) {
            Width = width;
            Height = height;
            RefreshRate = refreshRate;
        }
    }
}
