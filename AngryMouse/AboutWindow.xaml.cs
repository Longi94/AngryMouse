using System;
using System.Reflection;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

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

            ImageSource imageSource = Imaging.CreateBitmapSourceFromHBitmap(
                Properties.Resources.IconPng.GetHbitmap(),
                IntPtr.Zero,
                Int32Rect.Empty,
                BitmapSizeOptions.FromEmptyOptions());
            Logo.Source = imageSource;
        }

        private void Github_OnClick(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://github.com/Longi94/AngryMouse");
        }
    }
}
