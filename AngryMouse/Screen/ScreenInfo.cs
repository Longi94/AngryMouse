using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace AngryMouse.Screen
{

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
