using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace AngryMouse.Screen
{
    /// <summary>
    /// Class representing a screen.
    /// </summary>
    public class ScreenInfo : INotifyPropertyChanged
    {
        private string _name;

        /// <summary>
        /// The name of the screen.
        /// </summary>
        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                RaiseProperChanged();
            }
        }

        private int _bpp;

        /// <summary>
        /// Bits per pixel.
        /// </summary>
        public int Bpp
        {
            get => _bpp;
            set
            {
                _bpp = value;
                RaiseProperChanged();
            }
        }

        private int _boundX;

        /// <summary>
        /// X position of the screen.
        /// </summary>
        public int BoundX
        {
            get => _boundX;
            set
            {
                _boundX = value;
                RaiseProperChanged();
            }
        }

        private int _boundY;

        /// <summary>
        /// Y position of the screen.
        /// </summary>
        public int BoundY
        {
            get => _boundY;
            set
            {
                _boundY = value;
                RaiseProperChanged();
            }
        }

        private int _boundWidth;

        /// <summary>
        /// Width of the screen.
        /// </summary>
        public int BoundWidth
        {
            get => _boundWidth;
            set
            {
                _boundWidth = value;
                RaiseProperChanged();
            }
        }

        private int _boundHeight;

        /// <summary>
        /// Height of the screen.
        /// </summary>
        public int BoundHeight
        {
            get => _boundHeight;
            set
            {
                _boundHeight = value;
                RaiseProperChanged();
            }
        }

        private int _workX;

        /// <summary>
        /// X position of the working area.
        /// </summary>
        public int WorkX
        {
            get => _workX;
            set
            {
                _workX = value;
                RaiseProperChanged();
            }
        }

        private int _workY;

        /// <summary>
        /// Y position of the working area.
        /// </summary>
        public int WorkY
        {
            get => _workY;
            set
            {
                _workY = value;
                RaiseProperChanged();
            }
        }

        private int _workWidth;

        /// <summary>
        /// Width of the working area.
        /// </summary>
        public int WorkWidth
        {
            get => _workWidth;
            set
            {
                _workWidth = value;
                RaiseProperChanged();
            }
        }

        private int _workHeight;

        /// <summary>
        /// Height of the working area.
        /// </summary>
        public int WorkHeight
        {
            get => _workHeight;
            set
            {
                _workHeight = value;
                RaiseProperChanged();
            }
        }

        private bool _primary;

        /// <summary>
        /// Indicates that this is the primary screen.
        /// </summary>
        public bool Primary
        {
            get => _primary;
            set
            {
                _primary = value;
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
