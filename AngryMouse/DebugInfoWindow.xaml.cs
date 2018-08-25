using AngryMouse.Mouse;
using Gma.System.MouseKeyHook;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Forms;

namespace AngryMouse
{
    /// <summary>
    /// Interaction logic for DebugInfoWindow.xaml
    /// </summary>
    public partial class DebugInfoWindow : Window
    {
        /// <summary>
        /// Globa hook to show debug information
        /// </summary>
        private IKeyboardMouseEvents mouseEvents;

        /// <summary>
        /// List of screens.
        /// </summary>
        private ObservableCollection<ScreenInfo> ScreenInfos { get; } = new ObservableCollection<ScreenInfo>();

        /// <summary>
        /// Main constructor.
        /// </summary>
        /// <param name="detector"></param>
        public DebugInfoWindow(MouseShakeDetector detector)
        {
            InitializeComponent();

            detector.MouseShake += OnMouseShake;

            mouseEvents = Hook.GlobalEvents();
            mouseEvents.MouseMoveExt += OnMouseMove;

            foreach (Screen screen in Screen.AllScreens)
            {
                ScreenInfos.Add(new ScreenInfo()
                {
                    Name = screen.DeviceName,
                    X = screen.Bounds.X,
                    Y = screen.Bounds.Y,
                    Width = screen.Bounds.Width,
                    Height = screen.Bounds.Height,
                    Primary = screen.Primary
                });
            }

            ScreensTable.ItemsSource = ScreenInfos;
        }

        private void OnMouseShake(object sender, MouseShakeArgs e)
        {
            IsShaking.Content = e.IsShaking;
        }

        private void OnMouseMove(object sender, MouseEventExtArgs e)
        {
            Coordinates.Content = e.X + "," + e.Y;
        }
    }

    /// <summary>
    /// Class representing a screen.
    /// </summary>
    public class ScreenInfo : INotifyPropertyChanged
    {
        private string name;
        /// <summary>
        /// The name of the screen.
        /// </summary>
        public string Name
        {
            get => name;
            set
            {
                name = value;
                RaiseProperChanged();
            }
        }

        private int x;
        /// <summary>
        /// X position of the screen.
        /// </summary>
        public int X
        {
            get => x;
            set
            {
                x = value;
                RaiseProperChanged();
            }
        }

        private int y;
        /// <summary>
        /// Y position of the screen.
        /// </summary>
        public int Y
        {
            get => y;
            set
            {
                y = value;
                RaiseProperChanged();
            }
        }

        private int width;
        /// <summary>
        /// Width of the screen.
        /// </summary>
        public int Width
        {
            get => width;
            set
            {
                width = value;
                RaiseProperChanged();
            }
        }

        private int height;
        /// <summary>
        /// Height of the screen.
        /// </summary>
        public int Height
        {
            get => height;
            set
            {
                height = value;
                RaiseProperChanged();
            }
        }

        private bool primary;
        /// <summary>
        /// Indicates that this is the primary screen.
        /// </summary>
        public bool Primary
        {
            get => primary;
            set
            {
                primary = value;
                RaiseProperChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Fire the property changed event. Implement to make the datagrid work.
        /// </summary>
        /// <param name="caller"></param>
        private void RaiseProperChanged([CallerMemberName] string caller = "")
        {

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(caller));
        }
    }
}
