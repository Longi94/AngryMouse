using AngryMouse.Util;
using Gma.System.MouseKeyHook;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Shapes;

namespace AngryMouse
{
    /// <summary>
    /// Interaction logic for OverlayWindow.xaml
    /// </summary>
    public partial class OverlayWindow : Window
    {
        private IKeyboardMouseEvents mouseEvents;

        private Rectangle rect = new Rectangle();

        // TODO temporary
        const int screenWidth = 1920;
        const int screenHeight = 1080;

        private bool shaking = false;

        public OverlayWindow()
        {
            InitializeComponent();

            mouseEvents = Hook.GlobalEvents();
            mouseEvents.MouseMoveExt += OnMouseMove;

        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            var hwnd = new WindowInteropHelper(this).Handle;
            WindowUtil.SetWindowExTransparent(hwnd);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            rect.Fill = new SolidColorBrush(Colors.Black);
            rect.Width = 200;
            rect.Height = 200;
            OverlayCanvas.Children.Add(rect);

            OverlayCanvas.Width = screenWidth;
            OverlayCanvas.Height = screenHeight;

            // DPI scaling workaround
            PresentationSource presentationSource = PresentationSource.FromVisual(this);
            Matrix m = presentationSource.CompositionTarget.TransformToDevice;

            double dpiWidthFactor = m.M11;
            double dpiHeightFactor = m.M22;

            Viewbox.Width = screenWidth / dpiWidthFactor;
            Viewbox.Height = screenHeight / dpiHeightFactor;
        }

        private void OnMouseMove(object sender, MouseEventExtArgs e)
        {
            Coordinates.Content = e.X + "," + e.Y;
            Canvas.SetLeft(rect, e.X);
            Canvas.SetTop(rect, e.Y);
        }

        public void SetMouseShake(bool shaking)
        {
            if (this.shaking != shaking)
            {
                this.shaking = shaking;
                rect.Fill = new SolidColorBrush(shaking ? Colors.White : Colors.Black);
            }
        }
    }
}
