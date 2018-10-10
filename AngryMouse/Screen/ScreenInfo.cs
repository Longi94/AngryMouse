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

        private int bpp;
        /// <summary>
        /// Bits per pixel.
        /// </summary>
        public int BPP
        {
            get => bpp;
            set
            {
                bpp = value;
                RaiseProperChanged();
            }
        }

        private int boundX;
        /// <summary>
        /// X position of the screen.
        /// </summary>
        public int BoundX
        {
            get => boundX;
            set
            {
                boundX = value;
                RaiseProperChanged();
            }
        }

        private int boundY;
        /// <summary>
        /// Y position of the screen.
        /// </summary>
        public int BoundY
        {
            get => boundY;
            set
            {
                boundY = value;
                RaiseProperChanged();
            }
        }

        private int boundWidth;
        /// <summary>
        /// Width of the screen.
        /// </summary>
        public int BoundWidth
        {
            get => boundWidth;
            set
            {
                boundWidth = value;
                RaiseProperChanged();
            }
        }

        private int boundHeight;
        /// <summary>
        /// Height of the screen.
        /// </summary>
        public int BoundHeight
        {
            get => boundHeight;
            set
            {
                boundHeight = value;
                RaiseProperChanged();
            }
        }

        private int workX;
        /// <summary>
        /// X position of the working area.
        /// </summary>
        public int WorkX
        {
            get => workX;
            set
            {
                workX = value;
                RaiseProperChanged();
            }
        }

        private int workY;
        /// <summary>
        /// Y position of the working area.
        /// </summary>
        public int WorkY
        {
            get => workY;
            set
            {
                workY = value;
                RaiseProperChanged();
            }
        }

        private int workWidth;
        /// <summary>
        /// Width of the working area.
        /// </summary>
        public int WorkWidth
        {
            get => workWidth;
            set
            {
                workWidth = value;
                RaiseProperChanged();
            }
        }

        private int workHeight;
        /// <summary>
        /// Height of the working area.
        /// </summary>
        public int WorkHeight
        {
            get => workHeight;
            set
            {
                workHeight = value;
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
