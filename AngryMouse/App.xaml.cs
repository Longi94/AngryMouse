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
        /// Debug window. Only shown when the -d option is used.
        /// </summary>
        private DebugInfoWindow debugInfoWindow;

        /// <summary>
        /// The list of screens.
        /// </summary>
        private List<ScreenInfo> screenInfos;

        /// <summary>
        /// The list of overlay windows that draw the big mouse.
        /// </summary>
        private List<OverlayWindow> overlayWindows = new List<OverlayWindow>();

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

            screenInfos = GetScreenInfos();
            // TODO support multiple screens
            var primaryScreen = screenInfos.Find(info => info.Primary);

            if (debug)
            {
                debugInfoWindow = new DebugInfoWindow(detector, GetScreenInfos());
                debugInfoWindow.Show();
            }

            var primary = new OverlayWindow(primaryScreen);
            primary.OnLoad += OnPrimaryLoad;
            primary.Show();

            overlayWindows.Add(primary);
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
            overlayWindows.ForEach(window => window.Close());
            debugInfoWindow?.Close();
            notifyIcon.Dispose();
            notifyIcon = null;
        }

        /// <summary>
        /// Called when the primary overlay window is fully loaded.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnPrimaryLoad(object sender, WindowLoadedEventArgs e)
        {
            // Create and load windows on the secondary screens.
            foreach (var screen in screenInfos.FindAll(info => !info.Primary))
            {
                screen.BoundHeight = (int) (screen.BoundHeight / e.DpiHeightFactor);
                screen.BoundWidth = (int) (screen.BoundWidth / e.DpiWidthFactor);
                screen.BoundX = (int) (screen.BoundX / e.DpiWidthFactor);
                screen.BoundY = (int) (screen.BoundY / e.DpiHeightFactor);

                var window = new OverlayWindow(screen);
                window.Show();

                overlayWindows.Add(window);
            }
        }

        /// <summary>
        /// Called when mouse shake is detected or when shaking is stopped.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnMouseShake(object sender, MouseShakeArgs e)
        {
            overlayWindows.ForEach(window => window.SetMouseShake(e.IsShaking, e.Timestamp));
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
