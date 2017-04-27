using Microsoft.Win32;
using System;
using System.Configuration;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace AdaptiveTaskbar
{
    class Program
    {
        // Based on: http://stackoverflow.com/questions/42861156/how-to-change-windows-10-taskbar-icon-size-programmatically
        // SendNotifyMessage function: https://msdn.microsoft.com/es-es/library/windows/desktop/ms644953(v=vs.85).aspx
        // Windows data types: https://msdn.microsoft.com/en-us/library/windows/desktop/aa383751(v=vs.85).aspx

        private static int BIG_TASKBAR_RES = 1920; // Also set in App.config, where it can be modified by the user

        private const int NULL = 0;
        private const int HWND_BROADCAST = 0xffff;
        private const int WM_SETTINGCHANGE = 0x001a;

        [DllImport("User32.dll")]
        static extern bool SendNotifyMessage(IntPtr hWnd, uint Msg, UIntPtr wParam, string lParam);

        private const string REG_KEY_NAME = @"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced";
        private const string REG_VALUE_NAME = "TaskbarSmallIcons";

        private static void Main(string[] args)
        {
            // Read custom BIG_TASKBAR_RES setting from App.config
            try
            {
                BIG_TASKBAR_RES = Int32.Parse(ConfigurationManager.AppSettings["BIG_TASKBAR_RES"]);
            }
            catch (Exception)
            {
            }

            SystemEvents.DisplaySettingsChanged += SystemEvents_DisplaySettingsChanged; // We are never detaching the event handler
            SystemEvents_DisplaySettingsChanged(null, null);
            //ToggleTaskbarSize(); // For testing

            Application.Run(); // Blocking. This prevents the application from closing.
        }

        private static void SystemEvents_DisplaySettingsChanged(object sender, EventArgs e)
        {
            try
            {
                bool small = IsTaskbarSmall(); // Can throw exceptions, abort in that case
                int screenWidth = Screen.PrimaryScreen.Bounds.Width;
                // This resolution takes into account Windows' DPI setting, so even if a small 13 inch screen's native resolution is 1920,
                // if the DPI setting is set for example to 150% (which is a common thing), it returns 1280

                if (screenWidth < BIG_TASKBAR_RES && !small) // Update taskbar size to small if necessary
                {
                    UpdateTaskbarSize(true);
                }
                else if (screenWidth >= BIG_TASKBAR_RES && small) // Update taskbar size to big if necessary
                {
                    UpdateTaskbarSize(false);
                }
            }
            catch (Exception)
            {
            }
        }

        private static void ToggleTaskbarSize()
        {
            try
            {
                bool small = IsTaskbarSmall();
                UpdateTaskbarSize(!small);
            }
            catch (Exception)
            {
            }
        }

        private static bool IsTaskbarSmall() // Can throw exceptions, do not attemp to updateTaskbarSize() in that case
        {
            try
            {
                object regValue = Registry.GetValue(REG_KEY_NAME, REG_VALUE_NAME, 0);
                if (regValue != null && regValue.GetType() == typeof(int))
                {
                    return (int)regValue != 0;
                }
                else
                {
                    throw new Exception("Registry key not found or unexpected value type");
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        private static void UpdateTaskbarSize(bool small) // Does not throw exceptions
        {
            try
            {
                Registry.SetValue(REG_KEY_NAME, REG_VALUE_NAME, small ? 1 : 0);
                SendNotifyMessage((IntPtr)HWND_BROADCAST, WM_SETTINGCHANGE, (UIntPtr)NULL, "TraySettings");
            }
            catch (Exception)
            {
            }
        }

    }

}
