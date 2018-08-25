using AngryMouse.Mouse;
using AngryMouse.Util;
using CommandLine;
using System.Windows;
using System.Windows.Forms;

namespace AngryMouse
{
    /// <summary>
    /// Main app
    /// </summary>
    public partial class App : System.Windows.Application
    {

        /// <summary>
        /// Notification icon in the taskbar.
        /// </summary>
        private NotifyIcon notifyIcon;
            
        /// <summary>
        /// The thing that detects shakes.
        /// </summary>
        private MouseShakeDetector detector;

        /// <summary>
        /// The overlay window.
        /// </summary>
        private OverlayWindow overlayWindow;

        /// <summary>
        /// Debug window. Only shown when the -d option is used.
        /// </summary>
        private DebugInfoWindow debugInfoWindow;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            bool debug = false;
            ParserResult<Options> parserResult = Parser.Default.ParseArguments<Options>(e.Args);

            parserResult.WithParsed((options) => {
                debug = options.Debug;
            });

            notifyIcon = new NotifyIcon
            {
                Visible = true,
                Icon = AngryMouse.Properties.Resources.Cursor
            };

            CreateContextMenu();

            detector = new MouseShakeDetector();
            detector.MouseShake += OnMouseShake;

            if (debug)
            {
                debugInfoWindow = new DebugInfoWindow(detector);
                debugInfoWindow.Show();
            }

            overlayWindow = new OverlayWindow();
            overlayWindow.Show();
        }

        /// <summary>
        /// Create the context menu for the notification icon.
        /// </summary>
        private void CreateContextMenu()
        {
            ContextMenuStrip menu = new ContextMenuStrip();

            menu.Items.Add("Exit").Click += (s, e) => ExitApp();

            notifyIcon.ContextMenuStrip = menu;
        }

        /// <summary>
        /// Close the windows and remove the notification icon.
        /// </summary>
        private void ExitApp()
        {
            overlayWindow?.Close();
            debugInfoWindow?.Close();
            notifyIcon.Dispose();
            notifyIcon = null;
        }

        /// <summary>
        /// Called when mouse shake is detected or when shaking is stopped.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnMouseShake(object sender, MouseShakeArgs e)
        {
            overlayWindow.SetMouseShake(e.IsShaking, e.Timestamp);
        }
    }
}
