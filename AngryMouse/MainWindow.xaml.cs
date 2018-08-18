using Gma.System.MouseKeyHook;
using System.Windows;
using System.Windows.Input;

namespace AngryMouse
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private IKeyboardMouseEvents m_GlobalHook;

        public MainWindow()
        {
            InitializeComponent();

            m_GlobalHook = Hook.GlobalEvents();

            m_GlobalHook.MouseMoveExt += OnMouseMove;
        }

        private void OnMouseMove(object sender, MouseEventExtArgs e)
        {
            coordinates.Content = e.X + "," + e.Y;
        }
    }
}
