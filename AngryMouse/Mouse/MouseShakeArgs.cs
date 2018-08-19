using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AngryMouse.Mouse
{

    /// <summary>
    /// Arguments for the shaking mouse event.
    /// </summary>
    public class MouseShakeArgs : EventArgs
    {
        /// <summary>
        /// True if the mouse started shaking. False if it stopped.
        /// </summary>
        public readonly bool IsShaking;

        public MouseShakeArgs(bool shaking)
        {
            IsShaking = shaking;
        }
    }
}
