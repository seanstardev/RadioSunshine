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
    }
}
