using Microsoft.Win32;
using Squirrel;
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

        private const string REG_KEY = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\Advanced";
        private const string REG_NAME = "TaskbarSmallIcons";

        private const string REG_STARTUP_KEY = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run";
        private const string REG_STARTUP_NAME = "AdaptiveTaskbar";

        private const string GITHUB_UPDATE_URL = @"https://github.com/cprcrack/AdaptiveTaskbar";

        private const string ON_FIRST_RUN_TITLE = "Installation successful";
        private const string ON_FIRST_RUN_DESCRIPTION = "Adaptive Taskbar has been correctly installed in the background. It will automatically switch between big or small Windows taskbar depending on the current screen size. You can always uninstall it via \"Programs and features\".";

        private static void Main(string[] args)
        {
#if DEBUG
            //SetStartup(true); // For testing
#else
            // Customize Squirrel update events (this also prevents shortcuts from being created as happens in the default implementation)
            using (var updateManager = new UpdateManager(""))
            {
                SquirrelAwareApp.HandleEvents(
                  onInitialInstall: v => SetStartup(false),
                  onAppUpdate: v => SetStartup(false),
                  onAppUninstall: v => SetStartup(true),
                  onFirstRun: () => MessageBox.Show(ON_FIRST_RUN_DESCRIPTION, ON_FIRST_RUN_TITLE));
            }

            UpdateApp();
#endif

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

        private static void SetStartup(bool remove)
        {
            RegistryKey key = Registry.CurrentUser.OpenSubKey(REG_STARTUP_KEY, true);
            if (key != null)
            {
                if (remove)
                {
                    key.DeleteValue(REG_STARTUP_NAME, false);
                }
                else
                {
                    key.SetValue(REG_STARTUP_NAME, Application.ExecutablePath);
                    // This already takes care of overwritting old version paths with the new one, as they will have the same REG_STARTUP_NAME
                }
            }
        }

        // Check for updates in the background
        private async static void UpdateApp()
        {
            try
            {
                using (var updateManager = UpdateManager.GitHubUpdateManager(GITHUB_UPDATE_URL))
                {
                    await updateManager.Result.UpdateApp();
                }
            }
            catch (Exception)
            {
                // The GitHub update process is untested. At the time of testing exceptions are thrown,
                // maybe because there are no Releases setup on GitHub?
            }
        }

        private static void SystemEvents_DisplaySettingsChanged(object sender, EventArgs e)
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

        private static void ToggleTaskbarSize()
        {
            bool small = IsTaskbarSmall();
            UpdateTaskbarSize(!small);
        }

        private static bool IsTaskbarSmall()
        {
            try
            {
                object value = null;
                using (RegistryKey key = Registry.CurrentUser.OpenSubKey(REG_KEY, true))
                {
                    if (key != null)
                    {
                        value = key.GetValue(REG_NAME, 0);
                    }
                }
                if (value != null && value.GetType() == typeof(int))
                {
                    return (int)value != 0;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        private static void UpdateTaskbarSize(bool small)
        {
            try
            {
                using (RegistryKey key = Registry.CurrentUser.OpenSubKey(REG_KEY, true))
                {
                    if (key != null)
                    {
                        key.SetValue(REG_NAME, small ? 1 : 0);
                        SendNotifyMessage((IntPtr)HWND_BROADCAST, WM_SETTINGCHANGE, (UIntPtr)NULL, "TraySettings");
                    }
                }
            }
            catch (Exception)
            {
            }
        }

    }

}
