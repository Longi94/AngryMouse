using AngryMouse.Mouse;
using Gma.System.MouseKeyHook;
using System.Windows;

namespace AngryMouse
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private MouseShakeDetector detector;

        public MainWindow()
        {
            InitializeComponent();

            detector = new MouseShakeDetector();
            detector.MouseMove += OnMouseMove;
            detector.MouseShake += OnMouseShake;
        }

        private void OnMouseMove(object sender, MouseEventExtArgs e)
        {
            coordinates.Content = e.X + "," + e.Y;
        }

        private void OnMouseShake(object sender, MouseShakeArgs e)
        {
            isShaking.Content = e.IsShaking;
        }


    }
}
