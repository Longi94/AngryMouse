using System;
using AngryMouse.Mouse;
using AngryMouse.Screen;
using Gma.System.MouseKeyHook;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;

namespace AngryMouse
{
    /// <summary>
    /// Interaction logic for DebugInfoWindow.xaml
    /// </summary>
    public partial class DebugInfoWindow
    {
        /// <summary>
        /// Globa hook to show debug information
        /// </summary>
        private readonly IKeyboardMouseEvents _mouseEvents;

        /// <summary>
        /// List of screens.
        /// </summary>
        private ObservableCollection<ScreenInfo> ScreenInfos { get; }

        /// <summary>
        /// Main constructor.
        /// </summary>
        /// <param name="detector">detector to show info from</param>
        /// <param name="screenInfos">screen infos that will be shown in a table</param>
        public DebugInfoWindow(MouseShakeDetector detector, List<ScreenInfo> screenInfos)
        {
            InitializeComponent();

            detector.MouseShake += OnMouseShake;

            _mouseEvents = StaticHook.GlobalEvents();
            _mouseEvents.MouseMoveExt += OnMouseMove;

            ScreenInfos = new ObservableCollection<ScreenInfo>(screenInfos);

            ScreensTable.ItemsSource = ScreenInfos;

            VirtualScreen.Content = SystemParameters.VirtualScreenWidth + "x" + SystemParameters.VirtualScreenHeight;
            VirtualScreenTopLeft.Content = SystemParameters.VirtualScreenLeft + "x" + SystemParameters.VirtualScreenTop;
        }

        private void OnMouseShake(object sender, MouseShakeArgs e)
        {
            IsShaking.Content = e.IsShaking;
        }

        private void OnMouseMove(object sender, MouseEventExtArgs e)
        {
            Coordinates.Content = e.X + "," + e.Y;
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            _mouseEvents.MouseMoveExt -= OnMouseMove;
        }
    }
}
