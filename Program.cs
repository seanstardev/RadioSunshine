using System.Diagnostics;

namespace RadioSunshine {
    internal static class Program {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args) {
            Process currentProcess = Process.GetCurrentProcess();
            var runningProcesses = Process.GetProcessesByName(currentProcess.ProcessName);
            if (runningProcesses.Any(p => p.Id != currentProcess.Id)) {
                return;
            }

            ApplicationConfiguration.Initialize();

            
            if (args.Length < 1)
                RadioSunshineForm.AdminMode = true;

            var f = new RadioSunshineForm();

            if (!RadioSunshineForm.AdminMode)
                f.HandleMoonlightConnectStart(args);

            Application.Run(f);
        }
    }
}