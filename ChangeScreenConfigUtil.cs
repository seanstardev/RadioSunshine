using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace RadioSunshine {
    internal static class ChangeScreenConfigUtil {




        //[StructLayout(LayoutKind.Sequential)]
        //public struct DEVMODE {
        //    Define the DEVMODE structure with necessary fields
        //    ...

        //    public int dmDisplayFrequency;
        //    Add other fields as needed
        //}



        public static int GetRefreshRate() {
            DEVMODE devMode = new DEVMODE();
            devMode.dmSize = (short)Marshal.SizeOf(typeof(DEVMODE));

            if (EnumDisplaySettings(null, ENUM_CURRENT_SETTINGS, ref devMode)) {
                return devMode.dmDisplayFrequency;
            }

            return 0; // Default value if unable to get refresh rate
        }







        [DllImport("user32.dll")]
        public static extern bool EnumDisplaySettings(string deviceName, int modeNum, ref DEVMODE devMode);

        [DllImport("user32.dll")]
        public static extern int ChangeDisplaySettings(ref DEVMODE devMode, int flags);

        private const int ENUM_CURRENT_SETTINGS = -1;
        private const int CDS_UPDATEREGISTRY = 0x01;
        private const int DISP_CHANGE_SUCCESSFUL = 0;

        [StructLayout(LayoutKind.Sequential)]
        public struct DEVMODE {
            private const int CCHDEVICENAME = 32;
            private const int CCHFORMNAME = 32;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = CCHDEVICENAME)]
            public string dmDeviceName;
            public short dmSpecVersion;
            public short dmDriverVersion;
            public short dmSize;
            public short dmDriverExtra;
            public int dmFields;
            public int dmPositionX;
            public int dmPositionY;
            public int dmDisplayOrientation;
            public int dmDisplayFixedOutput;
            public short dmColor;
            public short dmDuplex;
            public short dmYResolution;
            public short dmTTOption;
            public short dmCollate;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = CCHFORMNAME)]
            public string dmFormName;
            public short dmLogPixels;
            public int dmBitsPerPel;
            public int dmPelsWidth;
            public int dmPelsHeight;
            public int dmDisplayFlags;
            public int dmDisplayFrequency;
            public int dmICMMethod;
            public int dmICMIntent;
            public int dmMediaType;
            public int dmDitherType;
            public int dmReserved1;
            public int dmReserved2;
            public int dmPanningWidth;
            public int dmPanningHeight;
        }

        //public static bool ChangeScreenConfig(ScreenConfig config, out string? error) {
        //    error = null;
        //    DEVMODE vDevMode = new DEVMODE();
        //    vDevMode.dmDeviceName = new string(new char[32]);
        //    vDevMode.dmFormName = new string(new char[32]);
        //    vDevMode.dmSize = (short)Marshal.SizeOf(vDevMode);
        //    if (EnumDisplaySettings(null, ENUM_CURRENT_SETTINGS, ref vDevMode)) { 

        //        //if (0 != EnumDisplaySettings(null, ENUM_CURRENT_SETTINGS, ref vDevMode)) {
        //        vDevMode.dmPelsWidth = config.Width;
        //        vDevMode.dmPelsHeight = config.Height;
        //        //vDevMode.dmDisplayFrequency = config.RefreshRate;

        //        // Change the screen settings
        //        int iRet = ChangeDisplaySettings(ref vDevMode, CDS_UPDATEREGISTRY);

        //        if (iRet == DISP_CHANGE_SUCCESSFUL) {
        //            //ShowNotification("Screen resolution changed successfully.");
        //        }
        //        else {
        //            error = iRet.ToString();
        //           return false;
        //        }
        //    }
        //    else {
        //        error = "Z";
        //        return false;
        //    }
        //    return true;
        //}
        public const int DM_PELSWIDTH = 0x80000;
        public const int DM_PELSHEIGHT = 0x100000;
        public const int DM_DISPLAYFREQUENCY = 0x400000;
        public static bool ChangeScreenConfig(ScreenConfig config, out string? error) {
            error = null;
            DEVMODE vDevMode = new DEVMODE {
                dmDeviceName = new string(new char[32]),
                dmFormName = new string(new char[32]),
                dmSize = (short)Marshal.SizeOf(typeof(DEVMODE))
            };

            if (EnumDisplaySettings(null, ENUM_CURRENT_SETTINGS, ref vDevMode)) {
                // Reset dmFields and then set the necessary flags
                vDevMode.dmFields = 0;
                vDevMode.dmFields = DM_PELSWIDTH | DM_PELSHEIGHT | DM_DISPLAYFREQUENCY;

                // Set the screen resolution and refresh rate
                vDevMode.dmPelsWidth = config.Width;
                vDevMode.dmPelsHeight = config.Height;
                vDevMode.dmDisplayFrequency = config.RefreshRate;

                int iRet = ChangeDisplaySettings(ref vDevMode, 0); // Using 0 as the flag for testing
                //int iRet = ChangeDisplaySettings(ref vDevMode, CDS_UPDATEREGISTRY);

                if (iRet == DISP_CHANGE_SUCCESSFUL) {
                    return true;
                }
                else {
                    error = "Failed to change screen resolution. Error code: " + iRet;
                    return false;
                }
            }
            else {
                error = "Failed to get display settings.";
                return false;
            }
        }
        public static void Test() {
            DEVMODE devMode = new DEVMODE();
            devMode.dmSize = (short)Marshal.SizeOf(typeof(DEVMODE));
            int modeNum = 0;

            while (EnumDisplaySettings(null, modeNum++, ref devMode)) {
                Debug.WriteLine($"Mode {modeNum}: {devMode.dmPelsWidth}x{devMode.dmPelsHeight}, {devMode.dmDisplayFrequency}Hz");
            }
        }
    }
}
