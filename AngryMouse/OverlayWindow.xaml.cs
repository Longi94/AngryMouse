using AngryMouse.Animation;
using AngryMouse.Screen;
using AngryMouse.Util;
using Gma.System.MouseKeyHook;
using System;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using static AngryMouse.Util.WindowUtil;

namespace AngryMouse
{
    /// <summary>
    /// Interaction logic for OverlayWindow.xaml
    /// </summary>
    public partial class OverlayWindow : Window
    {
        /// <summary>
        /// Show debug info
        /// </summary>
        private bool debug;

        /// <summary>
        /// We also subscribe to mouse move events so we know where to draw.
        /// </summary>
        private IKeyboardMouseEvents mouseEvents;

        /// <summary>
        /// Moves the cursor around the canvas.
        /// </summary>
        private TranslateTransform cursorTranslate = new TranslateTransform();

        /// <summary>
        /// Scales the cursor.
        /// </summary>
        private ScaleTransform cursorScale = new ScaleTransform {
            ScaleX = 0,
            ScaleY = 0
        };

        /// <summary>
        /// The screen this window is open in.
        /// </summary>
        private ScreenInfo screen;

        /// <summary>
        /// Whether to show the big boi or not.
        /// </summary>
        private bool shaking = false;

        /// <summary>
        /// DPI scale info of the current screen.
        /// </summary>
        private DpiScale dpiInfo;

        private MouseAnimator _mouseAnimator;

        /// <summary>
        /// Main constructor.
        /// </summary>
        /// <param name="screen">The window to show the screen in.</param>
        public OverlayWindow(ScreenInfo screen, bool debug = false)
        {
            InitializeComponent();

            this.debug = debug;
            this.screen = screen;

            mouseEvents = Hook.GlobalEvents();
            mouseEvents.MouseMoveExt += OnMouseMove;
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            // Do not capture any mouse events
            // TODO I suspect this is the reason we cannot replace the cursor (hide it) since
            // the cursor draws on top of the big cursor.
            // Also hide window from alt-tab menu
            SetWindowStyles(this, ExtendedWindowStyles.WS_EX_TOOLWINDOW | ExtendedWindowStyles.WS_EX_TRANSPARENT);
        }

        /// <summary>
        /// Called when the window is successfully loaded. Does some view initialization.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (!debug)
            {
                Root.Children.Remove(DebugInfo);
                OverlayCanvas.Children.Remove(MousePosDebug);
            }

            TransformGroup transformGroup = new TransformGroup();

            transformGroup.Children.Add(cursorTranslate);
            transformGroup.Children.Add(cursorScale);

            BigCursor.RenderTransform = transformGroup;

            // Open this window maximized on the appropriate screen
            Top = screen.BoundY;
            Left = screen.BoundX;
            WindowState = WindowState.Maximized;

            OverlayCanvas.Width = screen.BoundWidth;
            OverlayCanvas.Height = screen.BoundHeight;

            dpiInfo = VisualTreeHelper.GetDpi(this);

            Viewbox.Width = screen.BoundWidth / dpiInfo.PixelsPerDip;
            Viewbox.Height = screen.BoundHeight / dpiInfo.PixelsPerDip;

            if (debug)
            {
                MousePosDebug.Width = MousePosDebug.Width * dpiInfo.PixelsPerDip;
                MousePosDebug.Height = MousePosDebug.Height * dpiInfo.PixelsPerDip;
            }

            _mouseAnimator = new MouseAnimator(cursorScale, BigCursor, dpiInfo);
        }

        /// <summary>
        /// Called when the position of the mouse is changed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnMouseMove(object sender, MouseEventExtArgs e)
        {
            var mouseInScreen = e.X >= screen.BoundX && e.X <= screen.BoundX + screen.BoundWidth && 
                                e.Y >= screen.BoundY && e.Y <= screen.BoundY + screen.BoundHeight;
            if (debug)
            {
                var infoBuilder = new StringBuilder();
                infoBuilder
                    .AppendFormat("Name {0}", screen.Name).AppendLine()
                    .AppendFormat("Primary {0}", screen.Primary).AppendLine()
                    .AppendFormat("PixelsPerDip {0}", dpiInfo.PixelsPerDip).AppendLine()
                    .AppendFormat("Mouse {0},{1}", e.X, e.Y).AppendLine()
                    .AppendFormat("InScreen {0}", mouseInScreen).AppendLine()
                    .AppendFormat("Draw {0},{1}", e.X - screen.BoundX, e.Y - screen.BoundY);
                DebugInfo.Content = infoBuilder.ToString();

                Canvas.SetTop(MousePosDebug, e.Y - screen.BoundY);
                Canvas.SetLeft(MousePosDebug, e.X - screen.BoundX);
            }

            _mouseAnimator.MouseInScreen = mouseInScreen;
            if (!mouseInScreen)
            {
                return;
            }

            cursorTranslate.X = e.X - screen.BoundX;
            cursorTranslate.Y = e.Y - screen.BoundY;
            cursorScale.CenterX = e.X - screen.BoundX;
            cursorScale.CenterY = e.Y - screen.BoundY;
        }

        /// <summary>
        /// Causes the big mouse to appear or disappear depending on the parameter and the current state
        /// of the mouse.
        /// </summary>
        /// <param name="shaking">Whether the mouse is shaking or not.</param>
        /// <param name="timestamp">The timestamp the shake change occured at.</param>
        public void SetMouseShake(bool shaking, DateTime timestamp)
        {
            _mouseAnimator.SetMouseShake(shaking, timestamp);
        }
    }
}
