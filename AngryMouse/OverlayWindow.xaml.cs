using AngryMouse.Animation;
using AngryMouse.Screen;
using AngryMouse.Util;
using Gma.System.MouseKeyHook;
using System;
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
        /// Time of the animation if the scaling goes from 0 to Max or vice versa.
        /// </summary>
        private const int MaxAnimationLength = 200;

        /// <summary>
        /// Maximum cursor size.
        /// </summary>
        private const double MaxScale = 0.3;

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
        /// The start of the animation
        /// </summary>
        private int animationStart = 0;

        /// <summary>
        /// The current scale of the cursor. Stored so we can start a new animation from the middle scale if needed.
        /// </summary>
        private double currentScale = 0;

        /// <summary>
        /// The scale to start the animation from.
        /// </summary>
        private double scaleAnimStart = 0;

        /// <summary>
        /// The scale to end the animation at.
        /// </summary>
        private double scaleAnimEnd = MaxScale;

        /// <summary>
        /// The event fired when this window finishe loading.
        /// </summary>
        public event EventHandler<WindowLoadedEventArgs> OnLoad;

        /// <summary>
        /// Main constructor.
        /// </summary>
        /// <param name="screen">The window to show the screen in.</param>
        public OverlayWindow(ScreenInfo screen)
        {
            InitializeComponent();

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

            // DPI scaling workaround, Viewbox HACKK
            PresentationSource presentationSource = PresentationSource.FromVisual(this);
            Matrix m = presentationSource.CompositionTarget.TransformToDevice;

            double dpiWidthFactor = m.M11;
            double dpiHeightFactor = m.M22;

            Viewbox.Width = screen.BoundWidth / dpiWidthFactor;
            Viewbox.Height = screen.BoundHeight / dpiHeightFactor;

            // Fire the loaded event to inform the app about the scale factor of this screen.
            WindowLoadedEventArgs args = new WindowLoadedEventArgs(dpiWidthFactor, dpiHeightFactor);
            OnLoad?.Invoke(this, args);
        }

        /// <summary>
        /// Called when the position of the mouse is changed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnMouseMove(object sender, MouseEventExtArgs e)
        {
            if (e.X < screen.BoundX || e.X > screen.BoundX + screen.BoundWidth ||
                e.Y < screen.BoundY || e.Y > screen.BoundY + screen.BoundHeight)
            {
                // mouse is outside of this window
                cursorScale.ScaleX = cursorScale.ScaleY = 0;
                BigCursor.Visibility = Visibility.Hidden;
                return;
            }

            cursorTranslate.X = e.X - screen.BoundX;
            cursorTranslate.Y = e.Y - screen.BoundY;
            cursorScale.CenterX = e.X - screen.BoundX;
            cursorScale.CenterY = e.Y - screen.BoundY;

            if (e.Timestamp - animationStart > MaxAnimationLength)
            {
                if (shaking)
                {
                    cursorScale.ScaleX = cursorScale.ScaleY = MaxScale;
                    BigCursor.Visibility = Visibility.Visible;
                }
                else
                {
                    cursorScale.ScaleX = cursorScale.ScaleY = 0;
                    BigCursor.Visibility = Visibility.Hidden;
                }
            }
            else
            {
                double scale = CalculateScale(e.Timestamp);

                if (scale < 0 || scale > MaxScale)
                {
                    // This can happen when this function is called before SetMouseShake
                    // Just ignore it end everything will be fine ¯\_(ツ)_/¯
                    return;
                }

                currentScale = scale;
                cursorScale.ScaleX = cursorScale.ScaleY = scale;
                BigCursor.Visibility = Visibility.Visible;
            }
        }

        /// <summary>
        /// Causes the big mouse to appear or disappear depending on the parameter and the current state
        /// of the mouse.
        /// </summary>
        /// <param name="shaking">Whether the mouse is shaking or not.</param>
        /// <param name="timestamp">The timestamp the shake change occured at.</param>
        public void SetMouseShake(bool shaking, int timestamp)
        {
            if (this.shaking != shaking)
            {
                this.shaking = shaking;

                animationStart = timestamp;

                scaleAnimStart = currentScale;
                scaleAnimEnd = shaking ? MaxScale : 0;
            }
        }

        /// <summary>
        /// Calculate the current scale of the cursor.
        /// </summary>
        /// <param name="timestamp">The timestamp of the frame the calculate for.</param>
        /// <returns></returns>
        private double CalculateScale(int timestamp)
        {
            double animLength = Math.Abs(scaleAnimEnd - scaleAnimStart) / MaxScale * MaxAnimationLength;

            int elapsed = timestamp - animationStart;

            double t = elapsed / animLength;

            double tEase = Easing.CubicInOut(t);

            return (scaleAnimEnd - scaleAnimStart) * tEase + scaleAnimStart;
        }
    }
}
