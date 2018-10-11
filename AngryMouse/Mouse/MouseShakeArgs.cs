using System;

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

        /// <summary>
        /// The this thing occured.
        /// </summary>
        public readonly DateTime Timestamp;

        public MouseShakeArgs(bool shaking, DateTime timestamp)
        {
            IsShaking = shaking;
            Timestamp = timestamp;
        }
    }
}
