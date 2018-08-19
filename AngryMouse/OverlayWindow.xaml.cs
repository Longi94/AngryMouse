using AngryMouse.Util;
using Gma.System.MouseKeyHook;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media;

namespace AngryMouse
{
    /// <summary>
    /// Interaction logic for OverlayWindow.xaml
    /// </summary>
    public partial class OverlayWindow : Window
    {
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
            ScaleX = 0.3,
            ScaleY = 0.3
        };

        // TODO temporary, get the resolution from System.Windows.Forms.Screens
        const int screenWidth = 1920;
        const int screenHeight = 1080;

        /// <summary>
        /// Whether to show the big boi or not.
        /// </summary>
        private bool shaking = false;

        /// <summary>
        /// Main constructor.
        /// </summary>
        public OverlayWindow()
        {
            InitializeComponent();

            mouseEvents = Hook.GlobalEvents();
            mouseEvents.MouseMoveExt += OnMouseMove;
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            // Do not capture any mouse events
            // TODO I suspect this is the reason we cannot replace the cursor (hide it) since
            // the cursor draws on top of the big cursor.
            var hwnd = new WindowInteropHelper(this).Handle;
            WindowUtil.SetWindowExTransparent(hwnd);
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

            OverlayCanvas.Width = screenWidth;
            OverlayCanvas.Height = screenHeight;

            // DPI scaling workaround, Viewbox HACKK
            PresentationSource presentationSource = PresentationSource.FromVisual(this);
            Matrix m = presentationSource.CompositionTarget.TransformToDevice;

            double dpiWidthFactor = m.M11;
            double dpiHeightFactor = m.M22;

            Viewbox.Width = screenWidth / dpiWidthFactor;
            Viewbox.Height = screenHeight / dpiHeightFactor;
        }

        /// <summary>
        /// Called when the position of the mouse is changed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnMouseMove(object sender, MouseEventExtArgs e)
        {
            Coordinates.Content = e.X + "," + e.Y;

            cursorTranslate.X = e.X;
            cursorTranslate.Y = e.Y;
            cursorScale.CenterX = e.X;
            cursorScale.CenterY = e.Y;
        }

        /// <summary>
        /// Causes the big mouse to appear or disappear depending on the parameter and the current state
        /// of the mouse.
        /// </summary>
        /// <param name="shaking">Whether the mouse is shaking or not.</param>
        public void SetMouseShake(bool shaking)
        {
            if (this.shaking != shaking)
            {
                this.shaking = shaking;

                BigCursor.Visibility = shaking ? Visibility.Visible : Visibility.Hidden;
            }
        }
    }
}
