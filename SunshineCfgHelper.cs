using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RadioSunshine {
    internal static class SunshineCfgHelper {
        static string p1 = @"global_prep_cmd = [{""do"": ""powershell.exe -WindowStyle Hidden -Command \""$appID = $env:SUNSHINE_APP_ID; $appName = $env:SUNSHINE_APP_NAME; $clientWidth = $env:SUNSHINE_CLIENT_WIDTH; $clientHeight = $env:SUNSHINE_CLIENT_HEIGHT; $clientFPS = $env:SUNSHINE_CLIENT_FPS; $clientHDR = $env:SUNSHINE_CLIENT_HDR; $clientGCMAP = $env:SUNSHINE_CLIENT_GCMAP; $clientHostAudio = $env:SUNSHINE_CLIENT_HOST_AUDIO; $clientEnableSOPS = $env:SUNSHINE_CLIENT_ENABLE_SOPS; $clientAudioConfig = $env:SUNSHINE_CLIENT_AUDIO_CONFIGURATION; Start-Process '";
        static string p2 = @"' -WindowStyle Hidden -ArgumentList @($appID, $appName, $clientWidth, $clientHeight, $clientFPS, $clientHDR, $clientGCMAP, $clientHostAudio, $clientEnableSOPS, $clientAudioConfig)\"""",""elevated"":""true""}]";
        public static string CreatePrepCmd(string radioSunshineExeFullpath) {
            radioSunshineExeFullpath = radioSunshineExeFullpath.Replace(@"\", @"\\");
            string result = p1 + radioSunshineExeFullpath + p2;

            return result;
        }

        public static bool RemovePrepCmd(string cfgFullpath) {
            List<string> lines = File.ReadAllLines(cfgFullpath).ToList();
            int line = -1;
            for (int i = 0; i < lines.Count; i++) {
                if (lines[i].Trim().ToLower().StartsWith("global_prep_cmd")) {
                    line = i; break;
                }
            }
            if (line > -1) {
                try {
                    lines[line] = "global_prep_cmd = []";
                    File.WriteAllLines(cfgFullpath, lines);
                    MessageBox.Show("The removal was successful. Please close this application and start / restart the Sunshine service.", "OK", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (IOException ex) {
                    RadioSunshineForm.ShowErrorDialog("An IO exception occurred: " + ex.Message);
                    return false;
                }
                catch (UnauthorizedAccessException ex) {
                    RadioSunshineForm.ShowErrorDialog("Unauthorized to write to the file: " + ex.Message);
                    return false;
                }
                return true;
            }
            RadioSunshineForm.ShowErrorDialog("No changes were made as the 'global_prep_cmd' instruction could not be found.");
            return false;
            
        }
    }
}
