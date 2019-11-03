using AngryMouse.Mouse;
using AngryMouse.Screen;
using AngryMouse.Util;
using AngryMouse.Startup;
using CommandLine;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Forms;

namespace AngryMouse
{
    /// <summary>
    /// Main app
    /// </summary>
    public partial class App
    {
        /// <summary>
        /// Debug mode
        /// </summary>
        private bool _debug;

        /// <summary>
        /// Notification icon in the task bar.
        /// </summary>
        private NotifyIcon _notifyIcon;

        /// <summary>
        /// The thing that detects shakes.
        /// </summary>
        private MouseShakeDetector _detector;

        /// <summary>
        /// The list of screens.
        /// </summary>
        private List<ScreenInfo> _screenInfos;

        /// <summary>
        /// The list of overlay windows that draw the big mouse.
        /// </summary>
        private readonly List<OverlayWindow> _overlayWindows = new List<OverlayWindow>();

        private SettingsWindow _settingsWindow;

        private AboutWindow _aboutWindow;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            ParserResult<Options> parserResult = Parser.Default.ParseArguments<Options>(e.Args);

            parserResult.WithParsed((options) => { _debug = options.Debug; });

            _notifyIcon = new NotifyIcon
            {
                Visible = true,
                Icon = AngryMouse.Properties.Resources.icon,
                Text = "AngryMouse"
            };

            CreateContextMenu();

            _detector = new MouseShakeDetector();
            _detector.MouseShake += OnMouseShake;

            _screenInfos = GetScreenInfos();

            if (_debug)
            {
                // Debug window. Only shown when the -d option is used.
                var debugInfoWindow = new DebugInfoWindow(_detector, _screenInfos);
                debugInfoWindow.Show();
            }

            // Create and load windows on the secondary screens.
            foreach (var screen in _screenInfos)
            {
                var window = new OverlayWindow(screen, _debug);
                window.Show();

                _overlayWindows.Add(window);
            }
        }

        /// <summary>
        /// Create the context menu for the notification icon.
        /// </summary>
        private void CreateContextMenu()
        {
            ContextMenuStrip menu = new ContextMenuStrip();

            menu.Items.Add("Settings").Click += (s, e) =>
            {
                if (_settingsWindow != null)
                {
                    _settingsWindow.Focus();
                }
                else
                {
                    _settingsWindow = new SettingsWindow();
                    _settingsWindow.Show();
                    _settingsWindow.Closed += (sender, args) => { _settingsWindow = null; };
                }
            };
            menu.Items.Add("About").Click += (s, e) =>
            {
                if (_aboutWindow != null)
                {
                    _aboutWindow.Focus();
                }
                else
                {
                    _aboutWindow = new AboutWindow();
                    _aboutWindow.Show();
                    _aboutWindow.Closed += (sender, args) => { _aboutWindow = null; };
                }
            };
            
            menu.Items.Add("Run at Windows startup").Click += (s, e) =>
            {
                ToolStripMenuItem runStartup = (ToolStripMenuItem)s;
                runStartup.Checked = !runStartup.Checked;
                if (runStartup.Checked)
                {
                    RunOnStartup.setRunOnStartup(true);
                }
                else
                {
                    RunOnStartup.setRunOnStartup(false);
                }
            };
            foreach(ToolStripItem item in menu.Items)
            {
                if (item.Text.Equals("Run when Windows Startup")) 
                {
                    ((ToolStripMenuItem)item).Checked = RunOnStartup.isRunOnStartup();
                }
            }
            
            menu.Items.Add(new ToolStripSeparator());
            
            menu.Items.Add("Exit").Click += (s, e) => ExitApp();

            _notifyIcon.ContextMenuStrip = menu;
        }

        /// <summary>
        /// Close the windows and remove the notification icon.
        /// </summary>
        private void ExitApp()
        {
            _detector.Dispose();
            Current.Shutdown();
        }

        /// <summary>
        /// Called when mouse shake is detected or when shaking is stopped.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnMouseShake(object sender, MouseShakeArgs e)
        {
            _overlayWindows.ForEach(window => window.SetMouseShake(e.IsShaking, e.Timestamp));
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
                    Bpp = screen.BitsPerPixel
                });
            }

            return screenInfos;
        }
    }
}
