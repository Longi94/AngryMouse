using AngryMouse.Mouse;
using Gma.System.MouseKeyHook;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Media;

namespace AngryMouse
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : System.Windows.Application
    {

        private NotifyIcon notifyIcon;

        private MouseShakeDetector detector;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            notifyIcon = new NotifyIcon
            {
                Visible = true,
                Icon = AngryMouse.Properties.Resources.Cursor
            };

            CreateContextMenu();

            detector = new MouseShakeDetector();
            detector.MouseMove += OnMouseMove;
            detector.MouseShake += OnMouseShake;
        }

        private void CreateContextMenu()
        {
            ContextMenuStrip menu = new ContextMenuStrip();

            menu.Items.Add("Exit").Click += (s, e) => ExitApp();

            notifyIcon.ContextMenuStrip = menu;
        }

        private void ExitApp()
        {
            MainWindow.Close();
            notifyIcon.Dispose();
            notifyIcon = null;
        }

        private void OnMouseMove(object sender, MouseEventExtArgs e)
        {
        }

        private void OnMouseShake(object sender, MouseShakeArgs e)
        {
            OverlayWindow overlayWindow = (OverlayWindow)MainWindow;

            overlayWindow.SetMouseShake(e.IsShaking);
        }
    }
}
