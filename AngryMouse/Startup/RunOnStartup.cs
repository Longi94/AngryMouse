using Microsoft.Win32;

namespace AngryMouse.Startup
{
    /// <summary>
    /// This class defines if app would run on Windows startup.
    /// </summary>
    public class RunOnStartup
    {
        static RegistryKey registryKey = Registry.CurrentUser.OpenSubKey(
                                @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true);

        /// <summary>
        /// Returns whether app would be run on Windows startup.
        /// </summary>
        public static bool isRunOnStartup()
        {
            return registryKey.GetValue("AngryMouse") != null;
        }

        /// <summary>
        /// Set app to run or not on Windows startup.
        /// </summary>
        public static void setRunOnStartup(bool enable)
        {
            if (enable)
            {
                if (!isRunOnStartup())
                {
                    registryKey.SetValue("AngryMouse", System.Windows.Forms.Application.ExecutablePath.ToString());
                }
            }
            else
            {
                if (isRunOnStartup())
                {
                    registryKey.DeleteValue("AngryMouse", false);
                }
            }
        }

    }
}