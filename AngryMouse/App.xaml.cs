using AngryMouse.Mouse;
using AngryMouse.Screen;
using AngryMouse.Util;
using CommandLine;
using System.Collections.Generic;
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

            var screenInfos = GetScreenInfos();
            // TODO support multiple screens
            var primaryScreen = screenInfos.Find(info => info.Primary);

            if (debug)
            {
                debugInfoWindow = new DebugInfoWindow(detector, screenInfos);
                debugInfoWindow.Show();
            }

            overlayWindow = new OverlayWindow(primaryScreen);
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

        private List<ScreenInfo> GetScreenInfos()
        {
            List<ScreenInfo> screenInfos = new List<ScreenInfo>();

            foreach (System.Windows.Forms.Screen screen in System.Windows.Forms.Screen.AllScreens)
            {
                screenInfos.Add(new ScreenInfo()
                {
                    Name = screen.DeviceName,
                    BoundX = screen.Bounds.X,
                    BoundY = screen.Bounds.Y,
                    BoundWidth = screen.Bounds.Width,
                    BoundHeight = screen.Bounds.Height,
                    WorkX = screen.WorkingArea.X,
                    WorkY = screen.WorkingArea.Y,
                    WorkWidth = screen.WorkingArea.Width,
                    WorkHeight = screen.WorkingArea.Height,
                    Primary = screen.Primary,
                    BPP = screen.BitsPerPixel
                });
            }

            return screenInfos;
        }
    }
}
