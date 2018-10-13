using AngryMouse.Animation;
using AngryMouse.Screen;
using Gma.System.MouseKeyHook;
using System;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using AngryMouse.Mouse;
using static AngryMouse.Util.WindowUtil;

namespace AngryMouse
{
    /// <summary>
    /// Interaction logic for OverlayWindow.xaml
    /// </summary>
    public partial class OverlayWindow
    {
        /// <summary>
        /// Show debug info
        /// </summary>
        private readonly bool _debug;

        /// <summary>
        /// Moves the cursor around the canvas.
        /// </summary>
        private readonly TranslateTransform _cursorTranslate = new TranslateTransform();

        /// <summary>
        /// Scales the cursor.
        /// </summary>
        private readonly ScaleTransform _cursorScale = new ScaleTransform
        {
            ScaleX = 0,
            ScaleY = 0
        };

        /// <summary>
        /// The screen this window is open in.
        /// </summary>
        private readonly ScreenInfo _screen;

        /// <summary>
        /// DPI scale info of the current screen.
        /// </summary>
        private DpiScale _dpiInfo;

        /// <summary>
        /// Animates mouse growing and shrinking
        /// </summary>
        private MouseAnimator _mouseAnimator;

        /// <summary>
        /// We also subscribe to mouse move events so we know where to draw.
        /// </summary>
        private readonly IKeyboardMouseEvents _mouseEvents;

        /// <summary>
        /// Main constructor.
        /// </summary>
        /// <param name="screen">The window to show the screen in.</param>
        /// <param name="debug">Show debug information on screens</param>
        public OverlayWindow(ScreenInfo screen, bool debug = false)
        {
            InitializeComponent();

            _debug = debug;
            _screen = screen;

            _mouseEvents = StaticHook.GlobalEvents();
            _mouseEvents.MouseMoveExt += OnMouseMove;
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

        protected override void OnDpiChanged(DpiScale oldDpiScaleInfo, DpiScale newDpiScaleInfo)
        {
            _dpiInfo = newDpiScaleInfo;
            if (_mouseAnimator != null)
            {
                _mouseAnimator.DpiInfo = newDpiScaleInfo;
            }
        }

        /// <summary>
        /// Called when the window is successfully loaded. Does some view initialization.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (!_debug)
            {
                Root.Children.Remove(DebugInfo);
                OverlayCanvas.Children.Remove(MousePosDebug);
            }

            TransformGroup transformGroup = new TransformGroup();

            transformGroup.Children.Add(_cursorTranslate);
            transformGroup.Children.Add(_cursorScale);

            BigCursor.RenderTransform = transformGroup;

            // Open this window maximized on the appropriate screen
            Top = _screen.BoundY;
            Left = _screen.BoundX;
            WindowState = WindowState.Maximized;

            OverlayCanvas.Width = _screen.BoundWidth;
            OverlayCanvas.Height = _screen.BoundHeight;

            _dpiInfo = VisualTreeHelper.GetDpi(this);

            Viewbox.Width = _screen.BoundWidth / _dpiInfo.PixelsPerDip;
            Viewbox.Height = _screen.BoundHeight / _dpiInfo.PixelsPerDip;

            if (_debug)
            {
                MousePosDebug.Width = MousePosDebug.Width * _dpiInfo.PixelsPerDip;
                MousePosDebug.Height = MousePosDebug.Height * _dpiInfo.PixelsPerDip;
            }

            _mouseAnimator = new MouseAnimator(_cursorScale, BigCursor, _dpiInfo);
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            _mouseEvents.MouseMoveExt -= OnMouseMove;
        }

        /// <summary>
        /// Called when the position of the mouse is changed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnMouseMove(object sender, MouseEventExtArgs e)
        {
            var mouseInScreen = e.X >= _screen.BoundX && e.X <= _screen.BoundX + _screen.BoundWidth &&
                                e.Y >= _screen.BoundY && e.Y <= _screen.BoundY + _screen.BoundHeight;
            if (_debug)
            {
                var infoBuilder = new StringBuilder();
                infoBuilder
                    .AppendFormat("Name {0}", _screen.Name).AppendLine()
                    .AppendFormat("Primary {0}", _screen.Primary).AppendLine()
                    .AppendFormat("PixelsPerDip {0}", _dpiInfo.PixelsPerDip).AppendLine()
                    .AppendFormat("Mouse {0},{1}", e.X, e.Y).AppendLine()
                    .AppendFormat("InScreen {0}", mouseInScreen).AppendLine()
                    .AppendFormat("Draw {0},{1}", e.X - _screen.BoundX, e.Y - _screen.BoundY);
                DebugInfo.Content = infoBuilder.ToString();

                Canvas.SetTop(MousePosDebug, e.Y - _screen.BoundY);
                Canvas.SetLeft(MousePosDebug, e.X - _screen.BoundX);
            }

            _mouseAnimator.MouseInScreen = mouseInScreen;
            if (!mouseInScreen)
            {
                return;
            }

            _cursorTranslate.X = e.X - _screen.BoundX;
            _cursorTranslate.Y = e.Y - _screen.BoundY;
            _cursorScale.CenterX = e.X - _screen.BoundX;
            _cursorScale.CenterY = e.Y - _screen.BoundY;
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
