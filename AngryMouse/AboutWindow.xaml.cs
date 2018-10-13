using System.Reflection;
using System.Windows;

namespace AngryMouse
{
    /// <summary>
    /// Interaction logic for AboutWindow.xaml
    /// </summary>
    public partial class AboutWindow
    {
        public AboutWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            AppName.Content = Assembly.GetExecutingAssembly().GetName().Name;
            AppVersion.Content = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            AppCopyright.Content = Assembly.GetExecutingAssembly().GetCustomAttribute<AssemblyCopyrightAttribute>().Copyright;
        }

        private void Github_OnClick(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://github.com/Longi94/AngryMouse");
        }
    }
}
